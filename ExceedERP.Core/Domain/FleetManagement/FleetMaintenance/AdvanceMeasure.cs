using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class AdvanceMeasure : TrackUserSettingOperation
    {
        public Guid AdvanceMeasureId { get; set; }
          [Required]
        public string Description { get; set; }

    }
}
