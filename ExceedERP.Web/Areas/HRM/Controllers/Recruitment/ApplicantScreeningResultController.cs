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
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class ApplicantScreeningResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            List<Vacancy> list = new List<Vacancy>();
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();

            ViewData["Vacancy"] = vacancies;
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
        }
        private void GetEmployeRank(int applicantId)
        {
            decimal total;
            decimal passMark=0; 
            var configration = db.Configurations.Where(x => x.Attribute == "Promotion" && x.Category == "Promotion" && x.Reference == "PASS_MARK").FirstOrDefault();
            if (configration != null)
            {
                passMark = Convert.ToDecimal(configration.CurrentValue);
            }
            var extApplican = db.ApplicantRegistration.Where(x => x.ApplicantRegistrationId == applicantId).ToList();
            if (extApplican.Any())
            {
                foreach (var a in extApplican)
                {
                    db = new ExceedDbContext();
                    ApplicantTotalResult oneapplicant = new ApplicantTotalResult();
                    total = 0;


                    var externalApplcant =
                        db.ApplicantScreeningResult.Where(
                            x =>
                                x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                    if (externalApplcant.Any())
                    {
                        foreach (var i in externalApplcant)
                        {

                            switch (i.Criteria)
                            {
                                case "DISABILITY":
                                    oneapplicant.Disability = i.CurrentResult;
                                    break;
                                case "DISCIPLINE":
                                    oneapplicant.Discipline = i.CurrentResult;
                                    break;
                                case "GENDER":
                                    oneapplicant.Gender = i.CurrentResult;
                                    break;
                                case "EDUCATIONAL_FIELD":
                                    oneapplicant.EduField = i.CurrentResult;
                                    break;
                                case "EDUCATION_LEVEL":
                                    oneapplicant.EduLevel = i.CurrentResult;
                                    break;
                                case "PROFILE":
                                    oneapplicant.Profile = i.CurrentResult;
                                    break;
                                case "TRAINING":
                                    oneapplicant.Training = i.CurrentResult;
                                    break;
                                case "RELATED_EXPERIENCE":
                                    oneapplicant.RelatedExperience = i.CurrentResult;
                                    break;
                                case "RELEVANT_EXPERIENCE":
                                    oneapplicant.RelevantExperience = i.CurrentResult;
                                    break;
                                case "NATION":
                                    oneapplicant.Nation = i.CurrentResult;
                                    break;

                            }
                        }
                    }
                    var externalApplcantexam =
                        db.ApplicantExamResult.Where(
                            x =>
                                x.ApplicantRegistrationId == a.ApplicantRegistrationId
                        ).ToList();
                    if (externalApplcantexam.Any())
                    {
                        foreach (var i in externalApplcantexam)
                        {

                            switch (i.Criteria)
                            {
                                case "THEORY_EXAM":
                                    oneapplicant.TheoryExam = i.CurrentResult;
                                    break;
                                case "PRACTICAL_EXAM":
                                    oneapplicant.PracticalExam = i.CurrentResult;
                                    break;
                                case "INTERVIEW":
                                    oneapplicant.Interview = i.CurrentResult;
                                    break;


                            }

                        }


                    }
                    total = oneapplicant.Interview + oneapplicant.PracticalExam +
                            oneapplicant.TheoryExam +
                            oneapplicant.Disability + oneapplicant.Gender + oneapplicant.EduField +
                            oneapplicant.EduLevel +
                            oneapplicant.RelatedExperience + oneapplicant.RelevantExperience +
                            oneapplicant.Profile + oneapplicant.Training + oneapplicant.Discipline + oneapplicant.Nation;
                    oneapplicant.Total = total;
                    if (oneapplicant.Total >= passMark)
                    {
                        oneapplicant.RecruitmentResult = RecruitmentResult.PASS;
                    }
                    else
                    {
                        oneapplicant.RecruitmentResult = RecruitmentResult.FAIL;
                    }

                }
            }



        }


        private void Configartion(int applicationId)
        {
            ApplicantScreeningResult applicantScreeningResult = new ApplicantScreeningResult();
            List<ApplicantScreeningResult> applicantScreeningResultes = new List<ApplicantScreeningResult>();
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

                        applicantScreeningResult.ReferenceType = "External";
                        if (applicantScreeningResult.Criteria == "GENDER")
                        {
                            applicantScreeningResult.CurrentResult = GenderSetting(applicationId);
                        }
                        else
                        {
                            applicantScreeningResult.CurrentResult = 0;
                        }

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
            var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == id).Sex;

            decimal genderSetting = 0;
            var settings =
                db.Configurations.FirstOrDefault(x => x.Category == "Recruitment_External" && x.Attribute == "GENDER");
            if (settings != null)
            {
                if (applicant == Gender.FEMALE)
                {
                    genderSetting = Convert.ToDecimal(settings.CurrentValue);
                }
            }
            IQueryable<ApplicantScreeningResult> applicantexamresult = db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External");
            if (!applicantexamresult.Any())
            {
                Configartion(id);
                applicantexamresult = db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External");
            }
            DataSourceResult result = applicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                ScreeningResultId = applicantExamResult.ScreeningResultId,
                ApplicantRegistrationId = applicantExamResult.ApplicantRegistrationId,
                Criteria = applicantExamResult.Criteria,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                //VacancyCode = applicantExamResult.ApplicantRegistration.VacancyCode,
                Remark = applicantExamResult.Remark,
                ReferenceType = "External",
                TotalResult = applicantExamResult.TotalResult
            });

            return Json(result);
        }

        private decimal GenderSetting(int id)
        {
            var applicant = db.ApplicantRegistration.Where(x => x.ApplicantRegistrationId == id).FirstOrDefault().Sex;
            decimal genderSetting = 0;
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment_External" && x.Attribute == "GENDER").FirstOrDefault();
            if (settings != null)
            {
                if (applicant == Gender.FEMALE)
                {
                    genderSetting = Convert.ToDecimal(settings.CurrentValue);
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
                    ScreeningResultId = applicantScreeningResult.ScreeningResultId,

                    Criteria = applicantScreeningResult.Criteria,
                    SettingValue = applicantScreeningResult.SettingValue,
                    CurrentResult = applicantScreeningResult.CurrentResult,
                    Remark = applicantScreeningResult.Remark,
                    ReferenceType = "External",
                    TotalResult = applicantScreeningResult.TotalResult
                };

                db.ApplicantScreeningResult.Add(entity);
                db.SaveChanges();

                applicantScreeningResult.ScreeningResultId = entity.ScreeningResultId;

            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ApplicantRegistration> products)
        {
            ModelState.Clear();
            //var results = new List<Vacancy>();
            Person_Employee personEmployee = new Person_Employee();
            Education education;
            Experience experience = new Experience();
            Training training = new Training();
            Address address = new Address();
            List<Person_Employee> personEmployeeList = new List<Person_Employee>();
            

            if (products != null && ModelState.IsValid)
            {
                string code = "NewEmploye " + DateTime.Now.Year;


                foreach (var product in products)
                {
                    if (product.Check)
                    {
                        personEmployee = new Person_Employee();     
        
                        personEmployee.EmployeeCode = code;
                        personEmployee.IsActive = false;
                        personEmployee.Gender = product.Sex;
                        personEmployee.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        personEmployee.CreatedBy = User.Identity.Name;
                        personEmployee.DateCreated = DateTime.Now;
                        personEmployee.LastModified = product.LastModified;
                        personEmployee.CreatedBy = User.Identity.Name;
                        personEmployee.ModifiedBy = product.ModifiedBy;

                        db.Person_Employee.Add(personEmployee);
                        db.SaveChanges();

                        #region detailInfo


                        personEmployee.Education = new List<Education>();
                        var educationInfo = db.ApplicantEducation.Where(x=>x.ApplicantRegistrationId==product.ApplicantRegistrationId).ToList(); 
                        foreach (var ed in educationInfo)
                        {
                            education = new Education();
                            education.EduField = ed.EducField;
                            education.EduLevel = ed.EduLevel;
                            education.OrganizationName = ed.EducationInstitution;

                            db.Education.Add(education);
                            db.SaveChanges();

                        }

                        personEmployee.Experience = new List<Experience>();
                        var experienceInfo = db.ApplicantExperience.Where(x => x.ApplicantRegistrationId == product.ApplicantRegistrationId).ToList();
                        foreach (var ex in experienceInfo)
                        {
                            experience = new Experience();
                            experience.Organozation = ex.OrganizationName;
                            experience.Responsibility = ex.Postion;
                            experience.From = ex.From;
                            experience.FromExAmharic = ex.FromAmharic;
                            experience.ToExAmharic = ex.ToAmahric;
                            experience.TotalDuration = Convert.ToInt32(ex.Experience);

                            db.Experiences.Add(experience);
                            db.SaveChanges();
                        }

                        personEmployee.Training = new List<Training>();
                        var trainingInfo = db.ApplicantTraining.Where(x => x.ApplicantRegistrationId == product.ApplicantRegistrationId).ToList();
                        foreach (var tr in trainingInfo)
                        {
                            training = new Training();
                            training.TrainingName = tr.TrainingName;

                            db.EmpTrainingEducations.Add(training);
                            db.SaveChanges();
                        }
                        //var addressInfo = db.Addresses.ToList();
                        //foreach (var ad in addressInfo)
                        //{
                        //    address = new Address();
                        //    address.Category = ad.Category;
                        //    address.Value = ad.Value;
                        //    address.Remark = ad.Remark;
                        //}
                        #endregion

                    }
                    personEmployeeList.Add(personEmployee);


                }
            }


            return Json(personEmployeeList.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
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
                    ReferenceType = "External",
                    TotalResult = applicantScreeningResult.TotalResult

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

                    Remark = applicantScreeningResult.Remark,
                    ReferenceType = "External",
                    TotalResult = applicantScreeningResult.TotalResult
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant  screening result information of applicant" + applicantScreeningResult.ApplicantRegistrationId);
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
