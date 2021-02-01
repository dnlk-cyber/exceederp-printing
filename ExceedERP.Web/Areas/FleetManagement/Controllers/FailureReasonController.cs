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
    public class FailureReasonController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FailureReasons_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FailureReason> failurereasons = db.FailureReasons;
            DataSourceResult result = failurereasons.ToDataSourceResult(request, failureReason => new {
                FailureReasonId = failureReason.FailureReasonId,
                Name = failureReason.Name,
                Description = failureReason.Description,
                DateCreated = failureReason.DateCreated,
                LastModified = failureReason.LastModified,
                CreatedBy = failureReason.CreatedBy,
                ModifiedBy = failureReason.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FailureReasons_Create([DataSourceRequest]DataSourceRequest request, FailureReason failureReason)
        {
            ModelState.Remove("FailureReasonId");
            if (ModelState.IsValid)
            {
                var entity = new FailureReason
                {
                    FailureReasonId = Guid.NewGuid(),
                    Name = failureReason.Name,
                    Description = failureReason.Description,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name
                 };

                db.FailureReasons.Add(entity);
                db.SaveChanges();
                failureReason.FailureReasonId = entity.FailureReasonId;
            }

            return Json(new[] { failureReason }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FailureReasons_Update([DataSourceRequest]DataSourceRequest request, FailureReason failureReason)
        {
            if (ModelState.IsValid)
            {
                var entity = new FailureReason
                {
                    FailureReasonId = failureReason.FailureReasonId,
                    Name = failureReason.Name,
                    Description = failureReason.Description,
                    DateCreated = failureReason.DateCreated,
                    CreatedBy = failureReason.CreatedBy,
                    LastModified = DateTime.Now,
                     ModifiedBy = User.Identity.Name
                };

                db.FailureReasons.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { failureReason }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FailureReasons_Destroy([DataSourceRequest]DataSourceRequest request, FailureReason failureReason)
        {
            if (ModelState.IsValid)
            {
                var entity = new FailureReason
                {
                    FailureReasonId = failureReason.FailureReasonId,
                    Name = failureReason.Name,
                    Description = failureReason.Description,
                    DateCreated = failureReason.DateCreated,
                    LastModified = failureReason.LastModified,
                    CreatedBy = failureReason.CreatedBy,
                    ModifiedBy = failureReason.ModifiedBy
                };

                db.FailureReasons.Attach(entity);
                db.FailureReasons.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { failureReason }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
