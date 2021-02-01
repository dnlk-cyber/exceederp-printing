﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class DailyCareCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyCareCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DailyCareCategory> dailycarecategories = db.DailyCareCategories;
            DataSourceResult result = dailycarecategories.ToDataSourceResult(request, dailyCareCategory => new
            {
                DailyCareCategoryId = dailyCareCategory.DailyCareCategoryId,
                Name = dailyCareCategory.Name,
                Description = dailyCareCategory.Description,
                DateCreated = dailyCareCategory.DateCreated,
                LastModified = dailyCareCategory.LastModified,
                CreatedBy = dailyCareCategory.CreatedBy,
                ModifiedBy = dailyCareCategory.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareCategories_Create([DataSourceRequest]DataSourceRequest request, DailyCareCategory dailyCareCategory)
        {
            ModelState.Remove("DailyCareCategoryId");
            if (ModelState.IsValid)
            {
                var entity = new DailyCareCategory
                {
                    DailyCareCategoryId = Guid.NewGuid(),
                    Name = dailyCareCategory.Name,
                    Description = dailyCareCategory.Description,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.DailyCareCategories.Add(entity);
                db.SaveChanges();
                dailyCareCategory.DailyCareCategoryId = entity.DailyCareCategoryId;
            }

            return Json(new[] { dailyCareCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareCategories_Update([DataSourceRequest]DataSourceRequest request, DailyCareCategory dailyCareCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareCategory
                {
                    DailyCareCategoryId = dailyCareCategory.DailyCareCategoryId,
                    Name = dailyCareCategory.Name,
                    Description = dailyCareCategory.Description,
                    DateCreated = dailyCareCategory.DateCreated,
                    CreatedBy = dailyCareCategory.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.DailyCareCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dailyCareCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareCategories_Destroy([DataSourceRequest]DataSourceRequest request, DailyCareCategory dailyCareCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareCategory
                {
                    DailyCareCategoryId = dailyCareCategory.DailyCareCategoryId,
                    Name = dailyCareCategory.Name,
                    Description = dailyCareCategory.Description,
                    DateCreated = dailyCareCategory.DateCreated,
                    LastModified = dailyCareCategory.LastModified,
                    CreatedBy = dailyCareCategory.CreatedBy,
                    ModifiedBy = dailyCareCategory.ModifiedBy
                };

                db.DailyCareCategories.Attach(entity);
                db.DailyCareCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dailyCareCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
