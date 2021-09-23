using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetInsuranceType : TrackUserOperation
    {

        public int FleetInsuranceTypeId { get; set; }
       [Required]
       public string Name { get; set; }
       public string Description { get; set; }
   
    }
}
