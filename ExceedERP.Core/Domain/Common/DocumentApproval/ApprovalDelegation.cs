using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public class ApprovalDelegation : TrackUserSettingOperation
    {
        
        public int ApprovalDelegationId { get; set; }
        
        [Display(Name = "Delegating Employee")]
        public int DelegatingEmployeeId { get; set; }
        [Display(Name = "Delegated Employee")]
        public int DelegatedEmployeeId { get; set; }

        [Display(Name = "Effective From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }
        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? To { get; set; }
        public string Reason { get; set; }
    }
}