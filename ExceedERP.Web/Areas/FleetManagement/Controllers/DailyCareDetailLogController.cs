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
    public class DailyCareDetailLogController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyCareDetailLogs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DailyCareDetailLog> dailycaredetaillogs = db.DailyCareDetailLogs;
            DataSourceResult result = dailycaredetaillogs.ToDataSourceResult(request, dailyCareDetailLog => new {
                DailyCareDetailLogId = dailyCareDetailLog.DailyCareDetailLogId,
                DailyCareCategoryId = dailyCareDetailLog.DailyCareCategoryId,
                DailyCareId = dailyCareDetailLog.DailyCareId,
                Description = dailyCareDetailLog.Description,
                IsActive = dailyCareDetailLog.IsActive,
                DateCreated = dailyCareDetailLog.DateCreated,
                LastModified = dailyCareDetailLog.LastModified,
                CreatedBy = dailyCareDetailLog.CreatedBy,
                ModifiedBy = dailyCareDetailLog.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailLogs_Create([DataSourceRequest]DataSourceRequest request, DailyCareDetailLog dailyCareDetailLog, string Id)
        { 
                var entity = new DailyCareDetailLog
                {
                    DailyCareDetailLogId = Guid.NewGuid(),
                    DailyCareLogId = Guid.Parse(Id),
                    DailyCareCategoryId = dailyCareDetailLog.DailyCareCategoryId,
                    DailyCareId = dailyCareDetailLog.DailyCareId,
                    Description = dailyCareDetailLog.Description,
                    IsActive = dailyCareDetailLog.IsActive,
                    DateCreated = dailyCareDetailLog.DateCreated,
                    LastModified = dailyCareDetailLog.LastModified,
                    CreatedBy = dailyCareDetailLog.CreatedBy,
                    ModifiedBy = dailyCareDetailLog.ModifiedBy
                };

                db.DailyCareDetailLogs.Add(entity);
                db.SaveChanges();
                dailyCareDetailLog.DailyCareDetailLogId = entity.DailyCareDetailLogId;
           
            return Json(new[] { dailyCareDetailLog }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailLogs_Update([DataSourceRequest]DataSourceRequest request, DailyCareDetailLog dailyCareDetailLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareDetailLog
                {
                    DailyCareDetailLogId = dailyCareDetailLog.DailyCareDetailLogId,
                    DailyCareCategoryId = dailyCareDetailLog.DailyCareCategoryId,
                    Description = dailyCareDetailLog.Description,
                    IsActive = dailyCareDetailLog.IsActive,
                    DateCreated = dailyCareDetailLog.DateCreated,
                    LastModified = dailyCareDetailLog.LastModified,
                    CreatedBy = dailyCareDetailLog.CreatedBy,
                    ModifiedBy = dailyCareDetailLog.ModifiedBy
                };

                db.DailyCareDetailLogs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dailyCareDetailLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailLogs_Destroy([DataSourceRequest]DataSourceRequest request, DailyCareDetailLog dailyCareDetailLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareDetailLog
                {
                    DailyCareDetailLogId = dailyCareDetailLog.DailyCareDetailLogId,
                    DailyCareCategoryId = dailyCareDetailLog.DailyCareCategoryId,
                    Description = dailyCareDetailLog.Description,
                    IsActive = dailyCareDetailLog.IsActive,
                    DateCreated = dailyCareDetailLog.DateCreated,
                    LastModified = dailyCareDetailLog.LastModified,
                    CreatedBy = dailyCareDetailLog.CreatedBy,
                    ModifiedBy = dailyCareDetailLog.ModifiedBy
                };

                db.DailyCareDetailLogs.Attach(entity);
                db.DailyCareDetailLogs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dailyCareDetailLog }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
