using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class RecruitmentSummariesObjectDataSource : List<RecruitmentSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RecruitmentSummary> GetRecruitmentSummary(string jobTitle, string vacancyCod, string period, DateTime from, DateTime to, string branchId, string orgStructureId)
        {
            List<InternalApplicantDetail> internalVacancies = new List<InternalApplicantDetail>();
            List<RecruitmentSummary> summaries = new List<RecruitmentSummary>();
            RecruitmentSummary oneapplicant = new RecruitmentSummary();
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            decimal passMark = 0;
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            var GLPeriodId = db.GLPeriods.Where(x => x.GLPeriodId.ToString() == period).FirstOrDefault();


            var configration = db.Configurations.Where(x => x.Attribute == "Promotion" && x.Category == "Promotion" && x.Reference == "PASS_MARK").FirstOrDefault();
            if (configration != null)
            {
                passMark = Convert.ToDecimal(configration.CurrentValue);
            }
            //var recruitmentVacancy = db.RecruitmentVacancy.Where(x=>x.MediaVacancyCode==vacancyCod).ToList();
            //if(recruitmentVacancy.Any())
            //{
            //    foreach(var recruitment in recruitmentVacancy)
            //    {
            //        var vacancy = db.VaccancyPlans.FirstOrDefault(x=>x.VacancyId==recruitment.VacancyId);
            //        if(vacancy!=null)
            //        {
            //            var recruitmentPlan = db.RecruitmentPlans.Find(vacancy.RecruitmentPlanId);
            //            if(recruitmentPlan!=null)
            //            {
            //                var lookups = db.Lookups.FirstOrDefault(x=>x.LookupId==recruitmentPlan.LookupId);
            //                if(lookups!=null && lookups.LookupId.ToString()==jobTitle)
            //                {

            //                }
            //            }
            //        }
            //    }
            //}

            var recruits = db.RecruitmentPlans.Where(x => x.LookupId.ToString() == jobTitle).ToList();//&& x.WorkUnitId.ToString() == orgStructureId
            decimal total = 0;
            foreach (var r in recruits)
            {
                var vacan = new RecruitmentVacancy();

                //var vacancies = db.VaccancyPlans.Where(x => x.RecruitmentPlanId == r.RecruitmentPlanId).FirstOrDefault();
                //if (vacancies != null)
                //{
                //    DateTime dateOneC = new DateTime(vacancies.From.Year, vacancies.From.Month, vacancies.From.Day);
                //    DateTime dateTowC = new DateTime(vacancies.To.Year, vacancies.To.Month, vacancies.To.Day);
                //    if (dateOneC >= dateOne && dateTowC <= dateTow)
                //    {
                //        vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancies.VacancyId);
                //        break;
                //    }
                //}

                var vacancyPlan = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == r.RecruitmentPlanId);
                if (vacancyPlan != null)
                {
                    vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancyPlan.VacancyId);
                }

                var vacancydetail = db.VaccancyPlans.Where(x => x.VacancyId == vacan.VacancyId).FirstOrDefault();
                if (vacancydetail != null)
                {
                    var applicants = db.ApplicantionFormes.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                    if (applicants.Any())
                    {
                        foreach (var a in applicants)
                        {
                            db = new ExceedDbContext();
                            oneapplicant = new RecruitmentSummary();
                            total = 0;
                            var personem =
                                db.Person_Employee.Where(x => x.EmployeeId == a.EmployeeId && x.IsActive).FirstOrDefault();
                            oneapplicant.Name = personem.FirstName + " " + personem.MiddleName + " " +
                                                personem.LastName;
                            oneapplicant.Age = DateTime.Now.Year - personem.DateOfBirth.Year;
                            oneapplicant.Sex = personem.Gender.ToString();
                            var internalApplcant =
                                db.ApplicantScreeningResult.Where(
                                    x =>
                                        x.ApplicantRegistrationId == a.ApplicantionFormId).ToList();
                            if (internalApplcant.Any())
                            {
                                foreach (var i in internalApplcant)
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
                            var internalApplcantexam =
                                db.ApplicantExamResult.Where(
                                    x =>
                                        x.ApplicantRegistrationId == a.ApplicantionFormId
                                ).ToList();
                            if (internalApplcantexam.Any())
                            {
                                foreach (var i in internalApplcantexam)
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
                            if (vacancydetail.RecruSource == RecruitmentSource.INTERNAL)
                            {
                                if (GLPeriodId != null)
                                {
                                    var performance =
                                        db.EmployeeMeasurementDefinition.Where(
                                            x =>
                                                x.GLPeriodId == GLPeriodId.GLPeriodId &&
                                                x.EmployeeId == personem.EmployeeId).ToList();
                                    if (performance.Any())
                                    {
                                        oneapplicant.Performance = performance.Sum(x => x.Result);
                                    }
                                }
                            }
                            total = oneapplicant.Interview + oneapplicant.PracticalExam + oneapplicant.TheoryExam +
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
                            summaries.Add(oneapplicant);
                        }
                    }
                    var extApplican = db.ApplicantRegistration.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                    if (extApplican.Any())
                    {
                        foreach (var a in extApplican)
                        {
                            db = new ExceedDbContext();
                            oneapplicant = new RecruitmentSummary();
                            total = 0;

                            oneapplicant.Name = a.ApplicantName;
                            oneapplicant.Age = a.Age;
                            oneapplicant.Sex = a.Sex;
                            var internalApplcant =
                                db.ApplicantScreeningResult.Where(
                                    x =>
                                        x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                            if (internalApplcant.Any())
                            {
                                foreach (var i in internalApplcant)
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
                            var internalApplcantexam =
                                db.ApplicantExamResult.Where(
                                    x =>
                                        x.ApplicantRegistrationId == a.ApplicantRegistrationId
                                ).ToList();
                            if (internalApplcantexam.Any())
                            {
                                foreach (var i in internalApplcantexam)
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
                            summaries.Add(oneapplicant);
                        }
                    }



                }

            }


            return summaries;
        }
    }
}

