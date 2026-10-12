using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ExternalApplicantDetailObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExternalApplicantDetail> GetExternalApplicantDetail(string jobTitle, string vacancyCod, string vacancyId, DateTime from, DateTime to)
        {
            List<ExternalApplicantDetail> internalVacancies = new List<ExternalApplicantDetail>();
            var lookups = db.Lookups;
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            var educations = db.Education.ToList();
            var otherexpriances = db.ApplicantExperience.ToList();
            var educationsList = db.ApplicantEducation.ToList();
            var trainingList = db.ApplicantTraining.ToList();

            var rankList = db.Ranks;
            int TotalYear = 0;
            int TotalMonth = 0;
            int TotalDays = 0;

            if (vacancyCod != null && jobTitle != null)
            {
                var recruits = db.RecruitmentPlans.Where(x => x.LookupId.ToString() == jobTitle).ToList();
                foreach (var r in recruits)
                {
                    var vacan = new RecruitmentVacancy();
                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == r.RecruitmentPlanId);
                    if (vacancy != null)
                    {
                        //vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancy.VacancyId);
                        vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId.ToString() == vacancyId);
                    }

                    if (vacan != null)
                    {
                        var vacancydetail = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacan.VacancyId);
                        if (vacancydetail != null)
                        {
                            var applicant = db.ApplicantRegistration.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                            if (applicant.Any())
                            {
                                EmployeeEducation edu = new EmployeeEducation();
                                ExternalApplicantExpriance otherexplist = new ExternalApplicantExpriance();
                                ExternalApplicantEducation educationList = new ExternalApplicantEducation();
                                CurrentEmployeeExpriance currentEmployeeExpriance = new CurrentEmployeeExpriance();
                                EmpApplicantTraining empApplicantTraining = new EmpApplicantTraining();
                                foreach (var a in applicant)
                                {
                                    ExternalApplicantDetail internalVacancy = new ExternalApplicantDetail();
                                    TotalYear = 0;
                                    TotalMonth = 0;
                                    TotalDays = 0;
                                    internalVacancy.JobTitle = r.JobtitleName;

                                    internalVacancy.Salary = r.PlannedSalary;
                                    var firstOrDefault = rankList.Where(x => x.RankId == r.RankId).FirstOrDefault();
                                    if (firstOrDefault != null)
                                        internalVacancy.Rank = firstOrDefault.RankName;
                                    internalVacancy.Quntity = r.NoOfVacantPost;
                                    // internalVacancy.RecruitmentType = r.RecruitmentType;
                                    if (r.RecruitmentType == VacancyType.PERMANENT)
                                    {
                                        internalVacancy.RecruitmentType = VacancyType.PERMANENT.ToString().ToLower();
                                    }
                                    else
                                    {
                                        internalVacancy.RecruitmentType = VacancyType.CONTRACT.ToString().ToLower();
                                    }
                                    internalVacancy.No = internalVacancies.Count + 1;

                                    // internalVacancy.ApplicantName = a.ApplicantName;



                                    internalVacancy.OtherExpriance = new List<ExternalApplicantExpriance>();
                                    internalVacancy.Education = new List<ExternalApplicantEducation>();

                                    var empOtherEx = otherexpriances.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                    var empEduc = educationsList.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                    foreach (var e in empOtherEx)
                                    {
                                        otherexplist = new ExternalApplicantExpriance();
                                        //otherexplist.From = e.From.ToShortDateString();
                                        //otherexplist.To = e.To.ToShortDateString();
                                        otherexplist.CompanyName = e.OrganizationName;
                                        //var orDefault = lookups.Where(x => x.LookupId == e.EducField).FirstOrDefault();
                                        //if (orDefault != null)
                                        //    otherexplist.EduField = orDefault.Description;

                                        //var @default = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault();
                                        //if (@default != null)
                                        //    otherexplist.EduLevel = @default.Description;
                                        //otherexplist.JobTitleName = e.Postion;                                    

                                        //var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                                        //var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                                        //var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                        //totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                        //var years = totalmonths / 12;
                                        //var months = totalmonths % 12;
                                        //var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                        //int year = years;
                                        //int month = months;
                                        //int days = dayss;
                                        //otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                                        //TotalYear += year;
                                        //TotalMonth += month;
                                        //TotalDays += days;

                                        //string val = XAPI.Common.GetConcatenatedYearMonthDay(e.From, e.To);
                                        //otherexplist.DYear = Convert.ToInt32(val.Split('-')[0]);
                                        //otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);
                                        //otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);

                                        internalVacancy.OtherExpriance.Add(otherexplist);
                                    }

                                    int TotalOtherYear = internalVacancy.OtherExpriance.Sum(x => x.DYear);

                                    int TotalOtherMonth = internalVacancy.OtherExpriance.Sum(x => x.DMonth);
                                    internalVacancy.OtherMonth = TotalOtherMonth;
                                    internalVacancy.OtherYear = TotalOtherYear;
                                    if (TotalOtherMonth >= 12)
                                    {
                                        internalVacancy.OtherMonth = internalVacancy.OtherMonth % 12;
                                        internalVacancy.OtherYear += TotalOtherMonth / 12;
                                    }

                                    if (TotalYear < 0)
                                    {
                                        TotalYear = 0;
                                    }
                                    if (TotalMonth < 0)
                                    {
                                        TotalMonth = 0;
                                    }
                                    if (TotalDays < 0)
                                    {
                                        TotalDays = 0;
                                    }
                                    TotalYear = internalVacancy.DYear + internalVacancy.OtherYear;
                                    TotalMonth = internalVacancy.DMonth + internalVacancy.OtherMonth;
                                    if (TotalMonth >= 12)
                                    {
                                        TotalYear += TotalMonth / 12;
                                        TotalMonth = TotalMonth % 12;

                                    }
                                    internalVacancy.TotalYear = TotalYear.ToString();
                                    internalVacancy.TotalMonth = TotalMonth.ToString();
                                    internalVacancy.TotalExpriance = XAPI.Common.GetYearAndMonth(TotalYear, TotalMonth, TotalDays);
                                    foreach (var e in empEduc)
                                    {
                                        educationList = new ExternalApplicantEducation();
                                        //   educationList.From = e.From.ToShortDateString();
                                        //educationList.To = e.To.ToShortDateString();
                                        //educationList.EduInst = e.EducationInstitution;
                                        //var orDefault = lookups.Where(x => x.LookupId == e.EducField).FirstOrDefault();
                                        //if (orDefault != null)
                                        //    educationList.EduField = orDefault.Description;

                                        //var @default = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault();
                                        //if (@default != null)
                                        //    educationList.EduLevel = @default.Description;

                                        internalVacancy.Education.Add(educationList);
                                    }
                                    internalVacancy.Trainings = new List<EmpApplicantTraining>();
                                    var emptrainings = trainingList.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                    foreach (var t in emptrainings)
                                    {


                                        empApplicantTraining = new EmpApplicantTraining();
                                        empApplicantTraining.CourseName = t.TrainingName;

                                        internalVacancy.Trainings.Add(empApplicantTraining);
                                    }

                                    if (r.RecruitmentType == VacancyType.PERMANENT)
                                    {
                                        internalVacancy.RecruitmentType = "Permanent";
                                    }
                                    else
                                    {
                                        internalVacancy.RecruitmentType = "Contract";
                                    }

                                    internalVacancies.Add(internalVacancy);
                                }
                            }

                        }
                    }

                }
            }
            else if (vacancyCod != null && vacancyId != null && jobTitle != null)
            {
                var vaca = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId.ToString() == vacancyId);
                var recruits = db.RecruitmentPlans.FirstOrDefault(x => x.LookupId.ToString() == jobTitle && x.RecruitmentPlanId == vaca.RecruitmentPlanId);

                var vacan = new RecruitmentVacancy();
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == recruits.RecruitmentPlanId);
                if (vacancy != null)
                {
                    // vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancy.VacancyId);
                    vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId.ToString() == vacancyId);
                }

                if (vacan != null)
                {
                    var vacancydetail = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacan.VacancyId);
                    if (vacancydetail != null)
                    {
                        var applicant = db.ApplicantRegistration.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                        if (applicant.Any())
                        {
                            EmployeeEducation edu = new EmployeeEducation();
                            ExternalApplicantExpriance otherexplist = new ExternalApplicantExpriance();
                            ExternalApplicantEducation educationList = new ExternalApplicantEducation();
                            CurrentEmployeeExpriance currentEmployeeExpriance = new CurrentEmployeeExpriance();
                            EmpApplicantTraining empApplicantTraining = new EmpApplicantTraining();
                            foreach (var a in applicant)
                            {
                                ExternalApplicantDetail internalVacancy = new ExternalApplicantDetail();
                                TotalYear = 0;
                                TotalMonth = 0;
                                TotalDays = 0;
                                internalVacancy.JobTitle = recruits.JobtitleName;

                                internalVacancy.Salary = recruits.PlannedSalary;
                                var firstOrDefault = rankList.Where(x => x.RankId == recruits.RankId).FirstOrDefault();
                                if (firstOrDefault != null)
                                    internalVacancy.Rank = firstOrDefault.RankName;
                                internalVacancy.Quntity = recruits.NoOfVacantPost;
                                // internalVacancy.RecruitmentType = recruits.RecruitmentType;
                                if (recruits.RecruitmentType == VacancyType.PERMANENT)
                                {
                                    internalVacancy.RecruitmentType = "Permanent";
                                }
                                else
                                {
                                    internalVacancy.RecruitmentType = "Contract";
                                }
                                internalVacancy.No = internalVacancies.Count + 1;

                                //internalVacancy.ApplicantName = a.ApplicantName;

                                internalVacancy.OtherExpriance = new List<ExternalApplicantExpriance>();
                                internalVacancy.Education = new List<ExternalApplicantEducation>();

                                var empOtherEx = otherexpriances.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                var empEduc = educationsList.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                foreach (var e in empOtherEx)
                                {
                                    //otherexplist = new ExternalApplicantExpriance();
                                    //otherexplist.From = e.From.ToShortDateString();
                                    //otherexplist.To = e.To.ToShortDateString();
                                    //otherexplist.CompanyName = e.OrganizationName;
                                    //var orDefault = lookups.Where(x => x.LookupId == e.EducField).FirstOrDefault();
                                    //if (orDefault != null)
                                    //    otherexplist.EduField = orDefault.Description;

                                    //var @default = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault();
                                    //if (@default != null)
                                    //    otherexplist.EduLevel = @default.Description;
                                    //    otherexplist.JobTitleName = e.Postion;
                                    //var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                                    //var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                                    //var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    //totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    //var years = totalmonths / 12;
                                    //var months = totalmonths % 12;
                                    //var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    //int year = years;
                                    //int month = months;
                                    //int days = dayss;
                                    //otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                                    //TotalYear += year;
                                    //    TotalMonth += month;
                                    //    TotalDays += days;
                                    //string val = XAPI.Common.GetConcatenatedYearMonthDay(e.From, e.To);
                                    //otherexplist.DYear = Convert.ToInt32(val.Split('-')[0]);
                                    //otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);
                                    //otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);

                                    internalVacancy.OtherExpriance.Add(otherexplist);
                                }

                                int TotalOtherYear = internalVacancy.OtherExpriance.Sum(x => x.DYear);

                                int TotalOtherMonth = internalVacancy.OtherExpriance.Sum(x => x.DMonth);
                                internalVacancy.OtherMonth = TotalOtherMonth;
                                internalVacancy.OtherYear = TotalOtherYear;
                                if (TotalOtherMonth >= 12)
                                {
                                    internalVacancy.OtherMonth = internalVacancy.OtherMonth % 12;
                                    internalVacancy.OtherYear += TotalOtherMonth / 12;
                                }

                                if (TotalYear < 0)
                                {
                                    TotalYear = 0;
                                }
                                if (TotalMonth < 0)
                                {
                                    TotalMonth = 0;
                                }
                                if (TotalDays < 0)
                                {
                                    TotalDays = 0;
                                }
                                TotalYear = internalVacancy.DYear + internalVacancy.OtherYear;
                                TotalMonth = internalVacancy.DMonth + internalVacancy.OtherMonth;
                                if (TotalMonth >= 12)
                                {
                                    TotalYear += TotalMonth / 12;
                                    TotalMonth = TotalMonth % 12;

                                }
                                internalVacancy.TotalYear = TotalYear.ToString();
                                internalVacancy.TotalMonth = TotalMonth.ToString();
                                internalVacancy.TotalExpriance = XAPI.Common.GetYearAndMonth(TotalYear, TotalMonth, TotalDays);
                                foreach (var e in empEduc)
                                {
                                    //educationList = new ExternalApplicantEducation();
                                    ////   educationList.From = e.From.ToShortDateString();
                                    //educationList.To = e.To.ToShortDateString();
                                    //educationList.EduInst = e.EducationInstitution;
                                    //var orDefault = lookups.Where(x => x.LookupId == e.EducField).FirstOrDefault();
                                    //if (orDefault != null)
                                    //    educationList.EduField = orDefault.Description;

                                    //var @default = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault();
                                    //if (@default != null)
                                    //    educationList.EduLevel = @default.Description;

                                    //internalVacancy.Education.Add(educationList);
                                }
                                internalVacancy.Trainings = new List<EmpApplicantTraining>();
                                var emptrainings = trainingList.Where(x => x.ApplicantRegistrationId == a.ApplicantRegistrationId).ToList();
                                foreach (var t in emptrainings)
                                {


                                    empApplicantTraining = new EmpApplicantTraining();
                                    empApplicantTraining.CourseName = t.TrainingName;

                                    internalVacancy.Trainings.Add(empApplicantTraining);
                                }

                                //  internalVacancy.RecruitmentType = recruits.RecruitmentType;
                                if (recruits.RecruitmentType == VacancyType.PERMANENT)
                                {
                                    internalVacancy.RecruitmentType = "Permanent";
                                }
                                else
                                {
                                    internalVacancy.RecruitmentType = "Contract";
                                }

                                internalVacancies.Add(internalVacancy);
                            }
                        }

                    }
                }
            }

            return internalVacancies.OrderBy(x => x.No).ToList(); ; //internalVacancies.OrderBy(x => x.No).ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }


}

