using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk.ChangeManagement
{
    public enum ChangeType
    {
        Standard,
        Emergency,
        Normal,
        Minor,
        Strategic,
        Operational
    }

    public enum ChangeStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        InProgress,
        Completed,
        RolledBack
    }

    public class ChangeRequest: Operations
    {
        public int ChangeRequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DisplayName("Change Type")]
        public ChangeType ChangeType { get; set; }
        [DisplayName("Change Type Detail")]
        public string  ChangeTypeDetail { get; set; }
        [DisplayName("Change Status")]
        public ChangeStatus ChangeStatus { get; set; } = ChangeStatus.Draft;
        [DisplayName("Request-Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; } = DateTime.Now;
        [DisplayName("Planned Start-Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PlannedStartDate { get; set; }
        [DisplayName("Planned End-Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PlannedEndDate { get; set; }
        [DisplayName("Requestor Name")]
        public int? RequestorId { get; set; }
        [DisplayName("Approver Name")]
        public int? ApproverId { get; set; }
        [DisplayName("Implementation Team")]
        public string ImplementationTeam { get; set; }
        [DisplayName("Risk Assessment")]
        public string RiskAssessment { get; set; }
        [DisplayName("Rollback Plan")]
        public decimal RollbackPlan { get; set; }
        [DisplayName("Impact Analysis")]
        public string ImpactAnalysis { get; set; }
        public List<ChangeTask> Tasks { get; set; } = new List<ChangeTask>();
        public List<ChangeApproval> Approvals { get; set; } = new List<ChangeApproval>();
    }
    public class ChangeTask: TrackUserSettingOperation
    {
        public int ChangeTaskId { get; set; }
        public string Description { get; set; }
        [DisplayName("Responsible Team ")]
        public int? OrgStructureId { get; set; }
        [DisplayName("Head ")]
        public int? EmployeeId { get; set; }
        [DisplayName(" Respnsiblity")]
        public string Responsiblity { get; set; }
        public bool IsCompleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Completion Date")]
        public DateTime? CompletionDate { get; set; }
        public int ChangeRequestId { get; set; }
        public virtual ChangeRequest Request { get; set; }

    }
    public class ChangeApproval: TrackUserSettingOperation
    {
        public int ChangeApprovalId { get; set; }
        public int? ApproverId { get; set; }
        public bool IsApproved { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int ChangeRequestId { get; set; }
        public virtual ChangeRequest Request { get; set; }
    }


}
