using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class SeverancePayableObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public SeverancePayableVM GetLeavePayable(string id, string paidStatus, string branch, string workunit,string empId)
        {
            SeverancePayableVM result = new SeverancePayableVM();

            if (!int.TryParse(id, out int payableId))
                return result;

            var payable = db.SeverancePayables.FirstOrDefault(x=>x.SeverancePayableId == payableId);
            if (payable == null)
                return result;

            var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
            result.FiscalYear = fiscalYear?.Name;

            var detailsQuery = db.SeverancePayableDetails
                .Where(x => x.SeverancePayableId == payableId)
                .DistinctByField(x => x.EmployeeId)
                .AsQueryable();

            if (int.TryParse(branch, out int branchId))
                detailsQuery = detailsQuery.Where(x => x.BranchId == branchId);

            if (int.TryParse(workunit, out int orgId))
                detailsQuery = detailsQuery.Where(x => x.OrgStructureId == orgId);

            if (int.TryParse(empId, out int employeeId))
                detailsQuery = detailsQuery.Where(x => x.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(paidStatus))
            {
                if (paidStatus == "Paid")
                    detailsQuery = detailsQuery.Where(x => x.IsSendForPayment);
                else if (paidStatus == "NotPaid")
                    detailsQuery = detailsQuery.Where(x => !x.IsSendForPayment);
            }

            var detailList = detailsQuery.ToList();

            result.SeverancePayablesDetails = detailList.Select(detail =>
            {
                var emp = db.Person_Employee.Find(detail.EmployeeId);

                string fullName = emp != null? $"{emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}": string.Empty;

                string empCode = emp != null? $"{emp.EmployeeCode}": string.Empty;

                var orgStructure = db.MainCompanyStructures.Find(detail.OrgStructureId);
                var Branch = db.Branch.Find(detail.BranchId);
               

                return new SeverancePayableDetailVM
                {
                    EmployeeCode = empCode,
                    EmployeeFullName = fullName,
                    Branch = Branch?.Name??string.Empty,
                    WorkUnit = orgStructure?.Name ?? string.Empty,
                    Basicalary = detail.BasicSalary,
                    NomberOfYears = detail.NumberOFYears,
                    GrossComposition = detail.GrossComponsation,
                    GrossCompensationPayment = detail.GrossCompensationPayment,
                    IncomeTax = detail.TaxOnBasicSalary,
                    AccTaxDifference = detail.AccTaxDifference,
                    TaxOnMonthlySalaryAndCompensation = detail.TaxOnMonthlySalaryAndCompensation,
                    TaxDifference = detail.TaxDifference,
                    NetPay = detail.NetPay
                };
            }).ToList();

            return result;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPaidStatus()
        {
            List<string> list = new List<string>();
            list.Add("Paid");
            list.Add("NotPaid");
            return list;
        }

        public void Dispose()
        {
            db.Dispose();
        }

      
    }
}