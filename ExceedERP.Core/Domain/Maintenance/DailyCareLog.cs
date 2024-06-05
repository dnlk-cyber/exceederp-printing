using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class DailyCareLog : Operations
    {
        public Guid DailyCareLogId { get; set; }
        [Required]
        [DisplayName("Equipment")]
        public Guid FleetsId { get; set; }
        public string Operator { get; set; }
        public string Description { get; set; } 
        public string Location { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }
        public virtual Fleets Fleets { get; set; }

    }
}
