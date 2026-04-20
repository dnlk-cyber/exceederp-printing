using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lots
{
    public class LotTeam : TrackUserOperation
    {
        public int LotTeamId { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        public string Description { get; set; }
        public virtual ICollection<LotTeamMember> LotTeamMembers { get; set; }
        public virtual ICollection<LotTeamDecision> LotTeamDecisions { get; set; }
    }

    public class LotTeamMember : TrackUserOperation
    {
        public int LotTeamMemberId { get; set; }
        public int LotTeamId { get; set; }
        public virtual LotTeam LotTeam { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public string SignedBy { get; set; }
        public DateTime? SginTime { get; set; } 
        public bool Signed { get; set; }
        public string Title { get; set; }
    }

    public class LotTeamDecision : TrackUserOperation
    {
        public int LotTeamDecisionId { get; set; }
        public int LotTeamId { get; set; }
        public virtual LotTeam LotTeam { get; set; }
        public string Description { get; set; }
    }
}
