using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerforObjectDataSource : List<EmployeePerfromance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromance> GetEmployeePerformance(string branchId, string orgStructureId, string employeeType, string year,string status)
        {
            List<EmployeePerfromance> EmployeeInfoList = new List<EmployeePerfromance>();
            EmployeePerfromance EmployeeInfo = new EmployeePerfromance();

            DateTime dateTow = new DateTime(HrHelper.ReportDateTwo.Year, HrHelper.ReportDateTwo.Month, HrHelper.ReportDateTwo.Day);
            DateTime dateOne = new DateTime(HrHelper.ReportDateOne.Year, HrHelper.ReportDateOne.Month, HrHelper.ReportDateOne.Day);

            Result Results = new Result();

            //var fiscalyears = db.GLFiscalYears.ToList();
            List<Quarter> quarters=new List<Quarter>();
            if(year != null)
            {
                var quarterLength = db.QuarterLengths.FirstOrDefault(x=>x.GlFiscalYearId.ToString() == year);
                if(quarterLength != null)
                {
                    quarters = db.Quarters.Where(x=>x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                }
            }
            var performances = db.EmployeePerformanceScores.ToList();
            if (status == "የተሞላላቸው")
            {
                performances = performances.Where(x => x.IsApproved).ToList();
            }
            if (status == "ያልተሞላላቸው")
            {
                performances = performances.Where(x => !x.IsApproved).ToList();
            }
            //var periods = from p in db.QuarterLengths
            //              join f in fiscalyears
            //                  on p.GlFiscalYearId equals f.GlFiscalYearId
            //              join q in db.Quarters
            //              on p.QuarterLengthId equals q.QuarterLengthId
            //              where f.Status == FiscalYearStatus.Open
            //              select (new { q });

            //var period = periods.Select(x => x.q).ToList();
            List<Person_Employee> employees;

            employees = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, employeeType);

            var placements = db.Placements.ToList();
            decimal checkNullTotalResult = 0;
            if (employees.Any())
            {
                foreach (var e in employees)
                {
                    EmployeeInfo = new EmployeePerfromance();
                    decimal result = 0;
                    var id = e.EmployeeCode;

                    EmployeeInfo.Results = new List<Result>();

                    EmployeeInfo.EmployeeId = id;
                    EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);

                    var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == e.EmployeeId);
                    if(person != null)
                    {
                        EmployeeInfo.FullName = person.FullNameAmharic ;
                    }                  
               

                    var palcmentOne = placements.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                    if (palcmentOne != null)
                    {
                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == palcmentOne.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x=>x.BranchId == palcmentOne.BranchId);
                        if(branch != null)
                        {
                            EmployeeInfo.branchName = branch.AmharicName;
                        }
                    }
                    EmployeeInfo.From = HrHelper.ReportDateOneAm;
                    EmployeeInfo.To = HrHelper.ReportDateTwoAm;

                    decimal totalResult = 0;
                    if (quarters.Any())
                    {
                        foreach (var r in quarters)
                        {
                            result = 0;
                            Results = new Result();

                            var emplperformaScore = performances.Where(x => x.EmployeeId == e.EmployeeId && x.QuarterId == r.QuarterId).ToList();
                            if (emplperformaScore.Any())
                            {
                                result = emplperformaScore.Sum(x => x.Result);
                            }
                            Results.Total = result;
                           
                            Results.Period = r.AmharicName;
                            if(status == "የተሞላላቸው" && Results.Total > 0)
                            {
                                EmployeeInfo.Results.Add(Results);
                                totalResult = totalResult + result;
                            }
                            else if (status == "ያልተሞላላቸው" && Results.Total == 0)
                            {                               
                                var singleScore = db.EmployeePerformanceScores.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.QuarterId == r.QuarterId);
                                if (singleScore != null && singleScore.Result == 0)
                                {
                                    EmployeeInfo.Results.Add(Results);
                                    checkNullTotalResult = checkNullTotalResult + 1;
                                }
                                else if (singleScore == null)
                                {
                                    EmployeeInfo.Results.Add(Results);
                                    checkNullTotalResult = checkNullTotalResult + 1;
                                }
                            }
                        }
                        
                    }
                    if(status == "የተሞላላቸው" && totalResult > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                    else if (status == "ያልተሞላላቸው" && totalResult == 0 && checkNullTotalResult > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }

            }

            return EmployeeInfoList;
        }
    }
}