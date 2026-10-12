using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using ExceedERP.Helpers.Common;
using PerformanceEntity = ExceedERP.Core.Domain.HRM.PerformanceManagment.EmployeePerformance;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeQuarterPerforObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeQuarterPerfromance> GetEmployeeQuarterPerformance(string fiscalYear, string quarterId  , string branchId, string orgStructureId, string businessUnitId,  string status)
        {
            var employeePerformanceList = new List<EmployeeQuarterPerfromance>();

            int.TryParse(fiscalYear, out int yearId);

            var approved = db.EmployeePerformances.Where(x => x.IsSecondLevelApproved);
            var fiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId).ToList();
            if (fiscalYearIds.Any())
            {
                approved = approved.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));
            }

            IEnumerable<PerformanceEntity> filtered = approved.ToList();
            if (quarterId != null)
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
            if (businessUnitId != null)
            {
                int.TryParse(businessUnitId, out int businesUnitId);
                filtered = filtered.Where(x => x.BusinessUnitId == businesUnitId);
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
            var criteriaNames = db.MeasurementDefinitions.Where(x => criteriaIds.Contains(x.MeasurementDefinitionId)).ToDictionary(x => x.MeasurementDefinitionId, x => x.EvaluationCriteria);
            var periodNames = db.Quarters.Where(x => periodIds.Contains(x.QuarterId)).ToDictionary(x => x.QuarterId, x => x.Name);
            var totals = PerformanceReportLookup.LoadTotals(db, empPerformances.Select(x => x.EmployeePerformanceId).ToList());
            var performancesByEmployee = empPerformances.ToLookup(x => x.EmployeeId);

            foreach (var place in uniqueEmployeePerformances)
            {
                var empPerformance = new EmployeeQuarterPerfromance();

                if (persons.TryGetValue(place.EmployeeId, out var emp))
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = $"{emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}";
                    empPerformance.Sex = emp.Gender.ToString().ToLower();
                }
                if (departments.TryGetValue(place.OrgStructureId, out var departmentName))
                {
                    empPerformance.Department = departmentName;
                }
                if (branches.TryGetValue(place.BranchId, out var branchName))
                {
                    empPerformance.Branch = branchName;
                }

                empPerformance.QurterList = new List<QurterList>();

                // Rows grouped by period, in order of first appearance
                foreach (var quar in performancesByEmployee[place.EmployeeId].GroupBy(x => x.GlPeriodId).SelectMany(g => g))
                {
                    var quarter = new QurterList();
                    if (periodNames.TryGetValue(quar.GlPeriodId, out var periodName))
                    {
                        quarter.PeriodName = periodName;
                    }
                    if (criteriaNames.TryGetValue(quar.EvaluationCriteriaTypeId, out var criteriaName))
                    {
                        quarter.Criateria = criteriaName;
                    }
                    if (totals.TryGetValue(quar.EmployeePerformanceId, out var total))
                    {
                        quarter.Weight = total.Weight;
                        quarter.Score = total.Score;
                    }
                    empPerformance.QurterList.Add(quarter);
                }

                if (empPerformance.QurterList.Any())
                {
                    empPerformance.Total = empPerformance.QurterList.Sum(x => x.Score);
                    empPerformance.Totalweight = empPerformance.QurterList.Sum(x => x.Weight);
                    empPerformance.TotaOutOf100 = empPerformance.Totalweight > 0 ? (empPerformance.Total * 100) / empPerformance.Totalweight : 0;
                }
                empPerformance.Role = place.ResponsibilityLevel.ToString();
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
