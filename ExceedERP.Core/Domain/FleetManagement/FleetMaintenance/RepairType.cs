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
    public partial class RepairType : TrackUserOperation
    {

        public Guid RepairTypeID { get; set; }
        [Required]
        [DisplayName(" Code ")]
        public string Code { get; set; }
        [Required]
        [DisplayName(" Repair Type ")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
