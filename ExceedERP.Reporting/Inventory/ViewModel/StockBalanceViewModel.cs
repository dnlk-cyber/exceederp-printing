using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{


    public class StockBalanceViewModel
    {
        [Display(Name = "Item Category")]
        public string ItemCategory { get; set; }
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        [Display(Name = "Item Name")]
        public string InventoryItem { get; set; }
        public decimal Balance { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}