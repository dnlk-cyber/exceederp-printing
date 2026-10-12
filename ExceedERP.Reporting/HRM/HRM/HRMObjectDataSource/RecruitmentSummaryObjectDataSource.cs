using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class RecruitmentSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RecruitmentSummary> GetRecruitmentSummary(string vacancyId, string type)
        {
            List<InternalApplicantDetail> internalVacancies = new List<InternalApplicantDetail>();
            List<RecruitmentSummary> summaries = new List<RecruitmentSummary>();

            if (!string.IsNullOrEmpty(vacancyId))
            {
                int.TryParse(vacancyId, out int vacId);
                var applicantScreening = db.ApplicantScreenResults.FirstOrDefault(x => x.VacancyId == vacId);

                if (applicantScreening != null)
                {
                    var screenedApplicants = db.ApplicantScreenings.Where(x => x.ApplicantScreenResultId == applicantScreening.ApplicantScreenResultId).ToList();

                    foreach (var screen in screenedApplicants)
                    {
                        RecruitmentSummary recruitment = new RecruitmentSummary();

                        var applicant = db.ApplicantDetails.FirstOrDefault(x => x.ApplicantDetailId == screen.ApplicantDetailId);

                        if (applicant != null)
                        {
                            recruitment.Name = applicant.ApplicantName;
                            recruitment.Age = applicant.Age;
                            recruitment.Sex = GetGender(applicant.Sex);
                            recruitment.Training = applicant.Training;
                            recruitment.Discipline = "No";

                            var appEdus = db.ApplicantEducation.Where(x => x.ApplicantDetailId == applicant.ApplicantDetailId).ToList();

                            string comma = "";
                            foreach (var edu in appEdus)
                            {
                                var eduLevelLook = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EducationLevelSettingId);
                                if (eduLevelLook != null)
                                {
                                    recruitment.EduLevel = recruitment.EduLevel + comma + eduLevelLook.Description;
                                }
                                var eduFieldLook = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EducationFieldSettingId);
                                if (eduFieldLook != null)
                                {
                                    recruitment.EduField = recruitment.EduField + comma + eduFieldLook.Description;
                                }

                                comma = ", ";
                            }

                            var appExps = db.ApplicantExperience.Where(x => x.ApplicantDetailId == applicant.ApplicantDetailId).ToList();

                            foreach (var ex in appExps)
                            {
                                var look = db.Lookups.FirstOrDefault(x => x.LookupId == ex.JobTitleId);
                                if (look != null)
                                {
                                    recruitment.RelevantExperience = recruitment.RelevantExperience + look.Description + ", ";
                                }
                            }

                            var applicantExams = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == screen.ApplicantScreeningId).ToList();
                            if (!string.IsNullOrEmpty(type))
                            {
                                applicantExams = applicantExams.Where(x => x.ExamType.ToString() == type).ToList();
                            }
                            decimal totalSetting = 0;
                            foreach (var appExam in applicantExams)
                            {
                                switch (appExam.ExamType)
                                {
                                    case ExamType.THEORY_EXAM:
                                        recruitment.TheoryExam = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                    case ExamType.PRACTICAL_EXAM:
                                        recruitment.PracticalExam = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                    case ExamType.INTERVIEW:
                                        recruitment.Interview = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                    default:
                                        recruitment.OtherResult += appExam.CurrentResult;
                                        recruitment.Remark += $"ExamType: {appExam.ExamType.ToString()} : {appExam.CurrentResult} out of {appExam.SettingValue}";
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;


                                }
                            }

                            recruitment.Total = recruitment.TheoryExam + recruitment.PracticalExam + recruitment.Interview + recruitment.OtherResult;

                            var passMark = db.RecruitmentApplicantPassSettings.FirstOrDefault(x => x.VacancyId == vacId);

                            if (passMark != null)
                            {
                                var pass = (passMark.PassScore * Convert.ToDouble(totalSetting)) / passMark.TotalValue;
                                if (recruitment.Total >= Convert.ToDecimal(pass))
                                {
                                   // recruitment.RecruitmentResult = "አልፈዋል";
                                    recruitment.RecruitmentResult = "Passed";
                                }
                                else
                                {
                                   // recruitment.RecruitmentResult = "አላለፉም";
                                    recruitment.RecruitmentResult = "Fail";
                                }

                                summaries.Add(recruitment);
                            }
                        }

                    }

                }
                else
                {
                    var applicantScreeningInternal = db.InternalApplicantScreeningResults.FirstOrDefault(x => x.VacancyId == vacId);

                    if (applicantScreeningInternal != null)
                    {
                        var screenedApplicantsInternal = db.InternalApplicantScreenings.Where(x => x.InternalApplicantScreeningResultId == applicantScreeningInternal.InternalApplicantScreeningResultId).ToList();

                        foreach (var screen in screenedApplicantsInternal)
                        {
                            RecruitmentSummary recruitment = new RecruitmentSummary();

                            //var applicant = db.InternalApplicants.FirstOrDefault(x => x.EmployeeId == screen.EmployeeId);

                            //if (applicant != null)
                            //{
                            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == screen.EmployeeId);

                            if (person != null)
                            {
                                recruitment.Age = DateTime.Today.Year - person.DateOfBirth.Year;
                                recruitment.Sex = GetGender(person.Gender);
                                recruitment.Name = person.FullNameAmharic;
                            }

                           // recruitment.Training = screen.Training;
                            recruitment.Discipline = "No";

                            var appEdus = db.Education.Where(x => x.EmployeeId == screen.EmployeeId).ToList();

                            string comma = "";
                            foreach (var edu in appEdus)
                            {
                                var eduLevelLook = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduLevel);
                                if (eduLevelLook != null)
                                {
                                    recruitment.EduLevel = recruitment.EduLevel + comma + eduLevelLook.Description;
                                }
                                var eduFieldLook = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduField);
                                if (eduFieldLook != null)
                                {
                                    recruitment.EduField = recruitment.EduField + comma + eduFieldLook.Description;
                                }

                                comma = ", ";
                            }

                            var appExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();

                            foreach (var ex in appExps)
                            {
                                //var look = db.Lookups.FirstOrDefault(x => x.LookupId == ex.JobTitleId);
                                //if (look != null)
                                //{
                                    recruitment.RelevantExperience = recruitment.RelevantExperience + ex.Responsibility + ", ";
                                //}
                            }

                            var applicantExams = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == screen.InternalApplicantScreeningId).ToList();
                            decimal totalSetting = 0;
                            foreach (var appExam in applicantExams)
                            {
                                switch (appExam.ExamType)
                                {
                                    case ExamType.THEORY_EXAM:
                                        recruitment.TheoryExam = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                    case ExamType.PRACTICAL_EXAM:
                                        recruitment.PracticalExam = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                    case ExamType.INTERVIEW:
                                        recruitment.Interview = appExam.CurrentResult;
                                        totalSetting = totalSetting + appExam.SettingValue;
                                        break;
                                }
                            }

                            recruitment.Total = recruitment.TheoryExam + recruitment.PracticalExam + recruitment.Interview;

                            var passMark = db.RecruitmentApplicantPassSettings.FirstOrDefault(x => x.VacancyId == vacId);

                            if (passMark != null)
                            {
                                var pass = (passMark.PassScore * Convert.ToDouble(totalSetting)) / passMark.TotalValue;
                                if (recruitment.Total >= Convert.ToDecimal(pass))
                                {
                                    recruitment.RecruitmentResult = "Passed";
                                }
                                else
                                {
                                    recruitment.RecruitmentResult = "Failed";
                                }

                                summaries.Add(recruitment);
                            }
                            // }

                        }

                    }
                }

            }


            summaries = summaries.OrderByDescending(x => x.Total).ToList();

            return summaries;
        }

        private string GetGender(Gender gender)
        {
            if (gender == Gender.MALE)
            {
                return "Male";
            }
            else if (gender == Gender.FEMALE)
            {
                return "Female";
            }

            return null;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancyCodForRecruitmentSummary()
        {
            List<int> ids = new List<int>();
            var vacancyIds = db.ApplicantScreenResults.Select(x => x.VacancyId).Distinct();
            ids.AddRange(vacancyIds);
            var intVacancyIds = db.InternalApplicantScreeningResults.Select(x => x.VacancyId).Distinct();
            ids.AddRange(intVacancyIds);
            return db.VaccancyPlans.Where(x=> ids.Contains(x.VacancyId)).ToList();


           
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }


}

