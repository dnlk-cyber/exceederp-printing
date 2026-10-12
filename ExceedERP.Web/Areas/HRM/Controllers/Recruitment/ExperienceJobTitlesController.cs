using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    [Authorize]
    public class ExperienceJobTitlesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            ViewData["jobTitle"] = db.Lookups.
                Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                            ||x.LookupType ==LookUpTypes.OLD_JOB_TITLE
                            ||x.LookupType ==LookUpTypes.Jobtitle_External_Experience)
                            .Select(x=>new
                            {
                                x.LookupId,
                                RelatedJobTitleExperienceId=x.LookupId,
                                RelevantJobTitleExperienceId = x.LookupId,
                                x.Description }).ToList();
            ViewData["field"] = db.Lookups
                            .Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD)
                            .Select(x => new
                            {
                                 x.LookupId,
                                EducationFieldId = x.LookupId,
                                RelatedEducationFieldId =x.LookupId,
                                x.Description
                            }).ToList();
            ViewData["level"] = db.Lookups
                .Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL)
                .Select(x => new
                {
                    EducationLevelId = x.LookupId,
                    x.LookupId,
                    x.Description
                }).ToList();


            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ExperienceJobTitles_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
              IQueryable<ExperienceJobTitle> jobTitles = db.ExperienceJobTitles;

            DataSourceResult result = jobTitles.ToDataSourceResult(request, jobTitle => new
            {
                ExperienceJobTitleId = jobTitle.ExperienceJobTitleId,
                LookupId = jobTitle.LookupId,
                Remark = jobTitle.Remark,
                CreatedBy = jobTitle.CreatedBy,
                DateCreated = jobTitle.DateCreated,
                ModifiedBy = jobTitle.ModifiedBy,
                LastModified = jobTitle.LastModified,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Create([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {

            if (db.ExperienceJobTitles.Any(x => x.LookupId == experienceJobTitle.LookupId))
            {

                ModelState.AddModelError("exists", "Dublication is not Allowed ");

                return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                experienceJobTitle.DateCreated = DateTime.Now;
                experienceJobTitle.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.ExperienceJobTitles.Add(entity);
                db.SaveChanges();
                experienceJobTitle.ExperienceJobTitleId = entity.ExperienceJobTitleId;


            }
            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Update([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {
            if (db.ExperienceJobTitles.Any(x => x.LookupId == experienceJobTitle.LookupId
                                                &&x.ExperienceJobTitleId !=experienceJobTitle.ExperienceJobTitleId))
            {

                ModelState.AddModelError("exists", "Dublication is not Allowed ");
                return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                experienceJobTitle.LastModified = DateTime.Now;
                experienceJobTitle.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.ExperienceJobTitles.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExperienceJobTitles_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceJobTitle experienceJobTitle)
        {
            if (db.VaccancyPlans.Where(x => x.JobTitle == experienceJobTitle.LookupId).Any())
            {
                ModelState.AddModelError("exists", "You can't delete this JobTitle; it is used in Vaccancies.");
                return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(experienceJobTitle);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + experienceJobTitle.ExperienceJobTitleId);
                #endregion
                db.ExperienceJobTitles.Attach(entity);
                db.ExperienceJobTitles.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }

        private ExperienceJobTitle GetMappedObject(ExperienceJobTitle jobTitle)
        {
            var entity = new ExperienceJobTitle
            {
                ExperienceJobTitleId = jobTitle.ExperienceJobTitleId,
                LookupId = jobTitle.LookupId,
                //EducationFieldId = jobTitle.EducationFieldId,
                //EducationLevelId = jobTitle.EducationLevelId,
                //RelatedEducationFieldId = jobTitle.RelatedEducationFieldId,
                //RelatedJobTitleExperienceId = jobTitle.RelatedJobTitleExperienceId,
                //RelevantJobTitleExperienceId = jobTitle.RelevantJobTitleExperienceId,
                Remark = jobTitle.Remark,
                CreatedBy = jobTitle.CreatedBy,
                DateCreated = jobTitle.DateCreated,
                ModifiedBy = jobTitle.ModifiedBy,
                LastModified = jobTitle.LastModified,
            };
            return entity;
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
