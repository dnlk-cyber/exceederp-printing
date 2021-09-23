using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesQuoteVoucher : Operations
    {
        [Key]
        public int SalesQuoteVoucherID { get; set; }
   
        public int OrganizationCustomerId   { get; set; }
        [DisplayName("Issued Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        public bool IsVoid { get; set; }

        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }

        public string Period { get; set; }

        public string LastObjectState { get; set; }


        [DisplayName("Grand Total")]
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        

        [NotMapped]
        public string Status { get; set; }
    }

}
