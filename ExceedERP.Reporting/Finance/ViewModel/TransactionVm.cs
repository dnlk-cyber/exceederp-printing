using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class TransactionVm
    {
        public string TranssactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }

        [Display(Name = "Transaction Reference")]
        public string TransReference { get; set; }
        public string ReferenceDoc { get; set; }//manual
        public string PaymentReference { get; set; }//receipt or payment reference
        public string InvoiceReferenceDoc { get; set; }//manual
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public string Remark { get; set; }
    }
}
