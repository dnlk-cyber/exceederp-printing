using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class DailyCareDetailItem : TrackUserSettingOperation
    {
        public Guid DailyCareDetailItemId { get; set; }
        public Guid DailyCareDetailLogId { get; set; }

        [Required]
        public string Description { get; set; }
        public WeekDays Day { get; set; }

         public virtual DailyCareDetailLog DailyCareDetailLog { get; set; }
     }

   
}
