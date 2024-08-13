using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Health
{
    public class PatientCard
    {
        [Key]
		 public int PatientId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCard_PatientCardId", ResourceType = typeof(Localization.Resources))]
        public string PatientCardId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_PatientCard_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCard_Date", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCard_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCard_Age", ResourceType = typeof(Localization.Resources))]
        public int Age { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCard_Sex", ResourceType = typeof(Localization.Resources))]
        public string Sex { get; set; }
        [NotMapped]
        public bool IsSent { get; set; }
        [NotMapped]
        public bool IsTested { get; set; }
        [NotMapped]
        public bool IsClosed { get; set; }
        public bool IsNew { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}