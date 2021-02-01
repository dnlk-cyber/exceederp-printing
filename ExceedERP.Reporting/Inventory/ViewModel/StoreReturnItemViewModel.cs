using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class StoreReturnVm
    {
        public List<StoreReturnItemViewModel> StoreReturnItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class StoreReturnItemViewModel
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public string BranchOrProject { get; set; }
        public string WorkUnit { get; set; }
        public DateTime ReturnDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        [Display(Name = "Store")]
        public string StoreName { get; set; }
    }
}