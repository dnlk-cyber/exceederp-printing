using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeSalaryIncrementSetting: TrackUserOperation
    {
        [Key]
        public int EmployeeSalaryIncrementSettingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "Eligibility Service Months")]
        public int EligibilityServiceMonths { get; set; }  // staff :6   manageem// default 9
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }

}
