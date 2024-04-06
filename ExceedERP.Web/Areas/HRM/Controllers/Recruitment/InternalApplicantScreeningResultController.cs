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
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class InternalApplicantScreeningResultController : BaseController
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

            // var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.ToList();//listt.Where(x => x.IsActive);
            var recruitment = db.RecruitmentVacancy.ToList();
            ViewData["Department"] = db.MainCompanyStructures.ToList();
            ViewData["recruitmentVacancy"] = recruitment;
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();
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


            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public int GetBranch(int id)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);
            if (vacancy != null)
            {
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == vacancy.BranchId);
                if (branch != null)
                {
                    return branch.BranchId;
                }
            }
            return 0;
        }
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult InternalApplicantScreeningResult_Read([DataSourceRequest]DataSourceRequest request)
        {
            var applicants = db.InternalApplicantScreeningResults.ToList();

            DataSourceResult result = applicants.ToDataSourceResult(request, applicantExamResult => new
            {
                InternalApplicantScreeningResultId = applicantExamResult.InternalApplicantScreeningResultId,
                ApplicationFormId = applicantExamResult.ApplicationFormId,
                VacancyId = applicantExamResult.VacancyId,
                Remark = applicantExamResult.Remark,

            });

            return Json(result);
        }
        public decimal TotalResult(int id)
        {
            var applicant = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == id).ToList().Sum(x => x.CurrentResult);

            return applicant;
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantScreeningResult_Create([DataSourceRequest]DataSourceRequest request, InternalApplicantScreeningResult applicantScreeningResult, int id)
        {
            if (ModelState.IsValid)
            {

                var entity = new InternalApplicantScreeningResult
                {

                };

                db.InternalApplicantScreeningResults.Add(entity);
                db.SaveChanges();
                applicantScreeningResult.InternalApplicantScreeningResultId = entity.InternalApplicantScreeningResultId;
            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantScreeningResult_Update([DataSourceRequest]DataSourceRequest request, InternalApplicantScreeningResult applicantScreeningResult)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantScreeningResult
                {
                    InternalApplicantScreeningResultId = applicantScreeningResult.InternalApplicantScreeningResultId
                };

                db.InternalApplicantScreeningResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantScreeningResult_Destroy([DataSourceRequest]DataSourceRequest request, InternalApplicantScreeningResult applicantScreeningResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantScreeningResult
                {
                    InternalApplicantScreeningResultId = applicantScreeningResult.InternalApplicantScreeningResultId,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted internal screeining result information of applicant  " + applicantScreeningResult.InternalApplicantScreeningResultId);
                #endregion
                db.InternalApplicantScreeningResults.Attach(entity);
                db.InternalApplicantScreeningResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantScreeningResult }.ToDataSourceResult(request, ModelState));
        }
        // //////////
        /// <summary>
        /// /////////   Internal Applicant Screenings
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        /// 

        public string GetApplicantName(int id)
        {
            var internalApplicant = db.InternalApplicants.FirstOrDefault(x => x.InternalApplicantsId == id);
            if (internalApplicant != null)
            {
                if (internalApplicant.EmployeeId != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == internalApplicant.EmployeeId);
                    if (person != null)
                    {
                        return person.FullNameaEnglish;
                    }
                }
                else
                {
                    return internalApplicant.ApplicantName;
                }
            }
            return "";
        }
        public string PassOrFail(int applicantScreeningId, int vacancyId)
        {
            var applicantExam = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == applicantScreeningId).ToList();

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

            var appScreenResult = db.InternalApplicantScreeningResults.FirstOrDefault(x => x.InternalApplicantScreeningResultId == applicantScreenResultId);
            if (appScreenResult != null)
            {
                return appScreenResult.VacancyId;
            }

            return 0;
        }

        public decimal GetTotalResult(int id)
        {
            var applicantExam = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == id).ToList();

            if (applicantExam.Any())
            {
                return applicantExam.Sum(x => x.CurrentResult);
            }
            return 0;
        }
        public string GetDepartmentName(int employeeId)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
            if (placement != null)
            {
                var company = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (company != null)
                {
                    return company.Name;
                }
            }

            return null;
        }
        public ActionResult InternalApplicantScreening_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var applicants = db.InternalApplicantScreenings.Where(x => x.InternalApplicantScreeningResultId == id).ToList();

            DataSourceResult result = applicants.ToDataSourceResult(request, applicant => new
            {
                InternalApplicantScreeningResultId = applicant.InternalApplicantScreeningResultId,
                EmployeeId = applicant.EmployeeId,
                Department = GetDepartmentName(applicant.EmployeeId),
                InternalApplicantScreeningId = applicant.InternalApplicantScreeningId,
                TotalResult = GetTotalResult(applicant.InternalApplicantScreeningId),
                PassOrFail = PassOrFail(applicant.InternalApplicantScreeningId, GetVacancyId(applicant.InternalApplicantScreeningResultId))
            });

            return Json(result);
        }

        //public string GenerateApplicants(string vacancyId)
        //{
        //    int vacId = Convert.ToInt32(vacancyId);
        //    var applicationForms = db.ApplicantionFormes.Where(x => x.VacancyId == vacId).ToList();
        //    var checkEx = db.InternalApplicantScreeningResults.Where(x => x.VacancyId == vacId).ToList();

        //    if (applicationForms.Count() > checkEx.Count())
        //    {
        //        foreach (var app in applicationForms)
        //        {
        //            var checkExistance = db.InternalApplicantScreenings.FirstOrDefault(x => x.ApplicantionFormId == app.ApplicantionFormId);
        //            if (checkExistance == null)
        //            {
        //                var applicant = new InternalApplicantScreening();

        //                applicant.ApplicantionFormId = app.ApplicantionFormId;
        //                applicant.EmployeeId = app.EmployeeId;

        //                db.InternalApplicantScreenings.Add(applicant);
        //                db.SaveChanges();
        //            }
        //        }
        //        return "successfully generated";
        //    }
        //    else
        //    {
        //        return "applicants generated before for this vacancy";
        //    }
        //}
        public ActionResult ScreenAndGenerateApplicants(int vacancyId)
        {
            var result = new
            {
                status = "Fail",
                message = "Can't find screening setting"
            };

            List<ApplicantionForm> applicationForms = new List<ApplicantionForm>();

            var screeningSetting = db.ScreeningSettings.FirstOrDefault(x => x.VacancyId == vacancyId);

            if (screeningSetting != null)
            {
                var applicants = db.ApplicantionFormes.Where(x => x.VacancyId == screeningSetting.VacancyId).ToList();

                if (screeningSetting.Gender == GenderForVacancyGender.MALE || screeningSetting.Gender == GenderForVacancyGender.FEMALE)
                {
                    var employeeByGender = db.Person_Employee.Where(x => x.Gender.ToString() == screeningSetting.Gender.ToString())?.Select(x => x.EmployeeId).ToList();

                    if (employeeByGender.Any())
                    {
                        applicants = applicants.Where(x => employeeByGender.Contains(x.EmployeeId)).ToList();
                    }
                }

                var eduLevelScreening = db.ScreeningEducationLevels.Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList().Select(x => x.EducationLevelId);
                var eduFieldScreening = db.ScreeningEducationFields.Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList().Select(x => x.EducationFieldId);
                var expJobtitle = db.ScreeningExperienceJobtitleForInternals.Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).Select(x => x.ExperienceJobTitleId).ToList();

                foreach (var app in applicants)
                {
                    double sumOfExp = 0;
                    double expYear = 0;

                    var appEducations = db.Education.Where(x => x.EmployeeId == app.EmployeeId && eduLevelScreening.Contains(x.EduLevel) && eduFieldScreening.Contains(x.EduField)).ToList();

                    if (appEducations.Any())
                    {
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == app.EmployeeId);

                        if (person != null)
                        {
                            sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;

                            var empExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId /*&& expJobtitle.Contains(x.JobTitleId)*/).ToList();

                            foreach (var ex in empExps)
                            {
                                sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
                            }
                        }

                        expYear = sumOfExp / 365;

                        if (screeningSetting.ExperienceYear <= expYear)
                        {
                            applicationForms.Add(app);

                        }
                    }
                }

                if (applicationForms.Any())
                {
                    var checkExit = db.InternalApplicantScreeningResults.FirstOrDefault(x => x.VacancyId == screeningSetting.VacancyId);
                    if (checkExit != null)
                    {
                        checkExit.VacancyId = screeningSetting.VacancyId;

                        db.Entry(checkExit).State = EntityState.Modified;
                        db.SaveChanges();

                        foreach (var app in applicationForms)
                        {
                            var checkApp = db.InternalApplicantScreenings.FirstOrDefault(x => x.InternalApplicantScreeningResultId == checkExit.InternalApplicantScreeningResultId && x.EmployeeId == app.EmployeeId);

                            if (checkApp != null)
                            {
                                //checkApp.InternalApplicantScreeningResultId = checkExit.InternalApplicantScreeningResultId;
                                //checkApp.InternalApplicantsId = app.InternalApplicantsId;
                                //checkApp.ApplicantName = app.ApplicantName;
                                //checkApp.Address = app.Address;
                                //checkApp.Sex = app.Gender.ToString();
                                //checkApp.Age = app.Age;


                                //db.Entry(checkApp).State = EntityState.Modified;
                                //db.SaveChanges();

                                int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.INTERNAL);

                                    if (settingValue != null)
                                    {
                                        var appExam = db.InternalApplicantExamResults.FirstOrDefault(x => x.ExamType == (ExamType)i && x.InternalApplicantScreeningId == checkApp.InternalApplicantScreeningId);
                                        if (appExam != null)
                                        {
                                            appExam.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            appExam.ExamType = (ExamType)i;
                                            appExam.SettingValue = settingValue.SettingVlaue;

                                            db.Entry(appExam).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var aer = new InternalApplicantExamResult();

                                            aer.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            aer.ExamType = (ExamType)i;
                                            aer.SettingValue = settingValue.SettingVlaue;

                                            db.InternalApplicantExamResults.Add(aer);
                                            db.SaveChanges();
                                        }

                                    }

                                }

                            }
                            else
                            {
                                var asd = new InternalApplicantScreening();

                                asd.InternalApplicantScreeningResultId = checkExit.InternalApplicantScreeningResultId;
                                asd.EmployeeId = app.EmployeeId;

                                db.InternalApplicantScreenings.Add(asd);
                                db.SaveChanges();

                                int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var aer = new InternalApplicantExamResult();

                                    var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.INTERNAL);

                                    if (settingValue != null)
                                    {
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = (ExamType)i;
                                        aer.SettingValue = settingValue.SettingVlaue;

                                        db.InternalApplicantExamResults.Add(aer);
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
                        var asr = new InternalApplicantScreeningResult();

                        asr.VacancyId = screeningSetting.VacancyId;

                        db.InternalApplicantScreeningResults.Add(asr);
                        db.SaveChanges();

                        foreach (var app in applicationForms)
                        {
                            var checkApp = db.InternalApplicantScreenings.FirstOrDefault(x => x.InternalApplicantScreeningResultId == asr.InternalApplicantScreeningResultId && x.EmployeeId == app.EmployeeId);

                            if (checkApp == null)
                            {
                                var asd = new InternalApplicantScreening();

                                asd.InternalApplicantScreeningResultId = asr.InternalApplicantScreeningResultId;
                                asd.EmployeeId = app.EmployeeId;

                                db.InternalApplicantScreenings.Add(asd);
                                db.SaveChanges();

                                int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var aer = new InternalApplicantExamResult();

                                    var settingValue = db.ExamTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i && x.ExamSource == RecruitmentSource.INTERNAL);

                                    if (settingValue != null)
                                    {
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = (ExamType)i;
                                        aer.SettingValue = settingValue.SettingVlaue;

                                        db.InternalApplicantExamResults.Add(aer);
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
                    result = new
                    {
                        status = "Fail",
                        message = "No applicant pass the screening"
                    };

                    return Json(result);
                }

            }
            else
            {
                result = new
                {
                    status = "Fail",
                    message = "No screening setting defined for this vacancy"
                };
            }

            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
