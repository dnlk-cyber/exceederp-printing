using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class GlJournalViewModel : Organization

    {
        public string VoucherId { get; set; }
        public string JournalName { get; set; }
        public string Description { get; set; }
        public string FiscalYear { get; set; }
        public string PeriodName { get; set; }
        public string CategoryName { get; set; }
        public string SourcesName { get; set; }
        public string JournalNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string TransferFrom { get; set; }
        public string TransferTo { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class GlJournalLinesViewModel 
    {
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string AdditionalDescription { get; set; }
    }
}
