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
       
        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_LocationAccCode", ResourceType = typeof(Localization.Resources))]
        public string LocationAccCode { get; set; }
        [Display(Name = "HR_Payroll_PayrollSetting_PayrollEmployeeAccount_SubAccountCode", ResourceType = typeof(Localization.Resources))]
        public string SubAccCode { get; set; }       
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        
       
    }
}