using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class MaintenanceCenter : TrackUserOperation
    {

        public Guid MaintenanceCenterID { get; set; }
        [Required]
        [DisplayName(" Center ")]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Description { get; set; }
    }
}
