﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class BonusSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["PeriodList"] = db.GLFiscalYears.AsQueryable();
        }

        public ActionResult BonusSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BonusSetting> bonussettings = db.BonusSettings;
            DataSourceResult result = bonussettings.ToDataSourceResult(request, bonusSetting => new {
                BonusSettingId = bonusSetting.BonusSettingId,
                BunusType = bonusSetting.BunusType,
                Multiplier = bonusSetting.Multiplier,
                GlFiscalYearId = bonusSetting.GlFiscalYearId,
                CurrentValue = bonusSetting.CurrentValue,
                Remark = bonusSetting.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BonusSettings_Create([DataSourceRequest]DataSourceRequest request, BonusSetting bonusSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BonusSetting
                {
                    BunusType = bonusSetting.BunusType,
                    Multiplier = bonusSetting.Multiplier,
                    GlFiscalYearId = bonusSetting.GlFiscalYearId,
                    CurrentValue = bonusSetting.CurrentValue,
                    Remark = bonusSetting.Remark
                };

                db.BonusSettings.Add(entity);
                db.SaveChanges();
                bonusSetting.BonusSettingId = entity.BonusSettingId;
            }

            return Json(new[] { bonusSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BonusSettings_Update([DataSourceRequest]DataSourceRequest request, BonusSetting bonusSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BonusSetting
                {
                    BonusSettingId = bonusSetting.BonusSettingId,
                    BunusType = bonusSetting.BunusType,
                    Multiplier = bonusSetting.Multiplier,
                    GlFiscalYearId = bonusSetting.GlFiscalYearId,
                    CurrentValue = bonusSetting.CurrentValue,
                    Remark = bonusSetting.Remark
                };

                db.BonusSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bonusSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BonusSettings_Destroy([DataSourceRequest]DataSourceRequest request, BonusSetting bonusSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new BonusSetting
                {
                    BonusSettingId = bonusSetting.BonusSettingId,
                    BunusType = bonusSetting.BunusType,
                    Multiplier = bonusSetting.Multiplier,
                    GlFiscalYearId = bonusSetting.GlFiscalYearId,
                    CurrentValue = bonusSetting.CurrentValue,
                    Remark = bonusSetting.Remark
                };

                db.BonusSettings.Attach(entity);
                db.BonusSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bonusSetting }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
