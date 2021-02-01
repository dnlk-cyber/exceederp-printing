using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
        public  class FuelType : TrackUserOperation
    {
            public Guid FuelTypeId { get; set; }
            [Required]
            public string Name { get; set; }

            public decimal UnitPrice { get; set; }

            public string Remark { get; set; }
    }
}
