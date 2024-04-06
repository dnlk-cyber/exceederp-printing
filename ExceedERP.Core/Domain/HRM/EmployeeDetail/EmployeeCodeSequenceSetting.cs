using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCodeSequenceSetting
    {
        public int EmployeeCodeSequenceSettingId { get; set; }
        [Display(Name="Name",ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        //[DisplayName("Code Length")]
        [Display(Name = "HR_Employee_Code_Sequence_setting_Item_Code_Trim", ResourceType = typeof(Localization.Resources))]
        public int ItemCodeTrim { get; set; }
        [Display(Name = "HR_Employee_Code_Sequence_setting_Index_Count", ResourceType = typeof(Localization.Resources))]
        public int IndexCount { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public bool Status { get; set; }
    }
}
