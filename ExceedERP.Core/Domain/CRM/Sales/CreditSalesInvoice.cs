using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class CreditSalesInvoice : Operations
    {
        [Key]
        public int CreditSalesInvoiceID { get; set; }
        public string Consignee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        public bool IsVoid { get; set; }

        public decimal GrandTotal { get; set; }

        public string Period { get; set; }

        public string LastObjectState { get; set; }


        [UIHint("PaymentMethodType")]
        public PaymentMethodType PaymentMethod { get; set; }
        public string Remark { get; set; }


        public bool IsManual { get; set; }

        [Display(Name ="Receipt Reference")]
        public string ReceiptReference { get; set; }

        [Display(Name = "Payment Term")]
        public int PaymentTermsID { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }
    }
   

}
