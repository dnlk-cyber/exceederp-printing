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
    public class InternalApplicantDetailObjectDataSource : List<InternalApplicantDetail>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetInternalApplicantDetail(string jobTitle, string vacancyCod, string vacancyId, DateTime from, DateTime to)
        {
            List<InternalApplicantDetail> internalVacancies = new List<InternalApplicantDetail>();
            var lookups = db.Lookups;
            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            var educations = db.Education.ToList();
            var otherexpriances = db.Experiences.ToList();
            var currentex = db.Placements.ToList();
            var trainingList = db.EmpTrainingEducations.ToList();
            var rankList = db.Ranks;
            int TotalYear = 0;
            int TotalMonth = 0;
            int TotalDays = 0;

            var allPerfromace = db.EmployeeMeasurementDefinition.ToList();


            if (jobTitle != null && vacancyCod != null && vacancyId == null)
            {
                var recruits = db.RecruitmentPlans.Where(x => x.LookupId.ToString() == jobTitle).ToList();
                var internalVacancy = new InternalApplicantDetail();


                foreach (var r in recruits)
                {
                    var vacan = new RecruitmentVacancy();
                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == r.RecruitmentPlanId);
                    if (vacancy != null)
                    {
                        vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancy.VacancyId);
                    }
                    if (vacan != null)
                    {

                        var applicant = db.ApplicantionFormes.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                        if (applicant.Any())
                        {
                            EmployeeEducation edu = new EmployeeEducation();
                            OtherEmployeeExpriance otherexplist = new OtherEmployeeExpriance();
                            CurrentEmployeeExpriance currentEmployeeExpriance = new CurrentEmployeeExpriance();
                            EmpApplicantTraining empApplicantTraining = new EmpApplicantTraining();
                            foreach (var a in applicant)
                            {
                                internalVacancy = new InternalApplicantDetail();


                                #region Performance


                                var periods = from p in db.QuarterLengths
                                              join f in db.GLFiscalYears
                                              on p.GlFiscalYearId equals f.GlFiscalYearId
                                              join q in db.Quarters
                                              on p.QuarterLengthId equals q.QuarterLengthId
                                              //where f.Status == FiscalYearStatus.Open 
                                              select (new { q, f });



                                var listPeriod = periods.ToList();
                                var qaurters = listPeriod.Select(x => x.q).ToList();

                                qaurters = listPeriod.Where(f => f.f.Status == FiscalYearStatus.Open).Select(x => x.q).ToList();



                                if (qaurters.Any())
                                {
                                    decimal totalResult = 0;
                                    int div = 0;
                                    var performance = allPerfromace.Where(x => x.EmployeeId == a.EmployeeId)
                                        .ToList();
                                    foreach (var p in qaurters)
                                    {
                                        var quarterName = p.AmharicName;
                                        var oneQuarter = performance.Where(x => x.QuarterId == p.QuarterId).Sum(x => x.Result);
                                        totalResult = totalResult + oneQuarter;

                                    }

                                    internalVacancy.Perfromance = (Math.Round(totalResult / qaurters.Count(), 2)).ToString();
                                }

                                #endregion



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
                                    internalVacancy.RecruitmentType = "ቋሚ";
                                }
                                else
                                {
                                    internalVacancy.RecruitmentType = "ጊዜያዊ";
                                }
                                internalVacancy.No = internalVacancies.Count + 1;
                                var emp = employees.Where(x => x.EmployeeId == a.EmployeeId).FirstOrDefault();
                                internalVacancy.ApplicantName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                                internalVacancy.Educations = new List<EmployeeEducation>();

                                var empEdu = educations.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                                foreach (var e in empEdu)
                                {
                                    edu = new EmployeeEducation();
                                    if (e.From != null) edu.From = e.From.Value.ToShortDateString();
                                    edu.To = e.To.ToShortDateString();
                                    edu.EduField = lookups.Where(x => x.LookupId == e.EduField).FirstOrDefault().Description;
                                    edu.OrgName = e.OrganizationName;
                                    edu.EduLevel = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault().Description;
                                    internalVacancy.Educations.Add(edu);
                                }
                                #region Other
                                internalVacancy.OtherExpriance = new List<OtherEmployeeExpriance>();
                                var empOtherEx = otherexpriances.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                                foreach (var e in empOtherEx)
                                {
                                    otherexplist = new OtherEmployeeExpriance();
                                    otherexplist.From = e.From.ToShortDateString();
                                    otherexplist.To = e.To.ToShortDateString();
                                    otherexplist.CompanyName = e.Organozation;
                                    otherexplist.JobTitleName = e.Responsibility;
                                    var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                                    var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                                    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    int year = years;
                                    int month = months;
                                    int days = dayss;
                                    otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);

                                    string val = XAPI.Common.GetConcatenatedYearMonthDay(e.From, e.To);
                                    otherexplist.DYear = Convert.ToInt32(val.Split('-')[0]);
                                    otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);
                                    otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);

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
                                #endregion
                                #region Current
                                internalVacancy.Expriance = new List<CurrentEmployeeExpriance>();
                                var empCurrrEx = currentex.Where(x => x.EmployeeId == a.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                                for (int e = 0; e < empCurrrEx.Count(); e++)
                                {
                                    currentEmployeeExpriance = new CurrentEmployeeExpriance();


                                    int year = 0;
                                    int month;
                                    int days;
                                    if (e >= 1)
                                    {
                                        var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                        var toc = new DateTime(empCurrrEx[e - 1].AssignmentDate.Year, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Day);

                                        var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                        totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                        var years = totalmonths / 12;
                                        var months = totalmonths % 12;
                                        var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                        year = years;
                                        month = months;
                                        days = dayss;
                                        currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                                        currentEmployeeExpriance.To = empCurrrEx[e - 1].AssignmentDate.Date.ToShortDateString();
                                        currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                                        currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year).ToString();
                                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                                    }
                                    else
                                    {
                                        currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDate.Date.ToShortDateString();

                                        currentEmployeeExpriance.To = DateTime.Now.Date.ToShortDateString();

                                        var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                        var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

                                        var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                        totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                        var years = totalmonths / 12;
                                        var months = totalmonths % 12;
                                        var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                        year = years;
                                        month = months;
                                        days = dayss;
                                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                                        currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                                        currentEmployeeExpriance.ToString = " አሁን";
                                    }

                                    currentEmployeeExpriance.JobTitleName = empCurrrEx[e].JobTitleName;



                                    //currentEmployeeExpriance.Description = currentEmployeeExpriance.Year;
                                    internalVacancy.Expriance.Add(currentEmployeeExpriance);
                                }
                                #region History
                                var empHistory =
                                    db.PlacementHistories.Where(x => x.EmployeeId == a.EmployeeId)
                                        .OrderByDescending(x => x.PlacementHistoryId)
                                        .ToList();
                                for (int e = 0; e < empHistory.Count(); e++)
                                {
                                    currentEmployeeExpriance = new CurrentEmployeeExpriance();


                                    int year = 0;
                                    int month;
                                    int days;

                                    var fromc = new DateTime(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day);
                                    var toc = new DateTime(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day);

                                    var totalmonths = (toc.Year - from.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = from.Subtract(toc.AddMonths(totalmonths)).Days;

                                    year = years;
                                    month = months;
                                    days = dayss;
                                    currentEmployeeExpriance.From = empHistory[e].From.Date.ToShortDateString();
                                    currentEmployeeExpriance.To = empHistory[e].To.Date.ToShortDateString();
                                    currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day).ToString();
                                    currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day).ToString();
                                    currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empHistory[e].From, empHistory[e].To);
                                    string val = XAPI.Common.GetConcatenatedYearMonthDay(empHistory[e].From, empHistory[e].To);
                                    currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                    currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                                    currentEmployeeExpriance.JobTitleName = empHistory[e].JobTitle;
                                    internalVacancy.Expriance.Add(currentEmployeeExpriance);
                                }
                                #endregion
                                int TotalCurrentear = internalVacancy.Expriance.Sum(x => x.DYear);

                                int TotalCurrentMonth = internalVacancy.Expriance.Sum(x => x.DMonth);
                                internalVacancy.DMonth = TotalCurrentMonth;
                                internalVacancy.DYear = TotalCurrentear;
                                if (TotalCurrentMonth >= 12)
                                {
                                    internalVacancy.DMonth = internalVacancy.DMonth % 12;
                                    internalVacancy.DYear += TotalCurrentMonth / 12;
                                }
                                #endregion
                                internalVacancy.Trainings = new List<EmpApplicantTraining>();
                                var emptrainings = trainingList.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                                foreach (var t in emptrainings)
                                {


                                    empApplicantTraining = new EmpApplicantTraining();
                                    empApplicantTraining.CourseName = t.TrainingName;

                                    internalVacancy.Trainings.Add(empApplicantTraining);
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
                                //internalVacancy.RecruitmentType = r.RecruitmentType;
                                if (r.RecruitmentType == VacancyType.PERMANENT)
                                {
                                    internalVacancy.RecruitmentType = "ቋሚ";
                                }
                                else
                                {
                                    internalVacancy.RecruitmentType = "ጊዜያዊ";
                                }
                                internalVacancies.Add(internalVacancy);
                            }
                        }

                    }

                }
            }
            else if (jobTitle != null && vacancyCod != null && vacancyId != null)
            {
                var vaca = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId.ToString() == vacancyId);

                var recruits = db.RecruitmentPlans.FirstOrDefault(x => x.LookupId.ToString() == jobTitle && x.RecruitmentPlanId == vaca.RecruitmentPlanId);


                var vacan = new RecruitmentVacancy();
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == recruits.RecruitmentPlanId && x.VacancyId.ToString() == vacancyId);
                if (vacancy != null)
                {
                    vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancy.VacancyId);
                }
                if (vacan != null)
                {

                    var applicant = db.ApplicantionFormes.Where(x => x.VacancyId == vacan.VacancyId).ToList();
                    if (applicant.Any())
                    {
                        EmployeeEducation edu = new EmployeeEducation();
                        OtherEmployeeExpriance otherexplist = new OtherEmployeeExpriance();
                        CurrentEmployeeExpriance currentEmployeeExpriance = new CurrentEmployeeExpriance();
                        EmpApplicantTraining empApplicantTraining = new EmpApplicantTraining();
                        foreach (var a in applicant)
                        {
                            var internalVacancy = new InternalApplicantDetail();
                            TotalYear = 0;
                            TotalMonth = 0;
                            TotalDays = 0;


                            #region Performance


                            var periods = from p in db.QuarterLengths
                                          join f in db.GLFiscalYears
                                          on p.GlFiscalYearId equals f.GlFiscalYearId
                                          join q in db.Quarters
                                          on p.QuarterLengthId equals q.QuarterLengthId
                                          //where f.Status == FiscalYearStatus.Open 
                                          select (new { q, f });



                            var listPeriod = periods.ToList();
                            var qaurters = listPeriod.Select(x => x.q).ToList();

                            qaurters = listPeriod.Where(f => f.f.Status == FiscalYearStatus.Open).Select(x => x.q).ToList();



                            if (qaurters.Any())
                            {
                                decimal totalResult = 0;
                                int div = 0;
                                var performance = allPerfromace.Where(x => x.EmployeeId == a.EmployeeId)
                                    .ToList();
                                foreach (var p in qaurters)
                                {
                                    var quarterName = p.AmharicName;
                                    var oneQuarter = performance.Where(x => x.QuarterId == p.QuarterId).Sum(x => x.Result);
                                    totalResult = totalResult + oneQuarter;

                                }

                                internalVacancy.Perfromance = (Math.Round(totalResult / qaurters.Count(), 2)).ToString();
                            }

                            #endregion



                            internalVacancy.JobTitle = recruits.JobtitleName;

                            internalVacancy.Salary = recruits.PlannedSalary;
                            var firstOrDefault = rankList.Where(x => x.RankId == recruits.RankId).FirstOrDefault();
                            if (firstOrDefault != null)
                                internalVacancy.Rank = firstOrDefault.RankName;
                            internalVacancy.Quntity = recruits.NoOfVacantPost;
                            // internalVacancy.RecruitmentType = recruits.RecruitmentType;
                            if (recruits.RecruitmentType == VacancyType.PERMANENT)
                            {
                                internalVacancy.RecruitmentType = "ቋሚ";
                            }
                            else
                            {
                                internalVacancy.RecruitmentType = "ጊዜያዊ";
                            }
                            internalVacancy.No = internalVacancies.Count + 1;
                            var emp = employees.Where(x => x.EmployeeId == a.EmployeeId).FirstOrDefault();
                            internalVacancy.ApplicantName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            internalVacancy.Educations = new List<EmployeeEducation>();

                            var empEdu = educations.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                            foreach (var e in empEdu)
                            {
                                edu = new EmployeeEducation();
                                if (e.From != null) edu.From = e.From.Value.ToShortDateString();
                                edu.To = e.To.ToShortDateString();
                                edu.EduField = lookups.Where(x => x.LookupId == e.EduField).FirstOrDefault().Description;
                                edu.OrgName = e.OrganizationName;
                                edu.EduLevel = lookups.Where(x => x.LookupId == e.EduLevel).FirstOrDefault().Description;
                                internalVacancy.Educations.Add(edu);
                            }
                            #region Other
                            internalVacancy.OtherExpriance = new List<OtherEmployeeExpriance>();
                            var empOtherEx = otherexpriances.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                            foreach (var e in empOtherEx)
                            {
                                otherexplist = new OtherEmployeeExpriance();
                                otherexplist.From = e.From.ToShortDateString();
                                otherexplist.To = e.To.ToShortDateString();
                                otherexplist.CompanyName = e.Organozation;
                                otherexplist.JobTitleName = e.Responsibility;
                                var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                                var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                                var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                var years = totalmonths / 12;
                                var months = totalmonths % 12;
                                var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                int year = years;
                                int month = months;
                                int days = dayss;
                                otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);

                                string val = XAPI.Common.GetConcatenatedYearMonthDay(e.From, e.To);
                                otherexplist.DYear = Convert.ToInt32(val.Split('-')[0]);
                                otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);
                                otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);

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
                            #endregion
                            #region Current
                            internalVacancy.Expriance = new List<CurrentEmployeeExpriance>();
                            var empCurrrEx = currentex.Where(x => x.EmployeeId == a.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                            for (int e = 0; e < empCurrrEx.Count(); e++)
                            {
                                currentEmployeeExpriance = new CurrentEmployeeExpriance();


                                int year = 0;
                                int month;
                                int days;
                                if (e >= 1)
                                {
                                    var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                    var toc = new DateTime(empCurrrEx[e - 1].AssignmentDate.Year, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Day);

                                    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    year = years;
                                    month = months;
                                    days = dayss;
                                    currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                                    currentEmployeeExpriance.To = empCurrrEx[e - 1].AssignmentDate.Date.ToShortDateString();
                                    currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                                    currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year).ToString();
                                    currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                                    string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                                    currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                    currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                                }
                                else
                                {
                                    currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDate.Date.ToShortDateString();

                                    currentEmployeeExpriance.To = DateTime.Now.Date.ToShortDateString();

                                    var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                    var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

                                    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    year = years;
                                    month = months;
                                    days = dayss;
                                    string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                                    currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                    currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                                    currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                                    currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                                    currentEmployeeExpriance.ToString = " አሁን";
                                }

                                currentEmployeeExpriance.JobTitleName = empCurrrEx[e].JobTitleName;



                                //currentEmployeeExpriance.Description = currentEmployeeExpriance.Year;
                                internalVacancy.Expriance.Add(currentEmployeeExpriance);
                            }
                            #region History
                            var empHistory =
                                db.PlacementHistories.Where(x => x.EmployeeId == a.EmployeeId)
                                    .OrderByDescending(x => x.PlacementHistoryId)
                                    .ToList();
                            for (int e = 0; e < empHistory.Count(); e++)
                            {
                                currentEmployeeExpriance = new CurrentEmployeeExpriance();


                                int year = 0;
                                int month;
                                int days;

                                var fromc = new DateTime(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day);
                                var toc = new DateTime(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day);

                                var totalmonths = (toc.Year - from.Year) * 12 + toc.Month - fromc.Month;
                                totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                var years = totalmonths / 12;
                                var months = totalmonths % 12;
                                var dayss = from.Subtract(toc.AddMonths(totalmonths)).Days;

                                year = years;
                                month = months;
                                days = dayss;
                                currentEmployeeExpriance.From = empHistory[e].From.Date.ToShortDateString();
                                currentEmployeeExpriance.To = empHistory[e].To.Date.ToShortDateString();
                                currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day).ToString();
                                currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day).ToString();
                                currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empHistory[e].From, empHistory[e].To);
                                string val = XAPI.Common.GetConcatenatedYearMonthDay(empHistory[e].From, empHistory[e].To);
                                currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                                currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                                currentEmployeeExpriance.JobTitleName = empHistory[e].JobTitle;
                                internalVacancy.Expriance.Add(currentEmployeeExpriance);
                            }
                            #endregion
                            int TotalCurrentear = internalVacancy.Expriance.Sum(x => x.DYear);

                            int TotalCurrentMonth = internalVacancy.Expriance.Sum(x => x.DMonth);
                            internalVacancy.DMonth = TotalCurrentMonth;
                            internalVacancy.DYear = TotalCurrentear;
                            if (TotalCurrentMonth >= 12)
                            {
                                internalVacancy.DMonth = internalVacancy.DMonth % 12;
                                internalVacancy.DYear += TotalCurrentMonth / 12;
                            }
                            #endregion
                            internalVacancy.Trainings = new List<EmpApplicantTraining>();
                            var emptrainings = trainingList.Where(x => x.EmployeeId == a.EmployeeId).ToList();
                            foreach (var t in emptrainings)
                            {


                                empApplicantTraining = new EmpApplicantTraining();
                                empApplicantTraining.CourseName = t.TrainingName;

                                internalVacancy.Trainings.Add(empApplicantTraining);
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
                            //internalVacancy.RecruitmentType = r.RecruitmentType;
                            if (recruits.RecruitmentType == VacancyType.PERMANENT)
                            {
                                internalVacancy.RecruitmentType = "ቋሚ";
                            }
                            else
                            {
                                internalVacancy.RecruitmentType = "ጊዜያዊ";
                            }
                            internalVacancies.Add(internalVacancy);
                        }
                    }

                }
            }

            return internalVacancies.OrderBy(x => x.No).ToList();
        }
    }
}