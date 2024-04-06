using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
//using ExceedERP.Web.Areas.HRM.Controllers;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerformanceDepartmentODS : List<EmployeePerfromanceVM>
    {
        //HRMInputs input = new HRMInputs();
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string empId, string id, string year, string businessUnit)
        {
            List<EmployeePerfromanceVM> employeePerformanceList = new List<EmployeePerfromanceVM>();

            int.TryParse(year, out int yearId);
            decimal totalResult = 0;

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            if (id != null)
            {
                int.TryParse(id, out int personId);
                var place = placements.FirstOrDefault(x => x.EmployeeId == personId && x.isCurrent);
                if (place != null)
                {
                    placements = EmployeeHelper.GetEmployees_ByDepartment_FromPlacement(placements, place.OrgStructureId).OrderByDescending(x => x.AssignmentDate).ToList();
                }
            }
            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                placements = placements.Where(x => x.EmployeeId == employeeId).ToList();
            }

            if (businessUnit != null)
            {
                int.TryParse(businessUnit, out int businessUnitId);
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId).ToList();
            }

            foreach (var place in placements)
            {
                var empPerformance = new EmployeePerfromanceVM();

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
                    empPerformance.PerformanceResults = new List<PerformanceResultVM>();

                    var quarters = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();

                    foreach (var quar in quarters)
                    {
                        var quarter = new PerformanceResultVM();

                        quarter.Period = quar.Name;

                        var performance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlPeriodId == quar.QuarterId);

                        if (performance != null)
                        {
                            quarter.Score = GetPerformanceScore(performance.EmployeePerformanceId);
                            totalResult = totalResult + quarter.Score;

                        }

                        empPerformance.PerformanceResults.Add(quarter);
                    }
                }

                employeePerformanceList.Add(empPerformance);
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