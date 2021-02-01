using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public class AssignApprovalGroupAssignment:TrackUserSettingOperation
    {
        public int AssignApprovalGroupAssignmentId { get; set; }
        public int AssignApprovalGroupId { get; set; }
        public virtual AssignApprovalGroup AssignApprovalGroup { get; set; }
        [Display(Name = "Document Type")]
        public ApprovalDocumentType DocumentType { get; set; }
        [Display(Name = "Approval Group")]
        public int ApprovalGroupId { get; set; }
        public virtual ApprovalGroup ApprovalGroup { get; set; }
        [Display(Name = "Effective From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }
        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? To { get; set; }
    }
}