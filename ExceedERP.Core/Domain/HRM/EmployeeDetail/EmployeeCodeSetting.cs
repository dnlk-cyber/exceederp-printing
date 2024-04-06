using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCodeSetting
    {
        public int EmployeeCodeSettingId { get; set; }
        [Display(Name = "Enum_Employee_ID_Abbreviation_Type", ResourceType = typeof(Localization.Resources))]
        public EmployeeIDAbbreviationType Type { get; set; }
        [Display(Name = "EmployeeCodeSetting_Index", ResourceType = typeof(Localization.Resources))]
        public int Index { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
