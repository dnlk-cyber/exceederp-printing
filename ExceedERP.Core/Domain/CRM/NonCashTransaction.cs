using System;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class NonCashTransaction : Operations
    {
        public int NonCashTransactionID { get; set; }

        public string Voucher { get; set; }
        [DisplayName(" Customer ")]
        public string Consignee { get; set; }

        public bool IsIncoming { get; set; }
        [DisplayName(" Receive Method ")]
        public string ReceiveMethod { get; set; }
        [DisplayName(" Receive Processor")]
        public string ReceiveProcessor { get; set; }

        public decimal Index { get; set; }
           [DisplayName(" Issue Date ")]
        public DateTime IssueDate { get; set; }
           [DisplayName(" Maturity Date")]
        public DateTime MaturityDate { get; set; }

        public int Number { get; set; }

        public decimal Amount { get; set; }

        public bool Executed { get; set; }

        public string CurrencyCode { get; set; }

        public string Remark { get; set; }

    }
}
