using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    Employee Reward . </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/28/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class EmployeeReward : TrackUserOperation
    {
        [Key]
        public int EmployeeRewardId { get; set; }
        [Required]
        [Display(Name = "Full-Name")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Sub-Branch")]
        public string SubBranch { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string DateAm { get; set; }
        public RewardType RewardType { get; set; }
        [Display(Name = "Reason for Reward ")]
        public string ReasonForReward { get; set; }
        public string AwardedBy { get; set; }
        public decimal? Amount { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
    public enum RewardType
    {
        Certification,
        Money, 
        Kind_Gold_Or_Silver,
        Leave,
        Vacation,
        Others
    }


}