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
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeQuarterPerforObjectDataSource : List<EmployeeQuarterPerfromance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeQuarterPerfromance> GetEmployeeQuarterPerformance(string fiscalYear, string branchId, string orgStructureId, string status)
        {
            List<EmployeeQuarterPerfromance> employeePerformanceList = new List<EmployeeQuarterPerfromance>();

            int.TryParse(fiscalYear, out int yearId);

            decimal totalResult = 0;

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId).OrderByDescending(x => x.AssignmentDate).ToList();
            }

            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_FromPlacement(placements, orgId).OrderByDescending(x => x.AssignmentDate).ToList();
            }

            //if (empId != null)
            //{
            //    int.TryParse(empId, out int employeeId);
            //    placements = placements.Where(x => x.EmployeeId == employeeId).ToList();
            //}

            foreach (var place in placements)
            {
                totalResult = 0;

                var empPerformance = new EmployeeQuarterPerfromance();

                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if (emp != null)
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = emp.FullNameaEnglish;
                    empPerformance.Sex = emp.Gender.ToString();
                }

                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                if (mainCompany != null)
                {
                    empPerformance.Department = mainCompany.Name;
                }
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                if (branch != null)
                {
                    empPerformance.Branch = branch.Name;
                }

                var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == yearId);
                if (quarterLength != null)
                {
                    empPerformance.QurterList = new List<QurterList>();

                    var quarters = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();

                    foreach (var quar in quarters)
                    {
                        var quarter = new QurterList();

                        quarter.PeriodName = quar.Name;

                        var performance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlPeriodId == quar.QuarterId);

                        if (performance != null)
                        {
                            quarter.Score = GetPerformanceScore(performance.EmployeePerformanceId);
                            totalResult = quarter.Score;
                        }

                        empPerformance.QurterList.Add(quarter);
                    }

                    empPerformance.Total = empPerformance.QurterList.Sum(x=>x.Score);
                }

                if (status == "Reviewed" && totalResult > 0)
                {
                    employeePerformanceList.Add(empPerformance);
                }
                else if (status == "Not Reviewed" && totalResult == 0)
                {
                    employeePerformanceList.Add(empPerformance);
                }
                else if (status == null)
                {
                    employeePerformanceList.Add(empPerformance);
                }
            }

            return employeePerformanceList;
        }

        public decimal GetPerformanceScore(int empPerformanceId)
        {
            decimal totalScore = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalScore = totalScore + scores.Sum(x => x.Score);
                }
            }

            return totalScore;
        }
    }
}