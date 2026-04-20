using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenanceCenter : TrackUserOperation
    {

        public Guid MaintenanceCenterID { get; set; }
        [Display(Name="Is External")]
        public bool IsExternal { get; set; }
        [Required]
        [DisplayName(" Center ")]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Description { get; set; }
        [Display(Name="Business Unit")]
        public int? BusinessUnitId { get; set; }
        [Display(Name="Branch")]
        public int BranchId { get; set; }

    }

    public class ServiceCenter
    {
        [Key]
        public int ServiceCenterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid MaintenanceCenterId { get; set; }
        public Decimal Weight { get; set; }
    }
}
