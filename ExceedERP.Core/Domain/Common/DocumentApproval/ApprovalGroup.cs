using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public class ApprovalGroup:TrackUserSettingOperation
    {
        public ApprovalGroup()
        {
            GroupRules = new List<ApprovalGroupRule>();
        }
        public int ApprovalGroupId { get; set; }
        public virtual DocumentApprovalRule DocumentApprovalRule { get; set; }
        public int DocumentApprovalRuleId { get; set; }
        [Display(Name = "Opertating Unit")]
        public int ? BusinessUnit { get; set; }
        [Display(Name = "Branch Unit")]
        public int ? OpertatingUnit { get; set; }
       
        [Display(Name = "Name")]
        public string Name { get; set; }
        public bool Enabled { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public int Step { get; set; }
        public ICollection<ApprovalGroupRule> GroupRules { get; set; }
    }
}