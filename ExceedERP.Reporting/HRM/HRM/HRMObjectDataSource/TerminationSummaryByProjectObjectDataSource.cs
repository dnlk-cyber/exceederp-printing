using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TerminationSummaryByProjectObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<TerminationSummaryByProjectViewModel> GetTerminationSummaryByProject(object[] branchId, DateTime? fromDate, DateTime? toDate)
        {
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

            var query = from term in _db.Terminations
                        join emp in _db.Person_Employee on term.EmployeeId equals emp.EmployeeId
                        // Changed from .Where(p => p.isCurrent) to allowing historical/placement records for terminated staff
                        join pl in _db.Placements on emp.EmployeeId equals pl.EmployeeId
                        join branch in _db.Branch on pl.BranchId equals branch.BranchId
                        join lookup in _db.Lookups on term.LookupId equals lookup.LookupId into lookupJoin
                        from lk in lookupJoin.DefaultIfEmpty()
                        where term.IsOnlineApproved
                              && (!fromDate.HasValue || term.TerminationDate >= fromDate.Value)
                              && (!toDate.HasValue || term.TerminationDate <= toDate.Value)
                              && (branchIds.Count == 0 || branchIds.Contains(pl.BranchId))
                        select new
                        {
                            term.TerminationId, // Used to ensure unique termination events
                            ProjectName = branch.Name,
                            Reason = lk != null && !string.IsNullOrEmpty(lk.Description)
                                ? lk.Description
                                : (!string.IsNullOrEmpty(term.Reason) ? term.Reason : "Unspecified"),
                            Gender = emp.Gender
                        };

            // Use Distinct() on TerminationId to prevent duplicate rows if multiple placements match an employee
            var rawData = query.Distinct().ToList();

            var summaryList = rawData
                .GroupBy(x => new { x.ProjectName, x.Reason })
                .Select(g => new TerminationSummaryByProjectViewModel
                {
                    ProjectName = g.Key.ProjectName,
                    TerminationReason = g.Key.Reason,
                    MaleCount = g.Count(x => x.Gender == Gender.MALE),
                    FemaleCount = g.Count(x => x.Gender == Gender.FEMALE)
                })
                .OrderBy(x => x.ProjectName)
                .ThenBy(x => x.TerminationReason)
                .ToList();

            return summaryList;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}