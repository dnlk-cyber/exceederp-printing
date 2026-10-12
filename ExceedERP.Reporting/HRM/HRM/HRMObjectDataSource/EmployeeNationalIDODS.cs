using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class EmployeeNationalIDODS : IDisposable
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeNationalIDVM> GetEmployeeNationalIds(string branchId, string businessUnit)
        {
            var placements = db.Placements.Where(p => p.isCurrent);

            if (int.TryParse(branchId, out int bId))
                placements = placements.Where(p => p.BranchId == bId);

            if (int.TryParse(businessUnit, out int buId) && buId != 0)
                placements = placements.Where(p => p.BusinessUnitId == buId);

            var placementList = placements.ToList();

            var employeeIds = placementList.Select(p => p.EmployeeId).Distinct().ToList();
            var employeeIdcs = placementList.Select(p => p.EmployeeId.ToString()).Distinct().ToList();

            var employees = db.Person_Employee
                .Where(e => e.IsActive && employeeIds.Contains(e.EmployeeId))
                .ToDictionary(e => e.EmployeeId);

            var branches = db.Branch
                .Where(b => b.IsActive)
                .ToDictionary(b => b.BranchId);

            var identifications = db.Identifications
                .Where(i => i.ReferenceType== "Employee" && employeeIdcs.Contains(i.Reference))
                .ToList();

            var result = new List<EmployeeNationalIDVM>();

            foreach (var placement in placementList)
            {
                employees.TryGetValue(placement.EmployeeId, out var emp);
                branches.TryGetValue(placement.BranchId, out var branch);

                if (emp == null) continue;

                var pension = identifications.FirstOrDefault(i =>
                    i.Reference == placement.EmployeeId.ToString() &&
                    i.Type == Core.Domain.Common.IdentificationType.PENSION_NO);

                var fayda = identifications.FirstOrDefault(i =>
                    i.Reference == placement.EmployeeId.ToString() &&
                    i.Type == Core.Domain.Common.IdentificationType.Fayda_ID_Or_National_ID);

                result.Add(new EmployeeNationalIDVM
                {
                    EmployeeFullName = $"{emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}",
                    ProjectName = branch?.Name,
                    BenefitorFaydaNumber = fayda?.IdNumber,
                    SocialSecurityNumberOrPensionId = pension?.IdNumber,
                    EmployeeCode = emp.EmployeeCode ?? ""
                });
            }

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
