using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
        public  class JobCauses : TrackUserSettingOperation
    {
            public Guid JobCausesId { get; set; }
            public Guid OrderId { get; set; }
                [Required]
            public string Name { get; set; }
            public string Description  { get; set; }
    }
}
