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
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class DailyCareController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DailyCare> dailycares = db.DailyCares;
            DataSourceResult result = dailycares.ToDataSourceResult(request, dailyCare => new
            {
                DailyCareId = dailyCare.DailyCareId,
                Name = dailyCare.Name,
                Description = dailyCare.Description,
                Category = dailyCare.DailyCareCategory.Description,

                DateCreated = dailyCare.DateCreated,
                LastModified = dailyCare.LastModified,
                CreatedBy = dailyCare.CreatedBy,
                ModifiedBy = dailyCare.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCares_Create([DataSourceRequest]DataSourceRequest request, DailyCare dailyCare)
        {
            ModelState.Remove("DailyCareId");
            if (ModelState.IsValid)
            {
                var entity = new DailyCare
                {
                    DailyCareId = Guid.NewGuid(),
                    Name = dailyCare.Name,
                    DailyCareCategoryId = dailyCare.DailyCareCategoryId,
                    Description = dailyCare.Description,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.DailyCares.Add(entity);
                db.SaveChanges();
                dailyCare.DailyCareId = entity.DailyCareId;
            }

            return Json(new[] { dailyCare }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCares_Update([DataSourceRequest]DataSourceRequest request, DailyCare dailyCare)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCare
                {
                    DailyCareId = dailyCare.DailyCareId,
                    Name = dailyCare.Name,
                    Description = dailyCare.Description,
                    DateCreated = dailyCare.DateCreated,
                    CreatedBy = dailyCare.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.DailyCares.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dailyCare }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCares_Destroy([DataSourceRequest]DataSourceRequest request, DailyCare dailyCare)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCare
                {
                    DailyCareId = dailyCare.DailyCareId,
                    Name = dailyCare.Name,
                    Description = dailyCare.Description,
                    DateCreated = dailyCare.DateCreated,
                    LastModified = dailyCare.LastModified,
                    CreatedBy = dailyCare.CreatedBy,
                    ModifiedBy = dailyCare.ModifiedBy
                };

                db.DailyCares.Attach(entity);
                db.DailyCares.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dailyCare }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
