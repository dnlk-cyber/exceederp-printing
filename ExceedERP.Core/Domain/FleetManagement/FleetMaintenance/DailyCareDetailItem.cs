using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
