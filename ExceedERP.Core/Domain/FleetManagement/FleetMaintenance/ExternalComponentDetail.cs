using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ExternalComponentDetail : TrackUserSettingOperation
    {
        public Guid ExternalComponentDetailId { get; set; }
        public Guid ExternalComponentId { get; set; }
        [Required]
        public string Name { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }
        public string Remark { get; set; }
    }
}
