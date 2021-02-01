using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
  
    public class ContributionType:TrackUserOperation
    {
        [Key]
        public int ContributionTypeId { get; set; }
          [Display(Name = "HR_Payroll_PayrollSetting_ContributionType_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
         [Display(Name = "HR_Payroll_PayrollSetting_ContributionType_ContributionCategory", ResourceType = typeof(Localization.Resources))]
        public ContributionCategory ContributionCategory { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
