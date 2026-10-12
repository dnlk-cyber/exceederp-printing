using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;
using ExceedERP.Helpers.Common;
using PerformanceEntity = ExceedERP.Core.Domain.HRM.PerformanceManagment.EmployeePerformance;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerforObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId, string year, string quarterId, string status, string empId,string businessUnit)
        {
            var employeePerformanceList = new List<EmployeePerfromanceVM>();

            if (string.IsNullOrEmpty(year))
            {
                return employeePerformanceList;
            }

            int.TryParse(year, out int yearId);

            var approved = db.EmployeePerformances.Where(x => x.IsHRApproved);
            var fiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId).ToList();
            if (fiscalYearIds.Any())
            {
                approved = approved.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));
            }

            IEnumerable<PerformanceEntity> filtered = approved.ToList();
            if (!string.IsNullOrEmpty(quarterId))
            {
                int.TryParse(quarterId, out int quartId);
                filtered = filtered.Where(x => x.GlPeriodId == quartId);
            }
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                filtered = filtered.Where(x => x.BranchId == braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                filtered = filtered.Where(x => x.OrgStructureId == orgId);
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
            if (!string.IsNullOrEmpty(status))
            {
                filtered = filtered.Where(x => x.ResponsibilityLevel.ToString() == status);
            }

            var empPerformances = filtered.ToList();
            var uniqueEmployeePerformances = empPerformances.DistinctByField(x => x.EmployeeId).ToList();
            if (!uniqueEmployeePerformances.Any())
            {
                return employeePerformanceList;
            }

            var employeeIds = uniqueEmployeePerformances.Select(x => x.EmployeeId).ToList();
            var orgIds = uniqueEmployeePerformances.Select(x => x.OrgStructureId).Distinct().ToList();
            var branchIds = uniqueEmployeePerformances.Select(x => x.BranchId).Distinct().ToList();
            var criteriaIds = empPerformances.Select(x => x.EvaluationCriteriaTypeId).Distinct().ToList();
            var periodIds = empPerformances.Select(x => x.GlPeriodId).Distinct().ToList();

            var persons = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToDictionary(x => x.OrgStructureId, x => x.Name);
            var branches = db.Branch.Where(x => branchIds.Contains(x.BranchId)).ToDictionary(x => x.BranchId, x => x.Name);
            var criteriaLookup = db.MeasurementDefinitions.Where(x => criteriaIds.Contains(x.MeasurementDefinitionId)).ToDictionary(x => x.MeasurementDefinitionId);
            var quarters = db.Quarters.Where(x => periodIds.Contains(x.QuarterId)).ToDictionary(x => x.QuarterId, x => x.Name);
            var totals = PerformanceReportLookup.LoadTotals(db, empPerformances.Select(x => x.EmployeePerformanceId).ToList());
            var performancesByEmployee = empPerformances.ToLookup(x => x.EmployeeId);

            foreach (var place in uniqueEmployeePerformances)
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
                foreach (var performance in performancesByEmployee[place.EmployeeId].DistinctByField(x => x.EvaluationCriteriaTypeId))
                {
                    if (!criteriaLookup.TryGetValue(performance.EvaluationCriteriaTypeId, out var criteria))
                    {
                        continue;
                    }

                    var quarter = new PerformanceResultVM();
                    if (quarters.TryGetValue(performance.GlPeriodId, out var quarterName))
                    {
                        quarter.Period = quarterName;
                    }
                    quarter.Criteria = criteria.EvaluationCriteria + "-" + criteria.Role.ToString();
                    if (totals.TryGetValue(performance.EmployeePerformanceId, out var total))
                    {
                        quarter.Weight = total.Weight;
                        quarter.Score = total.Score;
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
