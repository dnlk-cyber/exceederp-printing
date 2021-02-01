using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Finance;

namespace ExceedERP.Core.Domain.CRM
{

    public class CashSalesInvoice : Operations
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
        public string References { get; set; }
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

        public bool IsVoid { get; set; }

        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }
        [DisplayName("Advance Payment")]
        public decimal AdvancPayment { get; set; }
        [DisplayName("OnHand Payment")]
        public decimal OnHandPayment { get; set; }
        public string Period { get; set; }
        public bool IsReceiptPrinted { get; set; }

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
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        public virtual ICollection<CashSalesPartialItem> CashSalesPartialItem { get; set; }
        public virtual ICollection<SalesInvoicePaymentDistibution> SalesInvoicePaymentDistibution { get; set; }
    }
}
