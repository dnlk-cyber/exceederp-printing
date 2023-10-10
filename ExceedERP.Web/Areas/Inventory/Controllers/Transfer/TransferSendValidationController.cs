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

    public class TransferSendValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult ItemTransferValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ItemTransferValidation> itemtransfervalidations = db.ItemTransferValidations.Where(iti => iti.ItemTransferID == id);
            DataSourceResult result = itemtransfervalidations.ToDataSourceResult(request, itemTransferValidation => new
            {
                ItemTransferValidationID = itemTransferValidation.ItemTransferValidationID,
                Status = itemTransferValidation.Status,
                Remark = itemTransferValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferValidations_Create([DataSourceRequest] DataSourceRequest request, ItemTransferValidation itemTransferValidation, int id)
        {
            var itemTransfer = db.ItemTransfers.Find(id);
            var storeItem = db.StoreItemAssignments.Where(sc => sc.StoreCode == itemTransfer.FromStore);

            if (itemTransfer != null && itemTransfer.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "Record is already approved,you can not manipulate it.");

            }
            if (itemTransfer != null && itemTransfer.ItemTransferItem.Count == 0)
            {
                ModelState.AddModelError("quantity", "No Item found in the ItemTransfer ");
            }

            #region check balance

            if (itemTransferValidation.Status == ApprovalStatuses.Approve)
            {
                if (itemTransfer != null)
                {
                    foreach (var transferedItem in itemTransfer.ItemTransferItem)
                    {
                        var balance = storeItem.FirstOrDefault(si => si.ItemCode == transferedItem.ItemCode);

                        if (balance != null && balance.Balance - transferedItem.ShippedQuantity < 0)
                            ModelState.AddModelError("enoughQuant", "No Enough Quantity To Transfer for item " + transferedItem.ItemCode);

                        if (transferedItem.ShippedQuantity<0)
                            ModelState.AddModelError("zeroQuantity", "Shipped Quantity should be more than zero " + transferedItem.ItemCode);

                        if (transferedItem.UPrice < 0 || transferedItem.TPrice<0)
                            ModelState.AddModelError("zeroPrice", "Item price should be filled " + transferedItem.ItemCode);

                    }
                }
            }

            #endregion

            if (ModelState.IsValid)
            {


                if (itemTransferValidation.Status == ApprovalStatuses.Approve)
                {
                    if (itemTransfer != null)
                    {
                        itemTransfer.Status = "Sender Approved";
                        itemTransfer.IsOnlineApproved = true;
                        itemTransfer.OnlineApprovedBy = User.Identity.Name;
                        itemTransfer.OnlineApprovedTime = DateTime.Now;
                        db.ItemTransfers.Attach(itemTransfer);
                        db.Entry(itemTransfer).State = EntityState.Modified;
                       // db.SaveChanges();


                        foreach (var i in itemTransfer.ItemTransferItem)
                        {
                            if (i.ShippedQuantity > 0)
                            {
                                var iteminStore = storeItem.FirstOrDefault(si => si.ItemCode == i.ItemCode);

                                if (iteminStore != null)
                                {
                                    iteminStore.Balance = iteminStore.Balance - i.ShippedQuantity;
                                    db.StoreItemAssignments.Attach(iteminStore);
                                    db.Entry(iteminStore).State = EntityState.Modified;
                                   // db.SaveChanges();

                                    #region update bin card

                                    #region bin card line

                                    BinCardLine binCardLine = new BinCardLine
                                    {
                                        StoreItemAssignment = iteminStore,
                                        ReferenceDoc = "Transfer Send " + itemTransfer.ItemTransferID,
                                        Reference = itemTransfer.DocumentRef,
                                        Date = itemTransfer.TransferDate,
                                        IssuedQTY = i.ShippedQuantity,
                                        IssuedUPrice = i.UPrice,
                                        IssuedTPrice = i.TPrice,
                                        BalanceQTY = iteminStore.Balance,
                                        BalanceUPrice = i.UPrice,
                                        BalanceTPrice = i.UPrice * iteminStore.Balance,
                                        //new    
                                        StoreCode = itemTransfer.ToStore,
                                        GLPeriodId = itemTransfer.GLPeriodId,
                                        TransactionType = BinCardTransactionTypes.TransferSend,
                                        StatusType = BinCardTransactionStatus.Regular,
                                        TransactionOrder = BinCardTransactionOrder.TransferSend
                                    };

                                    db.BinCardLines.Add(binCardLine);
                                    //db.SaveChanges();

                                    #endregion

                                    #endregion
                                }
                            }


                        }

                        //transfer to receive
                        TransferredItem(itemTransfer.ItemTransferID);
                    }
                }
                else if (itemTransferValidation.Status == ApprovalStatuses.Reject)
                {
                    if (itemTransfer != null)
                    {
                        itemTransfer.Status = "Sender Rejected";
                        itemTransfer.IsSendForApproval = false;
                        db.ItemTransfers.Attach(itemTransfer);
                        db.Entry(itemTransfer).State = EntityState.Modified;
                       // db.SaveChanges();
                    }
                }

                var entity = new ItemTransferValidation
                {
                    ItemTransferID = id,
                    Status = itemTransferValidation.Status,
                    Remark = itemTransferValidation.Remark,
                };

                db.ItemTransferValidations.Add(entity);
                db.SaveChanges();
                itemTransferValidation.ItemTransferValidationID = entity.ItemTransferValidationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send approval", "Success", User.Identity.Name, $"approved transfer send {entity.ItemTransferID} ", Modules.Inventory);
                #endregion
            }



            return Json(new[] { itemTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferValidations_Update([DataSourceRequest]DataSourceRequest request, ItemTransferValidation ItemTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferValidation
                {
                    ItemTransferValidationID = ItemTransferValidation.ItemTransferValidationID,
                    Status = ItemTransferValidation.Status,
                    Remark = ItemTransferValidation.Remark,
                };

                db.ItemTransferValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ItemTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferValidations_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferValidation ItemTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferValidation
                {
                    ItemTransferValidationID = ItemTransferValidation.ItemTransferValidationID,
                    Status = ItemTransferValidation.Status,
                    Remark = ItemTransferValidation.Remark,
                };

                db.ItemTransferValidations.Attach(entity);
                db.ItemTransferValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ItemTransferValidation }.ToDataSourceResult(request, ModelState));
        }


        private void TransferredItem(int transferSendId)
        {
            var itemTransfer = db.ItemTransfers.Find(transferSendId);

            
            if (itemTransfer != null)
            {
                #region transfer receive
                var transferReceive = new ItemTransferReceive
                {
                    ItemTransferID = itemTransfer.ItemTransferID,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(itemTransfer.ToStore),
                    BusinessUnitId = itemTransfer.BusinessUnitId,
                    ReceiveDate = itemTransfer.TransferDate,
                    FromStore = itemTransfer.FromStore,
                    ToStore = itemTransfer.ToStore,
                    DocumentRef = itemTransfer.DocumentRef,
                    DeliveredBy = itemTransfer.DeliveredBy,
                    PlateNo = itemTransfer.PlateNo,
                    SupplierName = itemTransfer.SupplierName,
                    DriverType = itemTransfer.DriverType.ToString(),
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = itemTransfer.Remark,
                    GLPeriodId = itemTransfer.GLPeriodId,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(itemTransfer.ToStore).ToString()
                };

                db.ItemTransferReceive.Add(transferReceive);
                db.SaveChanges();
                #endregion

                #region transfer items
                foreach (var item in itemTransfer.ItemTransferItem)
                {
                    var itemReceive = new ItemTransferItemReceive
                    {
                        ItemTransferReceiveId = transferReceive.ItemTransferReceiveId,
                        ItemCategoryCode = item.ItemCategoryCode,
                        ItemCode = item.ItemCode,
                        RequestedQuantity = item.RequestedQuantity,
                        UPrice = item.UPrice,
                        PartOrCodeNumber = item.PartOrCodeNumber,
                        ShippedQuantity = item.ShippedQuantity,
                        TPrice = item.ShippedQuantity * item.UPrice,
                        ReceivedQuantity = item.ShippedQuantity,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.ItemTransferItemReceive.Add(itemReceive);
                    db.SaveChanges();
                }
                #endregion
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
