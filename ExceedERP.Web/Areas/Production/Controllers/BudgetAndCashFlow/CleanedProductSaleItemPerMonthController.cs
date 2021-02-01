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
    public class CleanedProductSaleItemPerMonthController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CleanedProductSaleItemPerMonthController()
        {
            db = new ExceedDbContext();
        }       
        public ActionResult Sales_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            IQueryable<CleanedProductSaleItemPerMonth> sales = db.CleanedProductSaleItemPerMonth.Where(s => s.CleanedProductSaleItemId == itemId);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new 
            {
                CleanedProductSaleItemPerMonthId = sale.CleanedProductSaleItemPerMonthId,
                CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
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
        private bool ItemExists(CleanedProductSaleItemPerMonth record, int headerId)
        {
            return db.CleanedProductSaleItemPerMonth
                .FirstOrDefault(i =>
                    i.CleanedProductSaleItemId == headerId &&
                    i.ProductionPeriodId == record.ProductionPeriodId &&
                    i.CleanedProductSaleItemPerMonthId != record.CleanedProductSaleItemPerMonthId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Create([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItemPerMonth sale, int itemId)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, itemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSaleItemPerMonth
                    {
                        CleanedProductSaleItemPerMonthId = sale.CleanedProductSaleItemPerMonthId,
                        CleanedProductSaleItemId = itemId,
                        ProductionPeriodId = sale.ProductionPeriodId,
                        Amount = sale.Amount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };

                    db.CleanedProductSaleItemPerMonth.Add(entity);
                    db.SaveChanges();

                    sale.CleanedProductSaleItemPerMonthId = entity.CleanedProductSaleItemPerMonthId;

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CleanedProductSaleItemId));
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Update([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItemPerMonth sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, sale.CleanedProductSaleItemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSaleItemPerMonth
                    {
                        CleanedProductSaleItemPerMonthId = sale.CleanedProductSaleItemPerMonthId,
                        CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                        ProductionPeriodId = sale.ProductionPeriodId,
                        Amount = sale.Amount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.CleanedProductSaleItemPerMonth.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CleanedProductSaleItemId));
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Destroy([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItemPerMonth sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedProductSaleItemPerMonth
                {
                    CleanedProductSaleItemPerMonthId = sale.CleanedProductSaleItemPerMonthId,
                    CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                    ProductionPeriodId = sale.ProductionPeriodId,
                    Amount = sale.Amount,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.CleanedProductSaleItemPerMonth.Attach(entity);
                db.CleanedProductSaleItemPerMonth.Remove(entity);
                db.SaveChanges();

                ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CleanedProductSaleItemId));
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }
        private int GetBranchId(int itemId)
        {
            var branchId = -1;
            var item = db.CleanedProductSaleItems.Find(itemId);
            if(item!= null)
            {
                branchId = item.CleanedProductSale.BranchId;
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