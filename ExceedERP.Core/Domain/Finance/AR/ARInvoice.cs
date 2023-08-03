using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArInvoice : Operations
    {
        public int ArInvoiceId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }
        [Required]
        [Display(Name = "Transaction Class")]
        public TransactionClass TransactionClass { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Payment Term")]
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
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Invoice Status")]
        public InvoiceStatus InvoiceStatus { get; set; }
        [Display(Name = "Sales Order")]
        public int ArSalesOrderId { get; set; }
        public int ArSalesOrderPartialPrintId { get; set; }
        public string Reference { get; set; }
        public bool IsManual { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [DisplayName("Invoice No.")]
        public int? SourceInvoiceId { get; set; }//for credit and debit memo

        // Flag used to show if this invoice is fully used (example, in daily advert transmission
        // schedule if IsFullFilled is true, it means there is no remaining quantity)
        public bool IsFullFilled { get; set; }
        public virtual ICollection<ArInvoiceLines> ArInvoiceLines { get; set; }
        public virtual ICollection<ArStandardCollectionViewModel> ArStandardCollectionViewModel { get; set; }
        public virtual ICollection<ArInvoiceDistribution> ArInvoiceDistribution { get; set; }
        public virtual ICollection<ArInvoiceValidation> ArInvoiceValidation { get; set; }
    }
}
