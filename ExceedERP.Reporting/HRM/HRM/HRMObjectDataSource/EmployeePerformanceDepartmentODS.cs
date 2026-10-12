using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;
using PerformanceEntity = ExceedERP.Core.Domain.HRM.PerformanceManagment.EmployeePerformance;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerformanceDepartmentODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string empId, string id, string year, string businessUnit)
        {
            var employeePerformanceList = new List<EmployeePerfromanceVM>();

            if (string.IsNullOrEmpty(year))
            {
                return employeePerformanceList;
            }

            int.TryParse(year, out int yearId);

            IEnumerable<PerformanceEntity> filtered = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == yearId && x.IsSecondLevelApproved)
                .ToList();

            if (id != null)
            {
                int.TryParse(id, out int orgStructureId);
                filtered = filtered.Where(x => x.OrgStructureId == orgStructureId);
            }
            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                filtered = filtered.Where(x => x.EmployeeId == employeeId);
            }
            if (businessUnit != null)
            {
                int.TryParse(businessUnit, out int businessUnitId);
                filtered = filtered.Where(x => x.BusinessUnitId == businessUnitId);
            }

            var employeePerformances = filtered.ToList();
            var uniqEmpPerformances = employeePerformances.DistinctByField(x => x.EmployeeId).ToList();
            if (!uniqEmpPerformances.Any())
            {
                return employeePerformanceList;
            }

            var employeeIds = uniqEmpPerformances.Select(x => x.EmployeeId).ToList();
            var orgIds = uniqEmpPerformances.Select(x => x.OrgStructureId).Distinct().ToList();
            var branchIds = uniqEmpPerformances.Select(x => x.BranchId).Distinct().ToList();
            var periodIds = uniqEmpPerformances.Select(x => x.GlPeriodId).Distinct().ToList();
            var criteriaIds = employeePerformances.Select(x => x.EvaluationCriteriaTypeId).Distinct().ToList();

            var persons = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToDictionary(x => x.OrgStructureId, x => x.Name);
            var branches = db.Branch.Where(x => branchIds.Contains(x.BranchId)).ToDictionary(x => x.BranchId, x => x.Name);
            var periodNames = db.Quarters.Where(x => periodIds.Contains(x.QuarterId)).ToDictionary(x => x.QuarterId, x => x.Name);
            var criteriaLookup = db.MeasurementDefinitions.Where(x => criteriaIds.Contains(x.MeasurementDefinitionId)).ToDictionary(x => x.MeasurementDefinitionId);
            var totals = PerformanceReportLookup.LoadTotals(db, employeePerformances.Select(x => x.EmployeePerformanceId).ToList());
            var performancesByEmployee = employeePerformances.ToLookup(x => x.EmployeeId);

            foreach (var place in uniqEmpPerformances)
            {
                var empPerformance = new EmployeePerfromanceVM();
                if (persons.TryGetValue(place.EmployeeId, out var emp))
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = emp.FullNameaEnglish;
                    empPerformance.Sex = emp.Gender.ToString();
                }
                if (departments.TryGetValue(place.OrgStructureId, out var departmentName))
                {
                    empPerformance.Department = departmentName;
                }
                if (branches.TryGetValue(place.BranchId, out var branchName))
                {
                    empPerformance.Branch = branchName;
                }

                empPerformance.PerformanceResults = new List<PerformanceResultVM>();
                periodNames.TryGetValue(place.GlPeriodId, out var periodName);

                foreach (var quar in performancesByEmployee[place.EmployeeId])
                {
                    if (!criteriaLookup.TryGetValue(quar.EvaluationCriteriaTypeId, out var criteria))
                    {
                        continue;
                    }

                    var quarter = new PerformanceResultVM();
                    if (periodName != null)
                    {
                        quarter.Period = periodName;
                    }
                    quarter.Criteria = criteria.EvaluationCriteria + "-" + criteria.EvalutionType.ToString() + "-" + criteria.Role.ToString();
                    if (totals.TryGetValue(quar.EmployeePerformanceId, out var total))
                    {
                        quarter.Score = total.Score;
                        quarter.Weight = total.Weight;
                    }
                    empPerformance.PerformanceResults.Add(quarter);
                }

                employeePerformanceList.Add(empPerformance);
            }

            return employeePerformanceList;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
