using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
        public  class MechanicOnHand : TrackUserOperation
    {
            public Guid MechanicOnHandId { get; set; }
            public Guid ServiceId { get; set; }
            public string Name { get; set; }
            public string SpareName { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            [DisplayName("TotalPrice")]
            [NotMapped]
            public decimal TotalPrice { get; set; }
    }
}
