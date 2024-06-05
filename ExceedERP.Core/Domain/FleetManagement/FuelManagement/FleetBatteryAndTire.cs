using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
    public class FleetBatteryAndTire : TrackUserSettingOperation
    {
        [Key]
        public int FleetBatteryAndTireId { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public FleetBatteryOrTyreType FleetBatteryOrTireType { get; set; } 
    }
}
