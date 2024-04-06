using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantExamResult
    {
        [Key]
        public int ApplicantResultId { get; set; }       
        public int ApplicantScreeningId { get; set; }
        [Display(Name = "ExamType", ResourceType =typeof(Localization.Resources))]
        public ExamType ExamType { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_SettingValue", ResourceType = typeof(Localization.Resources))]
        public decimal SettingValue { get; set; }
        [Display(Name = "Common_Configuration_CurrentValue", ResourceType = typeof(Localization.Resources))]
        public decimal CurrentResult { get; set; }
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}