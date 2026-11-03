using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
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

            ViewData["Vacancy"] = db.VaccancyPlans.Select(x=>new { x.VacancyId,x.VaccancyCode}).AsQueryable(); ;
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).Select(x=>new { x.LookupId,x.Description}).AsQueryable();
            ViewData["JobTitle"] = jobtitles;

            ViewData["branch"] = db.Branch.AsQueryable();

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
            var sum = db.ApplicantExamResult
                .Where(x => x.ApplicantScreeningId == applicantScreeningId)
                .Sum(x => (double?)x.CurrentResult) ?? 0;

            var passScore = db.RecruitmentApplicantPassSettings
                .Where(x => x.VacancyId == vacancyId)
                .Select(x => (double?)x.PassScore)
                .FirstOrDefault();

            if (!passScore.HasValue)
                return "Check Pass Setting";

            return sum >= passScore.Value ? "Pass" : "Fail";
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
            try
            {
                var screening = db.ScreeningSettings.FirstOrDefault(x => x.VacancyId == vacancyId);
                if (screening == null)
                    return Json(new { status = "Fail", message = "Can't find screening setting" }, JsonRequestBehavior.AllowGet);

                var vacancy = db.VaccancyPlans.Find(screening.VacancyId);
                if (vacancy == null)
                    return Json(new { status = "Fail", message = "No such vacancy" }, JsonRequestBehavior.AllowGet);

                var eduLevelIds = db.ScreeningEducationLevels
                    .Where(x => x.ScreeingSettingId == screening.ScreeningSettingId)
                    .Select(x => x.EducationLevelId)
                    .ToList();

                var eduFieldIds = db.ScreeningEducationFields
                    .Where(x => x.ScreeingSettingId == screening.ScreeningSettingId)
                    .Select(x => x.EducationFieldId)
                    .ToList();

                var expJobTitleIds = db.ScreeningExperienceJobtitles
                    .Where(x => x.ScreeingSettingId == screening.ScreeningSettingId)
                    .Select(x => x.ExperienceJobTitleId)
                    .ToList();

                var applicantReg = db.ApplicantRegistration.FirstOrDefault(x => x.VacancyId == screening.VacancyId);
                if (applicantReg == null)
                    return Json(new { status = "Fail", message = "No such applicant registration" }, JsonRequestBehavior.AllowGet);

                // 1. Get ALL registered applicants without filtering out any up front
                var applicants = db.ApplicantDetails
                    .Where(x => x.ApplicantRegistrationId == applicantReg.ApplicantRegistrationId)
                    .ToList();

                if (!applicants.Any())
                    return Json(new { status = "Fail", message = "No applicants found for this vacancy" }, JsonRequestBehavior.AllowGet);

                var vacancyLanguages = db.VacancyLanguages
                    .Where(x => x.VacancyId == vacancyId && x.IsRequired)
                    .Select(x => x.LanguageName.Trim().ToLower())
                    .ToList();

                var screenResult = db.ApplicantScreenResults.FirstOrDefault(x => x.VacancyId == screening.VacancyId);
                if (screenResult == null)
                {
                    screenResult = new ApplicantScreenResult { VacancyId = screening.VacancyId };
                    db.ApplicantScreenResults.Add(screenResult);
                    db.SaveChanges();
                }

                var examSettings = db.InternalRecruitmentCriteriaTypeSettings
                    .Where(x => x.ExamSource == RecruitmentSource.EXTERNAL)
                    .ToList();

                if (!examSettings.Any())
                    return Json(new { status = "Fail", message = "No exam/criteria settings found. You have to set the Exam Type Criteria settings first before screening and generating applicants." }, JsonRequestBehavior.AllowGet);

                // 2. Evaluate all applicants
                foreach (var app in applicants)
                {
                    bool isPassed = true;
                    var failReasons = new List<string>();

                    // Criterion A: Gender Check
                    if (vacancy.Gender == GenderForVacancyGender.MALE || vacancy.Gender == GenderForVacancyGender.FEMALE)
                    {
                        if (app.Sex.ToString() != vacancy.Gender.ToString())
                        {
                            isPassed = false;
                            failReasons.Add($"Gender mismatch (Required: {vacancy.Gender})");
                        }
                    }

                    // Criterion B: Language Check
                    if (vacancyLanguages.Any())
                    {
                        string appLang = app.Language != Languages.Not_Required ? app.Language.ToString().Trim().ToLower() : "";
                        if (!vacancyLanguages.Contains(appLang))
                        {
                            isPassed = false;
                            failReasons.Add("Does not meet language requirements");
                        }
                    }

                    // Criterion C: Education Check
                    var appEducation = db.ApplicantEducation.FirstOrDefault(x =>
                        x.ApplicantDetailId == app.ApplicantDetailId &&
                        eduLevelIds.Contains(x.EducationLevelSettingId) &&
                        eduFieldIds.Contains(x.EducationFieldSettingId));

                    if (appEducation == null)
                    {
                        isPassed = false;
                        failReasons.Add("Education level or field criteria not met");
                    }

                    // Criterion D: Experience Check
                    var applicantExps = db.ApplicantExperience
                        .Where(x => x.ApplicantDetailId == app.ApplicantDetailId && expJobTitleIds.Contains(x.JobTitleId))
                        .ToList();

                    double totalDays = applicantExps
                        .Where(e => e.From.HasValue && e.To.HasValue)
                        .Sum(e => (e.To.Value - e.From.Value).TotalDays);

                    double expYears = Math.Round(totalDays / 365.0, 2);

                    if (expYears < vacancy.TotalYearOfExperience)
                    {
                        isPassed = false;
                        failReasons.Add($"Experience insufficient ({expYears} yrs vs {vacancy.TotalYearOfExperience} yrs required)");
                    }

                    // Criterion E: Organization Type Check
                    if (screening.OrganizationType != OrganizationType.All)
                    {
                        bool hasMatchingOrgType = db.ApplicantExperience
                            .Any(x => x.ApplicantDetailId == app.ApplicantDetailId && x.OrganizationType == screening.OrganizationType);

                        if (!hasMatchingOrgType)
                        {
                            isPassed = false;
                            failReasons.Add($"Experience with Organization Type {screening.OrganizationType} missing");
                        }
                    }

                    // 3. Save or Update ApplicantScreening record for EVERY applicant
                    var screeningRecord = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantDetailId == app.ApplicantDetailId && x.ApplicantScreenResultId == screenResult.ApplicantScreenResultId)
                        ?? new ApplicantScreening
                        {
                            ApplicantScreenResultId = screenResult.ApplicantScreenResultId,
                            ApplicantDetailId = app.ApplicantDetailId,
                            ApplicantName = app.ApplicantName,
                            Address = app.Address,
                            Sex = app.Sex,
                            Age = app?.Age ?? 0
                        };

                    // Set screening outcome values
                    screeningRecord.IsPassed = isPassed;
                    screeningRecord.Status = isPassed ? "Pass" : "Fail";
                    screeningRecord.Remark = isPassed ? $"Passed (Exp: {expYears} yrs)" : string.Join("; ", failReasons);
                    screeningRecord.Experience = Convert.ToDecimal(expYears); // If field exists on your model

                    if (screeningRecord.ApplicantScreeningId == 0)
                        db.ApplicantScreenings.Add(screeningRecord);
                    else
                        db.Entry(screeningRecord).State = EntityState.Modified;

                    db.SaveChanges();

                    // 4. Generate/Update Exam records (only for Passed applicants, or remove 'if (isPassed)' if all can be tested)
                    if (isPassed)
                    {
                        foreach (var exam in examSettings)
                        {
                            var existingExam = db.ApplicantExamResult.FirstOrDefault(x =>
                                x.ApplicantScreeningId == screeningRecord.ApplicantScreeningId &&
                                x.ExamType == exam.ExamType);

                            if (existingExam == null)
                            {
                                db.ApplicantExamResult.Add(new ApplicantExamResult
                                {
                                    ApplicantScreeningId = screeningRecord.ApplicantScreeningId,
                                    ExamType = exam.ExamType,
                                    SettingValue = exam.SettingVlaue
                                });
                            }
                            else
                            {
                                existingExam.SettingValue = exam.SettingVlaue;
                                db.Entry(existingExam).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                    }
                }

                return Json(new
                {
                    status = "OK",
                    message = "Applicants screened and results generated successfully."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "Fail",
                    message = ex.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GenerateProfile(int applicantScreeningId)
        {
            var applicantScreening = db.ApplicantScreenings
                .FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId);

            if (applicantScreening == null)
            {
                return Json(new
                {
                    Status = "FAIL",
                    Message = "Employee not found."
                });
            }

            if (applicantScreening.IsProfileGenerated)
            {
                return Json(new
                {
                    Status = "FAIL",
                    Message = "Profile already generated."
                });
            }

            var names = applicantScreening.ApplicantName?
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var fName = names?.Length > 0 ? names[0] : "Unknown";
            var mName = names?.Length > 1 ? names[1] : "Unknown";
            var lName = names?.Length > 2 ? names[2] : "Unknown";

            var currentDate = DateTime.Now;
            var birthDate = currentDate.AddYears(-applicantScreening.Age);

            var profile = new Person_Employee
            {
                EmployeeCode = "Not Generated",

                FirstNameEnglish = fName,
                FirstName = fName,

                MiddleNameEnglish = mName,
                MiddleName = mName,

                LastNameEnglish = lName,
                LastName = lName,

                Gender = applicantScreening.Sex,
                Region = 0,
                Religion = 0,
                Status = 0,
                IsActive = true,

                DateCreated = currentDate,
                DateOfBirth = birthDate,
                HiredDate = currentDate,

                HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    currentDate.Day,
                    currentDate.Month,
                    currentDate.Year),

                DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    birthDate.Day,
                    birthDate.Month,
                    birthDate.Year)
            };

            db.Person_Employee.Add(profile);
            db.SaveChanges();

            var appEducations = db.ApplicantEducation
                .Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId)
                .ToList();

            foreach (var edu in appEducations)
            {
                var education = new Education
                {
                    EduLevel = edu.EducationLevelSettingId,
                    EduField = edu.EducationFieldSettingId,
                    OrganizationName = edu.EducationInstitution??"x",
                    OrgAddress = edu?.Remark??"AA",
                    EmployeeId = profile.EmployeeId,
                    From = edu.GraduationDate,
                    To = edu.GraduationDate.HasValue? edu.GraduationDate.Value:DateTime.Now
                };

                db.Education.Add(education);
            }

            var appExperiences = db.ApplicantExperience
                .Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId)
                .ToList();

            foreach (var exp in appExperiences)
            {
                if (!exp.From.HasValue || !exp.To.HasValue)
                    continue;

                var experience = new Experience
                {
                    EmployeeId = profile.EmployeeId,
                    OrgFullAddress = "Hiring Company",
                    Organozation = "Hiring Company",
                    ClearanceDateTime = currentDate,

                    From = exp.From.Value,
                    FromExAmharic = HrHelper.GetECDate(exp.From.Value),

                    To = exp.To.Value,
                    ToExAmharic = HrHelper.GetECDate(exp.To.Value)
                };

                db.Experiences.Add(experience);
            }

            applicantScreening.IsProfileGenerated = true;

            db.Entry(applicantScreening).State = EntityState.Modified;

            db.SaveChanges();

            return Json(new
            {
                Status = "SUCCESS",
                Message = "Profile generated successfully."
            });
        }
        //public JsonResult GenerateProfile(int applicantScreeningId)
        //{
        //    var applicantScreening = db.ApplicantScreenings.FirstOrDefault(x => x.ApplicantScreeningId == applicantScreeningId);
        //    if(applicantScreening == null)
        //    {
        //        return Json(new
        //        {
        //            Status = "FAIL",
        //            Message = "Employee not found."
        //        });
        //    }
        //    if (applicantScreening.IsProfileGenerated)
        //    {
        //        return Json(new
        //        {
        //            Status = "FAIL",
        //            Message = "already Generating."
        //        });

        //    }
        //    var names = (applicantScreening.ApplicantName ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (names.Length < 2)
        //    {
        //        return Json(new
        //        {
        //            Status = "FAIL",
        //            Message = "Applicant name must contain at least First Name and Middle Name."
        //        });
        //    }

        //    var fName = names[0];
        //    var mName = names[1];
        //    var lName = names.Length > 2 ? names[2] : "Unknown";
        //    if (applicantScreening != null)
        //    {
        //        var profile = new Person_Employee();


        //        profile.EmployeeCode = "Not Generated";
        //        profile.FirstNameEnglish = fName;
        //        profile.FirstName = fName;
        //        profile.MiddleNameEnglish = mName;
        //        profile.MiddleName = mName;
        //        profile.LastNameEnglish = lName;
        //        profile.LastName = lName;
        //        profile.Gender = applicantScreening.Sex;
        //        profile.Region = 0;
        //        profile.Religion = 0;
        //        profile.Status = 0;
        //        profile.IsActive = true;
        //        //if (applicantScreening.Sex == Gender.MALE)
        //        //{
        //        //     profile.Gender = Gender.MALE;
        //        //}
        //        //else if (applicantScreening.Sex == Gender.FEMALE)
        //        //{
        //        //     profile.Gender = Gender.FEMALE;
        //        //}

        //        profile.DateCreated = DateTime.Now;
        //        DateTime currentDate = DateTime.Now;

        //        // Calculate birth date by subtracting age from current date
        //        DateTime birthDate = currentDate.AddYears(-applicantScreening.Age);
        //        profile.DateOfBirth = birthDate;
        //        // profile.EffectiveDate = DateTime.Now;
        //        profile.HiredDate = DateTime.Now;
        //        //profile.OrginalHiredDate = DateTime.Now;
        //        //profile.TerminationDate = DateTime.Now;
        //        profile.HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        //        profile.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(birthDate.Day, birthDate.Month, birthDate.Year);
        //        db.Person_Employee.Add(profile);
        //        db.SaveChanges();

        //        var appEducations = db.ApplicantEducation.Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId).ToList();
        //        foreach (var edu in appEducations)
        //        {
        //            var education = new Education();

        //            education.EduLevel = edu.EducationLevelSettingId;
        //            education.EduField = edu.EducationFieldSettingId;
        //            education.EmployeeId = profile.EmployeeId;
        //            education.OrganizationName = edu.EducationInstitution;

        //            db.Education.Add(education);
        //            db.SaveChanges();

        //        }

        //        var appExperiences = db.ApplicantExperience.Where(x => x.ApplicantDetailId == applicantScreening.ApplicantDetailId).ToList();
        //        foreach (var exp in appExperiences)
        //        {
        //            var experience = new Experience();

        //            //experience.Type = exp.OrganizationType;
        //            //experience.Jobtitle = exp.JobTitleId;
        //            experience.EmployeeId = profile.EmployeeId;
        //            experience.OrgFullAddress = "Hiring Campany";
        //            experience.Organozation = "Hiring Campnay";
        //            experience.ClearanceDateTime =  DateTime.Now;
        //            experience.From =  exp.From.Value;
        //            experience.FromExAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(exp.From.Value.Day, exp.From.Value.Month, exp.From.Value.Year); 
        //            experience.To =  exp.To.Value;
        //            experience.ToExAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(exp.To.Value.Day, exp.To.Value.Month, exp.To.Value.Year);
        //            experience.EmployeeId = profile.EmployeeId;

        //            db.Experiences.Add(experience);
        //            db.SaveChanges();

        //        }
        //        /// change status //// 
        //        /// 
        //        applicantScreening.IsProfileGenerated = true;

        //        db.Entry(applicantScreening).State = EntityState.Modified;
        //        db.SaveChanges();


        //        return "true";
        //    }
        //    return "false";
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
