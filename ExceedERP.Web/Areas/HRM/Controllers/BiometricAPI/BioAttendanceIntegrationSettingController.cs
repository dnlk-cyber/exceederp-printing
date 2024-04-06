﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.BiometricAPI
{
    public class BioAttendanceIntegrationSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BioAttendanceIntegrationSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BioAttendanceIntegrationSetting> bioattendanceintegrationsettings = db.BioAttendanceIntegrationSettings;
            DataSourceResult result = bioattendanceintegrationsettings.ToDataSourceResult(request, bioAttendanceIntegrationSetting => new {
                BioAttendanceIntegrationSettingId = bioAttendanceIntegrationSetting.BioAttendanceIntegrationSettingId,
                Type = bioAttendanceIntegrationSetting.Type,
                Value = bioAttendanceIntegrationSetting.Value,
                Remark = bioAttendanceIntegrationSetting.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BioAttendanceIntegrationSettings_Create([DataSourceRequest]DataSourceRequest request, BioAttendanceIntegrationSetting bioAttendanceIntegrationSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BioAttendanceIntegrationSetting
                {
                    Type = bioAttendanceIntegrationSetting.Type,
                    Value = bioAttendanceIntegrationSetting.Value,
                    Remark = bioAttendanceIntegrationSetting.Remark
                };

                db.BioAttendanceIntegrationSettings.Add(entity);
                db.SaveChanges();
                bioAttendanceIntegrationSetting.BioAttendanceIntegrationSettingId = entity.BioAttendanceIntegrationSettingId;
            }

            return Json(new[] { bioAttendanceIntegrationSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BioAttendanceIntegrationSettings_Update([DataSourceRequest]DataSourceRequest request, BioAttendanceIntegrationSetting bioAttendanceIntegrationSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BioAttendanceIntegrationSetting
                {
                    BioAttendanceIntegrationSettingId = bioAttendanceIntegrationSetting.BioAttendanceIntegrationSettingId,
                    Type = bioAttendanceIntegrationSetting.Type,
                    Value = bioAttendanceIntegrationSetting.Value,
                    Remark = bioAttendanceIntegrationSetting.Remark
                };

                db.BioAttendanceIntegrationSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bioAttendanceIntegrationSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BioAttendanceIntegrationSettings_Destroy([DataSourceRequest]DataSourceRequest request, BioAttendanceIntegrationSetting bioAttendanceIntegrationSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BioAttendanceIntegrationSetting
                {
                    BioAttendanceIntegrationSettingId = bioAttendanceIntegrationSetting.BioAttendanceIntegrationSettingId,
                    Type = bioAttendanceIntegrationSetting.Type,
                    Value = bioAttendanceIntegrationSetting.Value,
                    Remark = bioAttendanceIntegrationSetting.Remark
                };

                db.BioAttendanceIntegrationSettings.Attach(entity);
                db.BioAttendanceIntegrationSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bioAttendanceIntegrationSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
