using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class PromotedEmployeeObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<PromotedEmployee> GetPromotedEmployee(string branchId, string orgId, string empId)
        {
            List<PromotedEmployee> promotedEmployees = new List<PromotedEmployee>();
            var lookups = db.Lookups.AsNoTracking();
            var employees = from e in db.Person_Employee.Where(x => x.IsActive)
                            join p in db.Placements.Where(x => x.isCurrent)
                                on e.EmployeeId equals p.EmployeeId
                            join b in db.Branch
                                on p.BranchId equals b.BranchId
                            join m in db.MainCompanyStructures
                                on p.OrgStructureId equals m.OrgStructureId
                            where e.IsActive && p.isCurrent
                            select (new { e, p });
            var otherexpriances = db.Experiences.ToList();
            var currentex = db.Placements.ToList();

            //////////////////////////////////////////////////////////////////////////////////////////////////
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
            bool datePreference = HrHelper.GetDatePreference();
            var isLocal = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et") || (cultureName.ToLower() == "om");
            var isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et";
            var isOrom =  cultureName.ToLower() == "om";
            int TotalYear = 0;
            int totalInternalExperience = 0;
            int totalExternalExperience = 0;
            int totalExperience = 0;
            var jobTitles = lookups
                .Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.Jobtitle_External_Experience
                         || x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE
                         || x.LookupType == Core.Domain.Common.LookUpTypes.OLD_JOB_TITLE)
                .ToDictionary(x => x.LookupId);
            int TotalMonth = 0;
            var educations = db.Education.ToList();
            var list = employees.ToList();
            if (!string.IsNullOrEmpty(branchId))
            {
                list = list.Where(x => x.p.BranchId.ToString() == branchId).ToList();
            }
            if (!string.IsNullOrEmpty(orgId))
            {
                int.TryParse(orgId, out int orgStructureId);
                var orgIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId || x.OrgStructureId == orgStructureId).Select(x => x.OrgStructureId).ToList();
                list = list.Where(x => orgIds.Contains(x.p.OrgStructureId)).ToList();
            }
            if (!string.IsNullOrEmpty(empId))
            {
                list = list.Where(x => x.e.EmployeeId.ToString() == empId).ToList();
            }
            var promotedEmployee = new PromotedEmployee();
            if (!string.IsNullOrEmpty(branchId) || !string.IsNullOrEmpty(empId) || !string.IsNullOrEmpty(orgId))
            {
                foreach (var r in list)
                {
                    EmployeeEducation edu;
                    OtherEmployeeExpriance otherexplist;
                    CurrentEmployeeExpriance currentEmployeeExpriance;
                    promotedEmployee = new PromotedEmployee();
                    TotalYear = 0;
                    TotalMonth = 0;
                     totalInternalExperience = 0;
                     totalExternalExperience = 0;
                     totalExperience = 0;
                    promotedEmployee.No = promotedEmployees.Count + 1;
                    promotedEmployee.EmployeeCode = r.e.EmployeeCode;
                    promotedEmployee.ApplicantName = isLocal?r.e.FullNameAmharic: r.e.FullNameaEnglish;
                    promotedEmployee.Educations = new List<EmployeeEducation>();
                    // Adding Job perdormance
                    var employeePerformance = db.EmployeePerformanceSummary
                        .Where(x => x.EmployeeId==r.e.EmployeeId)
                        .OrderByDescending(x=>x.EmployeePerformanceSummaryId)
                        .FirstOrDefault();
                    if ( employeePerformance !=null)
                    {
                        promotedEmployee.Perfromance = employeePerformance.Result.ToString();

                    }

                    
                    
                    var empEdu = educations.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                    foreach (var e in empEdu)
                    {
                        edu = new EmployeeEducation();
                            edu.From = datePreference ? e?.FromEAmharic
                                                   : (e?.From.HasValue == true ? e.From.Value.ToString("dd.MM.yyyy") : null);
                            edu.To = datePreference ? e?.ToEAmharic
                                        : e?.To.ToString("dd.MM.yyyy");
                        var firstOrDefault = lookups.FirstOrDefault(x => x.LookupId == e.EduField);
                        if (firstOrDefault != null)
                            edu.EduField =
                                firstOrDefault.Description;
                        edu.OrgName = e.OrganizationName;
                        var orDefault = lookups.FirstOrDefault(x => x.LookupId == e.EduLevel);
                        if (orDefault != null)
                            edu.EduLevel =
                                orDefault.Description;
                        promotedEmployee.Educations.Add(edu);
                    }
                    #region Other
                    promotedEmployee.OtherExpriance = new List<OtherEmployeeExpriance>();
                    var empOtherEx = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                    foreach (var e in empOtherEx)
                    {
                        otherexplist = new OtherEmployeeExpriance();
                        jobTitles.TryGetValue(e.JobTitleId , out var jt);


                        otherexplist.From = datePreference ? e?.FromExAmharic
                                                   :  e.From.ToString("dd.MM.yyyy") ;

                        otherexplist.To = datePreference ? e?.ToExAmharic
                                                   :  e.To.ToString("dd.MM.yyyy") ;
                        otherexplist.CompanyName = e.Organozation;
                        otherexplist.JobTitleName = jt?.Description ?? e.Responsibility;

                        totalExternalExperience += e.To.Subtract(e.From).Days;
                        totalExperience += e.To.Subtract(e.From).Days;
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            otherexplist.Year = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(e.To.Subtract(e.From).Days));
                        else
                            otherexplist.Year = EmployeeHelper.GetDaystoWords(Convert.ToDouble(e.To.Subtract(e.From).Days));

                        //otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                        //otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                        string val = XAPI.Common.GetConcatenatedYearMonthDay(e.From, e.To);
                        otherexplist.DYear = Convert.ToInt32(val.Split('-')[0]);
                        otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);
                        otherexplist.DMonth = Convert.ToInt32(val.Split('-')[1]);

                        promotedEmployee.OtherExpriance.Add(otherexplist);
                    }


                    int TotalOtherYear = promotedEmployee.OtherExpriance.Sum(x => x.DYear);

                    int TotalOtherMonth = promotedEmployee.OtherExpriance.Sum(x => x.DMonth);
                    promotedEmployee.OtherMonth = TotalOtherMonth;
                    promotedEmployee.OtherYear = TotalOtherYear;
                    if (TotalOtherMonth >= 12)
                    {
                        promotedEmployee.OtherMonth = promotedEmployee.OtherMonth % 12;
                        promotedEmployee.OtherYear += TotalOtherMonth / 12;
                    }
                    #endregion
                    #region Current
                    promotedEmployee.Expriance = new List<CurrentEmployeeExpriance>();
                    #region Current
                    var empCurrrEx =
                        currentex.Where(x => x.EmployeeId == r.e.EmployeeId)
                            .OrderByDescending(x => x.PlacementId)
                            .ToList();
                    for (int e = 0; e < empCurrrEx.Count(); e++)
                    {
                        currentEmployeeExpriance = new CurrentEmployeeExpriance();


                        int year = 0;
                        int month;
                        int days;
                        if (e >= 1)
                        {
                            var from = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                            var to = new DateTime(empCurrrEx[e - 1].AssignmentDate.Year, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Day);
                            totalInternalExperience += to.Subtract(from).Days;
                            totalExperience += to.Subtract(from).Days;
                            var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                            totalmonths += to.Day < from.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

                            year = years;
                            month = months;
                            days = dayss;
                            currentEmployeeExpriance.From = datePreference ?empCurrrEx[e].AssignmentDateAmharic: empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy") ;
                            currentEmployeeExpriance.FromString = datePreference ? empCurrrEx[e].AssignmentDateAmharic : empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                            currentEmployeeExpriance.To = datePreference ? empCurrrEx[e-1].AssignmentDateAmharic : empCurrrEx[e-1].AssignmentDate.ToString("dd.MM.yyyy");
                            currentEmployeeExpriance.ToString = datePreference ? empCurrrEx[e-1].AssignmentDateAmharic : empCurrrEx[e-1].AssignmentDate.ToString("dd.MM.yyyy");
                            if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Subtract(from).Days));
                            else
                                currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWords(Convert.ToDouble(to.Subtract(from).Days));

                            string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                            currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                            currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                           
                           

                        }
                        else
                        {
                            currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDateAmharic;

                            currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);//DateTime.Now.Date.ToShortDateString();

                            var from = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                            var to = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

                            totalInternalExperience += to.Subtract(from).Days;
                            totalExperience += to.Subtract(from).Days;
                            string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                            currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                            currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                            if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Subtract(from).Days));
                            else
                                currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWords(Convert.ToDouble(to.Subtract(from).Days));

                            //currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                            currentEmployeeExpriance.FromString = datePreference ? empCurrrEx[e].AssignmentDateAmharic : empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                            currentEmployeeExpriance.ToString = isAmharic?" አሁን":isOrom? "Amma":"Present";
                        }
                        var lookupId = empCurrrEx[e].LookupId;
                        jobTitles.TryGetValue(lookupId, out var jt);

                        currentEmployeeExpriance.JobTitleName = jt?.Description ?? empCurrrEx[e].JobTitleName ?? "-";
                        promotedEmployee.Expriance.Add(currentEmployeeExpriance);
                    }
                    #endregion
                    #region History
                    var empHistory =
                        db.PlacementHistories.Where(x => x.EmployeeId == r.e.EmployeeId)
                            .OrderByDescending(x => x.PlacementHistoryId)
                            .ToList();
                    for (int e = 0; e < empHistory.Count(); e++)
                    {
                        var from = empHistory[e].From;
                        var to = empHistory[e].To;

                        currentEmployeeExpriance = new CurrentEmployeeExpriance();
                        totalInternalExperience += empHistory[e].To.Subtract(empHistory[e].From).Days;
                        totalExperience += empHistory[e].To.Subtract(empHistory[e].From).Days;
                        currentEmployeeExpriance.From = datePreference ? empHistory[e].FromAmharic : empHistory[e].From.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.To = datePreference ? empHistory[e].ToAmharic : empHistory[e].To.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.FromString = datePreference ? empHistory[e].FromAmharic : empHistory[e].From.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.ToString = datePreference ? empHistory[e].ToAmharic : empHistory[e].To.ToString("dd.MM.yyyy");
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Subtract(from).Days));
                        else
                            currentEmployeeExpriance.Year = EmployeeHelper.GetDaystoWords(Convert.ToDouble(to.Subtract(from).Days));

                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empHistory[e].From, empHistory[e].To);
                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                        currentEmployeeExpriance.JobTitleName = empHistory[e].JobTitle;
                        var lookupId = empHistory[e].LookupId;
                        jobTitles.TryGetValue(lookupId, out var jt);

                        currentEmployeeExpriance.JobTitleName = jt?.Description ?? empHistory[e].JobTitle ?? "-";
                        promotedEmployee.Expriance.Add(currentEmployeeExpriance);

                    }
                    #endregion
                    int TotalCurrentear = promotedEmployee.Expriance.Sum(x => x.DYear);
                    int TotalCurrentMonth = promotedEmployee.Expriance.Sum(x => x.DMonth);
                    promotedEmployee.DMonth = TotalCurrentMonth;
                    promotedEmployee.DYear = TotalCurrentear;
                    if (TotalCurrentMonth >= 12)
                    {
                        promotedEmployee.DMonth = promotedEmployee.DMonth % 12;
                        promotedEmployee.DYear += TotalCurrentMonth / 12;
                    }

                    #endregion
                    var activePlacment = currentex.FirstOrDefault(x => x.EmployeeId == r.e.EmployeeId && x.isCurrent);
                    if (activePlacment != null)
                    {
                        promotedEmployee.CurrentSalary = activePlacment.Salary;
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == activePlacment.BranchId);
                        if (branch != null)
                        {
                            promotedEmployee.Branch = branch.AmharicName;
                        }
                    }
                    TotalYear = promotedEmployee.DYear + promotedEmployee.OtherYear;
                    TotalMonth = promotedEmployee.DMonth + promotedEmployee.OtherMonth;
                    if (TotalMonth >= 12)
                    {
                        TotalYear += TotalMonth / 12;
                        TotalMonth = TotalMonth % 12;
                    }
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        promotedEmployee.ExtExp = EmployeeHelper.GetDaystoWordsAmharic(totalExternalExperience);
                        promotedEmployee.IntExp = EmployeeHelper.GetDaystoWordsAmharic(totalInternalExperience);
                        promotedEmployee.TotalYear = EmployeeHelper.GetDaystoWordsAmharic(totalExperience);
                        promotedEmployee.TotalMonth = EmployeeHelper.GetDaystoWordsAmharic(totalExperience);

                    }
                    else
                    {
                        promotedEmployee.ExtExp = EmployeeHelper.GetDaystoWords(totalExternalExperience);
                        promotedEmployee.IntExp = EmployeeHelper.GetDaystoWords(totalInternalExperience);
                        promotedEmployee.TotalYear = EmployeeHelper.GetDaystoWords(totalExperience);
                        promotedEmployee.TotalMonth = EmployeeHelper.GetDaystoWords(totalExperience);


                    }
                    promotedEmployee.RecruitmentType = HrHelper.GetEmployeeStatus(r.e.Status);
                    promotedEmployees.Add(promotedEmployee);
                }
            }
            return promotedEmployees.OrderBy(x => x.No).ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }
}