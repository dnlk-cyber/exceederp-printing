using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class ExperienceRequest: DepartmentOperations 
    {
        public int ExperienceRequestId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_SelfCare_ExperienceRequest_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_ExperienceRequest_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        public DateTime ApplicationDate { get; set; }
        [Display(Name = "HR_SelfCare_ExperienceRequest_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        //[Required]
        public string ApplicationDateAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        [Display(Name = "የጥያቄው ዓይነት")]
        public SelfServiceRequestType RequestType { get; set; }
        public bool IsDirect { get; set; }
    }
}