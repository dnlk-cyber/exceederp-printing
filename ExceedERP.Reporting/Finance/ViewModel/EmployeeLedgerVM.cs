using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class EmployeeLedgerVm
    {
        [Display(Name = "Account ID.")]
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public List<TransactionVm> EmployeeLedgerVmLines { get; set; }
    }

    
}
