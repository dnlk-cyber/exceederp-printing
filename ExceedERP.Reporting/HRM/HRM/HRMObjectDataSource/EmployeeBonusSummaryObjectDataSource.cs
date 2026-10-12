using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBonusSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBonusSummaryVM> GetEmployeeBonusSummary(string empId, string branchId, string orgStructureId, string year,string quarterId, string status,string BusinessUnit,object[] jobTitles, string rankName)
        {
            var employeePerformanceList = new List<EmployeeBonusSummaryVM>();

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
            var allApproved = approved.ToList();

            IEnumerable<ExceedERP.Core.Domain.HRM.PerformanceManagment.EmployeePerformance> filtered = allApproved;
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                filtered = filtered.Where(x => x.EmployeeId == employeeId);
            }
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
            if (jobTitles != null)
            {
                var jIds = jobTitles.Select(pp => Convert.ToInt32(pp)).ToList();
                if (jIds.Any())
                    filtered = filtered.Where(x => jIds.Contains(x.JobTitleId));
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

            // Preload everything the loop needs instead of querying per employee/period
            var employeeIds = uniqueEmployeePerformances.Select(x => x.EmployeeId).ToList();
            var orgIds = uniqueEmployeePerformances.Select(x => x.OrgStructureId).Distinct().ToList();
            var titleIds = uniqueEmployeePerformances.Select(x => x.JobTitleId).Distinct().ToList();
            var periodIds = empPerformances.Select(x => x.GlPeriodId).Distinct().ToList();

            var persons = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToDictionary(x => x.OrgStructureId, x => x.Name);
            var titles = db.Lookups.Where(x => titleIds.Contains(x.LookupId)).ToDictionary(x => x.LookupId, x => x.Description);
            var fiscalYears = db.GLFiscalYears.ToDictionary(x => x.GlFiscalYearId);
            var quarters = db.Quarters.Where(x => periodIds.Contains(x.QuarterId)).ToDictionary(x => x.QuarterId, x => x.Name);

            var corporateCriteria = PerformanceReportLookup.GetCriteriaIds(db, EvaluationType.Corporate_Result);
            var sectorCriteria = PerformanceReportLookup.GetCriteriaIds(db, EvaluationType.Sector_Result);
            var directCriteria = PerformanceReportLookup.GetCriteriaIds(db, EvaluationType.DirectBoss);

            var employeeIdSet = new HashSet<int>(employeeIds);
            var approvedByEmployee = allApproved.Where(x => employeeIdSet.Contains(x.EmployeeId)).ToLookup(x => x.EmployeeId);
            var totals = PerformanceReportLookup.LoadTotals(db, approvedByEmployee.SelectMany(g => g).Select(x => x.EmployeePerformanceId).ToList());
            var quartersByEmployee = empPerformances.ToLookup(x => x.EmployeeId);
            var ranks = PerformanceReportLookup.LoadRanks(db);

            foreach (var place in uniqueEmployeePerformances)
            {
                var employeePerformance = new EmployeeBonusSummaryVM();

                persons.TryGetValue(place.EmployeeId, out var person);
                if (person != null)
                {
                    employeePerformance.EmployeeId = person.EmployeeCode;
                    employeePerformance.FullName = person.EmployeeCode + "-" + person.FullNameaEnglish;
                }
                if (departments.TryGetValue(place.OrgStructureId, out var departmentName))
                {
                    employeePerformance.Departement = departmentName;
                }
                if (titles.TryGetValue(place.JobTitleId, out var titleName))
                {
                    employeePerformance.JobTitle = titleName;
                }
                fiscalYears.TryGetValue(place.GlFiscalYearId, out var fiscalyear);
                if (fiscalyear != null)
                {
                    employeePerformance.DatesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo - fiscalyear.DateFrom).TotalDays;
                }

                var employeeApproved = approvedByEmployee[place.EmployeeId].ToList();
                var evaluationQuarters = quartersByEmployee[place.EmployeeId]
                    .DistinctByField(x => x.GlPeriodId)
                    .OrderBy(x => x.GlPeriodId);

                employeePerformance.EvaluationPeiordBonusSummaries = new List<EvaluationPeiordBonusSummaryDetailsVM>();
                foreach (var quart in evaluationQuarters)
                {
                    var evaluationPeiord = new EvaluationPeiordBonusSummaryDetailsVM();
                    if (quarters.TryGetValue(quart.GlPeriodId, out var quarterName))
                    {
                        evaluationPeiord.Period = quarterName;
                    }

                    var periodPerfs = employeeApproved
                        .Where(x => x.GlPeriodId == quart.GlPeriodId && x.GlFiscalYearId == quart.GlFiscalYearId)
                        .ToList();

                    var corp = PerformanceReportLookup.Sum(totals, periodPerfs.Where(x => corporateCriteria.Contains(x.EvaluationCriteriaTypeId)).Select(x => x.EmployeePerformanceId));
                    var sector = PerformanceReportLookup.Sum(totals, periodPerfs.Where(x => sectorCriteria.Contains(x.EvaluationCriteriaTypeId)).Select(x => x.EmployeePerformanceId));
                    var direct = PerformanceReportLookup.Sum(totals, periodPerfs.Where(x => directCriteria.Contains(x.EvaluationCriteriaTypeId)).Select(x => x.EmployeePerformanceId));

                    evaluationPeiord.AverageCorpBScOutOf10 = corp.Weight != 0 ? (corp.Score * 10) / corp.Weight : 0;
                    evaluationPeiord.AverageSectorBScOutOf20 = sector.Weight != 0 ? sector.Score * 20 / sector.Weight : 0;
                    evaluationPeiord.EvaluationResultOf70 = direct.Weight != 0 ? direct.Score * 70 / direct.Weight : 0;

                    employeePerformance.EvaluationPeiordBonusSummaries.Add(evaluationPeiord);
                }

                if (employeePerformance.EvaluationPeiordBonusSummaries.Any())
                {
                    employeePerformance.AverageCorpBScOutOf10 = employeePerformance.EvaluationPeiordBonusSummaries
                        .Where(x => x.AverageCorpBScOutOf10 != 0)
                        .Select(x => x.AverageCorpBScOutOf10)
                        .DefaultIfEmpty(0)
                        .Average();

                    employeePerformance.AverageSectorBScOutOf20 = employeePerformance.EvaluationPeiordBonusSummaries
                        .Where(x => x.AverageSectorBScOutOf20 != 0)
                        .Select(x => x.AverageSectorBScOutOf20)
                        .DefaultIfEmpty(0)
                        .Average();

                    employeePerformance.AverageOf70 = employeePerformance.EvaluationPeiordBonusSummaries
                        .Where(x => x.EvaluationResultOf70 != 0)
                        .Select(x => x.EvaluationResultOf70)
                        .DefaultIfEmpty(0)
                        .Average();
                }

                employeePerformance.TotalScoreOutOf100 = employeePerformance.AverageCorpBScOutOf10 + employeePerformance.AverageSectorBScOutOf20 + employeePerformance.AverageOf70;
                if (fiscalyear != null && person != null)
                {
                    DateTime startDate = fiscalyear.DateFrom < person.HiredDate ? person.HiredDate : fiscalyear.DateFrom;
                    employeePerformance.DatesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo - startDate).TotalDays;
                    if (employeePerformance.DatesServedInThisFiscalYear >= 311 && employeePerformance.DatesServedInThisFiscalYear <= 315)
                    {
                        employeePerformance.DatesServedInThisFiscalYear -= (int)(employeePerformance.DatesServedInThisFiscalYear - 311);
                    }
                    else if (employeePerformance.DatesServedInThisFiscalYear > 315)
                    {
                        employeePerformance.DatesServedInThisFiscalYear -= 4;
                    }
                }
                if (employeePerformance.DatesServedInThisFiscalYear > 360)
                {
                    employeePerformance.DatesServedInThisFiscalYear = 360;
                }
                employeePerformance.Bonus = PerformanceReportLookup.CalculateBonus(ranks, employeePerformance.DatesServedInThisFiscalYear, employeePerformance.TotalScoreOutOf100);
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
