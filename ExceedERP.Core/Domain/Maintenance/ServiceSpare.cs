using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ServiceSpare : Operations
    {


        public Guid ServiceSpareID { get; set; }

        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }

        [DisplayName("Item Name")]
        public string InventoryItemId { get; set; }
        [Display(Name ="Item")]
        public string SparePartID { get; set; }
        [Display(Name = "Category")]
        public string SpareMaterialCategoryId { get; set; }
        [NotMapped]
        public string Category { get; set; }

        [NotMapped]
        [DisplayName("Name")]
        public string SparePartName { get; set; }

        [DisplayName("Name")]
        public string ServiceName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }

        public decimal Quantity { get; set; }
        [DisplayName("Used Quantity")]
        public decimal UsedQuantity { get; set; }
        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        public SpareType Type { get; set; }
        [DisplayName("Status")]
        public ApprovalStatuses ApprovalStatus { get; set; }

        public decimal TotalPrice { get; set; }
       // public virtual InventoryItem InventoryItems { get; set; }
      //public virtual SparePart SparePart { get; set; }

    }
}
