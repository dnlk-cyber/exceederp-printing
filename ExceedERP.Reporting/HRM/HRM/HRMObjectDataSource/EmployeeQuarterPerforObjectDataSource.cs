using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeQuarterPerforObjectDataSource : List<EmployeeQuarterPerfromance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeQuarterPerfromance> GetEmployeeQuarterPerformance(string fiscalYear, string branchId, string orgStructureId, string employeeType, string status)
        {
            List<EmployeeQuarterPerfromance> EmployeeInfoList = new List<EmployeeQuarterPerfromance>();
            EmployeeQuarterPerfromance EmployeeInfo = new EmployeeQuarterPerfromance();
            decimal totalResult = 0;
            decimal checkNullTotalResult = 0;

            List<Person_Employee> emplist;
            emplist = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, employeeType).ToList();
            List<Quarter> quarters = new List<Quarter>();
            if (fiscalYear != null)
            {
                var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId.ToString() == fiscalYear);
                if (quarterLength != null)
                {
                    quarters = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                }
            }
            var performances = db.EmployeePerformanceScores.ToList();
            if (status == "የተሞላላቸው")
            {
                performances = performances.Where(x=>x.IsApproved).ToList();
            }
            if (status == "ያልተሞላላቸው")
            {
                performances = performances.Where(x => !x.IsApproved).ToList();
            }
           
            if (emplist.Any())
            {
                foreach (var e in emplist)
                {
                    var empid = e.EmployeeCode;

                    EmployeeInfo = new EmployeeQuarterPerfromance();
                    EmployeeInfo.QurterList = new List<QurterList>();
                    totalResult = 0;
                    

                    EmployeeInfo.EmployeeId = empid;
                    EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);

                    // EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId);
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                    if (person != null)
                    {
                        EmployeeInfo.FullName = person.FullNameAmharic;
                    }
                    var placements = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    if (placements != null)
                    {
                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placements.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == placements.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.AmharicName;
                        }
                    }
                    var div = 0;
                    if (quarters.Any())
                    {
                       var performanceSinglePerson = performances.Where(x => x.EmployeeId == e.EmployeeId).ToList();

                        foreach (var p in quarters)
                        {
                            var qurterList = new QurterList();

                            qurterList.PeriodName = p.AmharicName;
                            qurterList.Performance = performanceSinglePerson.Where(x => x.QuarterId == p.QuarterId).Sum(x => x.Result);

                            if (status == "የተሞላላቸው" && qurterList.Performance > 0)
                            {
                                EmployeeInfo.QurterList.Add(qurterList);
                            }
                            else if(status == "ያልተሞላላቸው" && qurterList.Performance == 0)
                            {
                                var singleScore = db.EmployeePerformanceScores.FirstOrDefault(x=>x.EmployeeId == e.EmployeeId && x.QuarterId == p.QuarterId);
                                if(singleScore != null && singleScore.Result == 0)
                                {
                                    EmployeeInfo.QurterList.Add(qurterList);
                                    checkNullTotalResult = checkNullTotalResult + 1;
                                }
                                else if (singleScore == null)
                                {
                                    EmployeeInfo.QurterList.Add(qurterList);
                                    checkNullTotalResult = checkNullTotalResult + 1;
                                }
                            }
                            if (qurterList.Performance > 0)
                            {
                                div = div + 1;
                            }
                        }
                        //div = performance.Where(x=>quarters.Select(y=>y.QuarterId).Contains(x.QuarterId)).Count();//Math.Round(totalResult / div, 2);
                        //totalResult = performance.Where(x => quarters.Select(y => y.QuarterId).Contains(x.QuarterId)).Sum(x=>x.Result);
                        //if (div > 0)
                        //{
                        //    EmployeeInfo.Total = Math.Round(totalResult / div, 2);
                        //}
                    }
                    if (EmployeeInfo.QurterList.Any())
                    {
                        totalResult = EmployeeInfo.QurterList.Sum(x => x.Performance);

                        if (div > 0)
                        {
                            EmployeeInfo.Total = Math.Round(totalResult / div, 2);
                        }
                    }
                    if (status == "የተሞላላቸው" && EmployeeInfo.Total > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                    else if(status == "ያልተሞላላቸው" && EmployeeInfo.Total == 0 && checkNullTotalResult > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }

                }

            }

            return EmployeeInfoList;
        }
    }
}