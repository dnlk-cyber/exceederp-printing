
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class EquipmentIdentity :  TrackUserSettingOperation
    {

        public Guid EquipmentIdentityID { get; set; }
          [Required]
          [DisplayName(" Name ")]
          public string EquipmentCategoryName { get; set; }
          public string Description { get; set; }
        [DisplayName(" Fixed Asset Category ")]
        public int? FixedAssetCategoryId { get; set; }

        
    }
}