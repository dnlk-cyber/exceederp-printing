using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ExitInterview
    {
        [Key ]
        public int ExitIntervId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_ExitInterview_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_ToKeep", ResourceType = typeof(Localization.Resources))]
        public string ToKeep { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_ToRemove", ResourceType = typeof(Localization.Resources))]
        public string ToRemove { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_DueToWork", ResourceType = typeof(Localization.Resources))]
        public bool DueToWork { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_Sicknees", ResourceType = typeof(Localization.Resources))]
        public string Sicknees { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_MainReason", ResourceType = typeof(Localization.Resources))]
        public string MainReason { get; set; }
        [Display(Name = "HR_EmployeeDetail_ExitInterview_OtherReason", ResourceType = typeof(Localization.Resources))]
        public string OtherReason { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}