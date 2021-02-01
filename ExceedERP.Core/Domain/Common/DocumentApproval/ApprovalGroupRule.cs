using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public class ApprovalGroupRule:TrackUserSettingOperation
    {
        public int ApprovalGroupRuleId { get; set; }
        public int ApprovalGroupId { get; set; }
        public virtual ApprovalGroup ApprovalGroup { get; set; }
        [Display(Name = "Object")]
        public ApprovalGroupRuleObject RuleObject { get; set; }
        [Display(Name = "Type")]
        public ApprovalRuleType Type { get; set; }
        [Display(Name = "Amount Limit")]
        public decimal AmountLimit { get; set; }
        [Display(Name = "Low Value")]
        public string LowValue { get; set; }
        [Display(Name = "High Value")]
        public string HighValue { get; set; }
    }
}