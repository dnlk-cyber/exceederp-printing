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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class EducationFieldSettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult EducationFieldSettings_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            IQueryable<EducationFieldSetting> applicantEducations = db.EducationFieldSettings;


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, field => new
            {
                EducationFieldSettingId = field.EducationFieldSettingId,
                Field = field.Field,
                Remark = field.Remark,

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationFieldSettings_Create([DataSourceRequest]DataSourceRequest request, EducationFieldSetting educationFieldSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new EducationFieldSetting
                {
                    Field = educationFieldSetting.Field,
                    Remark = educationFieldSetting.Remark,
                };

                db.EducationFieldSettings.Add(entity);
                db.SaveChanges();
                educationFieldSetting.EducationFieldSettingId = entity.EducationFieldSettingId;


            }
            return Json(new[] { educationFieldSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationFieldSettings_Update([DataSourceRequest]DataSourceRequest request, EducationFieldSetting educationFieldSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new EducationFieldSetting
                {
                    EducationFieldSettingId = educationFieldSetting.EducationFieldSettingId,
                    Field = educationFieldSetting.Field,
                    Remark = educationFieldSetting.Remark,
                };

                db.EducationFieldSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { educationFieldSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationFieldSettings_Destroy([DataSourceRequest]DataSourceRequest request, EducationFieldSetting educationFieldSetting)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new EducationFieldSetting
                {
                    EducationFieldSettingId = educationFieldSetting.EducationFieldSettingId,
                    Field = educationFieldSetting.Field,
                    Remark = educationFieldSetting.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + educationFieldSetting.EducationFieldSettingId);
                #endregion
                db.EducationFieldSettings.Attach(entity);
                db.EducationFieldSettings.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { educationFieldSetting }.ToDataSourceResult(request, ModelState));
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
