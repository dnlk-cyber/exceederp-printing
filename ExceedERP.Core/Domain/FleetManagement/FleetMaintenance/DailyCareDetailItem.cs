using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class DailyCareDetailItem : TrackUserSettingOperation
    {
        public Guid DailyCareDetailItemId { get; set; }
        public Guid DailyCareDetailLogId { get; set; }

        [Required]
        public string Description { get; set; }
        public WeekDays Day { get; set; }

         public virtual DailyCareDetailLog DailyCareDetailLog { get; set; }
     }

   
}
