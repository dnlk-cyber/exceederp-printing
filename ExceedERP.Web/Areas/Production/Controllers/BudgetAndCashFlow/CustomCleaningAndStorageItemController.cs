using ExceedERP.Core.Domain.Finance.GL;
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
    public class CustomCleaningAndStorageItemController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CustomCleaningAndStorageItemController()
        {
            db = new ExceedDbContext();
        }
       
        public ActionResult Items_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CustomCleaningAndStorageItem> sales = db.CustomCleaningAndStorageItems.Where(i => i.CustomCleaningAndStorageId == id);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new 
            {
                CustomCleaningAndStorageItemId = sale.CustomCleaningAndStorageItemId,
                CustomCleaningAndStorageId = sale.CustomCleaningAndStorageId,
                ItemCode = sale.ItemCode,
                ExternalQuantity = sale.ExternalQuantity,
                OnFarmQuantity = sale.OnFarmQuantity,
                TotalQuantity = sale.TotalQuantity,
                UnitPriceExternal = sale.UnitPriceExternal,
                UnitPriceOnFarm = sale.UnitPriceOnFarm,
                ExternalAmount = sale.ExternalAmount,
                OnFarmAmount = sale.OnFarmAmount,
                TotalAmount = sale.TotalAmount,                

                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(CustomCleaningAndStorageItem record, int headerId)
        {
            return db.CustomCleaningAndStorageItems
                .FirstOrDefault(i =>
                    i.CustomCleaningAndStorageId == headerId &&
                    i.ItemCode == record.ItemCode &&
                    i.CustomCleaningAndStorageItemId != record.CustomCleaningAndStorageItemId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Create([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItem sale, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, id))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorageItem
                    {
                        CustomCleaningAndStorageItemId = sale.CustomCleaningAndStorageItemId,
                        CustomCleaningAndStorageId = id,
                        ItemCode = sale.ItemCode,
                        ExternalQuantity = sale.ExternalQuantity,
                        OnFarmQuantity = sale.OnFarmQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalAmount = sale.ExternalAmount,
                        OnFarmAmount = sale.OnFarmAmount,
                        TotalAmount = sale.TotalAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };
                    entity.TotalQuantity = entity.ExternalQuantity + entity.OnFarmQuantity;

                    entity.ExternalAmount = entity.ExternalQuantity * entity.UnitPriceExternal;
                    entity.OnFarmAmount = entity.OnFarmQuantity * entity.UnitPriceOnFarm;

                    entity.TotalAmount = entity.OnFarmAmount + entity.ExternalAmount;

                    db.CustomCleaningAndStorageItems.Add(entity);
                    db.SaveChanges();

                    sale.CustomCleaningAndStorageItemId = entity.CustomCleaningAndStorageItemId;

                    this.CreateMonthlyItems(entity);
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Update([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItem sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale, sale.CustomCleaningAndStorageId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorageItem
                    {
                        CustomCleaningAndStorageItemId = sale.CustomCleaningAndStorageItemId,
                        CustomCleaningAndStorageId = sale.CustomCleaningAndStorageId,
                        ItemCode = sale.ItemCode,
                        ExternalQuantity = sale.ExternalQuantity,
                        OnFarmQuantity = sale.OnFarmQuantity,
                        TotalQuantity = sale.TotalQuantity,
                        UnitPriceExternal = sale.UnitPriceExternal,
                        UnitPriceOnFarm = sale.UnitPriceOnFarm,
                        ExternalAmount = sale.ExternalAmount,
                        OnFarmAmount = sale.OnFarmAmount,
                        TotalAmount = sale.TotalAmount,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalQuantity = entity.ExternalQuantity + entity.OnFarmQuantity;

                    entity.ExternalAmount = entity.ExternalQuantity * entity.UnitPriceExternal;
                    entity.OnFarmAmount = entity.OnFarmQuantity * entity.UnitPriceOnFarm;

                    entity.TotalAmount = entity.OnFarmAmount + entity.ExternalAmount;

                    db.CustomCleaningAndStorageItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Destroy([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItem sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomCleaningAndStorageItem
                {
                    CustomCleaningAndStorageItemId = sale.CustomCleaningAndStorageItemId,
                    CustomCleaningAndStorageId = sale.CustomCleaningAndStorageId,
                    ItemCode = sale.ItemCode,
                    ExternalQuantity = sale.ExternalQuantity,
                    OnFarmQuantity = sale.OnFarmQuantity,
                    TotalQuantity = sale.TotalQuantity,
                    UnitPriceExternal = sale.UnitPriceExternal,
                    UnitPriceOnFarm = sale.UnitPriceOnFarm,
                    ExternalAmount = sale.ExternalAmount,
                    OnFarmAmount = sale.OnFarmAmount,
                    TotalAmount = sale.TotalAmount,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.CustomCleaningAndStorageItems.Attach(entity);
                db.CustomCleaningAndStorageItems.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        private void CreateMonthlyItems(CustomCleaningAndStorageItem item)
        {
            var header = db.CustomCleaningAndStorage.Find(item.CustomCleaningAndStorageId);
            if (header != null)
            {
                var year = db.GLFiscalYears.Find(header.GlFiscalYearId);
            if (year != null)
            {
                var periods = db.ProductionPeriods
                .Where(p => 
                        p.DateFrom >= year.DateFrom &&
                        p.DateTo <= year.DateTo).ToList();

                if (periods.Any())
                {
                    foreach (ProductionPeriod period in periods)
                    {
                        var entity = new CustomCleaningAndStorageItemPerMonth
                        {
                            ProductionPeriodId = period.ProductionPeriodId,
                            CustomCleaningAndStorageItemId = item.CustomCleaningAndStorageItemId
                        };
                        db.CustomCleaningAndStorageItemPerMonth.Add(entity);
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