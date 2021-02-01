using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using Telerik.Reporting.Charting;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource

{
    [DataObject]
    public class PromotedEmployeeObjectDataSource : List<PromotedEmployee>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PromotedEmployee> GetPromotedEmployee(string branchId, string orgId, string empId)
        {
            List<PromotedEmployee> promotedEmployees = new List<PromotedEmployee>();
            var lookups = db.Lookups;
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
            //if (fiscalYear != null)
            //{
            //    var filtredQaurters = list.Where(x => x.f.GlFiscalYearId.ToString() == fiscalYear).ToList();
            //    qaurters = filtredQaurters.Select(x => x.q).ToList();
            //}
            //else
            //{
                qaurters = listPeriod.Where(f => f.f.Status == FiscalYearStatus.Open).Select(x => x.q).ToList();
            //}




            /////////////////////////////////////////////////
            int TotalYear = 0;
            int TotalMonth = 0;
            int TotalDays = 0;
            var educations = db.Education.ToList();
            var list = employees.ToList();
            if (branchId != null)
            {
                list = list.Where(x => x.p.BranchId.ToString() == branchId).ToList();
            }
            if (orgId != null)
            {
                list = list.Where(x => x.p.OrgStructureId.ToString() == orgId).ToList();
            }
            if (empId != null)
            {
                list = list.Where(x => x.e.EmployeeId.ToString() == empId).ToList();
            }

            var promotedEmployee = new PromotedEmployee();
            if (branchId != null || empId != null || orgId != null)
            {
                var allPerfromace = db.EmployeeMeasurementDefinition.ToList();
                foreach (var r in list)
                {
                    EmployeeEducation edu;
                    OtherEmployeeExpriance otherexplist;
                    CurrentEmployeeExpriance currentEmployeeExpriance;

                    promotedEmployee = new PromotedEmployee();
                    TotalYear = 0;
                    TotalMonth = 0;
                    TotalDays = 0;

                    // internalVacancy.RecruitmentType = r.RecruitmentType;

                    promotedEmployee.No = promotedEmployees.Count + 1;

                    promotedEmployee.ApplicantName = r.e.FullNameaEnglish;// r.e.FirstName + " " + r.e.MiddleName + " " + r.e.LastName;
                    promotedEmployee.Educations = new List<EmployeeEducation>();



                    //decimal result = 0;
                    //var difiitions = db.MeasurementDefinitions.Where(x => x.EmployeeId == r.e.EmployeeId );
                    //foreach(var difinition in difiitions)
                    //{
                    //    var performance = db.EmployeeMeasurementDefinition.Where(x => x.MeasurementDefinitionId == difinition.MeasurementDefinitionId);

                    //    result = performance.Sum(x=>x.Result);
                    //}
                    if (qaurters.Any())
                    {
                        decimal totalResult = 0;
                        int div = 0;
                        var performance = allPerfromace.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                        foreach (var p in qaurters)
                        {
                            var quarterName = p.AmharicName;
                            if (performance.Any())
                            {
                                var oneQuarter = performance.Where(x => x.QuarterId == p.QuarterId).Sum(x => x.Result);

                                totalResult = totalResult + oneQuarter;
                            }
                           
                        }
                       
                            promotedEmployee.Perfromance = (Math.Round(totalResult / qaurters.Count(), 2)).ToString();
                       
                       
                    }


                    var empEdu = educations.Where(x => x.EmployeeId == r.e.EmployeeId).ToList();
                    foreach (var e in empEdu)
                    {
                        edu = new EmployeeEducation();
                        if (e.From != null)
                            edu.From = XAPI.EthiopicDateTime.GetEthiopicDate(e.From.Value.Day,e.From.Value.Month,e.From.Value.Year);// e.From.Value.ToShortDateString();
                        if (e.To != null)
                            edu.To = XAPI.EthiopicDateTime.GetEthiopicDate(e.To.Day, e.To.Month, e.To.Year); //e.To.ToShortDateString();
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
                        if (e.From != null)
                            otherexplist.From = XAPI.EthiopicDateTime.GetEthiopicDate(e.From.Day, e.From.Month, e.From.Year); // e.From.ToShortDateString();
                        if (e.To != null)
                            otherexplist.To = XAPI.EthiopicDateTime.GetEthiopicDate(e.To.Day, e.To.Month, e.To.Year); //e.To.ToShortDateString();
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

                            var totalmonths = (to.Year - from.Year) * 12 + to.Month - from.Month;
                            totalmonths += to.Day < from.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = from.Subtract(to.AddMonths(totalmonths)).Days;

                            year = years;
                            month = months;
                            days = dayss;
                            currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year); //empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                            currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year);// empCurrrEx[e - 1].AssignmentDate.Date.ToShortDateString();
                            currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year).ToString();
                            currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year).ToString();
                            currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                            string val = XAPI.Common.GetConcatenatedYearMonthDay(empCurrrEx[e].AssignmentDate, empCurrrEx[e - 1].AssignmentDate);
                            currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                            currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());

                        }
                        else
                        {
                            currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year); //empCurrrEx[e].AssignmentDate.Date.ToShortDateString();

                            currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year);//DateTime.Now.Date.ToShortDateString();

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
                        currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].From.Day, empHistory[e].From.Month, empHistory[e].From.Year); //empHistory[e].From.Date.ToShortDateString();
                        currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].To.Day, empHistory[e].To.Month, empHistory[e].To.Year); //empHistory[e].To.Date.ToShortDateString();
                        //currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].From.Year, empHistory[e].From.Month, empHistory[e].From.Day).ToString();
                        //currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].To.Year, empHistory[e].To.Month, empHistory[e].To.Day).ToString();
                        currentEmployeeExpriance.FromString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].From.Day, empHistory[e].From.Month, empHistory[e].From.Year).ToString();
                        currentEmployeeExpriance.ToString = XAPI.EthiopicDateTime.GetEthiopicDate(empHistory[e].To.Day, empHistory[e].To.Month, empHistory[e].To.Year).ToString();
                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(empHistory[e].From, empHistory[e].To);
                        string val = XAPI.Common.GetConcatenatedYearMonthDay(empHistory[e].From, empHistory[e].To);
                        currentEmployeeExpriance.DYear = Convert.ToInt32(val.Split('-')[0].ToString());
                        currentEmployeeExpriance.DMonth = Convert.ToInt32(val.Split('-')[1].ToString());
                        currentEmployeeExpriance.JobTitleName = empHistory[e].JobTitle;
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
                        promotedEmployee.Branch = activePlacment.Branch.AmharicName;
                    }


                    TotalYear = promotedEmployee.DYear + promotedEmployee.OtherYear;
                    TotalMonth = promotedEmployee.DMonth + promotedEmployee.OtherMonth;
                    if (TotalMonth >= 12)
                    {
                        TotalYear += TotalMonth / 12;
                        TotalMonth = TotalMonth % 12;

                    }
                    promotedEmployee.TotalYear = TotalYear.ToString();
                    promotedEmployee.TotalMonth = TotalMonth.ToString();
                    //promotedEmployee.TotalMonth = TotalDays.ToString();
                    promotedEmployee.RecruitmentType = HrHelper.GetEmployeeStatus(r.e.Status);

                    promotedEmployees.Add(promotedEmployee);
                }
            }
            return promotedEmployees.OrderBy(x => x.No).ToList();
        }
    }
}