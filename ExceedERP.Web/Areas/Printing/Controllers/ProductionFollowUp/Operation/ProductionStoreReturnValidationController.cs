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

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreReturnValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

      //  [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

       // [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StoreReturnValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreReturnValidation> storereturnvalidations = db.StoreReturnValidations.Where(sr => sr.StoreReturnID == id);
            DataSourceResult result = storereturnvalidations.ToDataSourceResult(request, storeReturnValidation => new
            {
                StoreReturnValidationID = storeReturnValidation.StoreReturnValidationID,
                Status = storeReturnValidation.Status,
                Remark = storeReturnValidation.Remark,
            });

            return Json(result);
        }

       // [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnValidations_Create([DataSourceRequest]DataSourceRequest request, StoreReturnValidation storeReturnValidation, int id)
        {
            var record = db.StoreReturns.Find(id);
            var storeItemAssignment = db.StoreItemAssignments.Where(st => st.StoreCode == record.StoreCode);

            if (record != null && record.IsOnlineApproved)
            {
                string message = "You cannot manipulate this record,it  is already approved";
                ModelState.AddModelError("approved", message);
            }
            if (record != null && record.StoreReturnItems.Count == 0)
            {
                string message = "The Record Does not Contain Any Item";
                ModelState.AddModelError(String.Empty, message);
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (storeReturnValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.StoreReturns.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        //db.SaveChanges();

                        var store = db.InventoryStores.Find(record.StoreCode);
                        foreach (var i in record.StoreReturnItems)
                        {
                            if (i.Quantity > 0)
                            {
                                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, record.StoreCode);
                                if (avgPrice == 0)
                                    avgPrice = i.UnitPrice;

                                #region Store Item Balance

                                var storeItem = storeItemAssignment.FirstOrDefault(it => it.ItemCode == i.ItemCode);
                                if (storeItem != null)
                                {
                                    storeItem.Balance = storeItem.Balance + i.Quantity;
                                    db.StoreItemAssignments.Attach(storeItem);
                                    db.Entry(storeItem).State = EntityState.Modified;
                                    // db.SaveChanges();

                                    #region bin card line

                                    BinCardLine binCardLine = new BinCardLine
                                    {
                                        StoreItemAssignment = storeItem,
                                        ReferenceDoc = "Return_" + record.StoreReturnID.ToString(),
                                        Reference = record.Remark,
                                        Date = record.ReturnDate,
                                        ReceivedQTY = i.Quantity,
                                        ReceivedUPrice = avgPrice,
                                        ReceivedTPrice = avgPrice * i.Quantity,
                                        BalanceQTY = storeItem.Balance,
                                        BalanceUPrice = avgPrice,
                                        BalanceTPrice = avgPrice * storeItem.Balance,

                                        //new    
                                        StoreCode = record.StoreCode,
                                        GLPeriodId = record.GLPeriodId,
                                        TransactionType = BinCardTransactionTypes.Return,
                                        StatusType = BinCardTransactionStatus.Regular,
                                    };
                                    //price if manual
                                    db.BinCardLines.Add(binCardLine);
                                    // db.SaveChanges();

                                    #endregion
                                }
                                else
                                {
                                    storeItem = new StoreItemAssignment
                                    {
                                        StoreCode = record.StoreCode,
                                        ItemCategoryCode = i.ItemCategoryCode,
                                        ItemCode = i.ItemCode,
                                        Balance = i.Quantity,
                                        AvgPrice = avgPrice,
                                    };
                                    db.StoreItemAssignments.Add(storeItem);
                                    //db.SaveChanges();

                                    #region bin card line

                                    BinCardLine binCardLine = new BinCardLine
                                    {
                                        StoreItemAssignment = storeItem,
                                        ReferenceDoc = "Return_" + record.StoreReturnID.ToString(),
                                        Reference = record.Remark,
                                        Date = record.ReturnDate,
                                        ReceivedQTY = i.Quantity,
                                        ReceivedUPrice = avgPrice,
                                        ReceivedTPrice = avgPrice * i.Quantity,
                                        BalanceQTY = storeItem.Balance,
                                        BalanceUPrice = avgPrice,
                                        BalanceTPrice = avgPrice * storeItem.Balance,
                                        //new    
                                        StoreCode = record.StoreCode,
                                        GLPeriodId = record.GLPeriodId,
                                        TransactionType = BinCardTransactionTypes.Return,
                                        StatusType = BinCardTransactionStatus.Regular,
                                    };
                                    //price if manual
                                    db.BinCardLines.Add(binCardLine);
                                    //db.SaveChanges();

                                    #endregion
                                }

                                #endregion

                                #region  branch Balance

                                var storeBranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode);
                                var branchItemAssignment = db.BranchItemAssignments.Where(x => x.BranchId == storeBranch)
                                    .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                                if (branchItemAssignment != null)
                                {
                                    branchItemAssignment.Balance = branchItemAssignment.Balance + i.Quantity;
                                    db.Entry(branchItemAssignment).State = EntityState.Modified;
                                    // db.SaveChanges();
                                }
                                else
                                {
                                    var branchItem = new BranchItemAssignment();
                                    if (store != null) branchItem.BranchId = store.BranchId;
                                    branchItem.ItemCategoryCode = i.ItemCategoryCode;
                                    branchItem.ItemCode = i.ItemCode;
                                    branchItem.Balance = i.Quantity;
                                    branchItem.AvgPrice = i.UnitPrice;
                                    db.BranchItemAssignments.Add(branchItem);
                                    //db.SaveChanges();
                                }

                                #endregion

                                #region  Item Balance

                                var inventoryItem = db.InventoryItems.Find(i.ItemCode);
                                if (inventoryItem != null)
                                {
                                    inventoryItem.Balance += i.Quantity;
                                    db.Entry(inventoryItem).State = EntityState.Modified;
                                }

                                #endregion
                            }
                        }

                    }
                    else if (storeReturnValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.StoreReturns.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        // db.SaveChanges();
                    }


                    var entity = new StoreReturnValidation
                    {
                        StoreReturnID = id,
                        Status = storeReturnValidation.Status,
                        Remark = storeReturnValidation.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };
                    db.StoreReturnValidations.Add(entity);
                    db.SaveChanges();
                    storeReturnValidation.StoreReturnValidationID = entity.StoreReturnValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store return approval", "Success", User.Identity.Name, $"approved store return {entity.StoreReturnID} ", Modules.Inventory);
                    #endregion
                }

            }
            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnValidations_Update([DataSourceRequest]DataSourceRequest request, StoreReturnValidation storeReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnValidation
                {
                    StoreReturnValidationID = storeReturnValidation.StoreReturnValidationID,
                    Status = storeReturnValidation.Status,
                    Remark = storeReturnValidation.Remark,
                };

                db.StoreReturnValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnValidations_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturnValidation storeReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnValidation
                {
                    StoreReturnValidationID = storeReturnValidation.StoreReturnValidationID,
                    Status = storeReturnValidation.Status,
                    Remark = storeReturnValidation.Remark,
                };

                db.StoreReturnValidations.Attach(entity);
                db.StoreReturnValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
