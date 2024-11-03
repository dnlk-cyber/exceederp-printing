using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class ApplicantEducationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmahric")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult ApplicantEducationLevel_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantEducation> applicantEducations = db.ApplicantEducation.Where(x=>x.ApplicantDetailId == id);


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, applicantEducation => new
            {
                ApplicantRegistrationId = applicantEducation.ApplicantRegistrationId,
                ApplicantDetailId = applicantEducation.ApplicantDetailId,
                ApplicantEducationId = applicantEducation.ApplicantEducationId,
                EducationLevelSettingId = applicantEducation.EducationLevelSettingId,
                EducationFieldSettingId = applicantEducation.EducationFieldSettingId,
                Remark = applicantEducation.Remark

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantEducationLevel_Create([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantEducation, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ApplicantEducation
                {
                    ApplicantEducationId = applicantEducation.ApplicantEducationId,
                    EducationLevelSettingId = applicantEducation.EducationLevelSettingId,
                    ApplicantRegistrationId = applicantEducation.ApplicantRegistrationId,
                    ApplicantDetailId = id,
                    EducationFieldSettingId = applicantEducation.EducationFieldSettingId,
                    Remark = applicantEducation.Remark
                };

                db.ApplicantEducation.Add(entity);
                db.SaveChanges();
                applicantEducation.ApplicantEducationId = entity.ApplicantEducationId;


            }
            return Json(new[] { applicantEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantEducationLevel_Update([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantEducation)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {


                var entity = new ApplicantEducation
                {
                    EducationLevelSettingId = applicantEducation.EducationLevelSettingId,
                    ApplicantRegistrationId = applicantEducation.ApplicantRegistrationId,
                    ApplicantEducationId = applicantEducation.ApplicantEducationId,
                    ApplicantDetailId = applicantEducation.ApplicantDetailId,
                    EducationFieldSettingId = applicantEducation.EducationFieldSettingId,
                    Remark = applicantEducation.Remark
                };

                db.ApplicantEducation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { applicantEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantEducationLevel_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantEducation)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new ApplicantEducation
                {

                    EducationLevelSettingId = applicantEducation.EducationLevelSettingId,
                    ApplicantRegistrationId = applicantEducation.ApplicantRegistrationId,
                    ApplicantEducationId = applicantEducation.ApplicantEducationId,
                    ApplicantDetailId = applicantEducation.ApplicantDetailId,
                    EducationFieldSettingId = applicantEducation.EducationFieldSettingId,
                    Remark = applicantEducation.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant Education information of applicant" + applicantEducation.ApplicantEducationId);
                #endregion
                db.ApplicantEducation.Attach(entity);
                db.ApplicantEducation.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { applicantEducation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
