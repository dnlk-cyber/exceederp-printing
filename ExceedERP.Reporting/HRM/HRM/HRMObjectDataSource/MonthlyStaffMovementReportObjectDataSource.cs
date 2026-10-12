using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class MonthlyStaffMovementReportObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        private class EmployeeBranchSegment
        {
            public int EmployeeId { get; set; }
            public int BranchId { get; set; }
            public DateTime From { get; set; }
            // null means the employee is still assigned to this branch (no transfer/termination yet)
            public DateTime? To { get; set; }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<MonthlyStaffMovementSummaryVM> GetMonthlyStaffMovementSummary(object[] branchId, string glFiscalYearId, object[] glPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            int.TryParse(glFiscalYearId, out int fiscalYearId);

            HashSet<int> periodIds = new HashSet<int>();
            if (glPeriodId != null && glPeriodId.Length > 0)
            {
                foreach (var p in glPeriodId)
                {
                    if (p != null && int.TryParse(p.ToString(), out int pId))
                    {
                        periodIds.Add(pId);
                    }
                }
            }

            var periodsQuery = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId);
            if (periodIds.Count > 0)
            {
                periodsQuery = periodsQuery.Where(x => periodIds.Contains(x.GLPeriodId));
            }
            var selectedPeriods = periodsQuery.OrderBy(x => x.Index).ToList();

            DateTime periodStart, periodEnd;
            if (selectedPeriods.Any())
            {
                periodStart = selectedPeriods.Min(x => x.DateFrom).Date;
                periodEnd = selectedPeriods.Max(x => x.DateTo).Date;
            }
            else
            {
                // Fallback when no fiscal year/period is selected yet
                periodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                periodEnd = DateTime.Today;
            }

            // An explicit date range narrows/overrides the selected period(s), for a specific-date or custom-range view
            if (fromDate.HasValue)
            {
                periodStart = fromDate.Value.Date;
            }
            if (toDate.HasValue)
            {
                periodEnd = toDate.Value.Date;
            }

            DateTime dayBeforeStart = periodStart.AddDays(-1);

            HashSet<int> branchIds = new HashSet<int>();
            if (branchId != null && branchId.Length > 0)
            {
                foreach (var b in branchId)
                {
                    if (b != null && int.TryParse(b.ToString(), out int bId))
                    {
                        branchIds.Add(bId);
                    }
                }
            }

            var branches = db.Branch.AsQueryable();
            if (branchIds.Count > 0)
            {
                branches = branches.Where(x => branchIds.Contains(x.BranchId));
            }
            var branchList = branches.ToList();

            // Latest approved termination date per employee, used to close an otherwise open-ended (current) placement segment
            var terminationByEmployee = db.Terminations
                .Where(x => x.IsOnlineApproved && x.TerminationDate.HasValue)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.TerminationDate).First().TerminationDate.Value);

            // Closed (historical) assignment periods per branch
            var historySegments = db.PlacementHistories
                //.Where(x => x.BranchId.HasValue)
                .Select(x => new { x.EmployeeId, BranchId = x.BranchId.Value, x.From, x.To })
                .ToList()
                .Select(x => new EmployeeBranchSegment { EmployeeId = x.EmployeeId, BranchId = x.BranchId, From = x.From, To = x.To });

            // Currently active assignment per branch; closed off by the employee's termination date, if any
            var currentSegments = db.Placements
                .Where(x => x.isCurrent)
                .Select(x => new { x.EmployeeId, x.BranchId, From = x.AssignmentDate })
                .ToList()
                .Select(x => new EmployeeBranchSegment
                {
                    EmployeeId = x.EmployeeId,
                    BranchId = x.BranchId,
                    From = x.From,
                    To = terminationByEmployee.TryGetValue(x.EmployeeId, out var termDate) ? (DateTime?)termDate : null
                });

            var allSegments = historySegments.Concat(currentSegments).ToList();

            var genderByEmployee = db.Person_Employee
                .Select(x => new { x.EmployeeId, x.Gender })
                .ToList()
                .ToDictionary(x => x.EmployeeId, x => x.Gender);

            var result = new List<MonthlyStaffMovementSummaryVM>();

            foreach (var branch in branchList)
            {
                var branchSegments = allSegments.Where(s => s.BranchId == branch.BranchId).ToList();

                var startIds = branchSegments
                    .Where(s => s.From <= dayBeforeStart && (!s.To.HasValue || s.To.Value >= dayBeforeStart))
                    .Select(s => s.EmployeeId).Distinct().ToList();

                var endIds = branchSegments
                    .Where(s => s.From <= periodEnd && (!s.To.HasValue || s.To.Value >= periodEnd))
                    .Select(s => s.EmployeeId).Distinct().ToList();

                var newIds = branchSegments
                    .Where(s => s.From >= periodStart && s.From <= periodEnd)
                    .Select(s => s.EmployeeId).Distinct().ToList();

                var separatedIds = branchSegments
                    .Where(s => s.To.HasValue && s.To.Value >= periodStart && s.To.Value <= periodEnd)
                    .Select(s => s.EmployeeId).Distinct().ToList();

                int CountMale(List<int> ids) => ids.Count(id => genderByEmployee.TryGetValue(id, out var g) && g == Gender.MALE);
                int CountFemale(List<int> ids) => ids.Count(id => genderByEmployee.TryGetValue(id, out var g) && g == Gender.FEMALE);

                result.Add(new MonthlyStaffMovementSummaryVM
                {
                    ProjectName = branch.Name,
                    StartMale = CountMale(startIds),
                    StartFemale = CountFemale(startIds),
                    NewMale = CountMale(newIds),
                    NewFemale = CountFemale(newIds),
                    SeparatedMale = CountMale(separatedIds),
                    SeparatedFemale = CountFemale(separatedIds),
                    EndMale = CountMale(endIds),
                    EndFemale = CountFemale(endIds)
                });
            }

            return result.OrderBy(x => x.ProjectName).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
