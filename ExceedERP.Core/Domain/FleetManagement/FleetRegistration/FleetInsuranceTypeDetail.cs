
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetInsuranceTypeDetail : TrackUserOperation
    {

        public int FleetInsuranceTypeDetailId { get; set; }

        public Guid InsuranceId { get; set; }
          [Required]
       public string Name { get; set; }
       public string Description { get; set; }
   
    }
}
