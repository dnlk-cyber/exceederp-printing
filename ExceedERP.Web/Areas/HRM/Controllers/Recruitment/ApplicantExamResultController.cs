using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class ApplicantExamResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private void Configartion(int applicationId)
        {
            ApplicantExamResult applicantScreeningResult = new ApplicantExamResult();
            List<ApplicantExamResult> applicantScreeningResultes = new List<ApplicantExamResult>();
            var setingvalues = db.Configurations.Where(x => x.Reference == "Recruitment" && x.Category == "Recruitment_External").ToList();
            var recruitmentSetting = Enum.GetValues(typeof(RecruitmentSetting))
                           .Cast<RecruitmentSetting>()
                           .ToList();
            if (recruitmentSetting.Any())
            {
                foreach (var p in recruitmentSetting)
                {
                    if (p.ToString() != "THEORY_EXAM" && p.ToString() != "PRACTICAL_EXAM" && p.ToString() != "INTERVIEW")
                    {
                        continue;
                    }

                    applicantScreeningResult = new ApplicantExamResult();
                   // applicantScreeningResult.Criteria = p.ToString();
                    if (setingvalues.Any())
                    {
                        var oneValue = setingvalues.Where(x => x.Attribute == p.ToString()).FirstOrDefault();
                        if (oneValue != null)
                        {
                            applicantScreeningResult.SettingValue = Convert.ToDecimal(oneValue.CurrentValue);
                        }
                    }
                  //  applicantScreeningResult.ReferenceType = "External";
                    applicantScreeningResult.CurrentResult = 0;
                 //   applicantScreeningResult.Total = 0;
                    applicantScreeningResult.ApplicantScreeningId = applicationId;
                    applicantScreeningResultes.Add(applicantScreeningResult);

                }
            }
            db = new ExceedDbContext();
            db.ApplicantExamResult.AddRange(applicantScreeningResultes);
            db.SaveChanges();
        }
        
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantExamResult_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantExamResult> applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id);
           
            DataSourceResult result = applicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                ApplicantResultId = applicantExamResult.ApplicantResultId,
                ApplicantScreeningId = applicantExamResult.ApplicantScreeningId,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                ExamType = applicantExamResult.ExamType,
                Remark = applicantExamResult.Remark,
            });

            return Json(result);
        }
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Create([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var settingValue = db.ExamTypeSettings.FirstOrDefault(x=>x.ExamType == applicantExamResult.ExamType && x.ExamSource == RecruitmentSource.EXTERNAL);
                if(settingValue != null)
                {
                    applicantExamResult.SettingValue = settingValue.SettingVlaue;
                }
                var entity = new ApplicantExamResult
                {
                    ApplicantScreeningId = id,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };

                db.ApplicantExamResult.Add(entity);
                db.SaveChanges();
                applicantExamResult.ApplicantResultId = entity.ApplicantResultId;
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Update([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == applicantExamResult.ExamType && x.ExamSource == RecruitmentSource.EXTERNAL);
                if (settingValue != null)
                {
                    applicantExamResult.SettingValue = settingValue.SettingVlaue;
                }

                var entity = new ApplicantExamResult
                {
                    ApplicantResultId = applicantExamResult.ApplicantResultId,
                    ApplicantScreeningId = applicantExamResult.ApplicantScreeningId,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };

                db.ApplicantExamResult.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantExamResult
                {
                    ApplicantResultId = applicantExamResult.ApplicantResultId,
                    ApplicantScreeningId = applicantExamResult.ApplicantScreeningId,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant exam result information of applicant" + applicantExamResult.ApplicantScreeningId);
                #endregion
                db.ApplicantExamResult.Attach(entity);
                db.ApplicantExamResult.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
