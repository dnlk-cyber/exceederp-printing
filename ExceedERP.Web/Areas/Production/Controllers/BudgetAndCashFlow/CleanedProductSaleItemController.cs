using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class CleanedProductSaleItemController : Controller
    {
        // GET: Production/Crop
        private ExceedDbContext db;
        public CleanedProductSaleItemController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sales_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CleanedProductSaleItem> sales = db.CleanedProductSaleItems.Where(i => i.CleanedProductSaleId == id);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new
            {
                CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                CleanedProductSaleId = sale.CleanedProductSaleId,
                ItemCode = sale.ItemCode,
                ExternalSaleQuantity = sale.ExternalSaleQuantity,
                OnFarmSaleQuantity = sale.OnFarmSaleQuantity,
                TotalQuantity = sale.TotalQuantity,
                UnitPriceExternal = sale.UnitPriceExternal,
                UnitPriceOnFarm = sale.UnitPriceOnFarm,
                ExternalSaleAmount = sale.ExternalSaleAmount,
                OnFarmSaleAmount = sale.OnFarmSaleAmount,
                TotalSaleAmount = sale.TotalSaleAmount,

                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(CleanedProductSaleItem record, int headerId)
        {
            return db.CleanedProductSaleItems
                .FirstOrDefault(i =>
                    i.CleanedProductSaleId == headerId &&
                    i.ItemCode == record.ItemCode &&
                    i.CleanedProductSaleItemId != record.CleanedProductSaleItemId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Create([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItem sale, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, id))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSaleItem
                    {
                        CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                        CleanedProductSaleId = id,
                        ItemCode = sale.ItemCode,
                        ExternalSaleQuantity = sale.ExternalSaleQuantity,
                        OnFarmSaleQuantity = sale.OnFarmSaleQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalSaleAmount = sale.ExternalSaleAmount,
                        OnFarmSaleAmount = sale.OnFarmSaleAmount,
                        TotalSaleAmount = sale.TotalSaleAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };
                    entity.TotalQuantity = entity.ExternalSaleQuantity + entity.OnFarmSaleQuantity;

                    entity.ExternalSaleAmount = entity.ExternalSaleQuantity * entity.UnitPriceExternal;
                    entity.OnFarmSaleAmount = entity.OnFarmSaleQuantity * entity.UnitPriceOnFarm;

                    entity.TotalSaleAmount = entity.OnFarmSaleAmount + entity.ExternalSaleAmount;

                    db.CleanedProductSaleItems.Add(entity);
                    db.SaveChanges();

                    sale.CleanedProductSaleItemId = entity.CleanedProductSaleItemId;

                    this.CreateMonthlySales(entity);
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Update([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItem sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, sale.CleanedProductSaleId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSaleItem
                    {
                        CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                        CleanedProductSaleId = sale.CleanedProductSaleId,
                        ItemCode = sale.ItemCode,
                        ExternalSaleQuantity = sale.ExternalSaleQuantity,
                        OnFarmSaleQuantity = sale.OnFarmSaleQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalSaleAmount = sale.ExternalSaleAmount,
                        OnFarmSaleAmount = sale.OnFarmSaleAmount,
                        TotalSaleAmount = sale.TotalSaleAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalQuantity = entity.ExternalSaleQuantity + entity.OnFarmSaleQuantity;

                    entity.ExternalSaleAmount = entity.ExternalSaleQuantity * entity.UnitPriceExternal;
                    entity.OnFarmSaleAmount = entity.OnFarmSaleQuantity * entity.UnitPriceOnFarm;

                    entity.TotalSaleAmount = entity.OnFarmSaleAmount + entity.ExternalSaleAmount;

                    db.CleanedProductSaleItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Destroy([DataSourceRequest]DataSourceRequest request, CleanedProductSaleItem sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedProductSaleItem
                {
                    CleanedProductSaleItemId = sale.CleanedProductSaleItemId,
                    CleanedProductSaleId = sale.CleanedProductSaleId,
                    ItemCode = sale.ItemCode,
                    ExternalSaleQuantity = sale.ExternalSaleQuantity,
                    OnFarmSaleQuantity = sale.OnFarmSaleQuantity,
                    TotalQuantity = sale.TotalQuantity,
                    UnitPriceExternal = sale.UnitPriceExternal,
                    UnitPriceOnFarm = sale.UnitPriceOnFarm,
                    ExternalSaleAmount = sale.ExternalSaleAmount,
                    OnFarmSaleAmount = sale.OnFarmSaleAmount,
                    TotalSaleAmount = sale.TotalSaleAmount,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.CleanedProductSaleItems.Attach(entity);
                db.CleanedProductSaleItems.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        private void CreateMonthlySales(CleanedProductSaleItem item)
        {
            var header = db.CleanedProductSales.Find(item.CleanedProductSaleId);
            if (header != null)
            {
                var year = db.GLFiscalYears.Find(header.GlFiscalYearId);
                if (year != null)
                {
                    var periods = db.ProductionPeriods
                    .Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open &&
                            p.DateFrom >= year.DateFrom &&
                            p.DateTo <= year.DateTo).ToList();

                    if (periods.Any())
                    {
                        foreach (ProductionPeriod period in periods)
                        {
                            var entity = new CleanedProductSaleItemPerMonth
                            {
                                ProductionPeriodId = period.ProductionPeriodId,
                                CleanedProductSaleItemId = item.CleanedProductSaleItemId
                            };
                            db.CleanedProductSaleItemPerMonth.Add(entity);
                            db.SaveChanges();
                        }
                    }

                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}