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

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ExperienceSettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ExamTypeSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExperienceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ExperienceOrganizationTypeSetting> settings = db.ExperienceOrganizationTypeSettings;
            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                ExperienceOrganizationTypeSettingId = setting.ExperienceOrganizationTypeSettingId,
                OrganizationType = setting.OrganizationType,
                Remark = setting.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceSettings_Create([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting setting)
        {
            ModelState.Clear();
          
            if (ModelState.IsValid)
            {

                var entity = new ExperienceOrganizationTypeSetting
                {
                    OrganizationType = setting.OrganizationType,
                    Remark = setting.Remark
                };

                db.ExperienceOrganizationTypeSettings.Add(entity);
                db.SaveChanges();

            }
            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceSettings_Update([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting setting)
        {
            ModelState.Clear();
           
            if (ModelState.IsValid)
            {
                var entity = new ExperienceOrganizationTypeSetting
                {
                    ExperienceOrganizationTypeSettingId = setting.ExperienceOrganizationTypeSettingId,
                    OrganizationType = setting.OrganizationType,
                    Remark = setting.Remark
                };

                db.ExperienceOrganizationTypeSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceSettings_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting setting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ExperienceOrganizationTypeSetting
                {
                    ExperienceOrganizationTypeSettingId = setting.ExperienceOrganizationTypeSettingId,
                    OrganizationType = setting.OrganizationType,
                    Remark = setting.Remark
                };
                #region delete log
                //  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment setting of " + configuration.Attribute);
                #endregion
                db.ExperienceOrganizationTypeSettings.Attach(entity);
                db.ExperienceOrganizationTypeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
