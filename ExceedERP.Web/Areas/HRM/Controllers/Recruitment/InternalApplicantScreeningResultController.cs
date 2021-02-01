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
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class InternalApplicantScreeningResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal EduLevel { get; set; }
        public static decimal EduField { get; set; }
        public static decimal RelevantExp { get; set; }
        public static decimal RelatedExp { get; set; }
        public static decimal Gender { get; set; }
        public static decimal Disabliy { get; set; }
        public static decimal Profile { get; set; }
        public static decimal Training { get; set; }
        public static decimal Dicipline { get; set; }
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
            var recruitment = db.RecruitmentVacancy.ToList();
            ViewData["recruitmentVacancy"] = recruitment;
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
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
            ViewData["Vacancy"] = list;
        }
        private void SetSettingValues()
        {
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment" && x.Reference == "Recruitment").ToList();
            if (settings.Any())
            {
                foreach (var s in settings)
                {
                    switch (s.Attribute)
                    {
                        case "EDUCATION_LEVEL":
                            EduLevel = Convert.ToDecimal(s.CurrentValue);
                            break;

                        case "EDUCATIONAL_FIELD":

                            EduField = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "RELATED_EXPERIENCE":

                            RelatedExp = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "RELEVANT_EXPERIENCE":

                            RelevantExp = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "GENDER":

                            Gender = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "DISABILITY":

                            Disabliy = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "PROFILE":

                            Profile = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "TRAINING":

                            Training = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "DISCIPLINE ":

                            Dicipline = Convert.ToDecimal(s.CurrentValue);
                            break;
                    }
                }

            }
        }
   
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantionFormes_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            SetSettingValues();
            var mediaCode = "";
            decimal screenSum = 0;
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApplicantionForm> applicantionformes = db.ApplicantionFormes.Where(x => mybraches.Contains(x.OrgBranchName));
            List<ApplicantionForm> applicantRegi = new List<ApplicantionForm>();
            var SettingSum = EduField + EduLevel + RelatedExp + RelevantExp + Disabliy + Gender + Profile + Training + Dicipline;

            if (id == "Screen")
            {
                foreach (var a in applicantionformes)
                {
                    db = new ExceedDbContext();
                    var screen =
                        db.ApplicantScreeningResult.Where(
                            x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
                            .ToList();
                    if (screen != null)
                    {

                        screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
                        a.TotalResult = screenSum;
                        var exam =
                            db.ApplicantExamResult.Where(
                                x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
                                .ToList();
                        if (exam != null)
                        {
                            decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
                        }
                        if (screenSum >= SettingSum)
                        {
                            applicantRegi.Add(a);


                        }
                    }

                }
                applicantionformes = applicantRegi.AsQueryable();
            }
            else
            {
                foreach (var a in applicantionformes)
                {
                    db = new ExceedDbContext();
                    var screen =
                        db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
                            .ToList();
                    if (screen.Any())
                    {

                        screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
                        a.TotalResult = screenSum;
                        var exam =
                           db.ApplicantExamResult.Where(
                               x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
                               .ToList();
                        if (exam.Any())
                        {
                            decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
                            a.TotalResult = a.TotalResult + exmSum;
                            screenSum = a.TotalResult + exmSum;
                        }
                        applicantRegi.Add(a);
                    }


                    var vacancyPlanforMedia = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == a.VacancyId);
                    var recruitmentMediaCode = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancyPlanforMedia.VacancyId);
                    if (recruitmentMediaCode != null)
                    {
                        mediaCode = recruitmentMediaCode.MediaVacancyCode;
                    }

                }
            }

            applicantionformes = applicantionformes;

            DataSourceResult result = applicantionformes.ToDataSourceResult(request, applicantionForm => new
            {
                ApplicantionFormId = applicantionForm.ApplicantionFormId,
                EmployeeId = applicantionForm.EmployeeId,
                ApplicationType = applicantionForm.ApplicationType,
                VacancyId = applicantionForm.VacancyId,
                TotalResult = applicantionForm.TotalResult,
                Remark = applicantionForm.Remark,
                MediaVacancyCode = mediaCode,
                EmployeeCode = applicantionForm.EmployeeCode,
                LookupId = applicantionForm.LookupId,
            });

            return Json(result);
        }
        private void Configartion(int applicationId)
        {
            ApplicantScreeningResult applicantScreeningResult = new ApplicantScreeningResult();
            List<ApplicantScreeningResult> applicantScreeningResultes = new List<ApplicantScreeningResult>();
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

                        applicantScreeningResult = new ApplicantScreeningResult();
                        applicantScreeningResult.Criteria = p.ToString();
                        if (setingvalues.Any())
                        {
                            var oneValue = setingvalues.Where(x => x.Attribute == p.ToString()).FirstOrDefault();
                            if (oneValue != null)
                            {
                                applicantScreeningResult.SettingValue = Convert.ToDecimal(oneValue.CurrentValue);
                            }
                        }
                        if (applicantScreeningResult.Criteria == "GENDER")
                        {
                            applicantScreeningResult.CurrentResult = GenderSetting(applicationId);
                        }
                        else
                        { applicantScreeningResult.CurrentResult = 0; }
                        applicantScreeningResult.ReferenceType = "Internal";

                        applicantScreeningResult.TotalResult = 0;
                        applicantScreeningResult.ApplicantRegistrationId = applicationId;
                        applicantScreeningResultes.Add(applicantScreeningResult);
                    }
                }
            }
            db = new ExceedDbContext();
            db.ApplicantScreeningResult.AddRange(applicantScreeningResultes);
            db.SaveChanges();
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantScreeningResult_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var applicant = db.ApplicantionFormes.Where(x => x.ApplicantionFormId == id).FirstOrDefault().EmployeeId;
            var personemployee = db.Person_Employee.Where(x => x.EmployeeId == applicant).FirstOrDefault().Gender;
            decimal genderSetting = 0;
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment" && x.Attribute == "GENDER").FirstOrDefault();
            if (settings != null)
            {
                if (personemployee == ExceedERP.Core.Domain.Common.Gender.FEMALE)
                {
                    genderSetting = Convert.ToDecimal(settings.CurrentValue);
                }
            }
            IQueryable<ApplicantScreeningResult> applicantexamresult = db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "Internal");
            if (!applicantexamresult.Any())
            {
                Configartion(id);
                applicantexamresult = db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "Internal");
            }
            DataSourceResult result = applicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                ScreeningResultId = applicantExamResult.ScreeningResultId,
                ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                Criteria = applicantExamResult.Criteria,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                Remark = applicantExamResult.Remark
                    ,
                ReferenceType = "Internal"
            });

            return Json(result);
        }
        private decimal GenderSetting(int id)
        {
            var applicant = db.ApplicantionFormes.Where(x => x.ApplicantionFormId == id).FirstOrDefault().EmployeeId;
            decimal genderSetting = 0;
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment" && x.Attribute == "GENDER").FirstOrDefault();
            if (settings != null)
            {
                var person = db.Person_Employee.Where(x => x.EmployeeId == applicant).FirstOrDefault();
                if (person != null)
                {
                    if (person.Gender == ExceedERP.Core.Domain.Common.Gender.FEMALE)
                    {
                        genderSetting = Convert.ToDecimal(settings.CurrentValue);
                    }
                }
            }
            return genderSetting;
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Create([DataSourceRequest]DataSourceRequest request, ApplicantScreeningResult applicantScreeningResult, int id)
        {
            if (ModelState.IsValid)
            {
                if (applicantScreeningResult.SettingValue < applicantScreeningResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new ApplicantScreeningResult
                {
                    ApplicantRegistrationId = id,
                    Criteria = applicantScreeningResult.Criteria,
                    SettingValue = applicantScreeningResult.SettingValue,
                    CurrentResult = applicantScreeningResult.CurrentResult,
                    Remark = applicantScreeningResult.Remark,
                    TotalResult = applicantScreeningResult.TotalResult,
                    ReferenceType = "Internal"
                };

                db.ApplicantScreeningResult.Add(entity);
                db.SaveChanges();
                applicantScreeningResult.ScreeningResultId = entity.ScreeningResultId;
            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Update([DataSourceRequest]DataSourceRequest request, ApplicantScreeningResult applicantScreeningResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (applicantScreeningResult.SettingValue < applicantScreeningResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new ApplicantScreeningResult
                {
                    ScreeningResultId = applicantScreeningResult.ScreeningResultId,
                    ApplicantRegistrationId = applicantScreeningResult.ApplicantRegistrationId,
                    Criteria = applicantScreeningResult.Criteria,
                    SettingValue = applicantScreeningResult.SettingValue,
                    CurrentResult = applicantScreeningResult.CurrentResult,
                    Remark = applicantScreeningResult.Remark,
                    TotalResult = applicantScreeningResult.CurrentResult,
                    ReferenceType = "Internal"
                };

                db.ApplicantScreeningResult.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantScreeningResult applicantScreeningResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantScreeningResult
                {
                    ScreeningResultId = applicantScreeningResult.ScreeningResultId,
                    ApplicantRegistrationId = applicantScreeningResult.ApplicantRegistrationId,
                    Criteria = applicantScreeningResult.Criteria,
                    SettingValue = applicantScreeningResult.SettingValue,
                    CurrentResult = applicantScreeningResult.CurrentResult,
                    Remark = applicantScreeningResult.Remark
                    ,
                    TotalResult = applicantScreeningResult.CurrentResult,
                    ReferenceType = "Internal"
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted internal screeining result information of applicant  " + applicantScreeningResult.ApplicantRegistrationId);
                #endregion
                db.ApplicantScreeningResult.Attach(entity);
                db.ApplicantScreeningResult.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
