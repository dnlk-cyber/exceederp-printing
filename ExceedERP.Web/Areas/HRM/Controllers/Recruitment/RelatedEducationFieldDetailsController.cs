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
    public class RelatedEducationFieldDetailsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();


        // GET: HRM/ExperienceJobTitleDetails/id
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
              IQueryable<RelatedEducationFieldDetail> educationFields = db.RelatedEducationFieldDetails.Where(x=>x.ExperienceJobTitleId==id);


            DataSourceResult result = educationFields.ToDataSourceResult(request, educationField => new
            {
                RelatedEducationFieldDetailId = educationField.RelatedEducationFieldDetailId,
                ExperienceJobTitleId = educationField.ExperienceJobTitleId,
                LookupId = educationField.LookupId,
                //ExamType = educationField.ExamType,
                Remark = educationField.Remark,
                CreatedBy = educationField.CreatedBy,
                DateCreated = educationField.DateCreated,
                ModifiedBy = educationField.ModifiedBy,
                LastModified = educationField.LastModified,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, RelatedEducationFieldDetail experienceJobTitle,int id)
        {
            ModelState.Clear();
            experienceJobTitle.ExperienceJobTitleId = id;
            if (ModelState.IsValid)
            {
                experienceJobTitle.DateCreated = DateTime.Now;
                experienceJobTitle.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.RelatedEducationFieldDetails.Add(entity);
                db.SaveChanges();
                experienceJobTitle.ExperienceJobTitleId = entity.ExperienceJobTitleId;


            }
            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, RelatedEducationFieldDetail experienceJobTitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                experienceJobTitle.LastModified = DateTime.Now;
                experienceJobTitle.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(experienceJobTitle);
                db.RelatedEducationFieldDetails.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, RelatedEducationFieldDetail experienceJobTitle)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(experienceJobTitle);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + experienceJobTitle.ExperienceJobTitleId);
                #endregion
                db.RelatedEducationFieldDetails.Attach(entity);
                db.RelatedEducationFieldDetails.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { experienceJobTitle }.ToDataSourceResult(request, ModelState));
        }

        private RelatedEducationFieldDetail GetMappedObject(RelatedEducationFieldDetail educationField)
        {
            var entity = new RelatedEducationFieldDetail
            {
                RelatedEducationFieldDetailId = educationField.RelatedEducationFieldDetailId,
                ExperienceJobTitleId = educationField.ExperienceJobTitleId,
                LookupId = educationField.LookupId,
                //ExamType = educationField.ExamType,
                Remark = educationField.Remark,
                CreatedBy = educationField.CreatedBy,
                DateCreated = educationField.DateCreated,
                ModifiedBy = educationField.ModifiedBy,
                LastModified = educationField.LastModified,
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
