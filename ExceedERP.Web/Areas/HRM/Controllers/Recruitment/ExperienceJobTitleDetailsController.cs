using System;
using System.Data;
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
    [Authorize]
    public class ExperienceJobTitleDetailsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();


        // GET: HRM/ExperienceJobTitleDetails/id
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
              IQueryable<ExperienceJobTitleDetail> jobTitles = db.ExperienceJobTitleDetails.Where(x=>x.ExperienceJobTitleId==id);


            DataSourceResult result = jobTitles.ToDataSourceResult(request, jobTitle => new
            {
                ExperienceJobTitleDetailId = jobTitle.ExperienceJobTitleDetailId,
                ExperienceJobTitleId = jobTitle.ExperienceJobTitleId,
                LookupId = jobTitle.LookupId,
                ExamType = jobTitle.ExamType,
                Remark = jobTitle.Remark,
                CreatedBy = jobTitle.CreatedBy,
                DateCreated = jobTitle.DateCreated,
                ModifiedBy = jobTitle.ModifiedBy,
                LastModified = jobTitle.LastModified
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, ExperienceJobTitleDetail experienceJobTitle,int id)
        {
            experienceJobTitle.ExperienceJobTitleId = id;
            if (ModelState.IsValid)
            {
                experienceJobTitle.DateCreated = DateTime.Now;
                experienceJobTitle.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.ExperienceJobTitleDetails.Add(entity);
                db.SaveChanges();
                experienceJobTitle.ExperienceJobTitleId = entity.ExperienceJobTitleId;


            }
            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, ExperienceJobTitleDetail experienceJobTitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                experienceJobTitle.LastModified = DateTime.Now;
                experienceJobTitle.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.ExperienceJobTitleDetails.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, ExperienceJobTitleDetail experienceJobTitle)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(experienceJobTitle);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + experienceJobTitle.ExperienceJobTitleId);
                #endregion
                db.ExperienceJobTitleDetails.Attach(entity);
                db.ExperienceJobTitleDetails.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }

        private ExperienceJobTitleDetail GetMappedObject(ExperienceJobTitleDetail jobTitle)
        {
            var entity = new ExperienceJobTitleDetail
            {
                ExperienceJobTitleDetailId = jobTitle.ExperienceJobTitleDetailId,
                ExperienceJobTitleId = jobTitle.ExperienceJobTitleId,
                LookupId = jobTitle.LookupId,
                ExamType = jobTitle.ExamType,
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
