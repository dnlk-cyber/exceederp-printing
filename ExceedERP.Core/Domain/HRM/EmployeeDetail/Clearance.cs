using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Clearance
    {
        [Key]
        public int ClearanceId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required ]
        [Display(Name = "HR_EmployeeDetail_Clearance_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required ]
        [Display(Name = "HR_EmployeeDetail_Clearance_ReasonOfTermination", ResourceType = typeof(Localization.Resources))]
        public string ReasonOfTermination { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Clearance_TerminatedSince", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TerminatedSince { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
	   
		
    }
}