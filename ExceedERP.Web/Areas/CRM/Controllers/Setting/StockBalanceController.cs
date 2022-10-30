using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin )] 
    public class StockBalanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();


        }

        public ActionResult StockBalances_Read([DataSourceRequest]DataSourceRequest request, int thingID,string stockBalanceThingType)
        {

            List<StoreItemAssignment> stockbalances = db.StoreItemAssignments.Where(sb => sb.ItemCode == thingID.ToString()).ToList();
            
           
            DataSourceResult result = stockbalances.ToDataSourceResult(request, stockBalance => new {
                   StockBalanceID = stockBalance.StoreItemAssignmentID,
                StockBalanceType = stockBalance.InventoryItem.UoMName,
                Thing = stockBalance.InventoryItem.Name,
                   StoreID = stockBalance.InventoryStore.StoreCode,
                Quantity = stockBalance.Balance,
                CreatedBy = stockBalance.CreatedBy,
                DateCreated = stockBalance.DateCreated,
                LastModified = stockBalance.LastModified,
                ModifiedBy = stockBalance.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockBalances_Create([DataSourceRequest]DataSourceRequest request, StockBalance stockBalance, int thingID, string stockBalanceThingType)
        {
            List<StockBalance> stockBalances = db.StockBalances.Where(s => s.StockBalanceType == stockBalance.StockBalanceType && s.Thing == stockBalance.Thing).ToList();
            List<StockBalance> stockstore = db.StockBalances.Where(s => s.StoreID == stockBalance.StoreID && s.Thing == stockBalance.Thing).ToList();
            if (stockBalances.Any())
              {
                  ModelState.AddModelError("DuplicateEntrie", "Stock Balance Type Already Exist");
              }

            else if (stockstore.Any())
              {
                  ModelState.AddModelError("DuplicateEntrie", "Store Already Assigned Stock Balance");
              }
              else
              {
                  if (ModelState.IsValid)
                  {
                      var entity = new StockBalance                                                                                                 
                      {
                          StockBalanceType = stockBalance.StockBalanceType,
                          Thing = thingID.ToString(),
                          ThingType = stockBalanceThingType,
                          Variety = stockBalance.Variety,
                          StoreID = stockBalance.StoreID,
                          Quantity = stockBalance.Quantity,
                          Period = stockBalance.Period,
                          Class = stockBalance.Class,
                          CreatedBy = User.Identity.Name,
                          DateCreated = DateTime.Now,
                          Remark = stockBalance.Remark
                      };

                      db.StockBalances.Add(entity);
                      db.SaveChanges();
                      stockBalance.StockBalanceID = entity.StockBalanceID;
                  }
              }
            return Json(new[] { stockBalance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockBalances_Update([DataSourceRequest]DataSourceRequest request, StockBalance stockBalance)
        {
            List<StockBalance> stockBalances = db.StockBalances.Where(s => s.StockBalanceType == stockBalance.StockBalanceType && s.StockBalanceID != stockBalance.StockBalanceID).ToList();

            List<StockBalance> stockstore = db.StockBalances.Where(s => s.StoreID == stockBalance.StoreID && s.StockBalanceID != stockBalance.StockBalanceID && s.Thing==stockBalance.Thing).ToList();
          
            if (stockBalances.Any())
                {
                    ModelState.AddModelError("DuplicateEntrie", "Stock Balance Type Already Exist");
                }
            else if (stockstore.Any())
                {
                    ModelState.AddModelError("DuplicateEntrie", "Store Already Exist");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var entity = new StockBalance
                        {
                            StockBalanceID = stockBalance.StockBalanceID,
                            StockBalanceType = stockBalance.StockBalanceType,
                            ThingType = stockBalance.ThingType,
                            Thing = stockBalance.Thing,
                            StoreID = stockBalance.StoreID,
                            Quantity = stockBalance.Quantity,
                            Period = stockBalance.Period,
                            Variety = stockBalance.Variety,
                            Class = stockBalance.Class,
                            Remark = stockBalance.Remark
                        };

                        db.StockBalances.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            return Json(new[] { stockBalance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockBalances_Destroy([DataSourceRequest]DataSourceRequest request, StockBalance stockBalance)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockBalance
                {
                    StockBalanceID = stockBalance.StockBalanceID,
                    StockBalanceType = stockBalance.StockBalanceType,
                    Thing = stockBalance.Thing,
                    ThingType = stockBalance.ThingType,
                    StoreID = stockBalance.StoreID,
                    Quantity = stockBalance.Quantity,
                    Period = stockBalance.Period,
                    Variety = stockBalance.Variety,
                    Class = stockBalance.Class,
                    Remark = stockBalance.Remark
                };

                db.StockBalances.Attach(entity);
                db.StockBalances.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Stock Balance number " + stockBalance.StockBalanceID);
            }

            return Json(new[] { stockBalance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
