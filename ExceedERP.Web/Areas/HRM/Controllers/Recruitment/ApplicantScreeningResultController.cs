using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.TrainingEducation;

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
            var vacancies = db.VaccancyPlans.ToList();

            ViewData["Vacancy"] = vacancies;
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;

            ViewData["branch"] = db.Branch.ToList();

        }
        public decimal GetTotalResult(int id)
        {
            var applicantExam = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id).ToList();

            if (applicantExam.Any())
            {
                return applicantExam.Sum(x => x.CurrentResult);
            }
            return 0;
        }
        public string PassOrFail(int applicantScreeningId, int vacancyId)
        {
            var applicantExam = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == applicantScreeningId).ToList();

            if (applicantExam.Any())
            {
                var sum = applicantExam.Sum(x => x.CurrentResult);

                var passScore = db.RecruitmentApplicantPassSettings.FirstOrDefault(x => x.VacancyId == vacancyId);
                if (passScore != null)
                {
                    if (sum >= Convert.ToDecimal(passScore.PassScore))
                    {
                        return "Pass";
                    }
                    else
                    {
                        return "Fail";
                    }
                }
            }
            return "Check Pass Setting";
        }
        public int GetVacancyId(int applicantScreenResultId)
        {

            var appScreenResult = db.ApplicantScreenResults.FirstOrDefault(x => x.ApplicantScreenResultId == applicantScreenResultId);
            if (appScreenResult != null)
            {
                return appScreenResult.VacancyId;
            }

            return 0;
        }
        public ActionResult ApplicantScreening_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            var applicantDetail = db.ApplicantScreenings.Where(x => x.ApplicantScreenResultId == id).ToList();

            DataSourceResult result = applicantDetail.ToDataSourceResult(request, screening => new
            {
                ApplicantName = screening.ApplicantName,
                Age = screening.Age,
                ApplicantScreenResultId = screening.ApplicantScreenResultId,
                ApplicantScreeningId = screening.ApplicantScreeningId,
                ApplicantDetailId = screening.ApplicantDetailId,
                Sex = screening.Sex,
                Address = screening.Address,
                IsProfileGenerated = screening.IsProfileGenerated,
                TotalResult = GetTotalResult(screening.ApplicantScreeningId),
                PassOrFail = PassOrFail(screening.ApplicantScreeningId, GetVacancyId(screening.ApplicantScreenResultId))
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreening_Update([DataSourceRequest]DataSourceRequest request, ApplicantScreening screening)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var screen = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantScreeningId == screening.ApplicantScreeningId);
                if (screen != null)
                {


                    db.Entry(screen).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }

            return Json(new[] { screening }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// ////////////////////////////////////////////////////
        /// </summary>
        /// <param name="Applicant screening result"></param>
        /// <returns></returns>

        public string GetJobTitle(int vacId)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacId);
            if (vacancy != null)
            {
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == vacancy.JobTitle && x.LookupType == LookUpTypes.JOB_TITLE);
                if (look != null)
                {
                    return look.Description;
                }
            }
            return "";
        }
        public string GetLocation(int vacId)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacId);
            if (vacancy != null)
            {
                //var location = db.BranchLocations.FirstOrDefault(x => x.BranchLocationId == vacancy.Location);
                //if (location != null)
                //{
                //    return location.Name;
                //}
            }
            return "";
        }
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantScreeningResult_Read([DataSourceRequest]DataSourceRequest request)
        {

            var applicantScreenResults = db.ApplicantScreenResults.ToList();
            DataSourceResult result = applicantScreenResults.ToDataSourceResult(request, applicant => new
            {
                VacancyId = applicant.VacancyId,
                ApplicantScreenResultId = applicant.ApplicantScreenResultId,
                JobTitle = GetJobTitle(applicant.VacancyId),
                Location = GetLocation(applicant.VacancyId),
                Remark = applicant.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Create([DataSourceRequest]DataSourceRequest request, ApplicantScreenResult applicantScreeningResult)
        {

            if (ModelState.IsValid)
            {
                var entity = new ApplicantScreenResult
                {
                    VacancyId = applicantScreeningResult.VacancyId,
                    Remark = applicantScreeningResult.Remark,
                };

                db.ApplicantScreenResults.Add(entity);
                db.SaveChanges();


            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Update([DataSourceRequest]DataSourceRequest request, ApplicantScreenResult applicantScreeningResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new ApplicantScreenResult
                {
                    VacancyId = applicantScreeningResult.VacancyId,
                    ApplicantScreenResultId = applicantScreeningResult.ApplicantScreenResultId,
                    Remark = applicantScreeningResult.Remark,
                };

                db.ApplicantScreenResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResult_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantScreenResult applicantScreeningResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantScreenResult
                {
                    VacancyId = applicantScreeningResult.VacancyId,
                    ApplicantScreenResultId = applicantScreeningResult.ApplicantScreenResultId,
                    Remark = applicantScreeningResult.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant  screening result information of applicant" + applicantScreeningResult.ApplicantScreenResultId);
                #endregion
                db.ApplicantScreenResults.Attach(entity);
                db.ApplicantScreenResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }
        #region Generate applicants 
        //public string GenerateApplicants(string vacancyId)
        //{
        //    int vacId = Convert.ToInt32(vacancyId);

        //    var applicantRegistrationn = db.ApplicantRegistration.FirstOrDefault(x => x.VacancyId == vacId);
        //    if (applicantRegistrationn != null)
        //    {
        //        var checkExistance = db.ApplicantScreenings.Where(x => x.VacancyId == vacId).ToList();

        //        var applicantDetails = db.ApplicantDetails.Where(x => x.ApplicantRegistrationId == applicantRegistrationn.ApplicantRegistrationId).ToList();
        //        if (applicantDetails.Count() > checkExistance.Count())
        //        {
        //            foreach (var app in applicantDetails)
        //            {
        //                var check = db.ApplicantScreenings.FirstOrDefault(x=>x.ApplicantDetailId == app.ApplicantDetailId);
        //                if (check == null)
        //                {
        //                    var applicant = new ApplicantScreening();

        //                    applicant.ApplicantDetailId = app.ApplicantDetailId;
        //                    applicant.Age = app.Age;
        //                    applicant.Address = app.Address;
        //                    applicant.Sex = app.Sex;
        //                    applicant.ApplicantName = app.ApplicantName;

        //                    db.ApplicantScreenings.Add(applicant);
        //                    db.SaveChanges();
        //                }
        //            }
        //            return "successfully generated";

        //        }
        //        else
        //        {
        //            return "applicant for this job title already generated";
        //        }
        //    }
        //    else
        //    {
        //        // string message = "no employess are recoreded in this code";
        //        // return new JavaScriptResult() { Script = "alert('" + message + "');" };
        //        return "No applicant recoreded in this vacancy code";
        //    }

        //}
        #endregion
        #region generate screening value 
        //public bool GenerateScreeningValues(string screeningId)
        //{
        //    bool rtrn = false;

        //    int applicantScreeningId = Convert.ToInt32(screeningId);
        //    var applicantScreeening = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId);
        //    if (applicantScreeening != null)
        //    {
        //        var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == applicantScreeening.VacancyId);
        //        if (vacancy != null)
        //        {
        //            if (vacancy.RecruSource == RecruitmentSource.EXTERNAL)
        //            {
        //                var screeningSettings = db.Configurations.Where(c => c.Category == "Recruitment_External").ToList().Count();
        //                var screeningresult = db.ApplicantScreenResult.Where(x => x.ApplicantScreeningId == applicantScreeningId).ToList().Count();
        //                if (screeningSettings > screeningresult)
        //                {

        //                    var configurations = db.Configurations.Where(c => c.Category == "Recruitment_External").ToList();
        //                    foreach (var conf in configurations)
        //                    {
        //                        var appResult = db.ApplicantScreenResult.FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId && x.Criteria == conf.Attribute);
        //                        if (appResult == null)
        //                        {
        //                            var result = new ApplicantScreenResult();

        //                            result.Criteria = conf.Attribute;
        //                            result.SettingValue = Convert.ToDecimal(conf.CurrentValue);
        //                            result.CurrentResult = 0;
        //                            result.ApplicantScreeningId = applicantScreeningId;

        //                            db.ApplicantScreenResult.Add(result);
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            appResult.Criteria = conf.Attribute;
        //                            appResult.SettingValue = Convert.ToDecimal(conf.CurrentValue);

        //                            db.Entry(appResult).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                        }

        //                    }

        //                    rtrn = true;

        //                }

        //                var externalSetting = db.ExamTypeSettings.Where(x => x.ExamSource == ExamSource.External).ToList().Count();
        //                var examresult = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == applicantScreeningId).ToList().Count;
        //                if (externalSetting > examresult)
        //                {
        //                    var examTypes = db.ExamTypeSettings.Where(x => x.ExamSource == ExamSource.External).ToList();

        //                    foreach (var res in examTypes)
        //                    {
        //                        var app = db.ApplicantExamResult.FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId && x.Criteria == res.ExamType.ToString());
        //                        if (app == null)
        //                        {
        //                            var result = new ApplicantExamResult();

        //                            result.Criteria = res.ExamType.ToString();
        //                            result.SettingValue = res.SettingVlaue;
        //                            result.CurrentResult = 0;
        //                            result.ApplicantScreeningId = applicantScreeningId;

        //                            db.ApplicantExamResult.Add(result);
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            app.Criteria = res.ExamType.ToString();
        //                            app.SettingValue = res.SettingVlaue;

        //                            db.Entry(app).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                        }
        //                    }
        //                    rtrn = true;
        //                }
        //            }

        //        }

        //    }

        //    return rtrn;
        //}
        #endregion
        public JsonResult FilterApplicant(int vacancyId)
        {
            //var applicantScreening = db.ApplicantScreenings.Where(x=>x.VacancyId == vacancyId).ToList();

            //return Json(applicantScreening);
            return null;
        }
        public JsonResult ScreenAndGenerateApplicants(int vacancyId)
        {
            List<ApplicantDetail> applicantList = new List<ApplicantDetail>();

            var result = new
            {
                status = "Fail",
                message = "Can't find screening setting"
            };

            var applicantScreening = db.ScreeningSettings.FirstOrDefault(x => x.VacancyId == vacancyId);
            if (applicantScreening != null)
            {
                var eduLevelScreening = db.ScreeningEducationLevels.Where(x => x.ScreeingSettingId == applicantScreening.ScreeningSettingId).ToList().Select(x => x.EducationLevelId);
                var eduFieldScreening = db.ScreeningEducationFields.Where(x => x.ScreeingSettingId == applicantScreening.ScreeningSettingId).ToList().Select(x => x.EducationFieldId);
                var expJobtitle = db.ScreeningExperienceJobtitles.Where(x => x.ScreeingSettingId == applicantScreening.ScreeningSettingId).ToList().Select(x => x.ExperienceJobTitleId);

                var applicantRegistration = db.ApplicantRegistration.FirstOrDefault(x => x.VacancyId == applicantScreening.VacancyId);

                if (applicantRegistration != null)
                {
                    var applicantDetails = db.ApplicantDetails.Where(x => x.ApplicantRegistrationId == applicantRegistration.ApplicantRegistrationId).ToList();

                    // screening with Gender
                    if (applicantScreening.Gender == Gender.MALE || applicantScreening.Gender == Gender.FEMALE)
                    {
                        applicantDetails = applicantDetails.Where(x => x.Sex == applicantScreening.Gender).ToList();
                    }

                    if (applicantScreening.Language != Languages.Not_Required)
                    {
                        applicantDetails = applicantDetails.Where(x => x.Language.ToString().Trim().ToLower() == applicantScreening.Language.ToString().Trim().ToLower()).ToList();
                    }

                    foreach (var app in applicantDetails)
                    {
                        // screening with Education Level and field
                        var appEducation = db.ApplicantEducation.FirstOrDefault(x => x.ApplicantDetailId == app.ApplicantDetailId && eduLevelScreening.Contains(x.EducationLevelSettingId) && eduFieldScreening.Contains(x.EducationFieldSettingId));
                        if (appEducation != null)
                        {
                            // screening with experience jobtitle
                            var applicantExperiences = db.ApplicantExperience.Where(x => x.ApplicantDetailId == app.ApplicantDetailId && expJobtitle.Contains(x.JobTitleId)).ToList();
                            double sumOfExp = 0;
                            double expYear = 0;
                            foreach (var appExp in applicantExperiences)
                            {
                                sumOfExp = sumOfExp + (appExp.To - appExp.From).Value.Days;
                            }

                            expYear = sumOfExp / 365;

                            //var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == applicantScreening.VacancyId);
                            //if (vacancy != null)
                            //{
                            if (applicantScreening.ExperienceYear <= expYear)
                            {
                                if (applicantScreening.OrganizationType != OrganizationType.All)
                                {
                                    var expOrgTypes = db.ApplicantExperience.Where(x => x.ApplicantDetailId == app.ApplicantDetailId && x.OrganizationType == applicantScreening.OrganizationType).ToList();
                                    if (expOrgTypes.Any())
                                    {
                                        applicantList.Add(app);
                                    }
                                }
                                else
                                {
                                    applicantList.Add(app);
                                }

                            }

                            // }
                        }
                    }
                    if (applicantList.Any())
                    {
                        var checkExit = db.ApplicantScreenResults.FirstOrDefault(x => x.VacancyId == applicantScreening.VacancyId);
                        if (checkExit != null)
                        {
                            foreach (var app in applicantList)
                            {
                                var checkApp = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantDetailId == app.ApplicantDetailId);

                                if (checkApp != null)
                                {
                                    checkApp.ApplicantScreenResultId = checkExit.ApplicantScreenResultId;
                                    checkApp.ApplicantDetailId = app.ApplicantDetailId;
                                    checkApp.ApplicantName = app.ApplicantName;
                                    checkApp.Address = app.Address;
                                    checkApp.Sex = app.Sex;
                                    checkApp.Age = app.Age;

                                    db.Entry(checkApp).State = EntityState.Modified;
                                    db.SaveChanges();

                                    int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                    for (int i = 0; i < countExamTypes; i++)
                                    {
                                        var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.EXTERNAL);

                                        if (settingValue != null)
                                        {
                                            var appExam = db.ApplicantExamResult.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ApplicantScreeningId == checkApp.ApplicantScreeningId);

                                            if (appExam != null)
                                            {
                                                appExam.ApplicantScreeningId = checkApp.ApplicantScreeningId;
                                                appExam.ExamType = (ExamType)i;
                                                appExam.SettingValue = settingValue.SettingVlaue;

                                                db.Entry(appExam).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                            else
                                            {
                                                var aer = new ApplicantExamResult();

                                                aer.ApplicantScreeningId = checkApp.ApplicantScreeningId;
                                                aer.ExamType = (ExamType)i;
                                                aer.SettingValue = settingValue.SettingVlaue;

                                                db.ApplicantExamResult.Add(aer);
                                                db.SaveChanges();
                                            }

                                        }

                                    }

                                }
                                else
                                {
                                    var asd = new ApplicantScreening();

                                    asd.ApplicantScreenResultId = checkExit.ApplicantScreenResultId;
                                    asd.ApplicantDetailId = app.ApplicantDetailId;
                                    asd.ApplicantName = app.ApplicantName;
                                    asd.Address = app.Address;
                                    asd.Sex = app.Sex;
                                    asd.Age = app.Age;

                                    db.ApplicantScreenings.Add(asd);
                                    db.SaveChanges();

                                    int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                    for (int i = 0; i < countExamTypes; i++)
                                    {
                                        var aer = new ApplicantExamResult();

                                        var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.EXTERNAL);

                                        if (settingValue != null)
                                        {
                                            aer.ApplicantScreeningId = asd.ApplicantScreeningId;
                                            aer.ExamType = (ExamType)i;
                                            aer.SettingValue = settingValue.SettingVlaue;

                                            db.ApplicantExamResult.Add(aer);
                                            db.SaveChanges();
                                        }

                                    }
                                }
                            }

                            result = new
                            {
                                status = "OK",
                                message = "Successfull Updated"
                            };
                        }
                        else
                        {
                            var asr = new ApplicantScreenResult();

                            asr.VacancyId = applicantScreening.VacancyId;

                            db.ApplicantScreenResults.Add(asr);
                            db.SaveChanges();

                            foreach (var app in applicantList)
                            {
                                var checkApp = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantDetailId == app.ApplicantDetailId);

                                if (checkApp == null)
                                {
                                    var asd = new ApplicantScreening();

                                    asd.ApplicantScreenResultId = asr.ApplicantScreenResultId;
                                    asd.ApplicantDetailId = app.ApplicantDetailId;
                                    asd.ApplicantName = app.ApplicantName;
                                    asd.Address = app.Address;
                                    asd.Sex = app.Sex;
                                    asd.Age = app.Age;

                                    db.ApplicantScreenings.Add(asd);
                                    db.SaveChanges();

                                    int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                    for (int i = 0; i < countExamTypes; i++)
                                    {
                                        var aer = new ApplicantExamResult();

                                        var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.EXTERNAL);

                                        if (settingValue != null)
                                        {
                                            aer.ApplicantScreeningId = asd.ApplicantScreeningId;
                                            aer.ExamType = (ExamType)i;
                                            aer.SettingValue = settingValue.SettingVlaue;

                                            db.ApplicantExamResult.Add(aer);
                                            db.SaveChanges();
                                        }

                                    }
                                }

                            }

                            result = new
                            {
                                status = "OK",
                                message = "Successfull Generated"
                            };
                        }

                    }
                    else
                    {
                        var applicantListsReturn = new
                        {
                            status = "Fail",
                            message = "No applicant pass the screening"
                        };

                        return Json(applicantListsReturn);
                    }
                }
                else
                {
                    var resultapplicantregistraion = new
                    {
                        status = "Fail",
                        message = "No such applicant registration"
                    };

                    return Json(resultapplicantregistraion);
                }
            }


            return Json(result);
        }
        public string GenerateProfile(int applicantScreeningId)
        {
            var applicantScreening = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId);
            if (applicantScreening != null)
            {
                var profile = new Person_Employee();

                string Fname = "";
                string Mname = "";
                string Lname = "";

                int splitLength = applicantScreening.ApplicantName.Split(' ').Length;

                for (int i = 0; i < splitLength; i++)
                {
                    if (i == 0)
                    {
                        Fname = applicantScreening.ApplicantName.Split(' ')[0];
                    }
                    else if (i == 1)
                    {
                        Mname = applicantScreening.ApplicantName.Split(' ')[1];
                    }
                    else if (i == 2)
                    {
                        Lname = applicantScreening.ApplicantName.Split(' ')[2];
                    }
                }


                profile.EmployeeCode = "Not Generated";
                profile.FirstNameEnglish = Fname;
                profile.MiddleNameEnglish = Mname;
                profile.LastNameEnglish = Lname;
                if (applicantScreening.Sex == Gender.MALE)
                {
                    // profile.Gender = "M";
                }
                else if (applicantScreening.Sex == Gender.FEMALE)
                {
                    // profile.Gender = "F";
                }

                profile.DateCreated = DateTime.Now;
                profile.DateOfBirth = DateTime.Now;
                // profile.EffectiveDate = DateTime.Now;
                profile.HiredDate = DateTime.Now;
                //profile.OrginalHiredDate = DateTime.Now;
                //profile.TerminationDate = DateTime.Now;

                db.Person_Employee.Add(profile);
                db.SaveChanges();

                var appEducations = db.ApplicantEducation.Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId).ToList();
                foreach (var edu in appEducations)
                {
                    var education = new Education();

                    //education.Level = edu.EducationLevelSettingId;
                    //education.Field = edu.EducationFieldSettingId;
                    //education.EmployeeId = profile.EmployeeId;
                    //education.Institution = edu.EducationInstitution;

                    db.Education.Add(education);
                    db.SaveChanges();

                }

                var appExperiences = db.ApplicantExperience.Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId).ToList();
                foreach (var exp in appExperiences)
                {
                    var experience = new Experience();

                    //experience.Type = exp.OrganizationType;
                    //experience.Jobtitle = exp.JobTitleId;
                    experience.Organozation = exp.OrganizationName;
                    experience.From = exp.From is null ? DateTime.Now : exp.From.Value;
                    experience.To = exp.To is null ? DateTime.Now : exp.To.Value;
                    experience.EmployeeId = profile.EmployeeId;

                    db.Experiences.Add(experience);
                    db.SaveChanges();

                }
                /// change status //// 
                /// 
                applicantScreening.IsProfileGenerated = true;

                db.Entry(applicantScreening).State = EntityState.Modified;
                db.SaveChanges();


                return "true";
            }
            return "false";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
