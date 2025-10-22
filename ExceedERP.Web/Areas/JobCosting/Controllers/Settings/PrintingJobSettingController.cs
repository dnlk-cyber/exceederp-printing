﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.JobCosting.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.JobCosting;

namespace ExceedERP.Web.Areas.JobCosting.Controllers.Settings
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingAdmin)]
    public class PrintingJobSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            var PrintingSetting = db.PrintingJobSetting.ToList();
            DataSourceResult result = PrintingSetting.ToDataSourceResult(request, idleTimeCategory => new {
                PrintingJobSettingId = idleTimeCategory.PrintingJobSettingId,
                Remark = idleTimeCategory.Remark,
                IsFromPrinting = idleTimeCategory.IsFromPrinting,
                
            });

            return Json(result);
        }

        public ActionResult JobSetting_Create([DataSourceRequest]DataSourceRequest request, PrintingJobSetting idleTimeCategory)
        {
            if (ModelState.IsValid)
            {
                //var entity = (PrintingJobSetting)this.GetObject(idleTimeCategory);
                db.PrintingJobSetting.Add(idleTimeCategory);
                db.SaveChanges();
                //idleTimeCategory.PrintingJobSettingId = entity.IdleTimeCategoryId;
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobSetting_Update([DataSourceRequest]DataSourceRequest request, PrintingJobSetting idleTimeCategory)
        {
            var fetchSetting = db.PrintingJobSetting.FirstOrDefault(x => x.PrintingJobSettingId == idleTimeCategory.PrintingJobSettingId);
            fetchSetting.IsFromPrinting = idleTimeCategory.IsFromPrinting;
            fetchSetting.Remark = idleTimeCategory.Remark;
            if (ModelState.IsValid)
            {
                
                //db.PrintingJobSetting.Attach(entity);
                db.Entry(fetchSetting).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobSetting_Destroy([DataSourceRequest]DataSourceRequest request, PrintingJobSetting idleTimeCategory)
        {
            var fetchSetting = db.PrintingJobSetting.FirstOrDefault(x => x.PrintingJobSettingId == idleTimeCategory.PrintingJobSettingId);
            if (ModelState.IsValid)
            {
                //var entity = (IdleTimeCategory)this.GetObject(idleTimeCategory);

               // db.IdleTimeCategories.Attach(entity);
                db.PrintingJobSetting.Remove(fetchSetting);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeCategory }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}