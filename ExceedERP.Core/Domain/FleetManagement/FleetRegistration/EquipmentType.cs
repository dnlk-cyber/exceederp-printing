using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DisplayName(" Name")]
        [Required]
        public string EquipmentTypeName { get; set; }
         public string CategoryName { get; set; }
        [DisplayName(" Asset Sub Category")]
        public int? FixedAssetSubCategoryId { get; set; }
        public string Description { get; set; }
        public int FixedAssetCategoryID { get; set; } // Added
        //public Guid EquipmentIdentityID { get; set; }
        [ForeignKey("EquipmentIdentityID")]
        public virtual EquipmentIdentity EquipmentIdentity { get; set; }
        [Display(Name ="TimeSheet Type")]
        public TimeSheetType TimeSheetType { get; set; }

    }

    public enum TimeSheetType
    {
        NotApplicable,
        Machinery,
        DumpTruck,
        DryLoading,
        FuelTruck,
        WagonDrill,
        LightVehicle
    }
}
