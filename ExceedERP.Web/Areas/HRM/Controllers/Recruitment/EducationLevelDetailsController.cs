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
    public class EducationLevelDetailsController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult EducationLevelDetailsForVacancy_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var educationFields = db.EducationLevelDetails
                .Where(x => x.ExperienceJob.LookupId == id);

            var result = educationFields.ToDataSourceResult(request, educationField => new
            {
                EducationLevelDetailId = educationField.EducationLevelDetailId,
                ExperienceJobTitleId = educationField.ExperienceJobTitleId,
                LookupId = educationField.LookupId,
                Experience = educationField.Experience,
                //ExamType = educationField.ExamType,
                Remark = educationField.Remark,
                CreatedBy = educationField.CreatedBy,
                DateCreated = educationField.DateCreated,
                ModifiedBy = educationField.ModifiedBy,
                LastModified = educationField.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EducationLevelDetails_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var educationFields = db.EducationLevelDetails
                .Where(x => x.ExperienceJobTitleId == id);

            var result = educationFields.ToDataSourceResult(request, educationField => new
            {
                EducationLevelDetailId = educationField.EducationLevelDetailId,
                ExperienceJobTitleId = educationField.ExperienceJobTitleId,
                LookupId = educationField.LookupId,
                Experience = educationField.Experience,
                //ExamType = educationField.ExamType,
                Remark = educationField.Remark,
                CreatedBy = educationField.CreatedBy,
                DateCreated = educationField.DateCreated,
                ModifiedBy = educationField.ModifiedBy,
                LastModified = educationField.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EducationLevelDetails_Create([DataSourceRequest] DataSourceRequest request, EducationLevelDetail educationLevelDetail, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                educationLevelDetail.ExperienceJobTitleId = id;
                educationLevelDetail.DateCreated = DateTime.Now;
                educationLevelDetail.CreatedBy = User.Identity.Name;
                db.EducationLevelDetails.Add(educationLevelDetail);
                db.SaveChanges();
            }

            return Json(new[] { educationLevelDetail }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult EducationLevelDetails_Update([DataSourceRequest] DataSourceRequest request, EducationLevelDetail educationLevelDetail)
        {
            ModelState.Clear();
            var educationLevel = db.EducationLevelDetails.Find(educationLevelDetail.EducationLevelDetailId);
            if (ModelState.IsValid
                && educationLevel !=null)
            {
                educationLevel.Experience = educationLevelDetail.Experience;
                educationLevel.LastModified = DateTime.Now;
                educationLevel.ModifiedBy = User.Identity.Name;

                db.Entry(educationLevel).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { educationLevelDetail }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult EducationLevelDetails_Destroy([DataSourceRequest] DataSourceRequest request, EducationLevelDetail educationLevelDetail)
        {

            if (ModelState.IsValid)
            {
                var entity = db.EducationLevelDetails.Find(educationLevelDetail.EducationLevelDetailId);
                if (entity != null)
                {
                    // Log the deletion
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                        $"Deleted Education Level with ID {educationLevelDetail.ExperienceJobTitleId}");

                    db.EducationLevelDetails.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { educationLevelDetail }.ToDataSourceResult(request, ModelState));
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


