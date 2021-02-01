using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Transfer
{
    
    public class TransferReceiveValidationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult ItemTransferReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ItemTransferReceiverValidation> itemtransferreceivevalidations = db.ItemTransferReceiverValidation.Where(st => st.ItemTransferReceiveId == id);
            DataSourceResult result = itemtransferreceivevalidations.ToDataSourceResult(request, itemTransferReceiveValidation => new
            {

                ItemTransferReceiverValidationId = itemTransferReceiveValidation.ItemTransferReceiverValidationId,
                Status = itemTransferReceiveValidation.Status,
                Remark = itemTransferReceiveValidation.Remark,
                DateCreated = itemTransferReceiveValidation.DateCreated,
                LastModified = itemTransferReceiveValidation.LastModified,
                CreatedBy = itemTransferReceiveValidation.CreatedBy,
                ModifiedBy = itemTransferReceiveValidation.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceiveValidations_Create([DataSourceRequest]DataSourceRequest request, ItemTransferReceiverValidation itemTransferReceiveValidation, int id)
        {
            var transferReceive = db.ItemTransferReceive.Find(id);
            int frombranch = 0;
            int tobranch=0;
            if (transferReceive != null)
            {
                 frombranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(transferReceive.FromStore);
                 tobranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(transferReceive.ToStore);
            }
            #region validations
            var storeItem = db.StoreItemAssignments.Where(sc => sc.StoreCode == transferReceive.ToStore);

            if (transferReceive != null && transferReceive.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "Record is already approved,you can not manipulate it.");
            }

            if (transferReceive != null && transferReceive.ItemTransferItemReceive.Count == 0 && itemTransferReceiveValidation.Status == ApprovalStatuses.Approve)
            {
                ModelState.AddModelError("quantity", "No Item Found in the ItemTransfer ");
            }

            var transferSend = db.ItemTransfers.Where(x => x.ItemTransferID == transferReceive.ItemTransferID)
                .Where(x => x.IsVoid).ToList();
            if(transferSend.Any())
                ModelState.AddModelError("void", "The transfer send number " + transferReceive.ItemTransferID + " is voided hence can't be approved");
            // #region check zero received qty

            if (itemTransferReceiveValidation.Status == ApprovalStatuses.Approve)
            {
                if (transferReceive != null)
                {
                    foreach (var receivedItem in transferReceive.ItemTransferItemReceive)
                    {
                        if (receivedItem.ReceivedQuantity < 0)
                            ModelState.AddModelError("zeroQuantity", "Received Quantity should be more than zero. " + receivedItem.ItemCode);

                        #region check if they receive extra
                        
                        var sentItem = db.ItemTransferItems
                            .Where(x => x.ItemTransferID == transferReceive.ItemTransferID)
                            .Where(x => x.ItemCode == receivedItem.ItemCode)
                            .FirstOrDefault();

                        if (sentItem != null)
                        {
                            decimal totalQty = sentItem.ShippedQuantity;
                            decimal receivedQty = 0;
                            decimal remainingQty = 0;

                            var alreadyReceivedQty = db.ItemTransferItemReceive
                                .Where(x => x.ItemTransferReceive.IsOnlineApproved)
                                 .Where(x => x.ItemTransferReceive.IsVoid == false)
                                .Where(x => x.ItemCode == receivedItem.ItemCode)
                                .Where(x => x.ItemTransferReceive.ItemTransferID == transferReceive.ItemTransferID).ToList();
                            if (alreadyReceivedQty != null)
                                receivedQty = alreadyReceivedQty.Sum(x => x.ReceivedQuantity);
                            remainingQty = totalQty - receivedQty;

                            if (remainingQty <= 0)
                                ModelState.AddModelError("over", $"The remaining unreceived balance for {receivedItem.ItemCode} is 0, hence you can't add.");
                            if (remainingQty < receivedItem.ReceivedQuantity)
                                ModelState.AddModelError("over", $"The remaining unreceived balance for {receivedItem.ItemCode} is {remainingQty}, hence you receive only {remainingQty}.");
                        }
                        #endregion

                    }
                }
            }

            #endregion

            if (ModelState.IsValid)
            {

                if (itemTransferReceiveValidation.Status == ApprovalStatuses.Approve)
                {
                    if (transferReceive != null)
                    {
                        transferReceive.Status = "Receiver Approved";
                        transferReceive.CreatedBy = User.Identity.Name;
                        transferReceive.DateCreated = DateTime.Now;
                        transferReceive.IsOnlineApproved = true;
                        transferReceive.OnlineApprovedBy = User.Identity.Name;
                        transferReceive.OnlineApprovedTime = DateTime.Now;
                        db.ItemTransferReceive.Attach(transferReceive);
                        db.Entry(transferReceive).State = EntityState.Modified;
                       // db.SaveChanges();
                    
                        var averagePriceSetting = db.AveragePriceSettings.FirstOrDefault();
                        if (averagePriceSetting != null)
                        {
                            var avgpricesetting = averagePriceSetting.Level;
                            foreach (var i in transferReceive.ItemTransferItemReceive)
                            {
                                if (i.ReceivedQuantity > 0)
                                {
                                    #region update to branch item

                                    var branchItemAssignment =
                                        db.BranchItemAssignments.Where(si => si.BranchId == tobranch)
                                            .FirstOrDefault(si => si.ItemCode == i.ItemCode);

                                    if (branchItemAssignment == null)
                                    {
                                        BranchItemAssignment newItemAssignment = new BranchItemAssignment
                                        {
                                            BranchId = tobranch,
                                            ItemCode = i.ItemCode,
                                            ItemCategoryCode = i.ItemCategoryCode,
                                            Balance = i.ReceivedQuantity,
                                            AvgPrice = i.UPrice,
                                        };
                                        db.BranchItemAssignments.Add(newItemAssignment);
                                       // db.SaveChanges();

                                    }
                                    else
                                    {
                                        if (frombranch != tobranch && avgpricesetting == AveragePriceLvel.PerBranch)
                                        {
                                            branchItemAssignment.AvgPrice = ((branchItemAssignment.Balance *
                                                                              branchItemAssignment.AvgPrice) +
                                                                             i.ReceivedQuantity *
                                                                             i.UPrice)
                                                                            /
                                                                            (branchItemAssignment.Balance +
                                                                             i.ReceivedQuantity);
                                        }
                                        branchItemAssignment.Balance = branchItemAssignment.Balance +
                                                                       i.ReceivedQuantity;
                                        db.BranchItemAssignments.Attach(branchItemAssignment);
                                        db.Entry(branchItemAssignment).State = EntityState.Modified;
                                        //db.SaveChanges();
                                    }

                                    #endregion

                                    #region update to store

                                    var iteminStore =storeItem.FirstOrDefault(si => si.ItemCode == i.ItemCode);

                                    if (iteminStore == null)
                                    {
                                        StoreItemAssignment newItemAssignment = new StoreItemAssignment
                                        {
                                            ItemCode = i.ItemCode,
                                            ItemCategoryCode = i.ItemCategoryCode,
                                            StoreCode = transferReceive.ToStore,
                                            Balance = i.ReceivedQuantity,
                                            AvgPrice = i.UPrice,
                                        };
                                        db.StoreItemAssignments.Add(newItemAssignment);
                                       // db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = newItemAssignment,
                                            ReferenceDoc = "Transfer Receive_" + transferReceive.ItemTransferReceiveId.ToString(),
                                            Reference = transferReceive.Remark,
                                            Date = transferReceive.ReceiveDate,
                                            ReceivedQTY = i.ReceivedQuantity,
                                            ReceivedUPrice = i.UPrice,
                                            ReceivedTPrice = i.TPrice,
                                            BalanceQTY = newItemAssignment.Balance,
                                            BalanceUPrice = newItemAssignment.AvgPrice,
                                            BalanceTPrice = newItemAssignment.AvgPrice * newItemAssignment.Balance,
                                            //new    
                                            StoreCode = transferReceive.FromStore,
                                            GLPeriodId = transferReceive.GLPeriodId,
                                            TransactionType = BinCardTransactionTypes.TransferReceive,
                                            StatusType = BinCardTransactionStatus.Regular,
                                        };

                                        db.BinCardLines.Add(binCardLine);
                                        //db.SaveChanges();

                                        #endregion

                                    }
                                    else
                                    {
                                        if (transferReceive.FromStore != transferReceive.ToStore &&
                                            avgpricesetting == AveragePriceLvel.PerStore)
                                        {
                                            iteminStore.AvgPrice = ((iteminStore.Balance * iteminStore.AvgPrice) +
                                                                    i.ReceivedQuantity *
                                                                    i.UPrice)
                                                                   /
                                                                   (iteminStore.Balance +
                                                                    i.ReceivedQuantity);
                                        }
                                        iteminStore.Balance = iteminStore.Balance + i.ReceivedQuantity;
                                        db.StoreItemAssignments.Attach(iteminStore);
                                        db.Entry(iteminStore).State = EntityState.Modified;
                                        //db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = iteminStore,
                                            ReferenceDoc = "Transfer Receive_" + transferReceive.ItemTransferReceiveId.ToString(),
                                            Reference = transferReceive.Remark,
                                            Date = transferReceive.ReceiveDate,
                                            ReceivedQTY = i.ReceivedQuantity,
                                            ReceivedUPrice = i.UPrice,
                                            ReceivedTPrice = i.TPrice,
                                            BalanceQTY = iteminStore.Balance,
                                            BalanceUPrice = iteminStore.AvgPrice,
                                            BalanceTPrice = iteminStore.AvgPrice * iteminStore.Balance,
                                            //new    
                                            StoreCode = transferReceive.FromStore,
                                            GLPeriodId = transferReceive.GLPeriodId,
                                            TransactionType = BinCardTransactionTypes.TransferReceive,
                                            StatusType = BinCardTransactionStatus.Regular,
                                        };
                                        db.BinCardLines.Add(binCardLine);
                                       // db.SaveChanges();

                                        #endregion
                                    }

                                    #endregion
                                }

                            }
                        }
                    }
                }
                else if (itemTransferReceiveValidation.Status == ApprovalStatuses.Reject)
                {
                    if (transferReceive != null)
                    {
                        transferReceive.Status = "Receiver Rejected";
                        transferReceive.IsSendForApproval = false;
                        db.ItemTransferReceive.Attach(transferReceive);
                        db.Entry(transferReceive).State = EntityState.Modified;
                       // db.SaveChanges();
                    }
                }

                var entity = new ItemTransferReceiverValidation
                {
                    ItemTransferReceiveId = id,
                    Status = itemTransferReceiveValidation.Status,
                    Remark = itemTransferReceiveValidation.Remark,
                };



                db.ItemTransferReceiverValidation.Add(entity);
                db.SaveChanges();
                itemTransferReceiveValidation.ItemTransferReceiverValidationId = entity.ItemTransferReceiverValidationId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer receive approval", "Success", User.Identity.Name, $"approved transfer receive {entity.ItemTransferReceiveId} ", Modules.Inventory);
                #endregion
            }




            return Json(new[] { itemTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, ItemTransferReciverValidation itemTransferReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferReciverValidation
                {
                    ItemTransferReciverValidationID = itemTransferReceiveValidation.ItemTransferReciverValidationID,
                    Status = itemTransferReceiveValidation.Status,
                    Remark = itemTransferReceiveValidation.Remark,
                    DateCreated = itemTransferReceiveValidation.DateCreated,
                    LastModified = itemTransferReceiveValidation.LastModified,
                    CreatedBy = itemTransferReceiveValidation.CreatedBy,
                    ModifiedBy = itemTransferReceiveValidation.ModifiedBy
                };

                db.ItemTransferReciverValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { itemTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ItemTransferReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferReceiveValidation ItemTransferReceiveValidation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ItemTransferReceiveValidation
        //        {
        //            ItemTransferReceiveValidationID = ItemTransferReceiveValidation.ItemTransferReceiveValidationID,
        //            Status = ItemTransferReceiveValidation.Status,
        //            Remark = ItemTransferReceiveValidation.Remark,
        //            DateCreated = ItemTransferReceiveValidation.DateCreated,
        //            LastModified = ItemTransferReceiveValidation.LastModified,
        //            CreatedBy = ItemTransferReceiveValidation.CreatedBy,
        //            ModifiedBy = ItemTransferReceiveValidation.ModifiedBy
        //        };

        //        db.ItemTransferReceiveValidations.Attach(entity);
        //        db.ItemTransferReceiveValidations.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { ItemTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
