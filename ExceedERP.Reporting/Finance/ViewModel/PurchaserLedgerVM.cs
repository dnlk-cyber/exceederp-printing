using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class PurchaserLedgerVm
    {
        [Display(Name = "Account ID.")]
        public string PurchaserId { get; set; }
        [Display(Name = "Purchaser Name")]
        public string PurchaserName { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public List<TransactionVm> PurchaserLedgerVmLines { get; set; }
    }

    
}
