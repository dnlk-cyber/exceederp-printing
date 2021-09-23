using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class OperatorPosition : TrackUserSettingOperation
    {

        public Guid OperatorPositionID { get; set; }
        [Required]
        [DisplayName(" Position ")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
