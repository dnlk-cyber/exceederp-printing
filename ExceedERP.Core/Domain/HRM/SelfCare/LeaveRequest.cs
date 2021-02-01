using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class LeaveRequest : DepartmentOperations
    {
        [Key]
        public int LeaveRequestId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required ]
        [Display(Name = "HR_SelfCare_LeaveRequest_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_RequestDate", ResourceType = typeof(Localization.Resources))]
        public DateTime RequestDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_From", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_LeaveDays", ResourceType = typeof(Localization.Resources))]
        public decimal LeaveDays { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_Type", ResourceType = typeof(Localization.Resources))]
        public LeaveType Type { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }

        [Display(Name = "HR_SelfCare_LeaveRequest_LeaveBalance", ResourceType = typeof(Localization.Resources))]
        public decimal LeaveBalance { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        public DateTime? DateCreated { get; set; }
        [Display(Name = "ቀን")]
        public LeaveDay Day { get; set; }
        [Display(Name = "ጊዜ")]
        public MorningOrAfternoon MorningOrAfternoon { get; set; }

        [Display(Name = "HR_LeaveManagment_Leave_Hour", ResourceType = typeof(Localization.Resources))]
        public int Hours { get; set; }
        [NotMapped]
        public bool IsApproved { get; set; }
        public int? RequesterId { get; set; }
        public LeaveActionTypes LeaveActionTypes { get; set; }
    }
}