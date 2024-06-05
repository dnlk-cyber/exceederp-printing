using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
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
