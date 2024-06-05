using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class FailureReason : TrackUserSettingOperation
    {
        public Guid FailureReasonId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
