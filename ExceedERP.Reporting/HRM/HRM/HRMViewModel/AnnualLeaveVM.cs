using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class AnnualLeaveVM
    {

        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string FiscalYear1 { get; set; }
        public decimal Balance1 { get; set; }
        public string FiscalYear2 { get; set; }
        public decimal Balance2 { get; set; }
        public string FiscalYear3 { get; set; }
        public decimal Balance3 { get; set; }
        //public string TotalLeaveType { get; set; }
        public decimal LeaveDays { get; set; }
        //public decimal Amount { get; set; }
        public decimal TotlalAmount { get; set; }
        public string Remark { get; set; }
        //public int BranchId { get; set; }
        //public bool IsAdded { get; set; }
        public string DepartmentApprover { get; set; }
        public string Approver { get; set; }
        public List<string> DynamicHeaders { get; set; } // Dynamic headers for crosstab
        public List<LeaveBalanceVM> LeaveBalances { get; set; }
        public List<LeaveRequestVM> RequesteLeaves { get; set; }
    }
    public class LeaveBalanceVM
    {
        public string FiscalYear { get; set; }
        public decimal Balance { get; set; }
    }
    public class LeaveRequestVM
    {
        public string LeaveType { get; set; }
        public DateTime From { get; set; }
        public string FromAm { get; set; }
        public DateTime To { get; set; }
        public string ToAm { get; set; }
        public decimal LeaveDays { get; set; }
        public string ApproverDepartment { get; set; }
        public string Approver { get; set; }
    }

}
