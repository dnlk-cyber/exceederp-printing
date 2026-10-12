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
        [Display(Name = "Leave_Request_Id", ResourceType = typeof(Localization.Resources))]
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
        [Display(Name = "Exact Deserve Leave Days")]
        public decimal ExactDeserveLeaveDays { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_Type", ResourceType = typeof(Localization.Resources))]
        public LeaveType Type { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]

        public int BranchId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]

        public int BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int? LookupId { get; set; }

        [Display(Name = "HR_SelfCare_LeaveRequest_LeaveBalance", ResourceType = typeof(Localization.Resources))]
        public decimal LeaveBalance { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "From Time")]
        public DateTime? FromTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "To Time")]
        public DateTime? ToTime { get; set; }
        [Display(Name = "Enum_Leave_Day", ResourceType = typeof(Localization.Resources))]
        public LeaveDay Day { get; set; }
        [Display(Name = "MorningOrAfternoon", ResourceType = typeof(Localization.Resources))]

        public MorningOrAfternoon MorningOrAfternoon { get; set; }

        [Display(Name = "HR_LeaveManagment_Leave_Hour", ResourceType = typeof(Localization.Resources))]
        public int Hours { get; set; }
        public bool? IsProjectManagerApproved { get; set; }
        public string ProjectManagerApprovedBy { get; set; }
        public DateTime? ProjectManagerApprovedDate { get; set; }
        public bool? IsHRManagerApproved { get; set; }
        public string HRManagerApprovedBy { get; set; }
        public DateTime? HRManagerApprovedDate { get; set; }
        [NotMapped]
        public bool IsApproved { get; set; }
        [NotMapped]
        public bool IsPending { get; set; }
        [NotMapped]
        public bool IsRejected { get; set; }
        [NotMapped]
        public bool IsWaiting { get; set; }
        [NotMapped]
        public bool IsChangeInDepartment { get; set; }
        public int? RequesterId { get; set; }
        public LeaveActionTypes LeaveActionTypes { get; set; }
    }

    public class LeaveRequestAttachement: TrackUserSettingOperation
    {
        [Key]
        public int LeaveRequestAttachementId { get; set; }
        public int LeaveRequestId { get; set; }
        public  LeaveRequest LeaveRequest { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
    public class LeaveRequestPMValidation : VoucherValidation
    {
        public int LeaveRequestPMValidationId { get; set; }
        public int LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; }

    }

    public class LeaveRequestHRManagerValidation : VoucherValidation
    {
        public int LeaveRequestHRManagerValidationId { get; set; }

        public int LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; }

    }
}