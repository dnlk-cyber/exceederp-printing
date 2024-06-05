using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
        public partial class EquipmentName : TrackUserSettingOperation
    {
        public Guid EquipmentNameID { get; set; }
        [Required]
        [DisplayName(" Equipment Type ")]
        public Guid EquipmentTypeId { get; set; }
        [DisplayName(" Fixed Asset SubCategory Name ")]
        public int? FixedAssetSubCategoryNameId { get; set; }
        [DisplayName(" Equipment Name ")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAssigned { get; set; }
    }
}
