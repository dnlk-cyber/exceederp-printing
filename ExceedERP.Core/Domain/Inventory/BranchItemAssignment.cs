using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Inventory
{
    public class BranchItemAssignment:TrackUserSettingOperation
    {
        public int BranchItemAssignmentId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Reorder Point")]
        public float ReOrderPoint { get; set; }
        [Display(Name = "Minimum Point")]
        public float MinPoint { get; set; }
        [Display(Name = "Maximum Point")]
        public float MaxPoint { get; set; }
        public decimal Balance { get; set; }
        public decimal AvgPrice { get; set; }
    }
}
