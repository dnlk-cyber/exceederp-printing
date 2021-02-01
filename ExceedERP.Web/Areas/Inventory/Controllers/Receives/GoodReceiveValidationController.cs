using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
    
    public class GoodReceiveValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, InventoryConstants.PropertyInspector)]
        public ActionResult GoodReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GoodReceiveValidation> goodreceivevalidations = db.GoodReceiveValidations.Where(gi => gi.GoodReceiveId == id);
            DataSourceResult result = goodreceivevalidations.ToDataSourceResult(request, goodReceiveValidation => new
            {
                GoodReceiveValidationID = goodReceiveValidation.GoodReceiveValidationID,
                Status = goodReceiveValidation.Status,
                Remark = goodReceiveValidation.Remark,
                OrgBranchName = goodReceiveValidation.OrgBranchName,
                Approver = goodReceiveValidation.Approver,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveValidations_Create([DataSourceRequest] DataSourceRequest request, GoodReceiveValidation goodReceiveValidation, int id)
        {
            var record = db.GoodReceives.Find(id);
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            if (pricesetting == null)
                ModelState.AddModelError("price setting", "Average price setting is not defined.");
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.GoodReceiveItem.Count == 0)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
                if (goodReceiveValidation.Status == ApprovalStatuses.Approve)
                {
                    foreach (var i in record.GoodReceiveItem)
                    {
                        var item = db.InventoryItems.FirstOrDefault(ic => ic.ItemCode == i.ItemCode);
                        if (item == null)
                        {
                            string message = "Item " + i.ItemCode + " is not defined in the system.";
                            ModelState.AddModelError("stockItem", message);
                        }
                    }

                    //validate against order quanity
                    if (!record.IsManual)
                    {
                        string error = InventoryValidationHelper.ValidateGrN(record.GoodReceiveId);
                        if(!string.IsNullOrEmpty(error))
                            ModelState.AddModelError("validation", error);
                    }
                }
                if (goodReceiveValidation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "GRN record not found");
            }


            #endregion
           
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (goodReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.GoodReceiveId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                       // db.SaveChanges();
                      
                        var store = db.InventoryStores.Find(record.StoreCode);
                        foreach (var i in record.GoodReceiveItem)
                        {
                            if (i.Quantity > 0)
                            {
                                #region update stock avg price and total
                                
                               
                                var inventoryItem = db.InventoryItems.Find(i.ItemCode);

                                var branchItem =
                                    db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                        .FirstOrDefault(x => x.BranchId == store.BranchId);
                                var storeItem =
                                    db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                        .FirstOrDefault(x => x.StoreCode == store.StoreCode);
                                var priceIndex =
                                    db.ItemPriceIndexes.Where(x => x.SourceType == SourceType.GRN)
                                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);

                                if (inventoryItem != null)
                                {
                                    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                    {
                                        inventoryItem.AvgPrice = ((inventoryItem.Balance * inventoryItem.AvgPrice) +
                                                                  (i.Quantity * i.UnitPrice))
                                                                 / (inventoryItem.Balance + i.Quantity);
                                    }

                                    inventoryItem.Balance = inventoryItem.Balance + i.Quantity;
                                    db.Entry(inventoryItem).State = EntityState.Modified;
                                    //db.SaveChanges();

                                    if (priceIndex != null)
                                    {
                                        priceIndex.PriceBeforeVAT = i.UnitPrice;
                                        priceIndex.SupplierName = record.SupplierName;
                                        db.Entry(priceIndex).State = EntityState.Modified;
                                       // db.SaveChanges();
                                    }

                                    if (priceIndex == null)
                                    {
                                        var priceindex = new ItemPriceIndex
                                        {
                                            BranchId=ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                                            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                            ItemCode = inventoryItem.ItemCode,
                                            ItemName= inventoryItem.Name,
                                            PriceBeforeVAT = i.UnitPrice,
                                            SourceOfData = "GRN",
                                            GLPeriodId=record.GLPeriodId,
                                            SupplierName = record.SupplierName,
                                            DateOfInformation = i.GoodReceive.ReceiveDate,
                                            SourceType = SourceType.GRN
                                        };
                                        db.ItemPriceIndexes.Add(priceindex);
                                       // db.SaveChanges();
                                    }

                                    if (branchItem != null)
                                    {
                                        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                        {
                                            branchItem.AvgPrice = ((branchItem.Balance * branchItem.AvgPrice) +
                                                                   (i.Quantity * i.UnitPrice))
                                                                  / (branchItem.Balance + i.Quantity);
                                        }

                                        branchItem.Balance = branchItem.Balance + i.Quantity;
                                        db.Entry(branchItem).State = EntityState.Modified;
                                       // db.SaveChanges();
                                    }
                                    else
                                    {
                                        branchItem = new BranchItemAssignment();
                                        if (store != null) branchItem.BranchId = store.BranchId;
                                        branchItem.ItemCategoryCode = inventoryItem.ItemCategoryCode;
                                        branchItem.ItemCode = inventoryItem.ItemCode;
                                        branchItem.Balance = i.Quantity;
                                        branchItem.AvgPrice = i.UnitPrice;
                                        db.BranchItemAssignments.Add(branchItem);
                                       // db.SaveChanges();
                                    }

                                    if (storeItem != null)
                                    {
                                        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                        {
                                            storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) +
                                                                  (i.Quantity * i.UnitPrice))
                                                                 / (storeItem.Balance + i.Quantity);
                                        }

                                        storeItem.Balance = storeItem.Balance + i.Quantity;
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        //db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = storeItem,
                                            ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                            Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef:record.Reference,
                                            Date = record.ReceiveDate,
                                            ReceivedQTY = i.Quantity,
                                            ReceivedUPrice = i.UnitPrice,
                                            ReceivedTPrice = i.Quantity* i.UnitPrice,//i.Total,
                                            BalanceQTY = storeItem.Balance,
                                            SupplierName=record.SupplierName,
                                            //new    
                                            StoreCode = record.StoreCode,
                                            GLPeriodId = record.GLPeriodId,
                                            TransactionType = BinCardTransactionTypes.Receive,
                                            StatusType = BinCardTransactionStatus.Regular,
                                        };

                                        #region bin card average price

                                        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                        {
                                            binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                            binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                        }

                                        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                        {
                                            binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                            binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                        }
                                        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                        {
                                            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                        }


                                        #endregion

                                        db.BinCardLines.Add(binCardLine);
                                       // db.SaveChanges();

                                        #endregion
                                    }
                                    else
                                    {
                                        storeItem = new StoreItemAssignment
                                        {
                                            StoreCode = record.StoreCode,
                                            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                            ItemCode = inventoryItem.ItemCode,
                                            Balance = i.Quantity,
                                            AvgPrice = i.UnitPrice,
                                        };
                                        db.StoreItemAssignments.Add(storeItem);
                                        //db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = storeItem,
                                            ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                            Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                            Date = record.ReceiveDate,
                                            ReceivedQTY = i.Quantity,
                                            ReceivedUPrice = i.UnitPrice,
                                            ReceivedTPrice = i.Quantity * i.UnitPrice,//i.Total,
                                            BalanceQTY = storeItem.Balance,
                                            SupplierName = record.SupplierName,

                                            //new    
                                            StoreCode = record.StoreCode,
                                            GLPeriodId = record.GLPeriodId,
                                            TransactionType = BinCardTransactionTypes.Receive,
                                            StatusType = BinCardTransactionStatus.Regular,

                                        };

                                        #region bin card average price

                                        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                        {
                                            binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                            binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                        }

                                        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                        {
                                            binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                            binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                        }
                                        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                        {
                                            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                        }


                                        #endregion

                                        db.BinCardLines.Add(binCardLine);
                                       // db.SaveChanges();

                                        #endregion
                                    }
                                }

                                #endregion
                            }
                        }
                       

                    }
                    if (goodReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        //db.SaveChanges();
                    }

                    var entity = new GoodReceiveValidation
                    {
                        GoodReceiveId = id,
                        Status = goodReceiveValidation.Status,
                        Remark = goodReceiveValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = goodReceiveValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = goodReceiveValidation.ModifiedBy
                    };

                    db.GoodReceiveValidations.Add(entity);
                    db.SaveChanges();
                    goodReceiveValidation.GoodReceiveValidationID = entity.GoodReceiveValidationID;


                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good receive approval", "Success", User.Identity.Name, $"approved good receive {entity.GoodReceiveId} ", Modules.Inventory);
                    #endregion
                }

            }
            

        return Json(new[] { goodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, GoodReceiveValidation goodReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveValidation
                {
                    GoodReceiveValidationID = goodReceiveValidation.GoodReceiveValidationID,
                    Status = goodReceiveValidation.Status,
                    Remark = goodReceiveValidation.Remark,
                    Approver = User.Identity.Name,
                    OrgBranchName = goodReceiveValidation.OrgBranchName,
                };

                db.GoodReceiveValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { goodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, GoodReceiveValidation goodReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveValidation
                {
                    GoodReceiveValidationID = goodReceiveValidation.GoodReceiveValidationID,
                    Status = goodReceiveValidation.Status,
                    Remark = goodReceiveValidation.Remark,
                    OrgBranchName = goodReceiveValidation.OrgBranchName,
                };

                db.GoodReceiveValidations.Attach(entity);
                db.GoodReceiveValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { goodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
