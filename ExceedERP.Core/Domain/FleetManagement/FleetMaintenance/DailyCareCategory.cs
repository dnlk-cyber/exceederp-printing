using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class DailyCareCategory : TrackUserSettingOperation
    {
        public Guid DailyCareCategoryId { get; set; }
          [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
