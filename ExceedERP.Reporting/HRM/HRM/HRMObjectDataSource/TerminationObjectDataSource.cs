using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TerminationObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetTerminationSummary(DateTime? from, DateTime? to, string branchId, string orgStructureId)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            var employee = new EmployeeSummary();

            var terminations = db.Terminations.Where(x => x.IsOnlineApproved).ToList();

            var placements = db.Placements.ToList();
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);

                var empIds = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId).Select(x => x.EmployeeId);
                terminations = terminations.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var empIds = placements
                    .Where(x => x.OrgStructureId == orgId)
                    .Select(x => x.EmployeeId)
                    .Distinct();

                terminations = terminations.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if (from.HasValue)
            {
                terminations = terminations.Where(x => x.TerminationDate.Value.Date >= from.Value.Date ).ToList();
            }
            if (to.HasValue)
            {
                terminations = terminations.Where(x => x.TerminationDate.Value.Date <= to.Value.Date).ToList();
            }

            var males = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId);
            var females = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId);

            employee.NumberOfMaleEmployee = terminations.Where(x=>males.Contains(x.EmployeeId)).Count();
            employee.NumberOfFemaleEmployee = terminations.Where(x => females.Contains(x.EmployeeId)).Count();

            employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

            list.Add(employee);
            
            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}