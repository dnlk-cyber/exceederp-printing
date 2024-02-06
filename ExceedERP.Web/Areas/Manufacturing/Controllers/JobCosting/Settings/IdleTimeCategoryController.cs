﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting.Settings
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingAdmin)]
    public class FurnitureIdleTimeCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureIdleTimeCategory> idletimecategories = db.FurnitureIdleTimeCategories;
            DataSourceResult result = idletimecategories.ToDataSourceResult(request, idleTimeCategory => new {
                FurnitureIdleTimeCategoryId = idleTimeCategory.FurnitureIdleTimeCategoryId,
                CategoryName = idleTimeCategory.CategoryName,
                IsControllable = idleTimeCategory.IsControllable,
                CategoryDescription = idleTimeCategory.CategoryDescription,
                DateCreated = idleTimeCategory.DateCreated,
                LastModified = idleTimeCategory.LastModified,
                CreatedBy = idleTimeCategory.CreatedBy,
                ModifiedBy = idleTimeCategory.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult IdleTimeCategories_Create([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeCategory idleTimeCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeCategory)this.GetObject(idleTimeCategory);
                db.FurnitureIdleTimeCategories.Add(entity);
                db.SaveChanges();
                idleTimeCategory.FurnitureIdleTimeCategoryId = entity.FurnitureIdleTimeCategoryId;
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeCategories_Update([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeCategory idleTimeCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeCategory)this.GetObject(idleTimeCategory);

                db.FurnitureIdleTimeCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeCategories_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeCategory idleTimeCategory)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeCategory)this.GetObject(idleTimeCategory);

                db.FurnitureIdleTimeCategories.Attach(entity);
                db.FurnitureIdleTimeCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureIdleTimeCategory idleTimeCategory)
        {
            var entity = new FurnitureIdleTimeCategory
            {
                FurnitureIdleTimeCategoryId = idleTimeCategory.FurnitureIdleTimeCategoryId,
                CategoryName = idleTimeCategory.CategoryName,
                IsControllable = idleTimeCategory.IsControllable,
                CategoryDescription = idleTimeCategory.CategoryDescription,
                DateCreated = idleTimeCategory.DateCreated,
                LastModified =idleTimeCategory.LastModified,
                CreatedBy = idleTimeCategory.CreatedBy,
                ModifiedBy =idleTimeCategory.ModifiedBy

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}