using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaybleInvoicesLines : TrackUserOperation
    {
        public int ApPaybleInvoicesLinesId { get; set; }
        public int ApPaybleInvoicesId { get; set; }
        public virtual ApPaybleInvoices APPaybleInvoices { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Invoice Amount")]
        public decimal InvoiceAmount { get; set; }
        [Display(Name = "Advance Amount")]
        public decimal AdvanceAmount { get; set; }
        [Display(Name = "Net Payable")]
        public decimal NetPayable { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; }
        [Required]
        [Display(Name = "Tax Type")]
        public string TaxType { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        //display
        [Display(Name = "PO.No/WO.No")]
        public string PoNo { get; set; }

        [Display(Name = "Grn No")]
        public string GrnNo { get; set; }
    }
}
