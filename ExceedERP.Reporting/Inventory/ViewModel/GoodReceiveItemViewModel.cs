using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class GoodReceiveVm
    {
        public List<GoodReceiveItemViewModel> GoodReceiveItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class GoodReceiveItemViewModel
    {
        public string Id { get; set; }
        public string Supplier { get; set; }
        public string Reference { get; set; }
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
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