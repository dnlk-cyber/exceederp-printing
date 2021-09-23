using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

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
