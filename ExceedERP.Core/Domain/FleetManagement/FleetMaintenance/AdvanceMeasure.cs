using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class AdvanceMeasure : TrackUserSettingOperation
    {
        public Guid AdvanceMeasureId { get; set; }
          [Required]
        public string Description { get; set; }

    }
}
