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
    public class CustomCleaningAndStorageController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CustomCleaningAndStorageController()
        {
            db = new ExceedDbContext();
        }
       
        public ActionResult Items_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CustomCleaningAndStorage> items = db.CustomCleaningAndStorage;
            DataSourceResult result = items.ToDataSourceResult(request, item => new 
            {
                CustomCleaningAndStorageId = item.CustomCleaningAndStorageId,
                BranchId = item.BranchId,
                GlFiscalYearId = item.GlFiscalYearId,                

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(CustomCleaningAndStorage record)
        {
            return db.CustomCleaningAndStorage
                .FirstOrDefault(i => i.BranchId == record.BranchId && i.GlFiscalYearId == record.GlFiscalYearId && i.CustomCleaningAndStorageId != record.CustomCleaningAndStorageId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Create([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorage item)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorage
                    {
                        CustomCleaningAndStorageId = item.CustomCleaningAndStorageId,
                        BranchId = item.BranchId,
                        GlFiscalYearId = item.GlFiscalYearId,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy,
                    };

                    db.CustomCleaningAndStorage.Add(entity);
                    db.SaveChanges();

                    item.CustomCleaningAndStorageId = entity.CustomCleaningAndStorageId;

                    //this.CreateMonthlyCashFlowItems(entity);
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Update([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorage item)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new CustomCleaningAndStorage
                    {
                        CustomCleaningAndStorageId = item.CustomCleaningAndStorageId,
                        BranchId = item.BranchId,
                        GlFiscalYearId = item.GlFiscalYearId,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.CustomCleaningAndStorage.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Destroy([DataSourceRequest]DataSourceRequest request, CustomCleaningAndStorage item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CustomCleaningAndStorage
                {
                    CustomCleaningAndStorageId = item.CustomCleaningAndStorageId,
                    BranchId = item.BranchId,
                    GlFiscalYearId = item.GlFiscalYearId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,

                };

                db.CustomCleaningAndStorage.Attach(entity);
                db.CustomCleaningAndStorage.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}