using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Data.Entity;
using System.Windows.Forms;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class MyProfileObjectDataSource : List<PromotedEmployee>
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationContext userdetail = new ApplicationContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PromotedEmployee> GetPromotedEmployee(string id)
        {



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
            var otherexpriances = db.Experiences.ToList();
            var currentex = db.Placements.ToList();

            int TotalYear = 0;
            int TotalMonth = 0;
            int TotalDays = 0;
            var educations = db.Education.ToList();


            if (id != null)
            {
                var list = employees.Where(x => x.e.EmployeeId.ToString() == id).ToList();


                var PromotedEmployee = new PromotedEmployee();

                foreach (var r in list)
                {
                    EmployeeEducation edu;
                    OtherEmployeeExpriance otherexplist;
                    CurrentEmployeeExpriance currentEmployeeExpriance;

                    PromotedEmployee = new PromotedEmployee();
                    TotalYear = 0;
                    TotalMonth = 0;
                    TotalDays = 0;
                    PromotedEmployee.LeaveBalance = new List<LeaveBalanceViewModel>();
                    var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == r.e.EmployeeId && !x.IsExpired);
                    foreach (var le in leaveEntitles)
                    {
                        LeaveBalanceViewModel leaveBalance = new LeaveBalanceViewModel();
                        var fisicalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == le.GlFiscalYearId);
                        if (fisicalYear != null)
                        {
                            leaveBalance.Year = fisicalYear.DateTo.Year.ToString();
                            leaveBalance.Balance = le.Balance;

                            PromotedEmployee.LeaveBalance.Add(leaveBalance);
                        }
                    }
                    // PromotedEmployee.RecruitmentType = r.RecruitmentType;

                    PromotedEmployee.No = PromotedEmployees.Count + 1;

                    PromotedEmployee.ApplicantName = r.e.FirstName + " " + r.e.MiddleName + " " +
                                                     r.e.LastName;

                    var empPerformances = db.MeasurementDefinitions.Where(x => x.EmployeeId.ToString() == id).ToList();
                    decimal addPerfor = 0;
                    foreach (var empPerformance in empPerformances)
                    {
                        // EmployeePerformances empPerform = new EmployeePerformances();
                        //var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == empPerformance.LookupId);
                        //if (lookup != null)
                        //{
                        //    empPerform.Type = lookup.Description;
                        //}
                        // empPerform.EvaluationCriteria = empPerformance.EvaluationCriteria;
                        addPerfor = addPerfor + empPerformance.Result;



                    }
                    PromotedEmployee.Perfromance = addPerfor.ToString();
                    PromotedEmployee.Educations = new List<EmployeeEducation>();

                    var empEdu = educations.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                    foreach (var e in empEdu)
                    {
                        edu = new EmployeeEducation();
                        if (e.From != null) edu.From = e.From.Value.ToShortDateString();
                        edu.To = e.To.ToShortDateString();
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
                    var empOtherEx = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
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

                        otherexplist.ExpExtSingle = EmployeeHelper.GetDaystoWordsAmharic(e.To.Subtract(e.From).Days);

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
                    //  var extExp = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();

                    var extExpeiences = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();

                    double extTotalDays = 0;
                    foreach (var ex in extExpeiences)
                    {
                        extTotalDays = extTotalDays + ex.To.Subtract(ex.From).Days;
                    }



                    PromotedEmployee.ExtExp = EmployeeHelper.GetDaystoWordsAmharic(extTotalDays);


                    #endregion
                    #region Current
                    PromotedEmployee.Expriance = new List<CurrentEmployeeExpriance>();
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

                            var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                            totalmonths += to.Day < from.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

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
                            currentEmployeeExpriance.IntExpSingle = EmployeeHelper.GetDaystoWordsAmharic(empCurrrEx[e - 1].AssignmentDate.Subtract(empCurrrEx[e].AssignmentDate).Days);


                        }
                        else
                        {
                            currentEmployeeExpriance.From = empCurrrEx[e].AssignmentDate.Date.ToShortDateString();

                            currentEmployeeExpriance.To = DateTime.Now.Date.ToShortDateString();

                            var from = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                            var to = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

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
                            currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                            currentEmployeeExpriance.ToString = " አሁን";
                        }

                        currentEmployeeExpriance.JobTitleName = empCurrrEx[e].JobTitleName;



                        //currentEmployeeExpriance.Description = currentEmployeeExpriance.Year;
                        PromotedEmployee.Expriance.Add(currentEmployeeExpriance);
                    }
                    #endregion
                    #region History
                    var empHistory =
                        db.PlacementHistories.Where(x => x.EmployeeId == r.e.EmployeeId)
                            .OrderByDescending(x => x.PlacementHistoryId)
                            .ToList();
                    for (int e = 0; e < empHistory.Count(); e++)
                    {
                        currentEmployeeExpriance = new CurrentEmployeeExpriance();


                        int year = 0;
                        int month;
                        int days;

                        var from = new DateTime(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day);
                        var to = new DateTime(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day);

                        var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                        totalmonths += to.Day < from.Day ? -1 : 0;

                        var years = totalmonths / 12;
                        var months = totalmonths % 12;
                        var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

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
                        // currentEmployeeExpriance.IntExp = EmployeeHelper.GetDaystoWordsAmharic(empHistory[e].To.Subtract(empHistory[e].From).Days);
                        PromotedEmployee.Expriance.Add(currentEmployeeExpriance);
                    }
                    #endregion
                    int TotalCurrentear = PromotedEmployee.Expriance.Sum(x => x.DYear);

                    int TotalCurrentMonth = PromotedEmployee.Expriance.Sum(x => x.DMonth);
                    PromotedEmployee.DMonth = TotalCurrentMonth;
                    PromotedEmployee.DYear = TotalCurrentear;
                    PromotedEmployee.IntExp = EmployeeHelper.GetDaystoWordsAmharic((DateTime.Today.Subtract(r.e.HiredDate)).Days);
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

                    var extExps = otherexpriances.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                    double extTotal = 0;
                    foreach (var ex in extExps)
                    {
                        extTotal = extTotal + ex.To.Subtract(ex.From).Days;
                    }


                    double intTotal = DateTime.Today.Subtract(r.e.HiredDate).Days;
                    double totalExp = extTotal + intTotal;

                    PromotedEmployee.TotalMonth = EmployeeHelper.GetDaystoWordsAmharic(totalExp);//TotalMonth.ToString();
                    PromotedEmployee.TotalYear = EmployeeHelper.GetDaystoWordsAmharic(totalExp); //TotalYear.ToString();                                                         //PromotedEmployee.TotalMonth = TotalDays.ToString();
                    PromotedEmployee.RecruitmentType = HrHelper.GetEmployeeStatus(r.e.Status);

                    PromotedEmployees.Add(PromotedEmployee);
                }
            }
            return PromotedEmployees.OrderBy(x => x.No).ToList();
        }

    }
}