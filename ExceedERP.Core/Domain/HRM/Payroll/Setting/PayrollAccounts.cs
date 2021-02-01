using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollAccounts:TrackUserOperation 
    {
        [Key]
        public int PayrollAccountsId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_IsPerEmployee", ResourceType = typeof(Localization.Resources))]
        public bool IsPerEmployee { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_IsPerBranch", ResourceType = typeof(Localization.Resources))]
        public bool IsPerBranch { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_Account", ResourceType = typeof(Localization.Resources))]
        public string Account { get; set; }
        
        [Display(Name = "HR_Payroll_EmployeePayroll_PayrollSetting_IsPerCostCenter", ResourceType = typeof(Localization.Resources))]
        public bool IsPerCostCenter { get; set; }
        public string ReferenceType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public string MainAccount { get; set; }
        public string BranchAccount { get; set; }
        public string CostCenter { get; set; }
        public string SubAccount { get; set; }

    }
}