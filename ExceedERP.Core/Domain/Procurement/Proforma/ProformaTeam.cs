using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class ProformaTeam : TrackUserOperation
    {
        public int ProformaTeamId { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ProformaTeamMember> ProformaTeamMembers { get; set; }
        public virtual ICollection<ProformaTeamDecision> ProformaTeamDecisions { get; set; }
    }

    public class ProformaTeamMember : TrackUserOperation
    {
        public int ProformaTeamMemberId { get; set; }
        public int ProformaTeamId { get; set; }
        public virtual ProformaTeam ProformaTeam { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public string SignedBy { get; set; }
        public DateTime? SginTime { get; set; }
    }

    public class ProformaTeamDecision : TrackUserOperation
    {
        public int ProformaTeamDecisionId { get; set; }
        public int ProformaTeamId { get; set; }
        public virtual ProformaTeam ProformaTeam { get; set; }
        public string Description { get; set; }
    }
}
