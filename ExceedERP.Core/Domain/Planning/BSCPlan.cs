using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public enum PlanApprovalStatus
    {
        Draft = 0,
        DepartmentSubmitted = 1,
        BranchApproved = 2,
        HeadOfficeApproved = 3,
        RevisionRequested = 4,
        WorkUnitApproved = 5
    }

    public enum PlanApprovalAction
    {
        Submitted = 0,
        Approved = 1,
        RevisionRequested = 2,
        Resubmitted = 3,
        FeedbackAdded = 4,
        Aggregated = 5
    }

    public class BSCPlan
    {
        [Key]
        public int BSCPlanId { get; set; }
        public Objective Objective { get; set; }
        [Required]
        [Display(Name ="Objective")]
        public int ObjectiveId { get; set; }
        [Display(Name = "Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }
        public int PlanId { get; set; }
        public bool IsCopied { get; set; }
        public bool IsFirstLevelApproved { get; set; }
        public bool IsSecondLevel { get; set; }
        public bool IsThirdLevel { get; set; }
        [Display(Name = "Approval Status")]
        public PlanApprovalStatus ApprovalStatus { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? SubmittedTime { get; set; }
        public string BranchApprovedBy { get; set; }
        public DateTime? BranchApprovedTime { get; set; }
        public string HeadOfficeApprovedBy { get; set; }
        public DateTime? HeadOfficeApprovedTime { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        [NotMapped]
        public bool HasChild { get; set; }
        [NotMapped]
        public bool CanApprove { get; set; }
        [NotMapped]
        public bool CanCopy { get; set; }
        [NotMapped]
        public bool CanEdit { get; set; }
        [NotMapped]
        public bool CanDelete { get; set; }
        [NotMapped]
        public bool CanRequestRevision { get; set; }
        [NotMapped]
        public bool CanAddFeedback { get; set; }
        [NotMapped]
        public string ApprovalAction { get; set; }
        [NotMapped]
        public string ApprovalStatusText { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public bool IsApprovalLocked
        {
            get
            {
                if (ApprovalStatus == PlanApprovalStatus.RevisionRequested)
                    return false;

                return ApprovalStatus != PlanApprovalStatus.Draft ||
                       IsFirstLevelApproved || IsSecondLevel || IsThirdLevel;
            }
        }
        public virtual  Plan GetPlan { get; set; }


    }

    public class PlanApprovalFeedback
    {
        [Key]
        public int PlanApprovalFeedbackId { get; set; }
        public int BSCPlanId { get; set; }
        public PlanApprovalAction Action { get; set; }
        public PlanApprovalStatus FromStatus { get; set; }
        public PlanApprovalStatus ToStatus { get; set; }
        [StringLength(2000)]
        public string Feedback { get; set; }
        [StringLength(256)]
        public string ActionBy { get; set; }
        public DateTime ActionTime { get; set; }
        [NotMapped]
        public string ActionText { get; set; }
        [NotMapped]
        public string FromStatusText { get; set; }
        [NotMapped]
        public string ToStatusText { get; set; }
        [NotMapped]
        [Display(Name = "Work Unit")]
        public string WorkUnitName { get; set; }
        [NotMapped]
        [Display(Name = "Branch")]
        public string BranchName { get; set; }
        [NotMapped]
        [Display(Name = "Objective")]
        public string ObjectiveName { get; set; }
        public virtual BSCPlan BSCPlan { get; set; }
    }

}
