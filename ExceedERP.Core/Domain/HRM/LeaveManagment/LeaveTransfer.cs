using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary>   A leave entitle transfer. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class LeaveTransfer:TrackUserOperation 
    {
        [Key]
        public int LeaveTransferId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveTransfer_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveTransfer_From", ResourceType = typeof(Localization.Resources))]
        public int From { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveTransfer_To", ResourceType = typeof(Localization.Resources))]
        public int To { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_LeaveTransfer_Amount", ResourceType = typeof(Localization.Resources))]
      
        public decimal Amount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public bool Check { get; set; }
    }
}