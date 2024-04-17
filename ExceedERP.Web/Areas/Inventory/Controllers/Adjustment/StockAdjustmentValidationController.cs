using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Adjustment
{
    
    public class StockAdjustmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover)]
        public ActionResult StockAdjustmentValidations_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StockAdjustmentValidation> stockadjustmentvalidations = db.StockAdjustmentValidations.Where(adj => adj.StockAdjustmentID == id);
            DataSourceResult result = stockadjustmentvalidations.ToDataSourceResult(request, stockAdjustmentValidation => new {
                StockAdjustmentValidationID = stockAdjustmentValidation.StockAdjustmentValidationID,
                Status = stockAdjustmentValidation.Status,
                Remark = stockAdjustmentValidation.Remark,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentValidations_Create([DataSourceRequest]DataSourceRequest request, StockAdjustmentValidation stockAdjustmentValidation , int id)
            {
                 var record = db.StockAdjustments.Find(id);

            #region validation 

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    
                    if (stockAdjustmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion

            if (ModelState.IsValid)
                    {
                        if (record != null)
                        {
                            if (stockAdjustmentValidation.Status == ApprovalStatuses.Approve)
                            {
                                record.Status = Enum.GetName(typeof(OperationStatuses), 1);
                                record.IsOnlineApproved = true;
                                record.OnlineApprovedBy = User.Identity.Name;
                                record.OnlineApprovedTime = DateTime.Now.Date;
                                foreach (var i in record.StockAdjustmentItems)
                                {
                                    var item = db.InventoryItems.Find(i.ItemCode);
                                    var storeItem =db.StoreItemAssignments.FirstOrDefault(
                                            it => it.ItemCode == i.ItemCode &&
                                                  it.StoreCode == record.StoreCode);
                            decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, record.StoreCode);
                            if (i.StockAdjustmentType == AdjustmentType.Shortage)
                                    {
                                        if (item != null) item.Balance -= i.AdjustedQuantity;
                                        if (storeItem != null) storeItem.Balance -= i.AdjustedQuantity;
                                #region bin card line

                                var binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Adjustment_Shortage-" + record.StockAdjustmentID.ToString(),
                                    Reference = record.Remark,
                                    Date = record.AdjustmentDate,
                                    IssuedQTY = i.AdjustedQuantity,
                                    IssuedUPrice = avgPrice,
                                    IssuedTPrice = i.AdjustedQuantity*avgPrice,
                                    BalanceQTY = storeItem.Balance,
                                    BalanceUPrice = avgPrice,
                                    BalanceTPrice = avgPrice * storeItem.Balance,
                                    IssuedTo = "",
                                    //new    
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Adjustment,
                                    StatusType = BinCardTransactionStatus.Regular,
                                    TransactionOrder=BinCardTransactionOrder.Adjustment

                                };
                                db.BinCardLines.Add(binCardLine);
                                // db.SaveChanges();

                                #endregion
                                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.AdjustmentDate));
                            }
                            else if (i.StockAdjustmentType == AdjustmentType.Overage)
                                    {
                                        if (item != null) item.Balance += i.AdjustedQuantity;
                                        if (storeItem != null) storeItem.Balance += i.AdjustedQuantity;
                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Adjustment_Shortage-" + record.StockAdjustmentID.ToString(),
                                    Reference =  record.Reference,
                                    Date = record.AdjustmentDate,
                                    ReceivedQTY = i.AdjustedQuantity,
                                    ReceivedUPrice = avgPrice,
                                    ReceivedTPrice = i.AdjustedQuantity * avgPrice,//i.Total,
                                    BalanceQTY = storeItem.Balance,
                                    BalanceUPrice = avgPrice,
                                    BalanceTPrice=avgPrice* storeItem.Balance,
                                    SupplierName = "",
                                    //new    
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Adjustment,
                                    StatusType = BinCardTransactionStatus.Regular,
                                };

                                

                                db.BinCardLines.Add(binCardLine);
                                // db.SaveChanges();

                                #endregion
                                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.AdjustmentDate));
                            }
                            if (i.PriceAdjustmentType == " Income")
                                    {
                                        if (item != null) item.AvgPrice += i.AdjustedPrice;
                                    }
                                    else if (i.PriceAdjustmentType == "Loss")
                                    {
                                        if (item != null) item.AvgPrice -= i.AdjustedPrice;
                                    }
                                    db.InventoryItems.Attach(item);
                                    db.Entry(item).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    db.StoreItemAssignments.Attach(storeItem);
                                    db.Entry(storeItem).State = EntityState.Modified;
                                    //db.SaveChanges();
                                }
                            }
                            else if (stockAdjustmentValidation.Status == ApprovalStatuses.Reject)
                            {
                                record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                                record.IsSendForApproval = false;
                            }
                            db.StockAdjustments.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                           // db.SaveChanges();
                            var entity = new StockAdjustmentValidation
                            {
                                Status = stockAdjustmentValidation.Status,
                                StockAdjustmentID = id,
                                Remark = stockAdjustmentValidation.Remark,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };

                            db.StockAdjustmentValidations.Add(entity);
                            db.SaveChanges();
                            stockAdjustmentValidation.StockAdjustmentValidationID = entity.StockAdjustmentValidationID;
                  
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment approval", "Success", User.Identity.Name, $"approved stock adjustment {entity.StockAdjustmentID} ", Modules.Inventory);
                        #endregion
                }

            }
                
              
            return Json(new[] { stockAdjustmentValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentValidations_Update([DataSourceRequest]DataSourceRequest request, StockAdjustmentValidation stockAdjustmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentValidation
                {
                    StockAdjustmentValidationID = stockAdjustmentValidation.StockAdjustmentValidationID,
                    Status = stockAdjustmentValidation.Status,
                    Remark = stockAdjustmentValidation.Remark,
                };

                db.StockAdjustmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { stockAdjustmentValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, StockAdjustmentValidation stockAdjustmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentValidation
                {
                    StockAdjustmentValidationID = stockAdjustmentValidation.StockAdjustmentValidationID,
                    Status = stockAdjustmentValidation.Status,
                    Remark = stockAdjustmentValidation.Remark,
                };

                db.StockAdjustmentValidations.Attach(entity);
                db.StockAdjustmentValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { stockAdjustmentValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
