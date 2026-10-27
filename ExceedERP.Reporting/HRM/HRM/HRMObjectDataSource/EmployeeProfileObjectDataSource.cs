using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Windows.Forms;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class EmployeeProfileObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationContext userdetail = new ApplicationContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PromotedEmployee> GetPromotedEmployeeProfile(string id,string bussinessUnitId)
        {

            string cultureName = GlobalHelper.GetCurrentCulture();
            List<PromotedEmployee> PromotedEmployees = new List<PromotedEmployee>();
            var lookups = db.Lookups;
            var employees = from e in db.Person_Employee
                            join p in db.Placements
                                on e.EmployeeId equals p.EmployeeId
                            join b in db.Branch
                                on p.BranchId equals b.BranchId
                            join m in db.MainCompanyStructures
                                on p.OrgStructureId equals m.OrgStructureId
                            where e.IsActive && p.isCurrent
                            select (new { e, p });
            var otherexpriances = db.Experiences.AsNoTracking().ToList().OrderByDescending(x => x.From).ThenByDescending(x => x.To);
            var currentex = db.Placements.ToList();
            int TotalYear = 0;
            int TotalMonth = 0;
            int totalInternalExperienceDays = 0;
            int totalExternalExperienceDays = 0;
            int totalExperienceDays = 0;
            var educations = db.Education.ToList();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            var list = employees.ToList();
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(id,out int employeeId);
                list = employees.Where(x => x.e.EmployeeId == employeeId).ToList();
            }
            if (!string.IsNullOrEmpty(bussinessUnitId))
            {
                int.TryParse(bussinessUnitId, out int bUId);
                list = list.Where(x => x.p.BusinessUnitId == bUId).ToList();
            }
            var PromotedEmployee = new PromotedEmployee();

            foreach (var r in list.OrderByDescending(x=>x.p.AssignmentDate))
            {
                EmployeeEducation edu;
                OtherEmployeeExpriance otherexplist;
                CurrentEmployeeExpriance currentEmployeeExpriance;

                PromotedEmployee = new PromotedEmployee();
                TotalYear = 0;
                totalInternalExperienceDays = 0;
                totalExternalExperienceDays = 0;
                totalExperienceDays = 0;
                TotalMonth = 0;
                PromotedEmployee.LeaveBalance = new List<LeaveBalanceViewModel>();
                var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == r.e.EmployeeId && !x.IsExpired).OrderByDescending(x=>x.GlFiscalYearId);
                foreach (var le in leaveEntitles)
                {
                    LeaveBalanceViewModel leaveBalance = new LeaveBalanceViewModel();
                    var fisicalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == le.GlFiscalYearId);
                    if (fisicalYear != null)
                    {
                        // leaveBalance.Year = fisicalYear.DateTo.Year.ToString();
                        leaveBalance.Year =datePreference? XAPI.EthiopicDateTime.GetEthiopicYear(fisicalYear.DateTo.Day, fisicalYear.DateTo.Month, fisicalYear.DateTo.Year).ToString():fisicalYear.DateTo.ToString("dd.MM.yyyy");
                        leaveBalance.Balance = le.Balance;
                        PromotedEmployee.LeaveBalance.Add(leaveBalance);
                    }
                }
                // PromotedEmployee.RecruitmentType = r.RecruitmentType;
                PromotedEmployee.No = PromotedEmployees.Count + 1;

                PromotedEmployee.DateOfBirth = r.e.DateOfBirth.ToString("dd.MM.yyyy");
                PromotedEmployee.Age = r.e.DateOfBirth.GetAge().ToString();
                PromotedEmployee.ApplicantName = r.e.FirstNameEnglish + " " + r.e.MiddleNameEnglish + " " +
                                                    r.e.LastNameEnglish;
                var empPerformance = db.EmployeePerformanceSummary
                                            .Where(x => x.EmployeeId.ToString() == id)
                                            .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                                            .FirstOrDefault();
                
                PromotedEmployee.Perfromance = (empPerformance?.Result??0).ToString();
                PromotedEmployee.Educations = new List<EmployeeEducation>();
                var empEdu = educations.OrderByDescending(x=>x.To)/*.ThenByDescending(x=>x.To)*/.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                foreach (var e in empEdu)
                {
                    edu = new EmployeeEducation();
                    edu.From = datePreference? e.FromEAmharic:e.From.HasValue?e.From.Value.ToString("dd.MM.yyyy"):null;
                    edu.To = datePreference? e.ToEAmharic:e.To.ToString("dd.MM.yyyy");
                    var firstOrDefault = lookups.FirstOrDefault(x => x.LookupId == e.EduField);
                    if (firstOrDefault != null)
                        edu.EduField =
                            firstOrDefault.Description;
                    edu.OrgName = e.OrganizationName;
                    var orDefault = lookups.FirstOrDefault(x => x.LookupId == e.EduLevel);
                    if (orDefault != null)
                        edu.EduLevel =
                            orDefault.Description;
                    PromotedEmployee.Educations.Add(edu);
                }
                #region Other
                PromotedEmployee.OtherExpriance = new List<OtherEmployeeExpriance>();
                var empOtherEx = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).OrderByDescending(x=>x.From).ToList();
                foreach (var e in empOtherEx)
                {
                    var jobtitle = lookups.FirstOrDefault(x=>x.LookupId==e.LookupId);
                    totalExternalExperienceDays +=  e.To.Subtract(e.From).Days;
                    totalExperienceDays +=  e.To.Subtract(e.From).Days;
                    otherexplist = new OtherEmployeeExpriance();
                    otherexplist.From =datePreference? e.FromExAmharic:e.From.ToString("dd.MM.yyyy");
                    otherexplist.To = datePreference? e.ToExAmharic:e.To.ToString("dd.MM.yyyy");
                    otherexplist.FromAm =datePreference? e.FromExAmharic:e.From.ToString("dd.MM.yyyy");
                    otherexplist.ToAm = datePreference? e.ToExAmharic:e.To.ToString("dd.MM.yyyy");
                    otherexplist.CompanyName = e.Organozation;
                    otherexplist.JobTitleName =
                        string.IsNullOrWhiteSpace(jobtitle?.Description)
                            ? e.Responsibility
                            : jobtitle.Description;

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
                    otherexplist.ExpExtSingle = EmployeeHelper.GetDaystoWords(e.To.Subtract(e.From).Days);
                    PromotedEmployee.OtherExpriance.Add(otherexplist);
                }
                int TotalOtherYear = PromotedEmployee.OtherExpriance.Sum(x => x.DYear);
                int TotalOtherMonth = PromotedEmployee.OtherExpriance.Sum(x => x.DMonth);
                PromotedEmployee.OtherMonth = TotalOtherMonth;
                PromotedEmployee.OtherYear = TotalOtherYear;
                if (TotalOtherMonth >= 12)
                {
                    PromotedEmployee.OtherMonth = PromotedEmployee.OtherMonth % 12;
                    PromotedEmployee.OtherYear += TotalOtherMonth / 12;
                }


                PromotedEmployee.ExtExp = EmployeeHelper.GetDaystoWords(totalExternalExperienceDays);
                #endregion
                #region Current
                PromotedEmployee.Expriance = new List<CurrentEmployeeExpriance>();
                #region Current
                var empCurrrEx =
                    currentex.Where(x => x.EmployeeId == r.e.EmployeeId)
                        .OrderByDescending(x => x.AssignmentDate)
                        .ToList();
                for (int e = 0; e < empCurrrEx.Count(); e++)
                {
                   
                    currentEmployeeExpriance = new CurrentEmployeeExpriance();
                    int year = 0;
                    int month;
                    int days;
                    if (e >= 1)
                    {
                        var from =empCurrrEx[e].AssignmentDate;
                        var to = empCurrrEx[e - 1].AssignmentDate;
                        totalInternalExperienceDays += to.Subtract(from).Days;
                        totalExperienceDays += to.Subtract(from).Days;

                        var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                        totalmonths += to.Day < from.Day ? -1 : 0;

                        var years = totalmonths / 12;
                        var months = totalmonths % 12;
                        var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

                        year = years;
                        month = months;
                        days = dayss;
                        currentEmployeeExpriance.From =datePreference? empCurrrEx[e].AssignmentDateAmharic:empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.To = datePreference? empCurrrEx[e - 1].AssignmentDateAmharic:empCurrrEx[e - 1].AssignmentDate.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.FromString =datePreference? empCurrrEx[e].AssignmentDateAmharic:empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.ToString = datePreference? empCurrrEx[e - 1].AssignmentDateAmharic:empCurrrEx[e - 1].AssignmentDate.ToString("dd.MM.yyyy");
                       
                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                        currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWords(to.Subtract(from).Days);


                    }
                    else
                    {

                        currentEmployeeExpriance.From = datePreference? empCurrrEx[e].AssignmentDateAmharic:empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.To = datePreference? XAPI.EthiopicDateTime.GetEthiopicDate( DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year):DateTime.Now.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.FromString = datePreference? empCurrrEx[e].AssignmentDateAmharic:empCurrrEx[e].AssignmentDate.ToString("dd.MM.yyyy");
                        currentEmployeeExpriance.ToString = datePreference? XAPI.EthiopicDateTime.GetEthiopicDate( DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year):DateTime.Now.ToString("dd.MM.yyyy");
                        var from = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                        var to = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);
                        totalInternalExperienceDays += to.Subtract(from).Days;
                        totalExperienceDays += to.Subtract(from).Days;
                        var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                        totalmonths += to.Day < from.Day ? -1 : 0;

                        var years = totalmonths / 12;
                        var months = totalmonths % 12;
                        var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

                        year = years;
                        month = months;
                        days = dayss;
                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, DateTime.Now);
                        currentEmployeeExpriance.FromString = empCurrrEx[e].AssignmentDateAmharic ;
                        currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWords(DateTime.Now.Subtract(empCurrrEx[e].AssignmentDate).Days);

                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {
                            currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWordsAmharic(DateTime.Now.Subtract(empCurrrEx[e].AssignmentDate).Days);

                            currentEmployeeExpriance.ToString = " አሁን";

                        }
                        if (cultureName.ToLower() == "om")
                        {
                            currentEmployeeExpriance.ToString = " Amma";
                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            currentEmployeeExpriance.ToString = " Now";

                        }
                    }

                    var lookupId = empCurrrEx[e].LookupId;
                    var jobtitle = lookups.FirstOrDefault(x => x.LookupId == lookupId);
                    currentEmployeeExpriance.JobTitleName =
                       string.IsNullOrWhiteSpace(jobtitle?.Description)
                           ? empCurrrEx[e].JobTitleName
                           : jobtitle.Description;
                    PromotedEmployee.Expriance.Add(currentEmployeeExpriance);
                }
                #endregion
                #region History
                var empHistories =
                    db.PlacementHistories.Where(x => x.EmployeeId == r.e.EmployeeId)
                        .OrderByDescending(x => x.From)
                        .ToList();
                foreach (var empHistory in empHistories)
                {
                    currentEmployeeExpriance = new CurrentEmployeeExpriance();


                    int year = 0;
                    int month;
                    int days;

                    var from = empHistory.From;
                    var to = empHistory.To;
                    totalInternalExperienceDays += to.Subtract(from).Days;
                    totalExperienceDays += to.Subtract(from).Days;
                    var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                    totalmonths += to.Day < from.Day ? -1 : 0;

                    var years = totalmonths / 12;
                    var months = totalmonths % 12;
                    var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

                    year = years;
                    month = months;
                    days = dayss;
                    currentEmployeeExpriance.From = datePreference? empHistory.FromAmharic:empHistory.From.ToString("dd.MM.yyyy");
                    currentEmployeeExpriance.To = datePreference? empHistory.ToAmharic:empHistory.To.ToString("dd.MM.yyyy");
                    currentEmployeeExpriance.FromString = datePreference? empHistory.FromAmharic:empHistory.From.ToString("dd.MM.yyyy");
                    currentEmployeeExpriance.ToString = datePreference? empHistory.ToAmharic:empHistory.To.ToString("dd.MM.yyyy");
                  
                    currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empHistory.From, empHistory.To);
                    string val = XAPI.Common.GetConcatenatedYearMonthDay(empHistory.From, empHistory.To);
                    currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                    currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                    var lookupId = empHistory.LookupId;
                    var jobtitle = lookups.FirstOrDefault(x => x.LookupId == lookupId);
                    currentEmployeeExpriance.JobTitleName =
                       string.IsNullOrWhiteSpace(jobtitle?.Description)
                           ? empHistory.JobTitle
                           : jobtitle.Description;
                    currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWords(empHistory.To.Subtract(empHistory.From).Days);

                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWordsAmharic(empHistory.To.Subtract(empHistory.From).Days);


                    }
                    PromotedEmployee.Expriance.Add(currentEmployeeExpriance);
                }
                #endregion
                int TotalCurrentear = PromotedEmployee.Expriance.Sum(x => x.DYear);

                int TotalCurrentMonth = PromotedEmployee.Expriance.Sum(x => x.DMonth);
                PromotedEmployee.DMonth = TotalCurrentMonth;
                PromotedEmployee.DYear = TotalCurrentear;
                PromotedEmployee.IntExp = EmployeeHelper.GetDaystoWords(totalInternalExperienceDays);
                if (TotalCurrentMonth >= 12)
                {
                    PromotedEmployee.DMonth = PromotedEmployee.DMonth % 12;
                    PromotedEmployee.DYear += TotalCurrentMonth / 12;

                }

                #endregion
                var activePlacment = currentex.FirstOrDefault(x => x.EmployeeId == r.e.EmployeeId && x.isCurrent);
                if (activePlacment != null)
                {
                    PromotedEmployee.CurrentSalary = activePlacment.Salary;
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == activePlacment.BranchId);
                    if (branch != null)
                    {
                        PromotedEmployee.Branch = branch.AmharicName;
                    }
                }
                TotalYear = PromotedEmployee.DYear + PromotedEmployee.OtherYear;
                TotalMonth = PromotedEmployee.DMonth + PromotedEmployee.OtherMonth;
                if (TotalMonth >= 12)
                {
                    TotalYear += TotalMonth / 12;
                    TotalMonth = TotalMonth % 12;

                }
                //PromotedEmployee.TotalYear = TotalYear.ToString();
                // double allExperienceDaySum = 0;
                //var extExps = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).OrderByDescending(x=>x.From).ThenByDescending(x=>x.To).ToList();
                //double extTotal = 0;
                //foreach (var ex in extExps)
                //{
                //    extTotal = extTotal + ex.To.Subtract(ex.From).Days;
                //}
                //double intTotal = DateTime.Today.Subtract(r.e.HiredDate).Days;
                //double totalExp = extTotal + intTotal;
                PromotedEmployee.TotalMonth = EmployeeHelper.GetDaystoWords(totalExperienceDays);//TotalMonth.ToString();
                PromotedEmployee.TotalYear = EmployeeHelper.GetDaystoWords(totalExperienceDays); //TotalYear.ToString();                                                         //PromotedEmployee.TotalMonth = TotalDays.ToString();
                PromotedEmployee.RecruitmentType = HrHelper.GetEmployeeStatus(r.e.Status);
                PromotedEmployees.Add(PromotedEmployee);
            }
            
            return PromotedEmployees.OrderBy(x => x.No).ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public HeaderEmployeeVM GetEmployeeHeader(string employee)
        {
            HeaderEmployeeVM record = new HeaderEmployeeVM();
            int.TryParse(employee, out int employeeId);
          
            var person_Employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (person_Employee == null)
                return record; // Return empty record safely

            record.FullName = person_Employee.EmployeeCode+"-"+person_Employee.FirstNameEnglish+" "+person_Employee.MiddleNameEnglish+" "+person_Employee.LastNameEnglish;
               

            return record;
        }
        public class HeaderEmployeeVM
        {

            public string FullName { get; set; }
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }
}