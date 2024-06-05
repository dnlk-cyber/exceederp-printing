using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class OutsourceMaintenance : Operations
    {

        public Guid OutsourceMaintenanceID { get; set; }
        public Guid FleetID { get; set; }

        [Required]
        [DisplayName(" Type Of Repair")]
        public string TypeOfRepair { get; set; }
        [DisplayName(" Start Date")]
        [Required]
        public Nullable<DateTime> fromDate { get; set; }
        [DisplayName(" End Date")]
        [Required]
        public Nullable<DateTime> toDate { get; set; }
        [DisplayName("Reason For Maintenance")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string ReasonForMaintenace { get; set; }
         [DataType(DataType.Currency)]
        [DisplayName("Total Cost")]
        [Required]
        public Nullable<decimal> TotalCost { get; set; }
        public string Description { get; set; }

        public virtual Fleets Fleet { get; set; }
    }
}
