using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Inventory
{
    public class PropertyCount:Operations
    {
        public int PropertyCountId { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        [DisplayName("Store")]
        public string StoreCode { get; set; }
        public InventoryStore Store { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CountDate { get; set; }
        public string StoreKeeperId { get; set; }
       
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }

        [DisplayName("Mid of Period?")]
        public bool IsMidOfPeriod { get; set; }//mid of term count
        public bool IsClosed { get; set; }
        public virtual List<PropertyCountItem> PropertyCountItems { get; set; }
    }

    public class PropertyCountItem: TrackUserSettingOperation
    {
        public int PropertyCountItemId { get; set; }
        public int PropertyCountId { get; set; }
        public virtual PropertyCount PropertyCount { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [DisplayName("Record Qty.")]
        public decimal PreviousBalance { get; set; }
        [DisplayName("Physical Count")]
        public decimal Count { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public bool Updated { get; set; }
        [DisplayName("Cost")]
        public decimal Price { get; set; }
        public string Location { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        public string ItemCodeReadOnly { get; set; }//for sort
    }

    public class PropertyCountVm
    {
        public int PropertyCountItemId { get; set; }
        public int PropertyCountId { get; set; }
        public virtual PropertyCount PropertyCount { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [DisplayName("Physical Qty.")]
        public decimal Count { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Unit")]
        public string Uom { get; set; }
    }

    public class VariationVM : PropertyCountItem
    {
        [DisplayName("Shortage Qty.")]
        public decimal ShortageQty { get; set; }
        [DisplayName("Overage Qty.")]
        public decimal OverageQty { get; set; }
        [DisplayName("Shortage Cost")]

        public decimal ShortagePrice { get; set; }
        [DisplayName("Overage Cost")]

        public decimal OveragePrice { get; set; }
        [DisplayName("Physical Amount")]
        public decimal Amouont { get; set; }
        public decimal TotalAmount { get; set; }
        [DisplayName("Amount")]
        public decimal PhysicalAmount { get; set; }
        [DisplayName("Unit")]
        public string Uom { get; set; }
        [DisplayName("Item Description")]
        public string ItemDescription { get; set; }

    }
    
}
