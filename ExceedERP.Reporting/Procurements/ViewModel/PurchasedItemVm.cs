using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class PurchasedItemVm
    {
        public string Id { get; set; }
        public string Supplier { get; set; }
        public string Reference { get; set; }
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Item")]
        public string ItemDescription { get; set; }
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
