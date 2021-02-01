using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollEmployeeAccount :TrackUserOperation
    {
        public int PayrollEmployeeAccountId { get; set; }
     
        [Required]
        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_CostCenterAccountCode", ResourceType = typeof(Localization.Resources))]
        public string CostCenterAccountCode { get; set; }
        public virtual GLChartOfAccountCostCenter AccountCostCenter { get; set; }

        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_LocationAccCode", ResourceType = typeof(Localization.Resources))]
        public string LocationAccCode { get; set; }
        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_LocationAccCode", ResourceType = typeof(Localization.Resources))]
        public string SubAccCode { get; set; }
        public virtual GLChartOfAccountLocation AccountLocation { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        //[Display(Name = "Staff Debtor Account Code")]
        //public string StaffDebtorAccountCode { get; set; }

        //[Display(Name = "Staff Advance Account Code")]
        //public string StaffAdvanceAccountCode { get; set; }
  
        //[Display(Name = "Staff Loan Account Code")]
        //public string StaffLoanAccountCode { get; set; }
        //[Display(Name = "Branch Name")]
        //public string OrgBranchName { get; set; }
        //public string Status { get; set; }
        //public bool isSendForApproval { get; set; }
        //public bool isOnlineApproved { get; set; }
        //public string OnlineApprovedBy { get; set; }
        //public DateTime? OnlineApprovedTime { get; set; }
        //public bool isVoid { get; set; }
        //public string VoidBy { get; set; }
        //public DateTime? VoidTime { get; set; }
       
    }
}