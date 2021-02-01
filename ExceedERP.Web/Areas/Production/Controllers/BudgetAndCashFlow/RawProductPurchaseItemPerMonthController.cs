using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class RawProductPurchaseItemPerMonthController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public RawProductPurchaseItemPerMonthController()
        {
            db = new ExceedDbContext();
        }       
        public ActionResult Purchases_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            IQueryable<RawProductPurchaseItemPerMonth> sales = db.RawProductPurchaseItemPerMonth.Where(s => s.RawProductPurchaseItemId == itemId);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new 
            {
                RawProductPurchaseItemPerMonthId = sale.RawProductPurchaseItemPerMonthId,
                RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                ProductionPeriodId = sale.ProductionPeriodId,
                Amount = sale.Amount,                

                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(RawProductPurchaseItemPerMonth record, int headerId)
        {
            return db.RawProductPurchaseItemPerMonth
                .FirstOrDefault(i =>
                    i.RawProductPurchaseItemId == headerId &&
                    i.ProductionPeriodId == record.ProductionPeriodId &&
                    i.RawProductPurchaseItemPerMonthId != record.RawProductPurchaseItemPerMonthId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Create([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItemPerMonth sale, int itemId)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, itemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchaseItemPerMonth
                    {
                        RawProductPurchaseItemPerMonthId = sale.RawProductPurchaseItemPerMonthId,
                        RawProductPurchaseItemId = itemId,
                        ProductionPeriodId = sale.ProductionPeriodId,
                        Amount = sale.Amount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };

                    db.RawProductPurchaseItemPerMonth.Add(entity);
                    db.SaveChanges();

                    sale.RawProductPurchaseItemPerMonthId = entity.RawProductPurchaseItemPerMonthId;

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.RawProductPurchaseItemId));
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Update([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItemPerMonth sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, sale.RawProductPurchaseItemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchaseItemPerMonth
                    {
                        RawProductPurchaseItemPerMonthId = sale.RawProductPurchaseItemPerMonthId,
                        RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                        ProductionPeriodId = sale.ProductionPeriodId,
                        Amount = sale.Amount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.RawProductPurchaseItemPerMonth.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.RawProductPurchaseItemId));
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Destroy([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItemPerMonth sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductPurchaseItemPerMonth
                {
                    RawProductPurchaseItemPerMonthId = sale.RawProductPurchaseItemPerMonthId,
                    RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                    ProductionPeriodId = sale.ProductionPeriodId,
                    Amount = sale.Amount,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.RawProductPurchaseItemPerMonth.Attach(entity);
                db.RawProductPurchaseItemPerMonth.Remove(entity);
                db.SaveChanges();

                ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.RawProductPurchaseItemId));
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }
        private int GetBranchId(int itemId)
        {
            var branchId = -1;
            var item = db.RawProductPurchaseItems.Find(itemId);
            if (item != null)
            {
                branchId = item.RawProductPurchase.BranchId;
            }
            return branchId;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}