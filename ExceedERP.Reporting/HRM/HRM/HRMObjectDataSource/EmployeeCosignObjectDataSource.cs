using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeCosignObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeCosignVM> GetEmployeeCosign(string branchId, string orgStructureId)
        {
            var placements = db.Placements.ToList();
            var cosigns = db.Cosigns.ToList();

            HashSet<int> employeeIds = new HashSet<int>();

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
                employeeIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
                cosigns = cosigns.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                employeeIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
                cosigns = cosigns.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            var employeeCosignVMs = new List<EmployeeCosignVM>();
            foreach (var cosign in cosigns)
            {
                var person = db.Person_Employee.Find(cosign.EmployeeId);
                var lookup = db.Lookups.Find(cosign.LookupId);
                if (person != null)
                {
                    employeeCosignVMs.Add(new EmployeeCosignVM
                    {
                        EmployeeCode = person.EmployeeCode,
                        EmployeeFullName = $"{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}",
                        CosignerFullName = cosign.FullName,
                        AgreementDate = cosign.AgreementDate,
                        AgreementDateAm = cosign.AgreementDateAm,
                        Occupation = cosign.Occupation,
                        Workorganization = cosign.Workorganization,
                        CosignTypeName = cosign.CosignType?.Description ?? lookup?.Description,
                        Remark = cosign.Remark,
                        Salary = cosign.Salary
                    });
                }
            }

            return employeeCosignVMs;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}