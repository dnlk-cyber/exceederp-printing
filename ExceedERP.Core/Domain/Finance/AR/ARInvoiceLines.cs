using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArInvoiceLines : TrackUserOperation
    {
        public int ArInvoiceLinesId { get; set; }
        public int ArInvoiceId { get; set; }
        public virtual ArInvoice ArInvoice { get; set; }
        [Required]
        [Display(Name = "Items (Product or Service)")]
        public string Items { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        [Required]
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public int ArSalesOrderLineId { get; set; }//partial line item
    }
}
