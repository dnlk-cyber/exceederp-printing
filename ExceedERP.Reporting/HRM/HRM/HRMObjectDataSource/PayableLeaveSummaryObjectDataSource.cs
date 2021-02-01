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
            List<Person_Employee> employees;//= ListOfEmployeeIdView.GetEmployees();
            if (branchId != null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && dept == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else
            {

                employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            if(year != null)
            {
                periods = periods.Where(x=>x.GlFiscalYearId.ToString() == year).ToList();
            }
            PaybaleLeaveList PaybaleLeaveList = new PaybaleLeaveList();
            var placments = db.Placements.Where(x => x.isCurrent);
            //var requestStatus = db.RequestesStatus.Where(x => x.ReferenceType == "LeaveTransfer").ToList();
            foreach (var e in employees)
            {
                sum = new PayableLeave();
                sum.ID = e.EmployeeCode;
                sum.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                sum.PaybaleLeaveList = new List<PaybaleLeaveList>();
                foreach (var p in periods)
                {
                    PaybaleLeaveList = new PaybaleLeaveList();
                    // var p= periods .Where (x=> x.DateTo.Year - )
                    var check = p.DateTo.Year - currentDate.Year;
                    if (check <= 2)
                    {
                        decimal salary = 0;
                        var basicSalary = placments.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                        if (basicSalary != null)
                        {
                            salary = basicSalary.Salary;
                        }
                        int period = p.GlFiscalYearId;
                        var entitlew = db.LeavesEntitle.Where(x => x.GlFiscalYearId == period).ToList();
                        var taken = db.Leaves.Where(x => x.GlFiscalYearId == period).ToList();
                        var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period).ToList();

                        var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.Check).ToList();
                        var periodName = db.GLFiscalYears.Where(x => x.GlFiscalYearId == period).FirstOrDefault();



                        if (periodName != null)
                        {
                            PaybaleLeaveList.LeavePeriod = periodName.Name;
                        }
                        PaybaleLeaveList.TransferedFrom = Convert.ToDecimal(tranferedList.Where(x => x.EmployeeId == e.EmployeeId).Sum(a => a.Amount));

                        var firstOrDefault = entitlew.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                        if (firstOrDefault != null)
                            PaybaleLeaveList.Entitlment = firstOrDefault.Amount;

                        PaybaleLeaveList.TotalAmountTaken = taken.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount);
                        PaybaleLeaveList.PaidInCash = Convert.ToDecimal(paidIncash.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount));
                        PaybaleLeaveList.Balance = (PaybaleLeaveList.Entitlment + PaybaleLeaveList.TransferedFrom) - PaybaleLeaveList.TotalAmountTaken - PaybaleLeaveList.PaidInCash;
                        PaybaleLeaveList.Total = (salary / 26) * PaybaleLeaveList.Balance;



                    }
                    else
                    {
                        PaybaleLeaveList.LeavePeriod = "-";
                        PaybaleLeaveList.PaidInCash = 0;
                        PaybaleLeaveList.Total = 0;
                    }
                    sum.PaybaleLeaveList.Add(PaybaleLeaveList);
                }
                summaries.Add(sum);
            }


            return summaries;
        }
    }
}