using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class PurchaseReturnVm
    {
        public List<PurchaseReturnItemViewModel> PurchaseReturnItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class PurchaseReturnItemViewModel
    {
        public DateTime? ReturnDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        [Display(Name = "Store")]
        public string StoreName { get; set; }

    }
}