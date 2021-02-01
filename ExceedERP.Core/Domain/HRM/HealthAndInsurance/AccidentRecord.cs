using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class AccidentRecord
    {
        [Key]
        public int AccidentRecordId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_DetailDescription", ResourceType = typeof(Localization.Resources))]
        public string DetailDescription { get; set; }

        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_AccidentPlace", ResourceType = typeof(Localization.Resources))]
        public string AccidentPlace { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_Witness", ResourceType = typeof(Localization.Resources))]
        public string Witness { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_IsInWorkPlace", ResourceType = typeof(Localization.Resources))]
        public bool IsInWorkPlace { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentRecord_IsInOnJob", ResourceType = typeof(Localization.Resources))]
        public bool IsInOnJob { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        //[Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        //public string EmployeeCode { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
    }
}