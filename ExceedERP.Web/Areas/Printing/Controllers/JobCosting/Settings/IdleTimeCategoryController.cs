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
using ExceedERP.Core.Domain.printing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting.Settings
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingAdmin)]
    public class IdleTimeCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IdleTimeCategory> idletimecategories = db.IdleTimeCategories;
            DataSourceResult result = idletimecategories.ToDataSourceResult(request, idleTimeCategory => new {
                IdleTimeCategoryId = idleTimeCategory.IdleTimeCategoryId,
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

        public ActionResult IdleTimeCategories_Create([DataSourceRequest]DataSourceRequest request, IdleTimeCategory idleTimeCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeCategory)this.GetObject(idleTimeCategory);
                db.IdleTimeCategories.Add(entity);
                db.SaveChanges();
                idleTimeCategory.IdleTimeCategoryId = entity.IdleTimeCategoryId;
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeCategories_Update([DataSourceRequest]DataSourceRequest request, IdleTimeCategory idleTimeCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeCategory)this.GetObject(idleTimeCategory);

                db.IdleTimeCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeCategories_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeCategory idleTimeCategory)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeCategory)this.GetObject(idleTimeCategory);

                db.IdleTimeCategories.Attach(entity);
                db.IdleTimeCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeCategory idleTimeCategory)
        {
            var entity = new IdleTimeCategory
            {
                IdleTimeCategoryId = idleTimeCategory.IdleTimeCategoryId,
                CategoryName = idleTimeCategory.CategoryName,
                IsControllable = idleTimeCategory.IsControllable,
                CategoryDescription = idleTimeCategory.CategoryDescription,
                DateCreated = idleTimeCategory.DateCreated,
                LastModified = idleTimeCategory.LastModified,
                CreatedBy = idleTimeCategory.CreatedBy,
                ModifiedBy = idleTimeCategory.ModifiedBy

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