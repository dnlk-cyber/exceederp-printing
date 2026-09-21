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
    public class TerminationReasonObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetTerminationReasonSummary(DateTime? from, DateTime? to, string branchId, string orgStructureId)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();

            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.TERMINATIONTYPE).ToList();
            var terminations = db.Terminations.Where(x => x.IsOnlineApproved).AsQueryable();
            var placements = db.Placements.AsQueryable();

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                var empIds = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).Distinct();
                terminations = terminations.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var empIds = placements.Where(x => x.OrgStructureId == orgId).Select(x => x.EmployeeId).Distinct();
                terminations = terminations.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (from.HasValue)
            {
                var fromDate = from.Value.Date;
                terminations = terminations.Where(x => x.TerminationDate >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value.Date;
                terminations = terminations.Where(x => x.TerminationDate <= toDate);
            }

            var terminationList = terminations.ToList();
            var placementList = placements.ToList();

            var males = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placementList).Select(x => x.EmployeeId).ToList();
            var females = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placementList).Select(x => x.EmployeeId).ToList();

            foreach (var type in types)
            {
                var typeTerminations = terminationList.Where(x => x.LookupId == type.LookupId).ToList();

                var maleCount = typeTerminations.Count(x => males.Contains(x.EmployeeId));
                var femaleCount = typeTerminations.Count(x => females.Contains(x.EmployeeId));

                var employee = new EmployeeSummary
                {
                    Criteria = type.Description,
                    NumberOfMaleEmployee = maleCount,
                    NumberOfFemaleEmployee = femaleCount,
                    TotalEmployee = maleCount + femaleCount
                };

                list.Add(employee);
            }

            return list;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}