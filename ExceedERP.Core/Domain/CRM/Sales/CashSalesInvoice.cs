using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{

    public class CashSalesInvoice : SalesInvoiceOperationBase
    {
        public CashSalesInvoice()
        {
            this.SalesInvoicePaymentDistibution = new HashSet<SalesInvoicePaymentDistibution>();
            this.CashSalesPartialItem = new HashSet<CashSalesPartialItem>();
        }
        [Key]
        public int CashSalesInvoiceID { get; set; }
        
       
        [DisplayName("Customer")]
        public int OrganizationCustomerId { get; set; }
        [Display(Name ="Reference (SO #)")]
        public string References { get; set; }
        [NotMapped]
        //public InvoiceReferenceSelectViewModel[] ReferencedSalesOrderIds { get; set; } = new InvoiceReferenceSelectViewModel[] { };
        public List<InvoiceReferenceSelectViewModel> ReferencedSalesOrderIds { get; set; } = new List<InvoiceReferenceSelectViewModel>();

        [DisplayName("Issued Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }
        public bool IsClosed { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }
        public bool IsVoid { get; set; }

        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }
        [DisplayName("Advance Payment")]
        public decimal AdvancPayment { get; set; }
        [DisplayName("OnHand Payment")]
        public decimal OnHandPayment { get; set; }
        public decimal AmountDue { get; set; }
        public string Period { get; set; }

        [Display(Name ="Status")]
        public string LastObjectState { get; set; }

        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }
        [DisplayName("Receive Method")]
        public string RecieveMethod { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public bool IsNew { get; set; }

        [NotMapped]
        [DisplayName("Customer Name")]
        public string NewCustomer { get; set; }

        [NotMapped]
        [DisplayName("Customer TIN")]
        public string CustomerTINNumber { get; set; }

        // this flag used to indicate wheter its paid in cash or bank
        [Display(Name ="Is Paid with Cash?")]
        public bool IsPaidWithCash { get; set; }

        [Display(Name ="Is Manual Sale?")]
        public bool IsManual { get; set; }

        [Display(Name = "Receipt Reference")]
        public string ReceiptReference { get; set; }

        [Display(Name ="Payment Term")]
        public int PaymentTermsID { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [DisplayName("Store")]
        public int StoreID { get; set; }
        [Display(Name ="Period")]
        public int CRMPeriodId { get; set; }

        public SalesInvoiceSource Source { get; set; }
        [Display(Name ="Shipment Generation Method")]
        public ShipmentGenerationMethod ShipmentGenerationMethod { get; set; }

        //
        // for POS Integration
        /// <summary>
        /// holds invoice number (unique for inoices printed from CRM and for offline printed invoices separetly)
        /// for CRM invoices, its the same with the primary key value
        /// for offline invoices, it comes from the restful api 
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// shows whether the invoice is created on CRM (online) or its imported from a specific POS 
        /// </summary>
        public PosPrintType PosPrintType { get; set; }

        [Display(Name ="FS Invoice No.")]
        public string FsInvoiceNo { get; set; }
        public string EJNo { get; set; }
        public string MachineID { get; set; }

        /// <summary>
        /// flag to hold if this invoice record is completely shipped or not
        /// </summary>
        public bool IsShipped { get; set; }

        public bool IsPosSyncLocked { get; set; }


        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        public virtual ICollection<CashSalesPartialItem> CashSalesPartialItem { get; set; }
        public virtual ICollection<SalesInvoicePaymentDistibution> SalesInvoicePaymentDistibution { get; set; }
        [NotMapped]
        public bool CanBeDeletedIfDuplicate { get; set; }

        /// <summary>
        /// maintenance job order number
        /// </summary>
        public string JobOrderNumber { get; set; }
        //public bool IsFromMaintenance { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }

    }

    public class InvoiceReferenceSelectViewModel
    {
        public string Display { get; set; }
        public int Value { get; set; }
    }
}
