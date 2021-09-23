using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class CashSalesPartialItem : Operations
    {
        public CashSalesPartialItem()
        {
            this.CashSalesPartialLineItem = new HashSet<CashSalesPartialLineItem>();

        }
        public int CashSalesPartialItemId { get; set; }
        public int CashSalesInvoiceId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public bool IsReceiptPrinted { get; set; }
        public bool IsPaymentProcessed { get; set; }
        [NotMapped]
        public string PaymentStatus { get; set; }
        public virtual CashSalesInvoice CashSalesInvoice { get; set; }
        public virtual ICollection<CashSalesPartialLineItem> CashSalesPartialLineItem { get; set; }

    }


}
