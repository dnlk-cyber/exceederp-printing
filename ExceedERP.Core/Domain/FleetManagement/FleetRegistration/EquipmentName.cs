using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class EquipmentName : TrackUserSettingOperation
    {

        public Guid EquipmentNameID { get; set; }
        [Required]
        [DisplayName(" Equipment Name ")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAssigned { get; set; }

    }
}
