using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ScreeningExperienceJobtitlesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ScreeningExperienceJobtitles_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            int sId = 0;
            if (!String.IsNullOrEmpty(id))
            {
                sId = int.Parse(id);
            }
            IQueryable<ScreeningExperienceJobtitle> applicantEducations = db.ScreeningExperienceJobtitles.Where(x => x.ScreeingSettingId == sId);


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, screeningExperienceJobtitle => new
            {

                ScreeningExperienceJobtitleId = screeningExperienceJobtitle.ScreeningExperienceJobtitleId,
                ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                Remark = screeningExperienceJobtitle.Remark,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningExperienceJobtitles_Create([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitle screeningExperienceJobtitle, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitle
                {
                    ScreeningExperienceJobtitleId = screeningExperienceJobtitle.ScreeningExperienceJobtitleId,
                    ScreeingSettingId = id,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                db.ScreeningExperienceJobtitles.Add(entity);
                db.SaveChanges();
                screeningExperienceJobtitle.ScreeningExperienceJobtitleId = entity.ScreeningExperienceJobtitleId;


            }
            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SScreeningExperienceJobtitles_Update([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitle screeningExperienceJobtitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitle
                {
                    ScreeningExperienceJobtitleId = screeningExperienceJobtitle.ScreeningExperienceJobtitleId,
                    ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                db.ScreeningExperienceJobtitles.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningExperienceJobtitles_Destroy([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitle screeningExperienceJobtitle)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitle
                {
                    ScreeningExperienceJobtitleId = screeningExperienceJobtitle.ScreeningExperienceJobtitleId,
                    ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + screeningExperienceJobtitle.ScreeningExperienceJobtitleId);
                #endregion
                db.ScreeningExperienceJobtitles.Attach(entity);
                db.ScreeningExperienceJobtitles.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// ///////////
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ScreeningExperienceJobtitleForInternal_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            int sId = 0;
            if (!String.IsNullOrEmpty(id))
            {
                sId = int.Parse(id);
            }
            IQueryable<ScreeningExperienceJobtitleForInternal> applicantEducations = db.ScreeningExperienceJobtitleForInternals.Where(x => x.ScreeingSettingId == sId);


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, screeningExperienceJobtitle => new
            {
                ScreeningExperienceJobtitleForInternalId = screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId,
                ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                Remark = screeningExperienceJobtitle.Remark,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningExperienceJobtitleForInternal_Create([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitleForInternal screeningExperienceJobtitle, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitleForInternal
                {
                    ScreeingSettingId = id,
                    ScreeningExperienceJobtitleForInternalId = screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                db.ScreeningExperienceJobtitleForInternals.Add(entity);
                db.SaveChanges();
                screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId = entity.ScreeningExperienceJobtitleForInternalId;


            }
            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningExperienceJobtitleForInternal_Update([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitleForInternal screeningExperienceJobtitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitleForInternal
                {
                    ScreeningExperienceJobtitleForInternalId = screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId,
                    ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                db.ScreeningExperienceJobtitleForInternals.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningExperienceJobtitleForInternal_Destroy([DataSourceRequest]DataSourceRequest request, ScreeningExperienceJobtitleForInternal screeningExperienceJobtitle)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new ScreeningExperienceJobtitleForInternal
                {
                    ScreeningExperienceJobtitleForInternalId = screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId,
                    ScreeingSettingId = screeningExperienceJobtitle.ScreeingSettingId,
                    ExperienceJobTitleId = screeningExperienceJobtitle.ExperienceJobTitleId,
                    Remark = screeningExperienceJobtitle.Remark,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + screeningExperienceJobtitle.ScreeningExperienceJobtitleForInternalId);
                #endregion
                db.ScreeningExperienceJobtitleForInternals.Attach(entity);
                db.ScreeningExperienceJobtitleForInternals.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { screeningExperienceJobtitle }.ToDataSourceResult(request, ModelState));
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
