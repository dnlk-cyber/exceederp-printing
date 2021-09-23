using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetCondition : TrackUserSettingOperation
    {

        public int FleetConditionId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
