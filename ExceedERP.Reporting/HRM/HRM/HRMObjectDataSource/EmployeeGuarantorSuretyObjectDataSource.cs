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
    public class EmployeeGuarantorSuretyObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeGuarantorOrSuretyVM> GetEmployeeCosign(string branchId, string orgStructureId)
        {
            var placements = db.Placements.ToList();
            var employeeGuarantors = new List<EmployeeGuarantorOrSuretyVM>();

            //var guarantorOrSureties = db.GuarantorOrSureties.ToList();
            //var guarantorOrSuretyDetails = db.GuarantorOrSuretyDetails.ToList();

            //HashSet<int> employeeIds = new HashSet<int>();

            //if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            //{
            //    placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            //    employeeIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
            //    guarantorOrSureties = guarantorOrSureties.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            //}

            //if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            //{
            //    placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            //    employeeIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
            //    guarantorOrSureties = guarantorOrSureties.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            //}

            //foreach (var guarantor in guarantorOrSureties)
            //{
            //    var person = db.Person_Employee.Find(guarantor.EmployeeId);
            //    if (person != null)
            //    {
            //        var employeeGuarantor = new EmployeeGuarantorOrSuretyVM
            //        {
            //            EmployeeCode = person.EmployeeCode,
            //            EmployeeFullName = $"{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}",
            //            Remark = guarantor.Remark,
            //            Details = db.GuarantorOrSuretyDetails
            //                        .Where(d => d.GuarantorOrSuretyId == guarantor.GuarantorOrSuretyId)
            //                        .Select(d => new GuarantorOrSuretyDetailVM
            //                        {
            //                            FullName = d.FullName,
            //                            Organiztion = d.Organiztion,
            //                            FromDate = d.FromDate,
            //                            FromDateAm = d.FromDateAm,
            //                            ToDate = d.ToDate.Value,
            //                            ToDateAm = d.ToDateAm,
            //                        }).ToList()
            //        };

            //        employeeGuarantors.Add(employeeGuarantor);
            //    }
            //}

            return employeeGuarantors;

        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}