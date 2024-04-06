using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeaveReturn : DepartmentOperations
    {
        public int LeaveReturnId { get; set; }
        public int LeaveRequestId { get; set; }
       // [Required]
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
       // [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
       // [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [NotMapped]
        public bool IsApproved { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime From { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_From", ResourceType = typeof(Localization.Resources))]
        public string FromAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_ToAmharic", ResourceType = typeof(Localization.Resources))]

        public string ToAm { get; set; }
        //[Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        //public bool IsForOther { get; set; }
    }
}
