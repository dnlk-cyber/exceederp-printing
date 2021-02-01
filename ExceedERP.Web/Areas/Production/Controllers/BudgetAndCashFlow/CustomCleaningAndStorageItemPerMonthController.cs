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
    public class CustomCleaningAndStorageItemPerMonthController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CustomCleaningAndStorageItemPerMonthController()
        {
            db = new ExceedDbContext();
        }       
        public ActionResult Items_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            IQueryable<CustomCleaningAndStorageItemPerMonth> items = db.CustomCleaningAndStorageItemPerMonth.Where(s => s.CustomCleaningAndStorageItemId == itemId);
            DataSourceResult result = items.ToDataSourceResult(request, item => new 
            {
                CustomCleaningAndStorageItemPerMonthId = item.CustomCleaningAndStorageItemPerMonthId,
                CustomCleaningAndStorageItemId = item.CustomCleaningAndStorageItemId,
                ProductionPeriodId = item.ProductionPeriodId,
                Amount = item.Amount,                

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(CustomCleaningAndStorageItemPerMonth record, int headerId)
        {
            return db.CustomCleaningAndStorageItemPerMonth
                .FirstOrDefault(i =>
                    i.CustomCleaningAndStorageItemId == headerId &&
                    i.ProductionPeriodId == record.ProductionPeriodId &&
                    i.CustomCleaningAndStorageItemPerMonthId != record.CustomCleaningAndStorageItemPerMonthId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Create([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItemPerMonth item, int itemId)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item, itemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorageItemPerMonth
                    {
                        CustomCleaningAndStorageItemPerMonthId = item.CustomCleaningAndStorageItemPerMonthId,
                        CustomCleaningAndStorageItemId = item.CustomCleaningAndStorageItemId,
                        ProductionPeriodId = item.ProductionPeriodId,
                        Amount = item.Amount,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy,
                    };

                    db.CustomCleaningAndStorageItemPerMonth.Add(entity);
                    db.SaveChanges();

                    item.CustomCleaningAndStorageItemPerMonthId = entity.CustomCleaningAndStorageItemPerMonthId;

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CustomCleaningAndStorageItemId));
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Update([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItemPerMonth item)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item, item.CustomCleaningAndStorageItemId))
                {
                    ModelState.AddModelError("", "Duplicate Values are not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorageItemPerMonth
                    {
                        CustomCleaningAndStorageItemPerMonthId = item.CustomCleaningAndStorageItemPerMonthId,
                        CustomCleaningAndStorageItemId = item.CustomCleaningAndStorageItemId,
                        ProductionPeriodId = item.ProductionPeriodId,
                        Amount = item.Amount,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.CustomCleaningAndStorageItemPerMonth.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CustomCleaningAndStorageItemId));
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Destroy([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorageItemPerMonth item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomCleaningAndStorageItemPerMonth
                {
                    CustomCleaningAndStorageItemPerMonthId = item.CustomCleaningAndStorageItemPerMonthId,
                    CustomCleaningAndStorageItemId = item.CustomCleaningAndStorageItemId,
                    ProductionPeriodId = item.ProductionPeriodId,
                    Amount = item.Amount,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,

                };

                db.CustomCleaningAndStorageItemPerMonth.Attach(entity);
                db.CustomCleaningAndStorageItemPerMonth.Remove(entity);
                db.SaveChanges();

                ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.CustomCleaningAndStorageItemId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        private int GetBranchId(int itemId)
        {
            var branchId = -1;
            var item = db.CustomCleaningAndStorageItems.Find(itemId);
            if (item != null)
            {
                branchId = item.CustomCleaningAndStorage.BranchId;
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