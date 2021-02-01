using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public enum BookTransactionType
    {
        Receipt,
        Payment
    }
    public class BankJournalVm
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string ReferenceDoc { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
        public string TransactDesc { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string ReconciliationStatus { get; set; }
        public BookTransactionType Type { get; set; }

    }

   
}
