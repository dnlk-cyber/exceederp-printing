using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{


    public class DocumentApprovalRule:TrackUserSettingOperation
    {
        public int DocumentApprovalRuleId { get; set; }
        [Display(Name = "Document Type")]
        public ApprovalDocumentType DocumentType { get; set; }
       
        //aproval
        [Display(Name = "Owner Can Approve")]
        public bool OwnerCanApprove { get; set; }
        [Display(Name = "Approver Can Modify")]
        public bool ApproverCanModify { get; set; }
        [Display(Name = "Can Change Forward To")]
        public bool CanChangeForwardTo { get; set; }
        [Display(Name = "Approval Work Flow")]
        public ApprovalWorkflow ApprovalWorkFlow { get; set; }
        [Display(Name = "Work Flow Start Process")]
        public string WorkFlowStartProcess { get; set; }
        [Display(Name = "Forward Method")]
        public ForwardMethod ForwardMethod { get; set; }
        [Display(Name = "Hierarchy Type")]
        public ApprovalHierarchy HierarchyType { get; set; }
        [Display(Name = "Default Hierarchy")]
        public string DefaultHierarchy { get; set; }
        //control
        [Display(Name = "Security Level")]
        public string SecurityLevel { get; set; }
        [Display(Name = "Access Level")]
        public string AccessLevel { get; set; }
        [Display(Name = "Archive On")]
        public string ArchiveOn { get; set; }
        public ICollection<ApprovalGroup> ApprovalGroups { get; set; }
    }
}
