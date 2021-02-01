using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class InventoryCountVariationHeaderVm:Organization
    {
        public string VoucherId { get; set; }
        public string StoreKeeper { get; set; }
        
    }
    public class InventoryCountVariationVm
    {
        public List<VariationItemViewModel> VariationItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
        public decimal CategoryShortageTotal { get; set; }
        public decimal CategoryOverageTotal { get; set; }
    }
    public class VariationItemViewModel
    {
       
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        public string ItemLocation { get; set; }
        [DisplayName("Unit")]
        public string UoM { get; set; }
        [DisplayName("Record Qty.")]
        public decimal PreviousBalance { get; set; }
        [DisplayName("Physical Count")]
        public decimal Count { get; set; }
        [DisplayName("Cost")]
        public decimal Price { get; set; }
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

    }

    public class VariationSummaryVm
    {
        public List<VariationItemViewModel> VariationItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal RecordAmount { get; set; }
        public decimal PhysicalAmount { get; set; }
        public decimal ShortageAmount { get; set; }
        public decimal OverageAmount { get; set; }
    }
}
