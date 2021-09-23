using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class Manufacturer : TrackUserSettingOperation
    {
           public Guid ManufacturerID { get; set; }
        [Required]
        [DisplayName(" Manufacturer ")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
