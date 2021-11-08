using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class EducationLevelSettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult EducationLevelSettings_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
              IQueryable<EducationLevelSetting> applicantEducations = db.EducationLevelSettings;


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, educationLevelSetting => new
            {
                EducationLevelSettingId = educationLevelSetting.EducationLevelSettingId,
                Level = educationLevelSetting.Level,
                Remark = educationLevelSetting.Remark,

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationLevelSettings_Create([DataSourceRequest]DataSourceRequest request, EducationLevelSetting educationLevelSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new EducationLevelSetting
                {
                    Level = educationLevelSetting.Level,
                    Remark = educationLevelSetting.Remark,
                };

                db.EducationLevelSettings.Add(entity);
                db.SaveChanges();
                educationLevelSetting.EducationLevelSettingId = entity.EducationLevelSettingId;


            }
            return Json(new[] { educationLevelSetting }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationLevelSettings_Update([DataSourceRequest]DataSourceRequest request, EducationLevelSetting educationLevelSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new EducationLevelSetting
                {
                    EducationLevelSettingId = educationLevelSetting.EducationLevelSettingId,
                    Level = educationLevelSetting.Level,
                    Remark = educationLevelSetting.Remark,
                };

                db.EducationLevelSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { educationLevelSetting }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationLevelSettings_Destroy([DataSourceRequest]DataSourceRequest request, EducationLevelSetting educationLevelSetting)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = new EducationLevelSetting
                {
                    EducationLevelSettingId = educationLevelSetting.EducationLevelSettingId,
                    Level = educationLevelSetting.Level,
                    Remark = educationLevelSetting.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + educationLevelSetting.EducationLevelSettingId);
                #endregion
                db.EducationLevelSettings.Attach(entity);
                db.EducationLevelSettings.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { educationLevelSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
