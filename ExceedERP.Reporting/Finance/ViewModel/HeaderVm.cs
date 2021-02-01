using System;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class GeneralLedgerHeaderVm : Organization
    {
        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class BankJournalHeaderVm : Organization
    {
        public string BankAccount { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class BankReconciliationHeaderVm : Organization
    {
        public string BankAccount { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PeriodName { get; set; }
    }

}