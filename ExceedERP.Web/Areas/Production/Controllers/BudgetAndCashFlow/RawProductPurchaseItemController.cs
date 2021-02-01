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
    public class RawProductPurchaseItemController : Controller
    {
        // GET: Production/Crop
        private ExceedDbContext db;
        public RawProductPurchaseItemController()
        {
            db = new ExceedDbContext();
        }

        public ActionResult Purchases_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<RawProductPurchaseItem> sales = db.RawProductPurchaseItems.Where(i => i.RawProductPurchaseId == id);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new
            {
                RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                RawProductPurchaseId = sale.RawProductPurchaseId,
                ItemCode = sale.ItemCode,
                ExternalPurchaseQuantity = sale.ExternalPurchaseQuantity,
                OnFarmPurchaseQuantity = sale.OnFarmPurchaseQuantity,
                TotalQuantity = sale.TotalQuantity,
                UnitPriceExternal = sale.UnitPriceExternal,
                UnitPriceOnFarm = sale.UnitPriceOnFarm,
                ExternalPurchaseAmount = sale.ExternalPurchaseAmount,
                OnFarmPurchaseAmount = sale.OnFarmPurchaseAmount,
                TotalPurchaseAmount = sale.TotalPurchaseAmount,

                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(RawProductPurchaseItem record, int headerId)
        {
            return db.RawProductPurchaseItems
                .FirstOrDefault(i =>
                    i.RawProductPurchaseId == headerId &&
                    i.ItemCode == record.ItemCode &&
                    i.RawProductPurchaseItemId != record.RawProductPurchaseItemId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Create([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItem sale, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, id))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchaseItem
                    {
                        RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                        RawProductPurchaseId = id,
                        ItemCode = sale.ItemCode,
                        ExternalPurchaseQuantity = sale.ExternalPurchaseQuantity,
                        OnFarmPurchaseQuantity = sale.OnFarmPurchaseQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalPurchaseAmount = sale.ExternalPurchaseAmount,
                        OnFarmPurchaseAmount = sale.OnFarmPurchaseAmount,
                        TotalPurchaseAmount = sale.TotalPurchaseAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };
                    entity.TotalQuantity = entity.ExternalPurchaseQuantity + entity.OnFarmPurchaseQuantity;

                    entity.ExternalPurchaseAmount = entity.ExternalPurchaseQuantity * entity.UnitPriceExternal;
                    entity.OnFarmPurchaseAmount = entity.OnFarmPurchaseQuantity * entity.UnitPriceOnFarm;

                    entity.TotalPurchaseAmount = entity.OnFarmPurchaseAmount + entity.ExternalPurchaseAmount;

                    db.RawProductPurchaseItems.Add(entity);
                    db.SaveChanges();

                    sale.RawProductPurchaseItemId = entity.RawProductPurchaseItemId;

                    this.CreateMonthlyItems(entity);
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Update([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItem sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, sale.RawProductPurchaseId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchaseItem
                    {
                        RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                        RawProductPurchaseId = sale.RawProductPurchaseId,
                        ItemCode = sale.ItemCode,
                        ExternalPurchaseQuantity = sale.ExternalPurchaseQuantity,
                        OnFarmPurchaseQuantity = sale.OnFarmPurchaseQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalPurchaseAmount = sale.ExternalPurchaseAmount,
                        OnFarmPurchaseAmount = sale.OnFarmPurchaseAmount,
                        TotalPurchaseAmount = sale.TotalPurchaseAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalQuantity = entity.ExternalPurchaseQuantity + entity.OnFarmPurchaseQuantity;

                    entity.ExternalPurchaseAmount = entity.ExternalPurchaseQuantity * entity.UnitPriceExternal;
                    entity.OnFarmPurchaseAmount = entity.OnFarmPurchaseQuantity * entity.UnitPriceOnFarm;

                    entity.TotalPurchaseAmount = entity.OnFarmPurchaseAmount + entity.ExternalPurchaseAmount;

                    db.RawProductPurchaseItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Destroy([DataSourceRequest]DataSourceRequest request, RawProductPurchaseItem sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductPurchaseItem
                {
                    RawProductPurchaseItemId = sale.RawProductPurchaseItemId,
                    RawProductPurchaseId = sale.RawProductPurchaseId,
                    ItemCode = sale.ItemCode,
                    ExternalPurchaseQuantity = sale.ExternalPurchaseQuantity,
                    OnFarmPurchaseQuantity = sale.OnFarmPurchaseQuantity,
                    TotalQuantity = sale.TotalQuantity,
                    UnitPriceExternal = sale.UnitPriceExternal,
                    UnitPriceOnFarm = sale.UnitPriceOnFarm,
                    ExternalPurchaseAmount = sale.ExternalPurchaseAmount,
                    OnFarmPurchaseAmount = sale.OnFarmPurchaseAmount,
                    TotalPurchaseAmount = sale.TotalPurchaseAmount,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.RawProductPurchaseItems.Attach(entity);
                db.RawProductPurchaseItems.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        private void CreateMonthlyItems(RawProductPurchaseItem item)
        {
            var header = db.RawProductPurchases.Find(item.RawProductPurchaseId);
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
                            var entity = new RawProductPurchaseItemPerMonth
                            {
                                ProductionPeriodId = period.ProductionPeriodId,
                                RawProductPurchaseItemId = item.RawProductPurchaseItemId
                            };
                            db.RawProductPurchaseItemPerMonth.Add(entity);
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