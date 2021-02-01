using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.Procurement
{
    public enum SourceType
    {
        GRN,
        External
    }
    public class ItemPriceIndex : TrackUserOperation
    {
        public int ItemPriceIndexID { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }//for display and search case
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Price Before VAT")]
        public decimal PriceBeforeVAT { get; set; }
        [Display(Name = "Source of Data")]
        public string SourceOfData { get; set; }
        public string Remark { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Of Information")]
        public DateTime DateOfInformation { get; set;  }
        public SourceType SourceType { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

    }
}
