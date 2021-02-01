using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PerdimeSetting:TrackUserOperation
    {
        [Key]
        public int PerdimeSettingId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PerdimeSetting_Minmum", ResourceType = typeof(Localization.Resources))]
        public decimal Minmum { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PerdimeSetting_Maximum", ResourceType = typeof(Localization.Resources))]
        public decimal Maximum { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_PerdimeSetting_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}