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
    public partial class EquipmentType : TrackUserSettingOperation
    {
        [ScaffoldColumn(false)]
        public Guid EquipmentTypeID { get; set; }
         [DisplayName("Equipment Category")]
        public Guid EquipmentIdentityID { get; set; }
        [Required]
        [DisplayName(" Asset Category")]
        public int FixedAssetCategoryID { get; set; }
        [DisplayName(" Name")]
        [Required]
        public string EquipmentTypeName { get; set; }
         public string CategoryName { get; set; }
        
        public string Description { get; set; }
       
        public virtual EquipmentIdentity EquipmentIdentity { get; set; }
    }
}
