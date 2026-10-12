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
    public class EmployeePerfromanceLeaveDispilineBonusSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceLeaveDispilineBonusSummaryVM> GetEmployeeBSCPerformance(string branchId, string orgStructureId, string year,string quarterId, string status,string BusinessUnit,string rankName)
        {
            var employeePerformanceList = new List<EmployeePerfromanceLeaveDispilineBonusSummaryVM>();

            if (string.IsNullOrEmpty(year))
            {
                return employeePerformanceList;
            }

            int.TryParse(year, out int yearId);

            var approved = db.EmployeePerformances.Where(x => x.IsSecondLevelApproved);
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
            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
                filtered = filtered.Where(x => x.BranchId == braId);
            }
            if (!string.IsNullOrEmpty(orgStructureId))
            {
                int.TryParse(orgStructureId, out int orgId);
                filtered = filtered.Where(x => x.OrgStructureId == orgId);
            }
            if (!string.IsNullOrEmpty(BusinessUnit))
            {
                int.TryParse(BusinessUnit, out int businessUnitId);
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
            var titleIds = uniqueEmployeePerformances.Select(x => x.JobTitleId).Distinct().ToList();
            var criteriaIds = empPerformances.Select(x => x.EvaluationCriteriaTypeId).Distinct().ToList();
            var periodIds = empPerformances.Select(x => x.GlPeriodId).Distinct().ToList();
            var currentYear = DateTime.Now.Year;

            var persons = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToDictionary(x => x.OrgStructureId, x => x.Name);
            var titles = db.Lookups.Where(x => titleIds.Contains(x.LookupId)).ToDictionary(x => x.LookupId, x => x.Description);
            var fiscalYears = db.GLFiscalYears.ToDictionary(x => x.GlFiscalYearId);
            var quarters = db.Quarters.Where(x => periodIds.Contains(x.QuarterId)).ToDictionary(x => x.QuarterId, x => x.Name);
            var criteriaLookup = db.MeasurementDefinitions.Where(x => criteriaIds.Contains(x.MeasurementDefinitionId)).ToDictionary(x => x.MeasurementDefinitionId);
            var leavesByEmployeeYear = db.Leaves
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Select(x => new { x.EmployeeId, x.GlFiscalYearId, x.Amount })
                .ToList()
                .ToLookup(x => new { x.EmployeeId, x.GlFiscalYearId });
            var disciplineCounts = db.DisciplinaryCases
                .Where(x => employeeIds.Contains(x.EmployeeId) && x.TransgressionDate.Year == currentYear)
                .Select(x => x.EmployeeId)
                .ToList()
                .GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());
            var totals = PerformanceReportLookup.LoadTotals(db, empPerformances.Select(x => x.EmployeePerformanceId).ToList());
            var ranks = PerformanceReportLookup.LoadRanks(db);
            var performancesByEmployee = empPerformances.ToLookup(x => x.EmployeeId);

            foreach (var place in uniqueEmployeePerformances)
            {
                var employeePerformance = new EmployeePerfromanceLeaveDispilineBonusSummaryVM();

                if (persons.TryGetValue(place.EmployeeId, out var person))
                {
                    employeePerformance.EmployeeId = person.EmployeeCode;

                    employeePerformance.Sex = person.Gender.ToString().ToLowerInvariant();
                    employeePerformance.FullName = person.EmployeeCode+"-"
                                                  +person.FirstNameEnglish+" "
                                                  +person.MiddleNameEnglish+" "
                                                  +person.LastNameEnglish;
                    fiscalYears.TryGetValue(place.GlFiscalYearId, out var period1);
                    DateTime periodFromDate = (period1 != null && person.HiredDate > period1.DateFrom) ? person.HiredDate : (period1 != null && person.HiredDate < period1.DateFrom) ? period1.DateFrom : DateTime.Now;
                    DateTime periodToDate = (period1 != null && DateTime.Now > period1.DateTo) ? period1.DateTo : (period1 != null && DateTime.Now < period1.DateTo) ? DateTime.Now : DateTime.Now;
                    employeePerformance.NumberOfMonthsServed = (periodToDate.Year - periodFromDate.Year) * 12 + periodToDate.Month - periodFromDate.Month;
                    employeePerformance.NumberOfDayssServed = (decimal)((periodToDate - periodFromDate).TotalDays);
                }

                if (departments.TryGetValue(place.OrgStructureId, out var departmentName))
                {
                    employeePerformance.Departement = departmentName;
                }
                if (titles.TryGetValue(place.JobTitleId, out var titleName))
                {
                    employeePerformance.JobTitle = titleName;
                }

                var leaves = leavesByEmployeeYear[new { place.EmployeeId, place.GlFiscalYearId }].ToList();
                employeePerformance.LeaveTaken = leaves.Any() ? leaves.Sum(x => x.Amount) : 0;
                employeePerformance.DisplineDays = disciplineCounts.TryGetValue(place.EmployeeId, out var disciplineCount) ? disciplineCount : 0;

                employeePerformance.Bonus = 0;
                employeePerformance.Level = "";
                employeePerformance.Remark = place.Remark;

                employeePerformance.EvaluationPeiordList = new List<EvaluationLeaveDispilineBonusPeiordVM>();
                var employeePerfs = performancesByEmployee[place.EmployeeId].ToList();

                foreach (var quart in employeePerfs.DistinctByField(x => x.GlPeriodId))
                {
                    var evaluationPeiord = new EvaluationLeaveDispilineBonusPeiordVM();
                    if (quarters.TryGetValue(quart.GlPeriodId, out var quarterName))
                    {
                        evaluationPeiord.Period = quarterName;
                    }
                    evaluationPeiord.EvaluationCriteriaList = new List<EvaluationLeaveDispilineCriteriaVM>();

                    foreach (var employeePerf in employeePerfs.Where(x => x.GlPeriodId == quart.GlPeriodId))
                    {
                        if (!criteriaLookup.TryGetValue(employeePerf.EvaluationCriteriaTypeId, out var criteria))
                        {
                            continue;
                        }

                        var criterias = new EvaluationLeaveDispilineCriteriaVM
                        {
                            Measurment = criteria.EvaluationCriteria + "-" + criteria.Role.ToString()
                        };
                        if (totals.TryGetValue(employeePerf.EmployeePerformanceId, out var total))
                        {
                            criterias.Weight = total.Weight;
                            criterias.Score = total.Score;
                        }
                        evaluationPeiord.EvaluationCriteriaList.Add(criterias);
                    }
                    if (evaluationPeiord.EvaluationCriteriaList.Any())
                    {
                        evaluationPeiord.CriteriaTotalScore = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Score);
                        evaluationPeiord.CriteriaTotalWeight = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Weight);
                    }

                    employeePerformance.EvaluationPeiordList.Add(evaluationPeiord);
                }

                if (employeePerformance.EvaluationPeiordList.Any())
                {
                    employeePerformance.TotalScoreInThePeriod = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                    employeePerformance.Total = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                    employeePerformance.TotalWeight = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalWeight);
                    employeePerformance.Percent = employeePerformance.TotalWeight>0? (employeePerformance.Total * 100) / employeePerformance.TotalWeight:0;
                    var rank = ranks.FirstOrDefault(x => x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);
                    if (rank != null)
                    {
                        employeePerformance.Rank = rank.Description;
                    }
                }

                employeePerformanceList.Add(employeePerformance);
            }

            if (!string.IsNullOrEmpty(rankName))
            {
                employeePerformanceList = employeePerformanceList.Where(x => x.Rank == rankName).ToList();
            }
            return employeePerformanceList;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
