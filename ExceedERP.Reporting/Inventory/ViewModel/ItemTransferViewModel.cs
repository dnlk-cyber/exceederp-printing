using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{

    public class ItemTransferVm
    {
        public List<ItemTransferViewModel> ItemTransferList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotalSent { get; set; }
        public decimal CategoryTotalReceived { get; set; }
    }
    public class ItemTransferViewModel
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime TransferDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
        [Display(Name = "From Store")]
        public string FromStore { get; set; }
        [Display(Name = "To Store")]
        public string Unit { get; set; }
        public string ToStore { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ShippedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UPrice { get; set; }
        public decimal TPrice { get; set; }
    }

    
}