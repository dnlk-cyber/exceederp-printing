using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class CustomerLedgerVm
    {
        [Display(Name = "Customer ID.")]
        public string CustomerId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public List<TransactionVm> CustomerLedgerVmLines { get; set; }
    }

    
}


