using System.Data;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ScreeningSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        public void PopulateViewData()
        {
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();
            ViewData["EducationLevel"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
            ViewData["EducationField"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();
            ViewData["OrganizationType"] = db.ExperienceOrganizationTypeSettings.ToList();
            ViewData["ExperienceJobtitle"] = db.ExperienceJobTitles.ToList();
            ViewData["Lookups"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["LookupsExternalExperienceJobTitleForInternalApplicants"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.Jobtitle_External_Experience);
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ScreeningSettings_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            IQueryable<ScreeningSetting> screenings = db.ScreeningSettings;

            DataSourceResult result = screenings.ToDataSourceResult(request, screeningSetting => new
            {
                OrganizationType = screeningSetting.OrganizationType,
                ScreeningSettingId = screeningSetting.ScreeningSettingId,
                VacancyId = screeningSetting.VacancyId,
                ExperienceYear = screeningSetting.ExperienceYear,
                Language = screeningSetting.Language,
                Gender = screeningSetting.Gender

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningSettings_Create([DataSourceRequest]DataSourceRequest request, ScreeningSetting screeningSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningSetting
                {
                    OrganizationType = screeningSetting.OrganizationType,
                    VacancyId = screeningSetting.VacancyId,
                    ExperienceYear = screeningSetting.ExperienceYear,
                    Language = screeningSetting.Language,
                    Gender = screeningSetting.Gender
                };

                db.ScreeningSettings.Add(entity);
                db.SaveChanges();
                screeningSetting.ScreeningSettingId = entity.ScreeningSettingId;


            }
            return Json(new[] { screeningSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningSettings_Update([DataSourceRequest]DataSourceRequest request, ScreeningSetting screeningSetting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningSetting
                {
                    ScreeningSettingId = screeningSetting.ScreeningSettingId,
                    OrganizationType = screeningSetting.OrganizationType,
                    VacancyId = screeningSetting.VacancyId,
                    ExperienceYear = screeningSetting.ExperienceYear,
                    Language = screeningSetting.Language,
                    Gender = screeningSetting.Gender
                };

                db.ScreeningSettings.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { screeningSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningSettings_Destroy([DataSourceRequest]DataSourceRequest request, ScreeningSetting screeningSetting)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new ScreeningSetting
                {
                    ScreeningSettingId = screeningSetting.ScreeningSettingId,
                    OrganizationType = screeningSetting.OrganizationType,
                    VacancyId = screeningSetting.VacancyId,
                    ExperienceYear = screeningSetting.ExperienceYear,
                    Language = screeningSetting.Language,
                    Gender = screeningSetting.Gender
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + screeningSetting.ScreeningSettingId);
                #endregion
                db.ScreeningSettings.Attach(entity);
                db.ScreeningSettings.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { screeningSetting }.ToDataSourceResult(request, ModelState));
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
