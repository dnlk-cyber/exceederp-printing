using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.SubLedger.ViewModel
{
    public class GlJournalreconcileSubLedgerViewModel
    {
        [Display(Name = "Journal Number")]
        public string JournalNumber { get; set; }
        [Display(Name = "Source")]
        public string Source { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Journal Category")]
        public string JournalCategory { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
    public class GlJournalStatusViewModel
    {
        [Display(Name = "Journal Category")]
        public string JournalCategory { get; set; }
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Journal Number")]
        public string JournalNumber { get; set; }
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
    public class ArGlTransferViewModel
    {
        [Display(Name = "Journal Category")]
        public string JournalCategory { get; set; }
        [Display(Name = "Transaction Number")]
        public string TransactionNumber { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public string Reference { get; set; }
    }

    public class ApGlTransferViewModel
    {
        [Display(Name = "Journal Category")]
        public string JournalCategory { get; set; }
        [Display(Name = "Transaction Number")]
        public string TransactionNumber { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public string Reference { get; set; }
    }

    public class InventoryGlTransferViewModel
    {
        [Display(Name = "Transaction Number")]
        public string TransactionNumber { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
    }

    public class ApJournalEntriesCategoryViewModel 
    {
        public string Category { get; set; }
        public List<ApJournalEntriesOdsViewModel> ApJournalEntriesOdsViewModels { get; set; }
    }
    public class ApJournalEntriesOdsViewModel
    {
        [Display(Name = "Supplier")]
        public string Supplier { get; set; }
        [Display(Name = "Transaction Number")]
        public string TransactionNumber { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public string Reference { get; set; }
        public string PaymentReference { get; set; }
    }

    public class ArJournalEntriesCategoryViewModel
    {
        public string Category { get; set; }
        public List<ArJournalEntriesOdsViewModel> ArJournalEntriesOdsViewModels { get; set; }
    }
    public class ArJournalEntriesOdsViewModel 
    {
       
        [Display(Name = "Customer")]
        public string Customer { get; set; }
        [Display(Name = "Transaction Number")]
        public string TransactionNumber { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public string Reference { get; set; }
        public string CollectionReference { get; set; }
    }
}
