using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    [Authorize]
    public class InternalRecruitmentCriteriaTypeSettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/InternalRecruitmentCriteriaTypeSettings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InternalRecruitmentCriteriaTypeSetting> criteriaTypeSettings = db.InternalRecruitmentCriteriaTypeSettings;
            DataSourceResult result = criteriaTypeSettings.ToDataSourceResult(request, criteriaTypeSetting => new
            {
                InternalRecruitmentCriteriaTypeSettingId = criteriaTypeSetting.InternalRecruitmentCriteriaTypeSettingId,
                IsPercent = criteriaTypeSetting.IsPercent,
                Squence = criteriaTypeSetting.Squence,
                ExamType = criteriaTypeSetting.ExamType,
                SettingVlaue = criteriaTypeSetting.SettingVlaue,
                ExamSource = criteriaTypeSetting.ExamSource,
                Rate = criteriaTypeSetting.Rate,
                Remark = criteriaTypeSetting.Remark,
                DateCreated = criteriaTypeSetting.DateCreated,
                CreatedBy = criteriaTypeSetting.CreatedBy,
                LastModified = criteriaTypeSetting.LastModified,
                ModifiedBy = criteriaTypeSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, InternalRecruitmentCriteriaTypeSetting setting)
        {
            ModelState.Clear();
            var checkExit = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x=>x.ExamSource == setting.ExamSource && x.ExamType == setting.ExamType);
            if(checkExit != null)
            {
                ModelState.AddModelError("","this setting is already exist");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(setting);
                db.InternalRecruitmentCriteriaTypeSettings.Add(entity);
                db.SaveChanges();
                
            }
            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, InternalRecruitmentCriteriaTypeSetting setting)
        {
            ModelState.Clear();
            var checkExit = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamSource == setting.ExamSource && x.ExamType == setting.ExamType && x.InternalRecruitmentCriteriaTypeSettingId != setting.InternalRecruitmentCriteriaTypeSettingId);
            if (checkExit != null)
            {
                ModelState.AddModelError("", "this setting is already exist");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(setting);
                db.InternalRecruitmentCriteriaTypeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, InternalRecruitmentCriteriaTypeSetting setting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(setting);

                #region delete log
                //  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment setting of " + configuration.Attribute);
                #endregion
                db.InternalRecruitmentCriteriaTypeSettings.Attach(entity);
                db.InternalRecruitmentCriteriaTypeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

         private InternalRecruitmentCriteriaTypeSetting GetMappedObject(InternalRecruitmentCriteriaTypeSetting criteriaTypeSetting)
        {
            var entity = new InternalRecruitmentCriteriaTypeSetting
            {
                InternalRecruitmentCriteriaTypeSettingId = criteriaTypeSetting.InternalRecruitmentCriteriaTypeSettingId,
                IsPercent = criteriaTypeSetting.IsPercent,
                Squence = criteriaTypeSetting.Squence,
                ExamType = criteriaTypeSetting.ExamType,
                SettingVlaue = criteriaTypeSetting.SettingVlaue,
                ExamSource = criteriaTypeSetting.ExamSource,
                Rate = criteriaTypeSetting.Rate,
                Remark = criteriaTypeSetting.Remark,
                DateCreated = criteriaTypeSetting.DateCreated,
                CreatedBy = criteriaTypeSetting.CreatedBy,
                LastModified = criteriaTypeSetting.LastModified,
                ModifiedBy = criteriaTypeSetting.ModifiedBy
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
