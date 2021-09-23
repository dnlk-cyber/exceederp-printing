using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class Educationallevel : TrackUserSettingOperation
    
        
        {
            public Guid EducationallevelID { get; set; }
            [Required]
            [DisplayName(" Level Name ")]
            public string Name { get; set; }
            public string Description { get; set; }

        }
}
