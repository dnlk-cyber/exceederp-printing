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
    public class ScreeningEducationLevelsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ScreeningEducationLevels_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            int sId = 0;
            if (!String.IsNullOrEmpty(id))
            {
                sId = int.Parse(id);
            }

            IQueryable<ScreeningEducationLevel> applicantEducations = db.ScreeningEducationLevels.Where(x=>x.ScreeingSettingId == sId);


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, screeningEducationLevel => new
            {
                ScreeningEducationLevelId = screeningEducationLevel.ScreeningEducationLevelId,
                ScreeingSettingId = screeningEducationLevel.ScreeingSettingId,
                EducationLevelId = screeningEducationLevel.EducationLevelId,
                Remark = screeningEducationLevel.Remark,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationLevels_Create([DataSourceRequest]DataSourceRequest request, ScreeningEducationLevel screeningEducationLevel, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationLevel
                {
                    ScreeningEducationLevelId = screeningEducationLevel.ScreeningEducationLevelId,
                    ScreeingSettingId = id,
                    EducationLevelId = screeningEducationLevel.EducationLevelId,
                    Remark = screeningEducationLevel.Remark,
                };

                db.ScreeningEducationLevels.Add(entity);
                db.SaveChanges();
                screeningEducationLevel.ScreeningEducationLevelId = entity.ScreeningEducationLevelId;


            }
            return Json(new[] { screeningEducationLevel }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationLevels_Update([DataSourceRequest]DataSourceRequest request, ScreeningEducationLevel screeningEducationLevel)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationLevel
                {
                    ScreeningEducationLevelId = screeningEducationLevel.ScreeningEducationLevelId,
                    ScreeingSettingId = screeningEducationLevel.ScreeingSettingId,
                    EducationLevelId = screeningEducationLevel.EducationLevelId,
                    Remark = screeningEducationLevel.Remark,
                };

                db.ScreeningEducationLevels.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { screeningEducationLevel }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationLevels_Destroy([DataSourceRequest]DataSourceRequest request, ScreeningEducationLevel screeningEducationLevel)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationLevel
                {
                    ScreeningEducationLevelId = screeningEducationLevel.ScreeningEducationLevelId,
                    ScreeingSettingId = screeningEducationLevel.ScreeingSettingId,
                    EducationLevelId = screeningEducationLevel.EducationLevelId,
                    Remark = screeningEducationLevel.Remark,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + screeningEducationLevel.ScreeningEducationLevelId);
                #endregion
                db.ScreeningEducationLevels.Attach(entity);
                db.ScreeningEducationLevels.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { screeningEducationLevel }.ToDataSourceResult(request, ModelState));
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
