using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
   

}
