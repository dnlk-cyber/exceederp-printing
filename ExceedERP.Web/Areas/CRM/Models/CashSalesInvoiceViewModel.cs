using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Web.Areas.CRM.Models
{

    public class CashSalesInvoiceViewModel
    {
      
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
        public bool IsNew { get; set; }
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

        public string LastObjectState { get; set; }

        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }
        [DisplayName("Receive Method")]
        public string RecieveMethod { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string Status { get; set; }

           }
}
