﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
     [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,HrmConstants.HRAdmin)]
    public class PerformanceSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformanceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PerformanceSetting> performances = db.PerformanceSettings;
            DataSourceResult result = performances.ToDataSourceResult(request, performance => new {
                PerformanceSettingId = performance.PerformanceSettingId,
                Value = performance.Value,
                DateCreated = performance.DateCreated,
                CreatedBy = performance.CreatedBy,
                LastModified = performance.LastModified,
                ModifiedBy = performance.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceSettings_Create([DataSourceRequest]DataSourceRequest request, PerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {

                performance.CreatedBy = User.Identity.Name;
                performance.DateCreated = DateTime.Now;
                var entity = GetMappedObject(performance);
                db.PerformanceSettings.Add(entity);
                db.SaveChanges();
                performance.PerformanceSettingId = entity.PerformanceSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Performance setting ", "Success", User.Identity.Name, User.Identity.Name + " add Performance setting called " + entity.Value + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);                
                #endregion
                
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceSettings_Update([DataSourceRequest]DataSourceRequest request, PerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {
                performance.ModifiedBy = User.Identity.Name;
                performance.LastModified = DateTime.Now;
                var entity = GetMappedObject(performance);
                db.PerformanceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Performance Setting ", "Success", User.Identity.Name, User.Identity.Name + " update Performance Seting called " + entity.Value + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);


                #endregion
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceSettings_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(performance);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance setting  Id " + performance.PerformanceSettingId);
                #endregion
                db.PerformanceSettings.Attach(entity);
                db.PerformanceSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }
        private PerformanceSetting GetMappedObject(PerformanceSetting performance)
        {
            var entity = new PerformanceSetting
            {
                PerformanceSettingId = performance.PerformanceSettingId,
                Value = performance.Value,
                DateCreated= performance.DateCreated,
                CreatedBy = performance.CreatedBy,
                LastModified = performance.LastModified,
                ModifiedBy =performance.ModifiedBy

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
