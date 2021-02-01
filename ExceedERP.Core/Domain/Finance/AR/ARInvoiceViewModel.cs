using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArInvoiceViewModel
    {
        public int ArInvoiceViewModelId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }
        [Required]
        [Display(Name = "Transaction Class")]
        public string TransactionClass { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Payment Term (Days)")]
        public string PaymentTerm { get; set; }
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        [Display(Name = "Line Amount")]
        public decimal LineAmount { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Amount Date")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Invoice Status")]
        public string InvoiceStatus { get; set; }
    }
}
