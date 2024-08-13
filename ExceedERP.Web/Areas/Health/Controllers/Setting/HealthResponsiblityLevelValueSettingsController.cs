using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers.Setting
{
    public class HealthResponsiblityLevelValueSettingsController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        public static DateTime RenewalDate { get; set; }
        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult HealthResponsiblityLevelValueSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healths = db.HealthResponsiblityLevelValueSettings.ToList();
            var accidents = healths.Select(health => new HealthResponsiblityLevelValueSetting
            {

                HealthResponsiblityLevelValueSettingId = health.HealthResponsiblityLevelValueSettingId,
                Level = health.Level,
                CurrentValue = health.CurrentValue               


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthResponsiblityLevelValueSettings_Create([DataSourceRequest] DataSourceRequest request, HealthResponsiblityLevelValueSetting health)
        {
            
            if(ModelState.IsValid)
            {
                var entity = new HealthResponsiblityLevelValueSetting
                {
                    HealthResponsiblityLevelValueSettingId = health.HealthResponsiblityLevelValueSettingId,
                    Level = health.Level,
                    CurrentValue = health.CurrentValue
                };
                db.HealthResponsiblityLevelValueSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
                var rank = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x=>x.Level == entity.Level);
                if(rank != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Value by Responsiblity Level setting", "Success", User.Identity.Name, User.Identity.Name + " add health value setting for resiponsiblity Level " + rank.Level.ToString() + " at date " + DateTime.Today, Modules.Health);
                }

                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthResponsiblityLevelValueSettings_Update([DataSourceRequest] DataSourceRequest request, HealthResponsiblityLevelValueSetting health)
        {

           
            if (ModelState.IsValid)
            {
                var entity = new HealthResponsiblityLevelValueSetting
                {
                    HealthResponsiblityLevelValueSettingId = health.HealthResponsiblityLevelValueSettingId,
                    Level = health.Level,
                    CurrentValue = health.CurrentValue
                };
                db.HealthResponsiblityLevelValueSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
                var rank = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == entity.Level);
                if (rank != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Value by Responsiblity Level setting", "Success", User.Identity.Name, User.Identity.Name + " add health value setting for resiponsiblity Level " + rank.Level.ToString() + " at date " + DateTime.Today, Modules.Health);
                }

                #endregion

            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthResponsiblityLevelValueSettings_Destroy([DataSourceRequest] DataSourceRequest request, HealthResponsiblityLevelValueSetting health)
        {

            if (ModelState.IsValid)
            {
                var entity = new HealthResponsiblityLevelValueSetting
                {
                    HealthResponsiblityLevelValueSettingId = health.HealthResponsiblityLevelValueSettingId,
                    Level = health.Level,
                    CurrentValue = health.CurrentValue
                };
                #region operation log                
                var rank = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == entity.Level);
                if (rank != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Value by Responsiblity Level setting", "Success", User.Identity.Name, User.Identity.Name + " add health value setting for resiponsiblity Level " + rank.Level.ToString() + " at date " + DateTime.Today, Modules.Health);
                }

                #endregion
                db.HealthResponsiblityLevelValueSettings.Attach(entity);
                db.HealthResponsiblityLevelValueSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}