using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeavePayableObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public LeavePayableVM GetLeavePayable(string id, string branch, string workunit)
        {
            var leavePayableVM = new LeavePayableVM();

            if (!int.TryParse(id, out int leavePayableId))
                return leavePayableVM;

            var payable = db.LeavePayables.Find(leavePayableId);
            if (payable == null)
                return leavePayableVM;

            // Get current fiscal year
            var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
            if (fiscalYear == null)
                return leavePayableVM;

            leavePayableVM.FiscalYear = fiscalYear.Name;

            // Get previous 2 fiscal years
            var previousFiscalYears = db.GLFiscalYears
                .Where(x => x.DateTo < fiscalYear.DateTo)
                .OrderByDescending(x => x.DateTo)
                .Take(2)
                .ToList();

            leavePayableVM.FiscalYear1 = previousFiscalYears.ElementAtOrDefault(0)?.Name ?? string.Empty;
            leavePayableVM.FiscalYear2 = previousFiscalYears.ElementAtOrDefault(1)?.Name ?? string.Empty;

            // Get and filter leave payable details
            var payableDetails = db.LeavePayableDetails
                .Where(x => x.LeavePayableId == leavePayableId)
                .DistinctByField(x => x.EmployeeId)
                .AsQueryable();

            if (int.TryParse(branch, out int branchId))
                payableDetails = payableDetails.Where(x => x.BranchId == branchId);

            if (int.TryParse(workunit, out int orgId))
                payableDetails = payableDetails.Where(x => x.OrgStructureId == orgId);

            var detailList = payableDetails.ToList();
            var employeeIds = detailList.Select(x => x.EmployeeId).Distinct().ToList();
            var orgStructureIds = detailList.Select(x => x.OrgStructureId).Distinct().ToList();
            var branchIds = detailList.Select(x => x.BranchId).Distinct().ToList();

            // Preload related entities to reduce DB calls
            var employees = db.Person_Employee.Where(e => employeeIds.Contains(e.EmployeeId)).ToDictionary(e => e.EmployeeId);
            var orgStructures = db.MainCompanyStructures.Where(o => orgStructureIds.Contains(o.OrgStructureId)).ToDictionary(o => o.OrgStructureId);
            var branches = db.Branch.Where(b => branchIds.Contains(b.BranchId)).ToDictionary(b => b.BranchId);

            leavePayableVM.LeavePayableDetails = detailList.Select(detail =>
            {
                employees.TryGetValue(detail.EmployeeId, out var emp);
                orgStructures.TryGetValue(detail.OrgStructureId, out var workUnit);
                branches.TryGetValue(detail.BranchId, out var branchObj);
                
                return new LeavePayableDetailVM
                {
                    EmployeeCode = emp != null ? $"{emp.EmployeeCode} " : string.Empty,
                    EmployeeFullName = emp != null ? $"{emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}" : string.Empty,
                    WorkUnit = workUnit?.Name ?? string.Empty,
                    Branch = branchObj?.Name ?? string.Empty,
                    BasicSalary = detail.BasicSalary,
                    FiscalYear1 = leavePayableVM.FiscalYear1,
                    FiscalYear2 = leavePayableVM.FiscalYear2,
                    Year1 = detail.Year1,
                    Year2 = detail.Year2,
                    TotalDays = detail.PayableDays,
                    Payable = detail.Balance
                };
            }).ToList();

            return leavePayableVM;
        }

        //public LeavePayableVM GetLeavePayable(string id, string branch, string workunit)
        //{
        //    LeavePayableVM leavepayable = new LeavePayableVM();
        //    int.TryParse(id, out int leavePayableId);
        //    string fiscalYear1 = "";
        //    string fiscalYear2 = "";

        //    var payable = db.LeavePayables.Find(leavePayableId);
        //    if(payable != null)
        //    {
        //        var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
        //        if(fiscalYear != null)
        //        {
        //            leavepayable.FiscalYear = fiscalYear.Name;
        //            var glfiscal = db.GLFiscalYears.Where(x => x.DateTo < fiscalYear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2).ToArray();
        //            int fiscalyear1 = glfiscal[0];
        //            int fiscalyear2 = glfiscal[1];
        //            var year1 = db.GLFiscalYears.Find(fiscalyear1);
        //            if(year1 != null)
        //            {
        //                fiscalYear1 = year1.Name;
        //            }
        //            var year2 = db.GLFiscalYears.Find(fiscalyear2);
        //            if(year2 != null)
        //            {
        //                fiscalYear2 = year2.Name;
        //            }
        //        }
        //        leavepayable.LeavePayableDetails = new List<LeavePayableDetailVM>();
        //        var PayableDetails = db.LeavePayableDetails.Where(x => x.LeavePayableId == leavePayableId).DistinctByField(x=>x.EmployeeId).ToList();
        //        if (int.TryParse(branch, out int branchId))
        //            PayableDetails = PayableDetails.Where(x => x.BranchId == branchId).ToList();

        //        if (int.TryParse(workunit, out int orgId))
        //            PayableDetails = PayableDetails.Where(x => x.OrgStructureId == orgId).ToList();
        //        foreach (var payableDetail in PayableDetails)
        //        {
        //            var payableList = new LeavePayableDetailVM();
        //            var employee = db.Person_Employee.Find(payableDetail.EmployeeId);
        //            if(employee != null)
        //            {
        //                payableList.EmployeeFullName =   employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;

        //            }
        //            var workUnit = db.MainCompanyStructures.Find(payableDetail.OrgStructureId);
        //            if(workUnit != null)
        //            {
        //                payableList.WorkUnit =   workUnit.Name;

        //            }
        //            var Branch = db.Branch.Find(payableDetail.BranchId);
        //            if(Branch != null)
        //            {
        //                payableList.Branch =   Branch.Name;

        //            }
        //            payableList.FiscalYear1 = fiscalYear1;
        //            payableList.FiscalYear2 = fiscalYear2;
        //            payableList.Year1 = payableDetail.Year1;
        //            payableList.Year2 = payableDetail.Year2;
        //            payableList.TotalDays = payableDetail.PayableDays;
        //            payableList.Payable = payableDetail.Balance;
        //            leavepayable.LeavePayableDetails.Add(payableList);



        //        }

        //    }

        //    return leavepayable;


        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}