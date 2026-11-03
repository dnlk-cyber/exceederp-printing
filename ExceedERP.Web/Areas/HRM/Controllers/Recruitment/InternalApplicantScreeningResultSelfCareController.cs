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
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class InternalApplicantScreeningResultSelfCareController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
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
            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var vacancyIds = db.RecruitmetnInterviewerPerseons.Where(x => x.EmployeeId == empId).Select(x => x.VacancyId);

            var applicants = db.InternalApplicantScreeningResults.Where(x=>vacancyIds.Contains(x.VacancyId)).ToList();

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
        private string PassOrFail(int applicantScreeningId, int vacancyId)
        {
            var applicantExam = db.InternalApplicantExamResults
                                  .Where(x => x.InternalApplicantScreeningId == applicantScreeningId);

            if (!applicantExam.Any())
                return "Check Pass Setting";

            var sum = applicantExam.Sum(x => x.CurrentResult);

            var passSetting = db.RecruitmentApplicantPassSettings
                                .FirstOrDefault(x => x.VacancyId == vacancyId);

            if (passSetting == null)
                return "Check Pass Setting";

            return (sum >= (decimal)passSetting.PassScore) ? "Pass" : "Fail";
        }

        //private string PassOrFail(int applicantScreeningId, int vacancyId)
        //{
        //    var applicantExam = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == applicantScreeningId).ToList();

        //    if (applicantExam.Any())
        //    {
        //        var sum = applicantExam.Sum(x => x.CurrentResult);

        //        var passScore = db.RecruitmentApplicantPassSettings.FirstOrDefault(x => x.VacancyId == vacancyId);
        //        if (passScore != null)
        //        {
        //            if (sum >= Convert.ToDecimal(passScore.PassScore))
        //            {
        //                return "Pass";
        //            }
        //            else
        //            {
        //                return "Fail";
        //            }
        //        }
        //    }
        //    return "Check Pass Setting";
        //}

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
        //private (decimal Min, decimal Max) GetMaximumAndMinimumExperience(decimal MinimuVacacyExpience, List<ApplicantionForm> applicantionForms)
        //{
        //    decimal minExperience = decimal.MaxValue;
        //    decimal maxExperience = decimal.MinValue;

        //    foreach (var app in applicantionForms)
        //    {
        //        var person = db.Person_Employee.Find(app.EmployeeId);
        //        if (person != null)
        //        {
        //            double sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
        //            var empExperiences = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();

        //            foreach (var ex in empExperiences)
        //            {
        //                sumOfExp += ex.To.Subtract(ex.From).TotalDays;
        //            }

        //            decimal expYears = ((decimal)(sumOfExp) / 365.25m);

        //            if (expYears >= MinimuVacacyExpience)
        //            {
        //                minExperience = Math.Min(minExperience, expYears);
        //                maxExperience = Math.Max(maxExperience, expYears);
        //            }
        //        }
        //    }

        //    if (minExperience == decimal.MaxValue) minExperience = 0;
        //    if (maxExperience == decimal.MinValue) maxExperience = 0;

        //    return (minExperience, maxExperience);
        //}
        //private (decimal Min, decimal Max,decimal Percentage,decimal FinalValue ,List<(int EmployeeId, decimal Experience)>) GetMaximumAndMinimumExperience(decimal MinimuVacacyExpience, List<ApplicantionForm> applicantionForms,decimal minimumPercentage,decimal settingValue)
        //{
        //    decimal minExperience = decimal.MaxValue;
        //    decimal maxExperience = decimal.MinValue;
        //    decimal percentage = 0;
        //    decimal finalValue = 0;
        //    List<(int EmployeeId, decimal Experience)> employeeExperiences = new List<(int, decimal)>();

        //    foreach (var app in applicantionForms)
        //    {
        //        var person = db.Person_Employee.Find(app.EmployeeId);
        //        if (person != null)
        //        {
        //            double sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
        //            var empExperiences = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();

        //            foreach (var ex in empExperiences)
        //            {
        //                sumOfExp += ex.To.Subtract(ex.From).TotalDays;
        //            }

        //            decimal expYears = ((decimal)(sumOfExp) / 365.25m);

        //            if (expYears >= MinimuVacacyExpience)
        //            {
        //                minExperience = Math.Min(minExperience, expYears);
        //                maxExperience = Math.Max(maxExperience, expYears);
        //                employeeExperiences.Add((person.EmployeeId, expYears));
        //            }
        //        }
        //    }

        //    if (minExperience == decimal.MaxValue) minExperience = 0;
        //    if (maxExperience == decimal.MinValue) maxExperience = 0;

        //    employeeExperiences = employeeExperiences.OrderBy(e => e.Experience).ToList();

        //    return (minExperience, maxExperience, percentage,finalValue, employeeExperiences);
        //}
        private (decimal Min, decimal Max, List<(int EmployeeId, decimal Experience, decimal FinalValue)>)
            GetMaximumAndMinimumExperience(decimal MinimuVacacyExpience, List<ApplicantionForm> applicantionForms, decimal minimumPercentage, decimal settingValue)
        {
            decimal minExperience = decimal.MaxValue;
            decimal maxExperience = decimal.MinValue;
            List<(int EmployeeId, decimal Experience, decimal FinalValue)> employeeExperiences = new List<(int, decimal, decimal)>();

            foreach (var app in applicantionForms)
            {
                var person = db.Person_Employee.Find(app.EmployeeId);
                if (person != null)
                {
                    double sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
                    var empExperiences = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();
                    foreach (var ex in empExperiences)
                    {
                        sumOfExp += ex.To.Subtract(ex.From).TotalDays;
                    }
                    decimal expYears = ((decimal)(sumOfExp) / 365.25m);
                    if (expYears >= MinimuVacacyExpience)
                    {
                        minExperience = Math.Min(minExperience, expYears);
                        maxExperience = Math.Max(maxExperience, expYears);
                        employeeExperiences.Add((person.EmployeeId, expYears, 0)); // FinalValue will be calculated later
                    }
                }
            }
            if (minExperience == decimal.MaxValue) minExperience = 0;
            if (maxExperience == decimal.MinValue) maxExperience = 0;
            // Sort employees by experience in ascending order
            employeeExperiences = employeeExperiences.OrderBy(e => e.Experience).ToList();
            // Assign final values based on sequence
            decimal percentage = minimumPercentage;
            decimal maxExpFinalValue = settingValue; // Maximum experience gets the full value
            for (int i = 0; i < employeeExperiences.Count; i++)
            {
                decimal finalValue;

                // Check if the current employee has the maximum experience
                if (employeeExperiences[i].Experience == maxExperience)
                {
                    finalValue = maxExpFinalValue; // Full value for max experience
                }
                else
                {
                    finalValue = (percentage * settingValue) / 100; // Other employees get calculated values
                    percentage = (percentage + 100) / 2; // Update percentage for the next employee
                }

                employeeExperiences[i] = (employeeExperiences[i].EmployeeId, employeeExperiences[i].Experience, finalValue);
            }

            return (minExperience, maxExperience, employeeExperiences);
        }

        private (decimal Min, decimal Max, List<(int EmployeeId, int eduIndex, decimal education)>)
      GetEmployeeEducationResult(List<ApplicantionForm> applicantionForms, decimal settingValue)
        {
            decimal minEdu = decimal.MaxValue;
            decimal maxEdu = decimal.MinValue;

            var employeeEducations = new List<(int EmployeeId, int index, decimal Education)>();

            foreach (var app in applicantionForms)
            {
                var eduLevelIds = db.Education
                    .Where(x => x.EmployeeId == app.EmployeeId)
                    .Select(x => x.EduLevel)
                    .ToList();

                var lookup = db.Lookups
                    .Where(x => eduLevelIds.Contains(x.LookupId))
                    .OrderByDescending(x => x.Index) // higere index = higher education level
                    .FirstOrDefault();

                if (lookup != null)
                {
                    minEdu = Math.Min(minEdu, lookup.Index);
                    maxEdu = Math.Max(maxEdu, lookup.Index);
                    employeeEducations.Add((app.EmployeeId, lookup.Index, 0)); // Placeholder
                }
            }

            if (minEdu == decimal.MaxValue) minEdu = 0;
            if (maxEdu == decimal.MinValue) maxEdu = 0;

            // Sort applicants by index (education level)
            employeeEducations = employeeEducations.OrderByDescending(e => e.index).ToList();

            // Get distinct indexes in descending order (higher education first)
            var distinctIndexes = employeeEducations
                .Select(e => e.index)
                .Distinct()
                .OrderByDescending(i => i)
                .ToList();

            // Map index to score
            var indexToScore = new Dictionary<int, decimal>();
            decimal currentValue = settingValue;

            foreach (var index in distinctIndexes)
            {
                indexToScore[index] = currentValue > 0 ? currentValue : 2;
                currentValue -= 2;
            }

            // Apply score per employee
            for (int i = 0; i < employeeEducations.Count; i++)
            {
                var (empId, index, _) = employeeEducations[i];
                decimal score = indexToScore[index];
                employeeEducations[i] = (empId, index, score);
            }

            return (minEdu, maxEdu, employeeEducations);
        }

        private decimal TotatExperience(int empId,List<int> relatedJobtitleIds, List<int> relevantJobtitleIds, List<int> nonRelatedjobtitleIds)
        {

            double sumOfExp = 0;
            decimal relevantex = 0;
            decimal relatedex = 0;
            decimal nonRelatedex = 0;
            decimal curretExp = 0;
            decimal totalExp = 0;
            decimal totalYear = 0;
            var person = db.Person_Employee.Find(empId);
            if (person != null)
            {
                sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
                var empExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();
                foreach (var ex in empExps)
                {
                    sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
                }
            }
            
            var experiences = db.InternalRecruitmentCriteriaTypeSettings
                .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE
                            || x.ExamType == ExamType.RELATED_EXPERIENCE
                            || x.ExamType == ExamType.NON_RELATED_EXPERIENCE).ToList();
            foreach (var experience in experiences)
            {
                var placement = db.Placements.Where(x => x.isCurrent&& x.EmployeeId == empId
                                                            && (relatedJobtitleIds.Contains(x.LookupId)
                                                            || nonRelatedjobtitleIds.Contains(x.LookupId)
                                                            || relevantJobtitleIds.Contains(x.LookupId))).ToList();



                if (experience.ExamType == ExamType.RELATED_EXPERIENCE)
                {
                    var placement1 = placement.Where(x => relatedJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    if (placement1 != null)
                    {
                        curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    }
                   
                    var empExps = db.PlacementHistories.Where(x => x.EmployeeId == empId 
                                            && relatedJobtitleIds.Contains(x.LookupId))
                                            .ToList();
                    foreach (var ex in empExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }
                    var empExpsAfterSystems = db.PlacementAfterSystems
                       .Where(x => x.EmployeeId == empId
                                   && relatedJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExpsAfterSystems)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                    var empExternalExps = db.Experiences
                       .Where(x => x.LookupId != null
                                   && x.EmployeeId == empId
                                   && relatedJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExternalExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                }
                if (experience.ExamType == ExamType.NON_RELATED_EXPERIENCE)
                {


                    nonRelatedex += (decimal)sumOfExp * experience.Rate;


                }
                if (experience.ExamType == ExamType.RELEVANT_EXPERIENCE)
                {
                    var placement1 = placement.Where(x => x.isCurrent 
                    && relevantJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    if (placement1 != null)
                    {
                        curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    }
                    relevantJobtitleIds = db.ExperienceJobTitleDetails
                               .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE)
                               .Select(x => x.LookupId).Distinct()
                               .ToList();
                    var empExps = db.PlacementHistories.Where(x => x.EmployeeId == empId
                                                                && relevantJobtitleIds.Contains(x.LookupId));
                    foreach (var ex in empExps)
                    {
                        relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }
                    var empExpsAfterSystems = db.PlacementAfterSystems.Where(x => x.EmployeeId == empId
                                                                && relevantJobtitleIds.Contains(x.LookupId));
                    foreach (var ex in empExpsAfterSystems)
                    {
                        relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }
                    var empExternalExps = db.Experiences
                        .Where(x => x.LookupId != null
                                    && x.EmployeeId == empId
                                    && relevantJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExternalExps)
                    {
                        relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }


                }


            }


            totalExp = relatedex + nonRelatedex + relevantex + curretExp;
            totalYear = (decimal)((double)totalExp / 365.25);
            return totalYear;
        }
        private decimal TotatNonRelatedExperience(int empId)
        {

            double sumOfExp = 0;
            decimal nonRelatedex = 0;
            var person = db.Person_Employee.Find(empId);
            if (person != null)
            {
                sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
                var empExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();
                foreach (var ex in empExps)
                {
                    sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
                }
            }
            
            var experience = db.InternalRecruitmentCriteriaTypeSettings
                .Where(x => x.ExamType == ExamType.NON_RELATED_EXPERIENCE).FirstOrDefault();
            if ( experience !=null)
            {

                

                nonRelatedex += (decimal)sumOfExp * experience.Rate;
                
            }


          
            return Math.Round(nonRelatedex / 365.25m, 2);
        }
        private decimal TotatRelatedExperience(int empId,List<int> relatedJobtitleIds)
        {

           
           
            decimal relatedex = 0;
            decimal curretExp = 0;
         
            var person = db.Person_Employee.Find(empId);
           
            
            var experience = db.InternalRecruitmentCriteriaTypeSettings
                .FirstOrDefault(x =>  x.ExamType == ExamType.RELATED_EXPERIENCE);
            if (experience !=null)
            {
                var placement = db.Placements.Where(x => x.isCurrent
                                                            && x.EmployeeId ==empId
                                                            && relatedJobtitleIds.Contains(x.LookupId)).ToList();



                if (experience.ExamType == ExamType.RELATED_EXPERIENCE)
                {
                    var placement1 = placement.Where(x => relatedJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    if (placement1 != null)
                    {
                        curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    }
                   
                    var empExps = db.PlacementHistories.Where(x => x.EmployeeId == empId 
                                            && relatedJobtitleIds.Contains(x.LookupId))
                                            .ToList();
                    foreach (var ex in empExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }
                    var empExpsAfterSystems = db.PlacementAfterSystems
                       .Where(x => x.EmployeeId == empId
                                   && relatedJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExpsAfterSystems)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                    var empExternalExps = db.Experiences
                       .Where(x => x.LookupId != null
                                   && x.EmployeeId == empId
                                   && relatedJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExternalExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                }
               

            }


            return Math.Round(((relatedex + curretExp) / 365.25m),2);
        }
        private decimal TotatRelevantExperience(int empId,List<int> relevantJobtitleIds)
        {

           
           
            decimal relatedex = 0;
            decimal curretExp = 0;
         
            var person = db.Person_Employee.Find(empId);
           
            
            var experience = db.InternalRecruitmentCriteriaTypeSettings
                .FirstOrDefault(x =>  x.ExamType == ExamType.RELEVANT_EXPERIENCE);
            if (experience !=null)
            {
                var placement = db.Placements.Where(x => x.isCurrent
                                                            && x.EmployeeId ==empId
                                                            && relevantJobtitleIds.Contains(x.LookupId)).ToList();



                if (experience.ExamType == ExamType.RELATED_EXPERIENCE)
                {
                    var placement1 = placement.Where(x => relevantJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    if (placement1 != null)
                    {
                        curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    }
                   
                    var empExps = db.PlacementHistories.Where(x => x.EmployeeId == empId 
                                            && relevantJobtitleIds.Contains(x.LookupId))
                                            .ToList();
                    foreach (var ex in empExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }
                    var empExpsAfterSystems = db.PlacementAfterSystems
                       .Where(x => x.EmployeeId == empId
                                   && relevantJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExpsAfterSystems)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                    var empExternalExps = db.Experiences
                       .Where(x => x.LookupId != null
                                   && x.EmployeeId == empId
                                   && relevantJobtitleIds.Contains((int)x.LookupId));
                    foreach (var ex in empExternalExps)
                    {
                        relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    }

                }
               

            }


            return Math.Round(((relatedex + curretExp) / 365.25m),2);
        }
        public ActionResult ScreenAndGenerateApplicants(int vacancyId)
        {
            var result = new
            {
                status = "Fail",
                message = "Can't find screening setting"
            };
            List<int> jobtitleIds = new List<int>();
            List<int> nonRelatedjobtitleIds = new List<int>();
            List<int> relatedJobtitleIds = new List<int>();
            List<int> relevantJobtitleIds = new List<int>();
            List<int> FieldIds = new List<int>();
            List<ApplicantionForm> applicationForms = new List<ApplicantionForm>();
           
            var vacancy = db.VaccancyPlans.Find(vacancyId);
            if (vacancy == null)
            {
                return Json(new
                {
                    status = "Not-Ok",
                    message = "No vacancy found"
                }, JsonRequestBehavior.AllowGet);
            }
           
            var expereiceJobTitle = db.ExperienceJobTitles
                                        .FirstOrDefault(x => x.LookupId == vacancy.JobTitle);
            if(expereiceJobTitle == null)
            {
                return Json(new
                {
                    status = "Not-Ok",
                    message = "You have to set all setting for this job Title"
                }, JsonRequestBehavior.AllowGet);
            }
          
           var experienceJobTitleDetails = db.ExperienceJobTitleDetails
                            .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
                            .ToList();
            jobtitleIds = experienceJobTitleDetails
                            .Select(x => x.LookupId).Distinct()
                            .ToList();
            relatedJobtitleIds = experienceJobTitleDetails
                            .Where(x => x.ExamType == ExamType.RELATED_EXPERIENCE)
                            .Select(x => x.LookupId).Distinct()
                            .ToList();
            nonRelatedjobtitleIds = experienceJobTitleDetails
                            .Where(x => x.ExamType == ExamType.NON_RELATED_EXPERIENCE)
                            .Select(x => x.LookupId).Distinct().ToList();

            relevantJobtitleIds = db.ExperienceJobTitleDetails
                            .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE)
                            .Select(x => x.LookupId).Distinct()
                            .ToList();
            // Step 1: Get all EducationLevelDetails for the given job title

            var levels = db.EducationLevelDetails
            .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
            .ToList();

            // Step 2: Get distinct level IDs from the above list
            var levelIds = levels
                .Select(x => x.LookupId)
                .Distinct()
                .ToList();

            FieldIds = db.EducationFieldDetails
                            .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
                            .Select(x => x.LookupId).Distinct()
                            .ToList();
            var relatedExperiance = db.RelatedEducationFieldDetails
                                          .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
                                          .Select(x => x.LookupId)
                                          .ToList();
            var screeningSetting = db.ScreeningSettings
                                        .FirstOrDefault(x => x.VacancyId == vacancyId);
            if(screeningSetting == null)
            {
                return Json(new
                {
                    status = "Not-Ok",
                    message = "No screening setting defined for this vacancy"
                }, JsonRequestBehavior.AllowGet);
            }
            var applicants = db.ApplicantionFormes.Where(x => x.VacancyId == screeningSetting.VacancyId).ToList();
            var settingValueForCal = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == ExamType.TOTAL_EXPERIENCE
                                                                               && x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));


            decimal settingValueCal = settingValueForCal?.SettingVlaue ?? 0;
            var (minExp, maxExp, employeeExperiences) = GetMaximumAndMinimumExperience(
                vacancy.TotalYearOfExperience,
                applicants,
                screeningSetting.MiniExperiencePercentage,
                settingValueCal
            );
            var educationsetting = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == ExamType.EDUCATION_LEVEL
                                                                               && x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));
            var educationsettingValue = educationsetting?.SettingVlaue ?? 0;
            var (maxEdu, minEdu,EmployeeEducationValues) = GetEmployeeEducationResult(applicants,educationsettingValue);

            int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;
            if (screeningSetting.IsPercentage)
            {
                //filtered by gender
                if (vacancy.Gender == GenderForVacancyGender.MALE
                    || vacancy.Gender == GenderForVacancyGender.FEMALE)
                {
                    var employeeByGender = db.Person_Employee
                        .Where(x => x.Gender.ToString() == vacancy.Gender.ToString())?.Select(x => x.EmployeeId).ToList();

                    if (employeeByGender.Any())
                    {
                        applicants = applicants.Where(x => employeeByGender.Contains(x.EmployeeId)).ToList();
                    }
                }
                var eduLevelScreening = db.ScreeningEducationLevels
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList()
                    .Select(x => x.EducationLevelId);
                var eduFieldScreening = db.ScreeningEducationFields
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList()
                    .Select(x => x.EducationFieldId);
                var expJobtitle = db.ScreeningExperienceJobtitleForInternals
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId)
                    .Select(x => x.ExperienceJobTitleId)
                    .ToList();

                var otherExperience = new List<Experience>();
                foreach (var app in applicants)
                {
                    //var appEducations = db.Education.Where(x => x.EmployeeId == app.EmployeeId
                    //                              && (levelIds.Contains(x.EduLevel)
                    //                              || eduLevelScreening.Contains(x.EduLevel)
                    //                               || eduFieldScreening.Contains(x.EduField)))
                    //                               .ToList();
                    //var applicalLevelIds = appEducations.Select(x => x.EduLevel);
                    // Step 3: Filter employee's education based on level or field screening
                    var appEducations = db.Education
                        .Where(x => x.EmployeeId == app.EmployeeId &&
                                   (levelIds.Contains(x.EduLevel) ||
                                    eduLevelScreening.Contains(x.EduLevel) ||
                                    eduFieldScreening.Contains(x.EduField)))
                        .ToList();

                    // Step 4: Get matched education level IDs from applicant
                    var applicableLevelIds = appEducations
                        .Select(x => x.EduLevel)
                        .Distinct()
                        .ToList();

                    #region commented
                    //decimal relevantex = 0;
                    //decimal relatedex = 0;
                    //decimal nonRelatedex = 0;
                    //decimal curretExp = 0;
                    //decimal totalExp = 0;
                    //decimal totalYear = 0;
                    //var person = db.Person_Employee.Find(app.EmployeeId);
                    //if (person != null)
                    //{
                    //    sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
                    //    employeId = person.EmployeeId;
                    //    var empExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();
                    //    otherExperience = empExps;
                    //    foreach (var ex in empExps)
                    //    {
                    //        sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
                    //    }
                    //}

                    //expYear = sumOfExp / 365;
                    //var appEducations = db.Education.Where(x => x.EmployeeId == app.EmployeeId 
                    //                               &&( levelIds.Contains(x.EduLevel) 
                    //                               || eduLevelScreening.Contains(x.EduLevel) 
                    //                                || eduFieldScreening.Contains(x.EduField))).ToList();
                    //var experiences = db.InternalRecruitmentCriteriaTypeSettings
                    //    .Where(x => x.ExamType==ExamType.RELEVANT_EXPERIENCE
                    //                ||x.ExamType==ExamType.RELATED_EXPERIENCE
                    //                ||x.ExamType==ExamType.NON_RELATED_EXPERIENCE).ToList();
                    //foreach(var experience in experiences)
                    //{
                    //    var placement = db.Placements.Where(x => x.isCurrent
                    //                                                && x.EmployeeId == app.EmployeeId
                    //                                                && (relatedJobtitleIds.Contains(x.LookupId)
                    //                                                || nonRelatedjobtitleIds.Contains(x.LookupId)
                    //                                                || relevantJobtitleIds.Contains(x.LookupId))).ToList();



                    //    if (experience.ExamType == ExamType.RELATED_EXPERIENCE)
                    //    {
                    //        var placement1 = placement.Where(x => relatedJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    //        if (placement1 != null)
                    //        {
                    //            curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    //        }
                    //        relatedJobtitleIds = experienceJobTitleDetails
                    //                                  .Where(x => x.ExamType == ExamType.RELATED_EXPERIENCE)
                    //                                  .Select(x => x.LookupId).Distinct()
                    //                                  .ToList();
                    //        var empExps = db.PlacementHistories.Where(x => x.EmployeeId == app.EmployeeId && relatedJobtitleIds.Contains(x.LookupId)).ToList();
                    //        foreach (var ex in empExps)
                    //        {
                    //            relatedex +=  ((decimal)ex.To.Subtract(ex.From).TotalDays+1) * experience.Rate;
                    //        }
                    //        var empExpsAfterSystems = db.PlacementAfterSystems
                    //           .Where(x =>x.EmployeeId == app.EmployeeId
                    //                       && relatedJobtitleIds.Contains((int)x.LookupId));
                    //        foreach (var ex in empExpsAfterSystems)
                    //        {
                    //            relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    //        }

                    //        var empExternalExps = db.Experiences
                    //           .Where(x => x.LookupId != null
                    //                       && x.EmployeeId == app.EmployeeId
                    //                       && relatedJobtitleIds.Contains((int)x.LookupId));
                    //        foreach (var ex in empExternalExps)
                    //        {
                    //            relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) * experience.Rate;
                    //        }

                    //    }
                    //    if (experience.ExamType == ExamType.NON_RELATED_EXPERIENCE)
                    //    {


                    //            nonRelatedex += (decimal)sumOfExp * experience.Rate;


                    //    }
                    //    if (experience.ExamType == ExamType.RELEVANT_EXPERIENCE)
                    //    {
                    //        var placement1 = placement.Where(x =>x.isCurrent&& relevantJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

                    //        if (placement1 != null)
                    //        {
                    //            curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;

                    //        }
                    //        relevantJobtitleIds = db.ExperienceJobTitleDetails
                    //                   .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE)
                    //                   .Select(x => x.LookupId).Distinct()
                    //                   .ToList();
                    //        var empExps = db.PlacementHistories.Where(x => x.EmployeeId== app.EmployeeId
                    //                                                    &&relevantJobtitleIds.Contains(x.LookupId));
                    //        foreach (var ex in empExps)
                    //        {
                    //            relevantex +=( (decimal)ex.To.Subtract(ex.From).TotalDays+1) * experience.Rate;
                    //        }
                    //        var empExpsAfterSystems = db.PlacementAfterSystems.Where(x => x.EmployeeId== app.EmployeeId
                    //                                                    &&relevantJobtitleIds.Contains(x.LookupId));
                    //        foreach (var ex in empExpsAfterSystems)
                    //        {
                    //            relevantex +=( (decimal)ex.To.Subtract(ex.From).TotalDays+1) * experience.Rate;
                    //        }
                    //        var empExternalExps = db.Experiences
                    //            .Where(x =>x.LookupId !=null
                    //                        && x.EmployeeId== app.EmployeeId
                    //                        &&relevantJobtitleIds.Contains((int)x.LookupId));
                    //        foreach (var ex in empExternalExps)
                    //        {
                    //            relevantex +=( (decimal)ex.To.Subtract(ex.From).TotalDays+1) * experience.Rate;
                    //        }


                    //    }


                    //}


                    //totalExp = relatedex + nonRelatedex + relevantex+curretExp;
                    #endregion

                    decimal totalYear = TotatExperience(app.EmployeeId,relatedJobtitleIds,relevantJobtitleIds,nonRelatedjobtitleIds);
                    // Step 5: Determine max experience based on matched level IDs or use default
                    decimal vacancyTotalExperience;

                    var matchingLevels = levels.Where(x => applicableLevelIds.Contains(x.LookupId)).ToList();

                    if (matchingLevels.Any())
                    {
                        vacancyTotalExperience = matchingLevels.Min(x => x.Experience);
                    }
                    else
                    {
                        vacancyTotalExperience = vacancy.TotalYearOfExperience;
                    }
                   
                    //if (vacancy.TotalYearOfExperience <= totalYear
                    //    && (!appEducations.Any()
                    //    || appEducations.Any()))
                    if (vacancyTotalExperience <= totalYear
                        && (!appEducations.Any()
                        || appEducations.Any()))
                    {
                        applicationForms.Add(app);

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
                            decimal relevantex = TotatRelevantExperience(app.EmployeeId,relevantJobtitleIds);
                            decimal relatedex = TotatRelatedExperience(app.EmployeeId, relatedJobtitleIds);
                            decimal nonRelatedex = TotatNonRelatedExperience(app.EmployeeId);
                            decimal totalYear = TotatExperience(app.EmployeeId, relatedJobtitleIds, relevantJobtitleIds, nonRelatedjobtitleIds);
                            var person = db.Person_Employee.Find(app.EmployeeId);

                           
                           
                            var checkApp = db.InternalApplicantScreenings
                                .FirstOrDefault(x => x.InternalApplicantScreeningResultId == checkExit.InternalApplicantScreeningResultId
                                                    && x.EmployeeId == app.EmployeeId);

                            if (checkApp != null)
                            {
                              

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var settingValue = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i 
                                                    &&x.IsPercent&& (x.ExamSource == RecruitmentSource.INTERNAL
                                                    || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                    || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));

                                    if (settingValue != null)
                                    {
                                        var appExam = db.InternalApplicantExamResults
                                            .FirstOrDefault(x => x.ExamType == (ExamType)i 
                                                                 && x.InternalApplicantScreeningId == checkApp.InternalApplicantScreeningId);
                                        if (appExam != null)
                                        {
                                            appExam.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            appExam.ExamType = (ExamType)i;
                                            appExam.SettingValue = settingValue.SettingVlaue;
                                            switch (appExam.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = 0;
                                                    }
                                                    appExam.ActualValue = appExam.SettingValue;

                                                    break;
                                                case ExamType.DISABILITY:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    appExam.CurrentResult = (appExam.ActualValue* appExam .SettingValue)/ 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = measure?.DisciplineMea?.Rank ?? appExam.SettingValue;
                                                    appExam.CurrentResult = appExam.ActualValue >= settingValue.SettingVlaue ? appExam.SettingValue : appExam.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    appExam.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {

                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.FinalValue;
                                                        }
                                                       
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = (appExam.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? settingValue.SettingVlaue
                                                            : appExam.ActualValue;
                                                    }


                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:
                                                    if (settingValue.IsPercent)
                                                    {
                                                        appExam.ActualValue = (decimal)relatedex;
                                                        appExam.CurrentResult = settingValue.SettingVlaue * settingValue.Rate;
                                                    }
                                                    if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && jobtitleIds.Contains(x.LookupId)) ||
                                                        db.Placements.Any(x => x.EmployeeId == app.EmployeeId && jobtitleIds.Contains(x.LookupId)))
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:
                                                    if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId 
                                                    && jobtitleIds.Contains(x.LookupId)) ||
                                                        db.Placements.Any(x => x.EmployeeId == app.EmployeeId
                                                        && jobtitleIds.Contains(x.LookupId)))
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    appExam.ActualValue = nonRelatedex;
                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            appExam.CurrentResult = appExam.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:
                                                    if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && jobtitleIds.Contains(x.LookupId)) ||
                                                        db.Placements.Any(x => x.EmployeeId == app.EmployeeId && jobtitleIds.Contains(x.LookupId)))
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;
                                            }

                                            db.Entry(appExam).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var aer = new InternalApplicantExamResult();

                                            aer.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            aer.ExamType = (ExamType)i;
                                            aer.SettingValue = settingValue.SettingVlaue;
                                            switch (settingValue.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        aer.CurrentResult = 0;
                                                    }
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                                case ExamType.DISABILITY:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = aer.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                    aer.CurrentResult = aer.ActualValue >= settingValue.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    aer.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {
                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.FinalValue;
                                                        }
                                                       
                                                    }
                                                    else
                                                    {
                                                        aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? settingValue.SettingVlaue
                                                            : aer.ActualValue;
                                                    }


                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:
                                                    
                                                    if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && relatedExperiance.Contains(x.LookupId)) ||
                                                        db.Placements.Any(x => x.EmployeeId == app.EmployeeId && relatedJobtitleIds.Contains(x.LookupId)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                    aer.ActualValue = aer.SettingValue;

                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:
                                                    if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && nonRelatedjobtitleIds.Contains(x.LookupId)) ||
                                                        db.Placements.Any(x => x.EmployeeId == app.EmployeeId && nonRelatedjobtitleIds.Contains(x.LookupId)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                        aer.ActualValue = nonRelatedex;

                                                    }
                                                    aer.ActualValue = appExam.SettingValue;

                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    aer.ActualValue = aer.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            aer.CurrentResult = aer.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:


                                                    aer.ActualValue = relevantex;
                                                   
                                                    aer.CurrentResult =relevantex> aer.SettingValue?aer.SettingValue:aer.ActualValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    aer.CurrentResult = aer.SettingValue;
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                            }

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

                                //int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var aer = new InternalApplicantExamResult();

                                    var settingValue = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i
                                                                               &&x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));

                                    if (settingValue != null)
                                    {
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = (ExamType)i;
                                        aer.SettingValue = settingValue.SettingVlaue;
                                      
                                        switch (settingValue.ExamType)
                                        {
                                            case ExamType.THEORY_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.PRACTICAL_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.INTERVIEW:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.GENDER:
                                                if (person != null && person.Gender == Gender.FEMALE)
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = 0;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                            case ExamType.DISABILITY:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = aer.SettingValue;
                                                break;

                                            case ExamType.PERFORMANCE:
                                                var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                var EmpPerformance = db.EmployeePerformanceSummary
                                                    .Where(x => x.EmployeeId == app.EmployeeId)
                                                    .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                    .FirstOrDefault();
                                                aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                break;

                                            case ExamType.DISCIPLINE:
                                                var measure = db.EmpDisciplineMeasure
                                                    .Where(x => db.DisciplinaryCases
                                                        .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                        .Select(dc => dc.DisciplinaryCaseId)
                                                        .Contains(x.DisciplinaryCaseId))
                                                    .FirstOrDefault();
                                                aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                aer.CurrentResult = aer.ActualValue >= settingValue.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.TOTAL_EXPERIENCE:
                                                aer.ActualValue = totalYear;
                                                if (screeningSetting.IsBasedOnMaxExperience
                                                    && screeningSetting.MiniExperiencePercentage > 0)
                                                {
                                                    var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.FinalValue;
                                                    }
                                                    //aer.CurrentResult = (expYear == (double)minExperience) ?
                                                    //                                      (screeningSetting.MiniExperiencePercentage * settingValue.SettingVlaue) / 100
                                                    //                            : (expYear > (double)minExperience && expYear < (double)maxExperience) ?
                                                    //                            (((screeningSetting.MiniExperiencePercentage + 100) / 2) * settingValue.SettingVlaue) / 100
                                                    //                            : settingValue.SettingVlaue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                        ? settingValue.SettingVlaue
                                                        : aer.ActualValue;
                                                }

                                               
                                                break;

                                            case ExamType.RELATED_EXPERIENCE:

                                                if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && relatedExperiance.Contains(x.LookupId)) ||
                                                    db.Placements.Any(x => x.EmployeeId == app.EmployeeId && relatedJobtitleIds.Contains(x.LookupId)))
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                aer.ActualValue = aer.SettingValue;

                                                break;

                                            case ExamType.NON_RELATED_EXPERIENCE:

                                                    aer.ActualValue = nonRelatedex;
                                                    aer.CurrentResult = nonRelatedex>aer.SettingValue?aer.SettingValue:aer.ActualValue;
                                                break;

                                            case ExamType.EDUCATION_LEVEL:
                                                aer.ActualValue = aer.SettingValue;
                                                if (screeningSetting.ApplyEducationRanking)
                                                {
                                                    var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.education;
                                                    }
                                                   
                                                }
                                                else
                                                {
                                                    if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                }

                                               
                                                break;

                                            case ExamType.RELEVANT_EXPERIENCE:
                                                if (db.PlacementHistories.Any(x => x.EmployeeId == app.EmployeeId && relevantJobtitleIds.Contains(x.LookupId)) ||
                                                    db.Placements.Any(x => x.EmployeeId == app.EmployeeId && relevantJobtitleIds.Contains(x.LookupId)))
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;

                                            case ExamType.OTHERS:
                                                aer.CurrentResult = aer.SettingValue;
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                        }
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
                            decimal relevantex = TotatRelevantExperience(app.EmployeeId, relevantJobtitleIds);
                            decimal relatedex = TotatRelatedExperience(app.EmployeeId, relatedJobtitleIds);
                            decimal nonRelatedex = TotatNonRelatedExperience(app.EmployeeId);
                            decimal totalYear = TotatExperience(app.EmployeeId, relatedJobtitleIds, relevantJobtitleIds, nonRelatedjobtitleIds);
                            var person = db.Person_Employee.Find(app.EmployeeId);

                            var checkApp = db.InternalApplicantScreenings.FirstOrDefault(x => x.InternalApplicantScreeningResultId == asr.InternalApplicantScreeningResultId && x.EmployeeId == app.EmployeeId);

                            if (checkApp == null)
                            {
                                var asd = new InternalApplicantScreening();

                                asd.InternalApplicantScreeningResultId = asr.InternalApplicantScreeningResultId;
                                asd.EmployeeId = app.EmployeeId;
                                db.InternalApplicantScreenings.Add(asd);
                                db.SaveChanges();

                                //int countExamTypes = Enum.GetNames(typeof(ExamType)).Length;

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var aer = new InternalApplicantExamResult();

                                    var settingValue = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i
                                                                               && x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));

                                    if (settingValue != null)
                                    {
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = (ExamType)i;
                                        aer.SettingValue = settingValue.SettingVlaue;
                                        switch (settingValue.ExamType)
                                        {
                                            case ExamType.THEORY_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.PRACTICAL_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.INTERVIEW:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.GENDER:
                                                if (person != null && person.Gender == Gender.FEMALE)
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = 0;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                            case ExamType.DISABILITY:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = aer.SettingValue;
                                                break;

                                            case ExamType.PERFORMANCE:
                                                var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                var EmpPerformance = db.EmployeePerformanceSummary
                                                    .Where(x => x.EmployeeId == app.EmployeeId)
                                                    .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                    .FirstOrDefault();
                                                aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                break;

                                            case ExamType.DISCIPLINE:
                                                var measure = db.EmpDisciplineMeasure
                                                    .Where(x => db.DisciplinaryCases
                                                        .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                        .Select(dc => dc.DisciplinaryCaseId)
                                                        .Contains(x.DisciplinaryCaseId))
                                                    .FirstOrDefault();
                                                aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                aer.CurrentResult = aer.ActualValue >= settingValue.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.TOTAL_EXPERIENCE:
                                                aer.ActualValue = totalYear;
                                                if (screeningSetting.IsBasedOnMaxExperience
                                                    && screeningSetting.MiniExperiencePercentage > 0)
                                                {
                                                    var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.FinalValue;
                                                    }
                                                  
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                        ? settingValue.SettingVlaue
                                                        : aer.ActualValue;
                                                }

                                                break;

                                            case ExamType.RELATED_EXPERIENCE:


                                                aer.ActualValue = relatedex;

                                                aer.CurrentResult = relatedex>aer.SettingValue?aer.SettingValue:aer.ActualValue;
                                               

                                                break;

                                            case ExamType.NON_RELATED_EXPERIENCE:




                                                aer.ActualValue = nonRelatedex;

                                                
                                                aer.CurrentResult =nonRelatedex> aer.SettingValue?aer.SettingValue:aer.ActualValue;

                                                break;

                                            case ExamType.EDUCATION_LEVEL:
                                                aer.ActualValue = aer.SettingValue;
                                                if (screeningSetting.ApplyEducationRanking)
                                                {
                                                    var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.education;
                                                    }

                                                }
                                                else
                                                {
                                                    if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                }
                                                break;

                                            case ExamType.RELEVANT_EXPERIENCE:
                                               
                                                aer.ActualValue = relevantex;
                                                aer.CurrentResult = relevantex> aer.SettingValue?aer.SettingValue:aer.ActualValue;
                                                break;

                                            case ExamType.OTHERS:
                                                aer.CurrentResult = aer.SettingValue;
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                        }
                                        db.InternalApplicantExamResults.Add(aer);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var asd = new InternalApplicantScreening();

                                asd.InternalApplicantScreeningResultId = asr.InternalApplicantScreeningResultId;
                                asd.EmployeeId = app.EmployeeId;
                                db.InternalApplicantScreenings.Add(asd);
                                db.SaveChanges();

                                for (int i = 0; i < countExamTypes; i++)
                                {
                                    var aer = new InternalApplicantExamResult();

                                    var settingValue = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x => x.ExamType == (ExamType)i
                                                                               && x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER));
                                    if (settingValue != null)
                                    {
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = (ExamType)i;
                                        aer.SettingValue = settingValue.SettingVlaue;
                                        switch (settingValue.ExamType)
                                        {
                                            case ExamType.THEORY_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.PRACTICAL_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.INTERVIEW:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.GENDER:
                                                if (person != null && person.Gender == Gender.FEMALE)
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = 0;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                            case ExamType.DISABILITY:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = aer.SettingValue;
                                                break;

                                            case ExamType.PERFORMANCE:
                                                var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                var EmpPerformance = db.EmployeePerformanceSummary
                                                    .Where(x => x.EmployeeId == app.EmployeeId)
                                                    .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                    .FirstOrDefault();
                                                aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                break;

                                            case ExamType.DISCIPLINE:
                                                var measure = db.EmpDisciplineMeasure
                                                    .Where(x => db.DisciplinaryCases
                                                        .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                        .Select(dc => dc.DisciplinaryCaseId)
                                                        .Contains(x.DisciplinaryCaseId))
                                                    .FirstOrDefault();
                                                aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                aer.CurrentResult = aer.ActualValue >= settingValue.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.TOTAL_EXPERIENCE:
                                                aer.ActualValue = totalYear;
                                                if (screeningSetting.IsBasedOnMaxExperience
                                                    && screeningSetting.MiniExperiencePercentage > 0)
                                                {
                                                   
                                                    var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.FinalValue;
                                                    }
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                        ? settingValue.SettingVlaue
                                                        : aer.ActualValue;
                                                }

                                                break;

                                            case ExamType.RELATED_EXPERIENCE:
                                                aer.ActualValue = relevantex;
                                                aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;


                                                break;

                                            case ExamType.NON_RELATED_EXPERIENCE:
                                                aer.ActualValue = nonRelatedex;
                                                aer.CurrentResult = nonRelatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;

                                                break;

                                            case ExamType.EDUCATION_LEVEL:
                                                aer.ActualValue = aer.SettingValue;
                                                if (screeningSetting.ApplyEducationRanking)
                                                {
                                                    var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.education;
                                                    }

                                                }
                                                else
                                                {
                                                    if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                }
                                                break;

                                            case ExamType.RELEVANT_EXPERIENCE:

                                                aer.ActualValue = relevantex;
                                                aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.OTHERS:
                                                aer.CurrentResult = aer.SettingValue;
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                        }

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
                        status = "Not-Ok",
                        message = "No applicant pass the screening"
                    };

                    return Json(result);
                }

            }
            else if (screeningSetting != null
                && !screeningSetting.IsPercentage)
            {

                //var applicants = db.ApplicantionFormes
                //                        .Where(x => x.VacancyId == screeningSetting.VacancyId).ToList();
                //filtered by gender
                if (vacancy.Gender == GenderForVacancyGender.MALE 
                    || vacancy.Gender == GenderForVacancyGender.FEMALE)
                {
                    var employeeByGender = db.Person_Employee
                        .Where(x => x.Gender.ToString() == vacancy.Gender.ToString())?.Select(x => x.EmployeeId).ToList();

                    if (employeeByGender.Any())
                    {
                        applicants = applicants.Where(x => employeeByGender.Contains(x.EmployeeId)).ToList();
                    }

                }

                var eduLevelScreening = db.ScreeningEducationLevels
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList()
                    .Select(x => x.EducationLevelId);
                var eduFieldScreening = db.ScreeningEducationFields
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId).ToList()
                    .Select(x => x.EducationFieldId);
                var expJobtitle = db.ScreeningExperienceJobtitleForInternals
                    .Where(x => x.ScreeingSettingId == screeningSetting.ScreeningSettingId)
                    .Select(x => x.ExperienceJobTitleId).ToList();
                var otherExperience = new List<Experience>();
                var sequences = db.InternalRecruitmentCriteriaTypeSettings.Where(x => x.IsPercent && (x.ExamSource == RecruitmentSource.INTERNAL
                                                || x.ExamSource == RecruitmentSource.INTERNALPROMOTION
                                                || x.ExamSource == RecruitmentSource.INTERNALTRANSFER)).OrderBy(x => x.Squence);
                // pre screening
                foreach (var sequence in sequences)
                {
                    var applicantEmpIds = applicants.Select(x => x.EmployeeId).ToList().Distinct();
                    var persons = db.Person_Employee.Where(x => applicantEmpIds.Contains(x.EmployeeId) && x.IsActive);

                    if (sequence.ExamType == ExamType.PERFORMANCE)
                    {
                        var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                        var empIds = db.EmployeePerformanceSummary
                                                .Where(x => x.Result >= sequence.SettingVlaue)
                                                         .Select(x => x.EmployeeId).ToList();
                        applicants = applicants.Where(x => empIds.Contains(x.EmployeeId)).ToList();


                    }
                    else if (sequence.ExamType == ExamType.DISCIPLINE)
                    {
                        
                        var empIds =  db.DisciplinaryCases.Where(dc => dc.IsActive)
                                                                    .Select(dc => dc.EmployeeId).ToList();
                        applicants = applicants.Where(x => !empIds.Contains(x.EmployeeId)).ToList();


                    }
                   
                    else if (sequence.ExamType == ExamType.TOTAL_EXPERIENCE)
                    {
                        List<int> activeEmpIds = new List<int>();

                     
                        foreach (var person in persons)
                        {
                            decimal totalYear = TotatExperience(person.EmployeeId, relatedJobtitleIds, relevantJobtitleIds, nonRelatedjobtitleIds);
                            if (sequence.SettingVlaue <= totalYear)
                            {
                                activeEmpIds.Add(person.EmployeeId);

                            }
                        }

                        applicants = applicants.Where(x => activeEmpIds.Contains(x.EmployeeId)).ToList();



                    }
                    else if (sequence.ExamType == ExamType.RELATED_EXPERIENCE)
                    {
                        List<int> activeEmpIds = new List<int>();
                        foreach (var person in persons)
                        {
                            decimal relatedex = TotatRelatedExperience(person.EmployeeId, relatedJobtitleIds);
                            if (sequence.SettingVlaue <= relatedex)
                            {
                                activeEmpIds.Add(person.EmployeeId);

                            }

                        }
                        applicants = applicants.Where(x => activeEmpIds.Contains(x.EmployeeId)).ToList();


                    }
                    else if (sequence.ExamType == ExamType.NON_RELATED_EXPERIENCE)
                    {
                        //var empIds = db.Placements.Where(x => nonRelatedjobtitleIds.Contains(x.LookupId)).Select(x => x.EmployeeId).Distinct();

                        //applicants = applicants.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                        List<int> activeEmpIds = new List<int>();
                        foreach (var person in persons)
                        {
                            decimal nonRelatedex = TotatNonRelatedExperience(person.EmployeeId);
                            if (sequence.SettingVlaue <= nonRelatedex)
                            {
                                activeEmpIds.Add(person.EmployeeId);

                            }

                        }
                        applicants = applicants.Where(x => activeEmpIds.Contains(x.EmployeeId)).ToList();


                    }
                    else if (sequence.ExamType == ExamType.RELEVANT_EXPERIENCE)
                    {
                        //var empIds = db.Placements.Where(x => relevantJobtitleIds.Contains(x.LookupId)).Select(x => x.EmployeeId).Distinct();

                        //applicants = applicants.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                        List<int> activeEmpIds = new List<int>();
                        foreach (var person in persons)
                        {
                            decimal relevantex = TotatRelevantExperience(person.EmployeeId, relevantJobtitleIds);
                            if (sequence.SettingVlaue <= relevantex)
                            {
                                activeEmpIds.Add(person.EmployeeId);

                            }

                        }
                        applicants = applicants.Where(x => activeEmpIds.Contains(x.EmployeeId)).ToList();


                    }

                }

                //give therir result for prescreening passed applicants
                if (applicants.Any())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        var checkExit = db.InternalApplicantScreeningResults.FirstOrDefault(x => x.VacancyId == screeningSetting.VacancyId);
                        if (checkExit != null)
                        {
                            
                            foreach (var app in applicants)
                            {
                                decimal relevantex = TotatRelevantExperience(app.EmployeeId, relevantJobtitleIds);
                                decimal relatedex = TotatRelatedExperience(app.EmployeeId, relatedJobtitleIds);
                                decimal nonRelatedex = TotatNonRelatedExperience(app.EmployeeId);
                                decimal totalYear = TotatExperience(app.EmployeeId, relatedJobtitleIds, relevantJobtitleIds, nonRelatedjobtitleIds);

                                var checkApp = db.InternalApplicantScreenings
                                    .FirstOrDefault(x => x.InternalApplicantScreeningResultId == checkExit.InternalApplicantScreeningResultId
                                                        && x.EmployeeId == app.EmployeeId);

                                if (checkApp != null)
                                {
                                   

                                    foreach (var squence in sequences)
                                    {
                                      
                                        var appExam = db.InternalApplicantExamResults
                                            .FirstOrDefault(x => x.ExamType == squence.ExamType
                                                                 && x.InternalApplicantScreeningId == checkApp.InternalApplicantScreeningId);
                                        if (appExam != null)
                                        {
                                            appExam.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            appExam.ExamType = squence.ExamType;
                                            appExam.SettingValue = squence.SettingVlaue;
                                            switch (appExam.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    var person = db.Person_Employee.Find(app.EmployeeId);
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = 0;
                                                    }
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;
                                                case ExamType.DISABILITY:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    //appExam.CurrentResult = appExam.ActualValue >= squence.SettingVlaue ? appExam.SettingValue : 0;
                                                    appExam.CurrentResult = (appExam.ActualValue * appExam.SettingValue) / 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = measure?.DisciplineMea?.Rank ?? squence.SettingVlaue;
                                                    appExam.CurrentResult = appExam.ActualValue >= squence.SettingVlaue ? squence.SettingVlaue : appExam.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    appExam.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {
                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.FinalValue;
                                                        }
                                                       
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = (appExam.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? appExam.SettingValue
                                                            : appExam.ActualValue;
                                                    }

                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:

                                                    appExam.ActualValue = relatedex;
                                                    appExam.CurrentResult = relatedex > appExam.SettingValue ? squence.SettingVlaue : appExam.ActualValue;
                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:

                                                    appExam.ActualValue = nonRelatedex;
                                                    appExam.CurrentResult = nonRelatedex > appExam.SettingValue ? appExam.SettingValue : appExam.ActualValue;
                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            appExam.CurrentResult = appExam.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:

                                                    appExam.ActualValue =relevantex;
                                                    appExam.ActualValue = relevantex > appExam.SettingValue ? squence.SettingVlaue : appExam.ActualValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;
                                            }
                                            db.Entry(appExam).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var aer = new InternalApplicantExamResult();

                                            aer.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            aer.ExamType = squence.ExamType;
                                            aer.SettingValue = squence.SettingVlaue;
                                            switch (squence.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    var person = db.Person_Employee.Find(app.EmployeeId);
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        aer.CurrentResult = 0;
                                                    }
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                                case ExamType.DISABILITY:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = aer.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                    aer.CurrentResult = aer.ActualValue >= squence.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    aer.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {
                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.FinalValue;
                                                        }
                                                       
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = (appExam.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? squence.SettingVlaue
                                                            : appExam.ActualValue;
                                                    }

                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:
                                                    aer.ActualValue = relatedex;
                                                    aer.CurrentResult =relatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;


                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:
                                                    aer.ActualValue =nonRelatedex;
                                                    aer.CurrentResult = nonRelatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;

                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    aer.ActualValue = aer.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            aer.CurrentResult = aer.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:

                                                    aer.ActualValue = relevantex;
                                                    aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    aer.CurrentResult = aer.SettingValue;
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                            }
                                            db.InternalApplicantExamResults.Add(aer);
                                            db.SaveChanges();
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


                                    foreach (var squence in sequences)
                                    {
                                        var aer = new InternalApplicantExamResult();
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = squence.ExamType;
                                        aer.SettingValue = squence.SettingVlaue;
                                        switch (squence.ExamType)
                                        {
                                            case ExamType.THEORY_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.PRACTICAL_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.INTERVIEW:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.GENDER:
                                                var person = db.Person_Employee.Find(app.EmployeeId);
                                                if (person != null && person.Gender == Gender.FEMALE)
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = 0;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                            case ExamType.DISABILITY:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = aer.SettingValue;
                                                break;

                                            case ExamType.PERFORMANCE:
                                                var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                var EmpPerformance = db.EmployeePerformanceSummary
                                                    .Where(x => x.EmployeeId == app.EmployeeId)
                                                    .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                    .FirstOrDefault();
                                                aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                break;

                                            case ExamType.DISCIPLINE:
                                                var measure = db.EmpDisciplineMeasure
                                                    .Where(x => db.DisciplinaryCases
                                                        .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                        .Select(dc => dc.DisciplinaryCaseId)
                                                        .Contains(x.DisciplinaryCaseId))
                                                    .FirstOrDefault();
                                                aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                aer.CurrentResult = aer.ActualValue >= squence.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.TOTAL_EXPERIENCE:
                                                aer.ActualValue = totalYear;
                                                if (screeningSetting.IsBasedOnMaxExperience
                                                    && screeningSetting.MiniExperiencePercentage > 0)
                                                {
                                                    var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.FinalValue;
                                                    }
                                                   
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                        ? squence.SettingVlaue
                                                        : aer.ActualValue;
                                                }

                                                break;

                                            case ExamType.RELATED_EXPERIENCE:
                                                aer.ActualValue = relatedex;
                                                aer.CurrentResult = relatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;


                                                break;

                                            case ExamType.NON_RELATED_EXPERIENCE:
                                                aer.ActualValue = nonRelatedex;
                                                aer.CurrentResult = nonRelatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;

                                                break;

                                            case ExamType.EDUCATION_LEVEL:
                                                aer.ActualValue = aer.SettingValue;
                                                if (screeningSetting.ApplyEducationRanking)
                                                {
                                                    var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.education;
                                                    }

                                                }
                                                else
                                                {
                                                    if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                }
                                                break;

                                            case ExamType.RELEVANT_EXPERIENCE:

                                                aer.ActualValue = relevantex;
                                                aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.OTHERS:
                                                aer.CurrentResult = aer.SettingValue;
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                        }
                                       
                                        db.InternalApplicantExamResults.Add(aer);
                                        db.SaveChanges();
                                        

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
                            foreach (var app in applicants)
                            {
                                decimal relevantex = TotatRelevantExperience(app.EmployeeId, relevantJobtitleIds);
                                decimal relatedex = TotatRelatedExperience(app.EmployeeId, relatedJobtitleIds);
                                decimal nonRelatedex = TotatNonRelatedExperience(app.EmployeeId);
                                decimal totalYear = TotatExperience(app.EmployeeId, relatedJobtitleIds, relevantJobtitleIds, nonRelatedjobtitleIds);


                                var checkApp = db.InternalApplicantScreenings.FirstOrDefault(x => x.InternalApplicantScreeningResultId == asr.InternalApplicantScreeningResultId
                                && x.EmployeeId == app.EmployeeId);

                                if (checkApp == null)
                                {
                                    var asd = new InternalApplicantScreening();

                                    asd.InternalApplicantScreeningResultId = asr.InternalApplicantScreeningResultId;
                                    asd.EmployeeId = app.EmployeeId;

                                    db.InternalApplicantScreenings.Add(asd);
                                    db.SaveChanges();


                                    foreach (var squence in sequences)
                                    {
                                        var aer = new InternalApplicantExamResult();

                                        
                                        aer.InternalApplicantScreeningId = asd.InternalApplicantScreeningId;
                                        aer.ExamType = squence.ExamType;
                                        aer.SettingValue = squence.SettingVlaue;
                                     
                                        switch (squence.ExamType)
                                        {
                                            case ExamType.THEORY_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.PRACTICAL_EXAM:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.INTERVIEW:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = 0;
                                                break;
                                            case ExamType.GENDER:
                                                var person = db.Person_Employee.Find(app.EmployeeId);
                                                if (person != null && person.Gender == Gender.FEMALE)
                                                {
                                                    aer.CurrentResult = aer.SettingValue;
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = 0;
                                                }
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                            case ExamType.DISABILITY:
                                                aer.ActualValue = aer.SettingValue;
                                                aer.CurrentResult = aer.SettingValue;
                                                break;

                                            case ExamType.PERFORMANCE:
                                                var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                var EmpPerformance = db.EmployeePerformanceSummary
                                                    .Where(x => x.EmployeeId == app.EmployeeId)
                                                    .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                    .FirstOrDefault();
                                                aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                break;

                                            case ExamType.DISCIPLINE:
                                                var measure = db.EmpDisciplineMeasure
                                                    .Where(x => db.DisciplinaryCases
                                                        .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                        .Select(dc => dc.DisciplinaryCaseId)
                                                        .Contains(x.DisciplinaryCaseId))
                                                    .FirstOrDefault();
                                                aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                aer.CurrentResult = aer.ActualValue >= squence.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.TOTAL_EXPERIENCE:
                                                aer.ActualValue = totalYear;
                                                if (screeningSetting.IsBasedOnMaxExperience
                                                    && screeningSetting.MiniExperiencePercentage > 0)
                                                {
                                                    var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.FinalValue;
                                                    }
                                                   
                                                }
                                                else
                                                {
                                                    aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                        ? squence.SettingVlaue
                                                        : aer.ActualValue;
                                                }

                                                break;

                                            case ExamType.RELATED_EXPERIENCE:
                                                aer.ActualValue = relatedex;
                                                aer.CurrentResult = relatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;


                                                break;

                                            case ExamType.NON_RELATED_EXPERIENCE:
                                                aer.ActualValue = nonRelatedex;
                                                aer.CurrentResult = nonRelatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;

                                                break;

                                            case ExamType.EDUCATION_LEVEL:
                                                aer.ActualValue = aer.SettingValue;
                                                if (screeningSetting.ApplyEducationRanking)
                                                {
                                                    var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                    // Check if employee data was found
                                                    if (employeeData.EmployeeId != 0)
                                                    {
                                                        aer.CurrentResult = employeeData.education;
                                                    }

                                                }
                                                else
                                                {
                                                    if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                }
                                                break;

                                            case ExamType.RELEVANT_EXPERIENCE:

                                                aer.ActualValue = relevantex;
                                                aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;
                                                break;

                                            case ExamType.OTHERS:
                                                aer.CurrentResult = aer.SettingValue;
                                                aer.ActualValue = aer.SettingValue;
                                                break;
                                        }
                                        db.InternalApplicantExamResults.Add(aer);
                                    }
                                    db.SaveChanges();
                                    transaction.Commit();

                                }
                                else
                                {

                                    foreach (var squence in sequences)
                                    {
                                       
                                        var appExam = db.InternalApplicantExamResults
                                            .FirstOrDefault(x => x.ExamType == squence.ExamType
                                                                 && x.InternalApplicantScreeningId == checkApp.InternalApplicantScreeningId);
                                        if (appExam != null)
                                        {
                                            appExam.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            appExam.ExamType = squence.ExamType;
                                            appExam.SettingValue = squence.SettingVlaue;
                                            switch (appExam.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    var person = db.Person_Employee.Find(app.EmployeeId);
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        appExam.CurrentResult = appExam.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = 0;
                                                    }
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;
                                                case ExamType.DISABILITY:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    appExam.CurrentResult = (appExam.ActualValue * appExam.SettingValue) / 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    appExam.ActualValue = measure?.DisciplineMea?.Rank ?? appExam.SettingValue;
                                                    appExam.CurrentResult = appExam.ActualValue >= squence.SettingVlaue ? appExam.SettingValue : appExam.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    appExam.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {
                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.FinalValue;
                                                        }
                                                        
                                                    }
                                                    else
                                                    {
                                                        appExam.CurrentResult = (appExam.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? squence.SettingVlaue
                                                            : appExam.ActualValue;
                                                    }

                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:
                                                   
                                                    appExam.ActualValue = relatedex;
                                                    appExam.CurrentResult = relatedex>appExam.SettingValue?squence.SettingVlaue:appExam.ActualValue;
                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:
                                                    
                                                    appExam.ActualValue = nonRelatedex;
                                                    appExam.CurrentResult = nonRelatedex>appExam.SettingValue?appExam.SettingValue:appExam.ActualValue;
                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            appExam.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            appExam.CurrentResult = appExam.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:
                                                    
                                                    appExam.ActualValue = relatedex;
                                                    appExam.ActualValue =relevantex> appExam.SettingValue?squence.SettingVlaue:appExam.ActualValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    appExam.CurrentResult = appExam.SettingValue;
                                                    appExam.ActualValue = appExam.SettingValue;
                                                    break;
                                            }
                                            db.Entry(appExam).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            var aer = new InternalApplicantExamResult();

                                            aer.InternalApplicantScreeningId = checkApp.InternalApplicantScreeningId;
                                            aer.ExamType = squence.ExamType;
                                            aer.SettingValue = squence.SettingVlaue;
                                            switch (squence.ExamType)
                                            {
                                                case ExamType.THEORY_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.PRACTICAL_EXAM:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.INTERVIEW:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = 0;
                                                    break;
                                                case ExamType.GENDER:
                                                    var person = db.Person_Employee.Find(app.EmployeeId);
                                                    if (person != null && person.Gender == Gender.FEMALE)
                                                    {
                                                        aer.CurrentResult = aer.SettingValue;
                                                    }
                                                    else
                                                    {
                                                        aer.CurrentResult = 0;
                                                    }
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                                case ExamType.DISABILITY:
                                                    aer.ActualValue = aer.SettingValue;
                                                    aer.CurrentResult = aer.SettingValue;
                                                    break;

                                                case ExamType.PERFORMANCE:
                                                    var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
                                                    var EmpPerformance = db.EmployeePerformanceSummary
                                                        .Where(x => x.EmployeeId == app.EmployeeId)
                                                        .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                                        .FirstOrDefault();
                                                    aer.ActualValue = EmpPerformance != null ? EmpPerformance.Result : 0;
                                                    aer.CurrentResult = (aer.ActualValue * aer.SettingValue) / 100;
                                                    break;

                                                case ExamType.DISCIPLINE:
                                                    var measure = db.EmpDisciplineMeasure
                                                        .Where(x => db.DisciplinaryCases
                                                            .Where(dc => dc.EmployeeId == app.EmployeeId && dc.IsActive)
                                                            .Select(dc => dc.DisciplinaryCaseId)
                                                            .Contains(x.DisciplinaryCaseId))
                                                        .FirstOrDefault();
                                                    aer.ActualValue = measure?.DisciplineMea?.Rank ?? aer.SettingValue;
                                                    aer.CurrentResult = aer.ActualValue >= squence.SettingVlaue ? aer.SettingValue : aer.ActualValue;
                                                    break;

                                                case ExamType.TOTAL_EXPERIENCE:
                                                    aer.ActualValue = totalYear;
                                                    if (screeningSetting.IsBasedOnMaxExperience
                                                        && screeningSetting.MiniExperiencePercentage > 0)
                                                    {
                                                        var employeeData = employeeExperiences.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.FinalValue;
                                                        }
                                                        
                                                    }
                                                    else
                                                    {
                                                        aer.CurrentResult = (aer.ActualValue >= vacancy.TotalYearOfExperience)
                                                            ? squence.SettingVlaue
                                                            : aer.ActualValue;
                                                    }

                                                    break;

                                                case ExamType.RELATED_EXPERIENCE:
                                                    aer.ActualValue = relatedex;
                                                    aer.CurrentResult = relatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;


                                                    break;

                                                case ExamType.NON_RELATED_EXPERIENCE:
                                                    aer.ActualValue =nonRelatedex;
                                                    aer.CurrentResult = nonRelatedex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;

                                                    break;

                                                case ExamType.EDUCATION_LEVEL:
                                                    aer.ActualValue = aer.SettingValue;
                                                    if (screeningSetting.ApplyEducationRanking)
                                                    {
                                                        var employeeData = EmployeeEducationValues.FirstOrDefault(e => e.EmployeeId == app.EmployeeId);

                                                        // Check if employee data was found
                                                        if (employeeData.EmployeeId != 0)
                                                        {
                                                            aer.CurrentResult = employeeData.education;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (db.Education.Any(x => x.EmployeeId == app.EmployeeId && levelIds.Contains(x.EduLevel)))
                                                        {
                                                            aer.CurrentResult = aer.SettingValue;
                                                        }
                                                    }
                                                    break;

                                                case ExamType.RELEVANT_EXPERIENCE:

                                                    aer.ActualValue = relevantex;
                                                    aer.CurrentResult = relevantex > aer.SettingValue ? aer.SettingValue : aer.ActualValue;
                                                    break;

                                                case ExamType.OTHERS:
                                                    aer.CurrentResult = aer.SettingValue;
                                                    aer.ActualValue = aer.SettingValue;
                                                    break;
                                            }

                                            db.InternalApplicantExamResults.Add(aer);
                                        
                                        }

                                      

                                    }
                                    db.SaveChanges();
                                    transaction.Commit();

                                }
                            }
                            result = new
                            {
                                status = "OK",
                                message = "Successfull Generated"
                            };

                        }
                    }
                }
                else
                {
                    result = new
                    {
                        status = "Not-Ok",
                        message = "No applicant pass the screening"
                    };

                    return Json(result);
                }

            }
            else
            {
                result = new
                {
                    status = "Not-Okey",
                    message = "No screening setting defined for this vacancy"
                };
            }

            return Json(result);
        }
        private decimal GetPerformanceScore(int empPerformanceId)
        {
            decimal totalScore = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalScore = totalScore + scores.Sum(x => x.Score);

                }
            }

            return totalScore;
        }
        private decimal GetPerformanceWeight(int empPerformanceId)
        {
            decimal totalweight = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalweight += scores.Sum(x => x.Weight);
                }
            }

            return totalweight;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
