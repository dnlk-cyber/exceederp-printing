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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Adjustment
{
    
    public class StockAdjustmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
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

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentValidations_Create([DataSourceRequest]DataSourceRequest request, StockAdjustmentValidation stockAdjustmentValidation , int id)
            {
                 var stockAdjustment = db.StockAdjustments.Find(id);

                if (stockAdjustment != null && stockAdjustment.IsOnlineApproved)
                {
                    ModelState.AddModelError("already", "You Can not Manipulate This Record : It is Already Approved");
                }
               
                if (ModelState.IsValid)
                    {
                        if (stockAdjustment != null)
                        {
                            if (stockAdjustmentValidation.Status == ApprovalStatuses.Approve)
                            {
                                stockAdjustment.Status = Enum.GetName(typeof(OperationStatuses), 1);
                                stockAdjustment.IsOnlineApproved = true;
                                stockAdjustment.OnlineApprovedBy = User.Identity.Name;
                                stockAdjustment.OnlineApprovedTime = DateTime.Now.Date;
                                foreach (var adjustedItem in stockAdjustment.StockAdjustmentItems)
                                {
                                    var item = db.InventoryItems.Find(adjustedItem.ItemCode);
                                    var storeItem =db.StoreItemAssignments.FirstOrDefault(
                                            it => it.ItemCode == adjustedItem.ItemCode &&
                                                  it.StoreCode == stockAdjustment.StoreCode);
                                    if (adjustedItem.StockAdjustmentType == "Shortage")
                                    {
                                        if (item != null) item.Balance -= adjustedItem.AdjustedQuantity;
                                        if (storeItem != null) storeItem.Balance -= adjustedItem.AdjustedQuantity;
                                    }
                                    else if (adjustedItem.StockAdjustmentType == "Overage")
                                    {
                                        if (item != null) item.Balance += adjustedItem.AdjustedQuantity;
                                        if (storeItem != null) storeItem.Balance += adjustedItem.AdjustedQuantity;
                                    }
                                    if (adjustedItem.PriceAdjustmentType == " Income")
                                    {
                                        if (item != null) item.AvgPrice += adjustedItem.AdjustedPrice;
                                    }
                                    else if (adjustedItem.PriceAdjustmentType == "Loss")
                                    {
                                        if (item != null) item.AvgPrice -= adjustedItem.AdjustedPrice;
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
                                stockAdjustment.Status = Enum.GetName(typeof(OperationStatuses), 8);
                                stockAdjustment.IsSendForApproval = false;
                            }
                            db.StockAdjustments.Attach(stockAdjustment);
                            db.Entry(stockAdjustment).State = EntityState.Modified;
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

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
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

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
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
