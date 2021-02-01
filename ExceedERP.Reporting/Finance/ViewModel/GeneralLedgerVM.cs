using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class GeneralLedgerVm
    {
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        //for total
        [Display(Name = "Debit")]
        public decimal GrandDebit { get; set; }
        [Display(Name = "Credit")]
        public decimal GrandCredit { get; set; }
        [Display(Name = "Balance")]
        public decimal GrandBalance { get; set; }
        public List<GeneralLedgerVmLines> GeneralLedgerVmLines { get; set; }
        public List<GeneralLedgerVmLastLines> GeneralLedgerVmLastLineses { get; set; }
    }

    public class GeneralLedgerVmLines
    {
        public string Id { get; set; }
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        public DateTime? Date { get; set; }
        public string Reference { get; set; }//system
        public string ReferenceDoc { get; set; }//manual
        public string PaymentReference { get; set; }//receipt or payment reference
        public string InvoiceReferenceDoc { get; set; }//manual
        public string Journal { get; set; }
        [Display(Name = "Transaction Description")]
        public string TransDesc { get; set; }//from additional description
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public int Type { get; set; }
        //for total
        //[Display(Name = "Debit")]
        //public decimal TotalDebit { get; set; }
        //[Display(Name = "Credit")]
        //public decimal TotalCredit { get; set; }
        [Display(Name = "Balance")]
        public decimal TotalBalance { get; set; }
        public string Remark { get; set; }
    }
    public class GeneralLedgerVmLastLines
    {
        public string Id { get; set; }
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        public DateTime? Date { get; set; }
        public string Reference { get; set; }
        public string Journal { get; set; }
        [Display(Name = "Transaction Description")]
        public string TransDesc { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public int Type { get; set; }
        //for total
        //[Display(Name = "Debit")]
        //public decimal TotalDebit { get; set; }
        //[Display(Name = "Credit")]
        //public decimal TotalCredit { get; set; }
        [Display(Name = "Balance")]
        public decimal TotalBalance { get; set; }
        public string Remark { get; set; }
    }

    public class GeneralLedgerAccountVm
    {
        public string Id { get; set; }
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        public DateTime? Date { get; set; }
        public string Reference { get; set; }
        public string Journal { get; set; }
        [Display(Name = "Transaction Description")]
        public string TransDesc { get; set; }
        public decimal BeginningBalance { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public decimal NetChange { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public int Type { get; set; }
    }
}
