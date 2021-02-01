using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class PatientsLeave
    {
        [Key]
        public int PatientsLeaveId { get; set; }
        [Required ]
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_PatientCardId", ResourceType = typeof(Localization.Resources))]
        public int PatientId { get; set; }
        public virtual PatientCard PatientCard { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_Date", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_Date", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_TentativeDiagnosis", ResourceType = typeof(Localization.Resources))]
        public string TentativeDiagnosis { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_RecommendedRest", ResourceType = typeof(Localization.Resources))]
        public string RecommendedRest { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_Days", ResourceType = typeof(Localization.Resources))]
        public decimal Days { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientsLeave_TreatedBy", ResourceType = typeof(Localization.Resources))]
        public string TreatedBy { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_From", ResourceType = typeof(Localization.Resources))]
        public string  FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_To", ResourceType = typeof(Localization.Resources))]
        public string  ToAmharic { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}