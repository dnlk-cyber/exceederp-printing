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
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]
    public class PmSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PmSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PmSetting> PmSetting = db.PmSettings;
            DataSourceResult Result = PmSetting.ToDataSourceResult(request, pmSetting => new
            {
                PmSettingId = pmSetting.PmSettingId,
                PmName = pmSetting.PmName,
                Description = pmSetting.Description,
                DateCreated = pmSetting.DateCreated,
                LastModified = pmSetting.LastModified,
                CreatedBy = pmSetting.CreatedBy,
                ModifiedBy = pmSetting.ModifiedBy
            });

            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PmSetting_Readd()
        {
            IQueryable<PmSetting> PmSetting = db.PmSettings;

            var Result = PmSetting.Select(x => new
            {
                PmName = x.PmName,
                PmSettingId = x.PmSettingId,
            }).ToList();
            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PmSetting_Create([DataSourceRequest]DataSourceRequest request, PmSetting pmSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new PmSetting
                {
                    PmSettingId = pmSetting.PmSettingId,
                    PmName = pmSetting.PmName,
                    Description = pmSetting.Description,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name
                 };

                db.PmSettings.Add(entity);
                db.SaveChanges();
                pmSetting.PmSettingId = entity.PmSettingId;
            }

            return Json(new[] { pmSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FailureReasons_Update([DataSourceRequest]DataSourceRequest request, PmSetting pmSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new PmSetting
                {
                    PmSettingId = pmSetting.PmSettingId,
                    PmName = pmSetting.PmName,
                    Description = pmSetting.Description,
                    DateCreated = pmSetting.DateCreated,
                    CreatedBy = pmSetting.CreatedBy,
                    LastModified = DateTime.Now,
                     ModifiedBy = User.Identity.Name
                };

                db.PmSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { pmSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PmSetting_Destroy([DataSourceRequest]DataSourceRequest request, PmSetting pmSetting)
        {
            var pmsetting = db.PmSettings.FirstOrDefault(x => x.PmSettingId == pmSetting.PmSettingId);
            if(pmsetting==null)
            {
                ModelState.AddModelError("Record Not Found", "Record Not Found");
            }
            if (ModelState.IsValid)
            {
                
                db.PmSettings.Remove(pmsetting);
                db.SaveChanges();
            }

            return Json(new[] { pmSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
