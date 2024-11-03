using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ExperienceOrganizationTypeSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ExperienceOrganizationTypeSettings_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
              IQueryable<ExperienceOrganizationTypeSetting> experiences = db.ExperienceOrganizationTypeSettings;


            DataSourceResult result = experiences.ToDataSourceResult(request, experience => new
            {
                ExperienceOrganizationTypeSettingId = experience.ExperienceOrganizationTypeSettingId,
                OrganizationType = experience.OrganizationType,
                Remark = experience.Remark,

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceOrganizationTypeSettings_Create([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting experience)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ExperienceOrganizationTypeSetting
                {
                    OrganizationType = experience.OrganizationType,
                    Remark = experience.Remark,
                };

                db.ExperienceOrganizationTypeSettings.Add(entity);
                db.SaveChanges();
                experience.ExperienceOrganizationTypeSettingId = entity.ExperienceOrganizationTypeSettingId;


            }
            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceOrganizationTypeSettings_Update([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting experience)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ExperienceOrganizationTypeSetting
                {
                    ExperienceOrganizationTypeSettingId = experience.ExperienceOrganizationTypeSettingId,
                    OrganizationType = experience.OrganizationType,
                    Remark = experience.Remark,
                };

                db.ExperienceOrganizationTypeSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceOrganizationTypeSettings_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceOrganizationTypeSetting experience)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = new ExperienceOrganizationTypeSetting
                {
                    ExperienceOrganizationTypeSettingId = experience.ExperienceOrganizationTypeSettingId,
                    OrganizationType = experience.OrganizationType,
                    Remark = experience.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + experience.ExperienceOrganizationTypeSettingId);
                #endregion
                db.ExperienceOrganizationTypeSettings.Attach(entity);
                db.ExperienceOrganizationTypeSettings.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
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
