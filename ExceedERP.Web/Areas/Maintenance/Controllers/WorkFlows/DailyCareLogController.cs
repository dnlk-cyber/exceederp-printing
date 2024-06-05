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
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.MaintenanceOrderUser)]
    public class DailyCareLogController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["DailyCareCategory"] = db.DailyCareCategories.ToList();
            ViewData["DailyCare"] = db.DailyCares.ToList();
            ViewData["Fleets"] = db.Fleets.ToList();

            return View();
        }

        public ActionResult DailyCareLogs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DailyCareLog> dailycarelogs = db.DailyCareLogs;
            DataSourceResult result = dailycarelogs.ToDataSourceResult(request, dailyCareLog => new
            {
                DailyCareLogId = dailyCareLog.DailyCareLogId,
                Location = dailyCareLog.Location,
                    FleetsId =    dailyCareLog.FleetsId,
                Operator = dailyCareLog.Operator,
                Description = dailyCareLog.Description,
                DateCreated = dailyCareLog.DateCreated,
                LastModified = dailyCareLog.LastModified,
                CreatedBy = dailyCareLog.CreatedBy,
                ModifiedBy = dailyCareLog.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareLogs_Create([DataSourceRequest]DataSourceRequest request, DailyCareLog dailyCareLog)
        {
            ModelState.Remove("DailyCareLogId");
            IFleetService _FleetService = new FleetService();
            var Equipment = _FleetService.GetById(dailyCareLog.FleetsId);
            if (Equipment == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("Fleet", " Equipment Required");
                return Json(new[] { dailyCareLog }.ToDataSourceResult(request, ModelState));
            }
            if (Equipment != null)
            {
                var entity = new DailyCareLog
                {
                    DailyCareLogId = Guid.NewGuid(),
                    FleetsId = dailyCareLog.FleetsId,
                    Location = Equipment.Location,
                    Operator = Equipment.OperatorName,
                    Description = dailyCareLog.Description,
                    DateCreated = DateTime.Now,
                    LastModified = dailyCareLog.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = dailyCareLog.ModifiedBy,
                    Status = MaintenanceConst.Pending
                };

                db.DailyCareLogs.Add(entity);
                db.SaveChanges();
                dailyCareLog.DailyCareLogId = entity.DailyCareLogId;
            }

            return Json(new[] { dailyCareLog }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareLogs_Update([DataSourceRequest]DataSourceRequest request, DailyCareLog dailyCareLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareLog
                {
                    DailyCareLogId = dailyCareLog.DailyCareLogId,
                    Description = dailyCareLog.Description,
                    DateCreated = dailyCareLog.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = dailyCareLog.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.DailyCareLogs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dailyCareLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareLogs_Destroy([DataSourceRequest]DataSourceRequest request, DailyCareLog dailyCareLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareLog
                {
                    DailyCareLogId = dailyCareLog.DailyCareLogId,
                    Description = dailyCareLog.Description,
                    DateCreated = dailyCareLog.DateCreated,
                    LastModified = dailyCareLog.LastModified,
                    CreatedBy = dailyCareLog.CreatedBy,
                    ModifiedBy = dailyCareLog.ModifiedBy
                };

                db.DailyCareLogs.Attach(entity);
                db.DailyCareLogs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dailyCareLog }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
