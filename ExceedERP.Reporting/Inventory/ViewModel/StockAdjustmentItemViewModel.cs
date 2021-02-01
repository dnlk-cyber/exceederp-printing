using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class StockAdjustmenVm
    {
        public List<StockAdjustmentItemViewModel> StockAdjustmenItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class StockAdjustmentItemViewModel
    {
        public DateTime AdjustmentDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
        [Display(Name = "Balance")]
        public decimal BinCardBalance { get; set; }
        [DisplayName("Adjusted Price")]
        public decimal AdjustedPrice { get; set; }
        [DisplayName("Stock Adjustment Type")]
        public string StockAdjustmentType { get; set; }
        [DisplayName("Price Adjustment Type")]
        public string PriceAdjustmentType { get; set; }
        [DisplayName("Adjusted Quantity")]
        public int AdjustedQuantity { get; set; }
        [Display(Name = "Store")]
        public string StoreName { get; set; }
    }
}