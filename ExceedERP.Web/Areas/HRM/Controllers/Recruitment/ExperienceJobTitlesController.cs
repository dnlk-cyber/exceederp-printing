using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ExperienceJobTitlesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ExperienceJobTitles_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
              IQueryable<ExperienceJobTitle> applicantEducations = db.ExperienceJobTitles;


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, experienceJobTitle => new
            {
                ExperienceJobTitleId = experienceJobTitle.ExperienceJobTitleId,
                JobTitle = experienceJobTitle.JobTitle,
                Remark = experienceJobTitle.Remark,

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Create([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ExperienceJobTitle
                {
                    JobTitle = experienceJobTitle.JobTitle,
                    Remark = experienceJobTitle.Remark,
                };

                db.ExperienceJobTitles.Add(entity);
                db.SaveChanges();
                experienceJobTitle.ExperienceJobTitleId = entity.ExperienceJobTitleId;


            }
            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Update([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ExperienceJobTitle
                {
                    ExperienceJobTitleId = experienceJobTitle.ExperienceJobTitleId,
                    JobTitle = experienceJobTitle.JobTitle,
                    Remark = experienceJobTitle.Remark,
                };

                db.ExperienceJobTitles.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = new ExperienceJobTitle
                {
                    ExperienceJobTitleId = experienceJobTitle.ExperienceJobTitleId,
                    JobTitle = experienceJobTitle.JobTitle,
                    Remark = experienceJobTitle.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + experienceJobTitle.ExperienceJobTitleId);
                #endregion
                db.ExperienceJobTitles.Attach(entity);
                db.ExperienceJobTitles.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
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
