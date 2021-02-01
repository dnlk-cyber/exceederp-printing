﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class InternalApplicantExamResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);

            var appTypes = Enum.GetValues(typeof(ApplicationType))
               .Cast<ApplicationType>().ToList();
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            ViewData["Media"] = media;
            var appTypeses = appTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["AppTypes"] = appTypeses;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var ranks = db.Ranks.ToList();
            ViewData["RankList"] = ranks;
            List<Vacancy> list = new List<Vacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                var recruit = db.VaccancyPlans.ToList();
                var vac = new Vacancy();
                foreach (var r in recruit)
                {
                    foreach (var v in vacancies)
                    {
                        var vacancydetail = db.VaccancyPlans.Where(x => x.VacancyId == v.VacancyId).FirstOrDefault();
                        if (vacancydetail != null)
                        {
                            if (vacancydetail.RecruSource == RecruitmentSource.INTERNAL)
                            {
                                if (v.VacancyId == r.VacancyId)
                                {
                                    vac = new Vacancy();
                                    vac = vacancydetail;
                                    list.Add(vac);
                                }
                            }
                        }
                    }
                }

            }
            ViewData["Vacancy"] = list;
        }
        private void Configartion(int applicationId)
        {
            ApplicantExamResult applicantScreeningResult = new ApplicantExamResult();
            List<ApplicantExamResult> applicantScreeningResultes = new List<ApplicantExamResult>();
            var setingvalues = db.Configurations.Where(x => x.Reference == "Recruitment" && x.Category == "Recruitment").ToList();
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
                    applicantScreeningResult.ReferenceType = "Internal";
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
            IQueryable<ApplicantExamResult> applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "Internal");
            if (!applicantexamresult.Any())
            {
                Configartion(id);
                applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "Internal");
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
                ReferenceType = "Internal"
            });

            return Json(result);
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
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
                    ReferenceType = "Internal"
                };

                db.ApplicantExamResult.Add(entity);
                db.SaveChanges();
                applicantExamResult.ApplicantResultId = entity.ApplicantResultId;
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
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
                    ReferenceType = "Internal"
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
                    ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                    Criteria = applicantExamResult.Criteria,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,

                    Remark = applicantExamResult.Remark,
                    ReferenceType = "Internal"
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted internal applicant information of applicant  " + applicantExamResult.ApplicantRegistrationId);
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
