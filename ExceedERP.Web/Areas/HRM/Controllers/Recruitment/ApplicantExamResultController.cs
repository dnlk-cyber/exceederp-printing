using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

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
                    applicantScreeningResult.Criteria = p.ToString();
                    if (setingvalues.Any())
                    {
                        var oneValue = setingvalues.Where(x => x.Attribute == p.ToString()).FirstOrDefault();
                        if (oneValue != null)
                        {
                            applicantScreeningResult.SettingValue = Convert.ToDecimal(oneValue.CurrentValue);
                        }
                    }
                    applicantScreeningResult.ReferenceType = "External";
                    applicantScreeningResult.CurrentResult = 0;
                    applicantScreeningResult.Total = 0;
                    applicantScreeningResult.ApplicantRegistrationId = applicationId;
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
            IQueryable<ApplicantExamResult> applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External");
            if (!applicantexamresult.Any())
            {
                Configartion(id);
                applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External");
            }
            DataSourceResult result = applicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                ApplicantResultId = applicantExamResult.ApplicantResultId,
                ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                Criteria = applicantExamResult.Criteria,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                Total = applicantExamResult.Total,
                Remark = applicantExamResult.Remark,
                ReferenceType = "External"
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
                if (applicantExamResult.SettingValue < applicantExamResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new ApplicantExamResult
                {
                    ApplicantRegistrationId = id,
                    Criteria = applicantExamResult.Criteria,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,

                    Remark = applicantExamResult.Remark,
                    ReferenceType = "External"
                };

                db.ApplicantExamResult.Add(entity);
                db.SaveChanges();
                applicantExamResult.ApplicantResultId = entity.ApplicantResultId;

                #region operation log

                var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                if (applicant != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Applicant Exam Result ", "Success", User.Identity.Name, User.Identity.Name + " add new applicant exam result for " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                }

                #endregion

            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Update([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                if (applicantExamResult.SettingValue < applicantExamResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new ApplicantExamResult
                {
                    ApplicantResultId = applicantExamResult.ApplicantResultId,
                    ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                    Criteria = applicantExamResult.Criteria,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,

                    Remark = applicantExamResult.Remark,
                    ReferenceType = "External"
                };

                db.ApplicantExamResult.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                if (applicant != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Applicant Exam Result ", "Success", User.Identity.Name, User.Identity.Name + " update applicant exam result of " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);

                }

                #endregion

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
                    ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                    Criteria = applicantExamResult.Criteria,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,

                    Remark = applicantExamResult.Remark,
                    ReferenceType = "External"
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant exam result information of applicant" + applicantExamResult.ApplicantRegistrationId);
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
