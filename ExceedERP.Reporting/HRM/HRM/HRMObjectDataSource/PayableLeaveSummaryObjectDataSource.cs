using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class PayableLeaveSummaryObjectDataSource : List<PayableLeave>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayableLeave> GetLeavePayableSummary(string branchId, string dept, string year)
        {
            List<PayableLeave> summaries = new List<PayableLeave>();
            PayableLeave sum = new PayableLeave();
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var periods = db.GLFiscalYears.ToList();

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                var empIds = db.Placements.Where(x => x.BranchId == braId && x.isCurrent).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgId);
                var empIds = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }

            if (year != null)
            {
                int.TryParse(year, out int periodId);
                periods = periods.Where(x => x.GlFiscalYearId == periodId).ToList();
            }

            PaybaleLeaveList PaybaleLeaveList = new PaybaleLeaveList();

           // var placments = db.Placements.Where(x => x.isCurrent);

            foreach (var e in employees)
            {
                sum = new PayableLeave();
                sum.PaybaleLeaveList = new List<PaybaleLeaveList>();

                sum.ID = e.EmployeeCode;
                sum.FullName = e.FullNameAmharic;

                db = new ExceedDbContext();

                decimal salary = 0;
                var basicSalary = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                if (basicSalary != null)
                {
                    salary = basicSalary.Salary;
                }

                foreach (var p in periods)
                {
                    PaybaleLeaveList = new PaybaleLeaveList();

                    db = new ExceedDbContext();
                    var leaveTaken = db.Leaves.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == p.GlFiscalYearId).ToList();

                    db = new ExceedDbContext();
                    var paidInCash = db.LeavePaid.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == p.GlFiscalYearId).ToList();

                    PaybaleLeaveList.LeavePeriod = p.Name;

                    db = new ExceedDbContext();
                    var entitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == p.GlFiscalYearId && x.EmployeeId == e.EmployeeId);
                    if (entitle != null)
                    {
                        PaybaleLeaveList.Entitlment = entitle.Amount;
                        PaybaleLeaveList.Balance = entitle.Balance;
                    }

                    PaybaleLeaveList.TotalAmountTaken = leaveTaken.Sum(x => x.Amount);
                    PaybaleLeaveList.PaidInCash = Convert.ToDecimal(paidInCash.Sum(x => x.Amount));
                    //PaybaleLeaveList.Balance = entitle.Balance;//PaybaleLeaveList.Entitlment - PaybaleLeaveList.TotalAmountTaken - PaybaleLeaveList.PaidInCash;
                    PaybaleLeaveList.Total = (salary / 26) * PaybaleLeaveList.Balance;

                    sum.PaybaleLeaveList.Add(PaybaleLeaveList);
                }

                summaries.Add(sum);
            }


            return summaries;
        }
    }

}