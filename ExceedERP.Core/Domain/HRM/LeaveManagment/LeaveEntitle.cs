using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeaveEntitle : TrackUserOperation
    {
        [Key]
        public int LeaveEntitleId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveEntitle_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
    
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveEntitle_LeavePeriod", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveEntitle_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveEntitle_TotlalAmount", ResourceType = typeof(Localization.Resources))]
        public decimal TotlalAmount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_LeaveManagment_LeaveEntitle_IsExpired", ResourceType = typeof(Localization.Resources))]
        public bool IsExpired { get; set; }
        public decimal Balance { get; set; }
        public int BranchId { get; set; }
        public string ExpiredBy { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string ReversedBy { get; set; }
        public DateTime? ReversedDate { get; set; }
        public string Void { get; set; }
        public bool IsAdded { get; set; }
        
    }
}