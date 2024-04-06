using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Lookup : TrackUserOperation
    {
        [Key]
        public int LookupId { get; set; } 
        public string Code { get; set; }
        [Display(Name = "Common_Lookup_Category", ResourceType = typeof(Localization.Resources))]
        public string Category { get; set; }
        public int Index { get; set; }
        [Display(Name = "Common_Lookup_LookupType", ResourceType = typeof(Localization.Resources))]
        public LookUpTypes LookupType { get; set; }
        [Display(Name = "HR_PerformanceManagment_EmployeePerformance_EvaluationType", ResourceType = typeof(Localization.Resources))]
        public EvaluationType EvalutionType { get; set; }
        [Display(Name = "Common_Lookup_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Common_Lookup_Description_Eng", ResourceType = typeof(Localization.Resources))]
        public string DescriptionEng { get; set; }
        public bool IsDefault { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_MainFunction", ResourceType = typeof(Localization.Resources))]
        public string MainFunction { get; set; }
        [Display(Name = "Common_Lookup_Description_Eng_Information_one", ResourceType = typeof(Localization.Resources))]
        public string InformationOne { get; set; }
        [Display(Name = "Common_Lookup_Description_Eng_Information_Two", ResourceType = typeof(Localization.Resources))]
        public string InformationTwo { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "HR_Allowance_AllowanceTypeRule_TaxTrasholdAmount", ResourceType = typeof(Localization.Resources))]
        public decimal TaxTresholdAmount { get; set; }
        [Display(Name = "HR_Allowance_AllowanceTypeRule_Percentage", ResourceType = typeof(Localization.Resources))]
        [Range(0, 100, ErrorMessage = "Please enter valid integer Number")]
        public decimal Percentage { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}