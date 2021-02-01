using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class SupplierLedgerVm
    {
        [Display(Name = "Supplier ID.")]
        public string SupplierId { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public List<TransactionVm> SupplierLedgerVmLines { get; set; }
    }
}
