using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public enum LedgerPostTypes
    {
        Standard,
        Begining,
        Ending,
        NA
    }
    public class GLLedgerPostingLines : TrackUserOperation
    {
        public int GLLedgerPostingLinesID { get; set; }
        public int GLLedgerPostingID { get; set; }
        public virtual GLLedgerPosting GLLedgerPosting { get; set; }
        [Display(Name = "Type")]
        public LedgerPostTypes Type { get; set; }
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Journal Number")]
        public string JournalNumber { get; set; }
        [Display(Name = "Journal Reference")]
        public string JournalReference { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
        [Display(Name = "Sources")]
        public string SourcesName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string AdditionalDescription { get; set; }//comes from distribution line
        public string Remark { get; set; }
        public string ReferenceDoc { get; set; }//manual
        public string PaymentReference { get; set; }//receipt or payment reference
        public string InvoiceReferenceDoc { get; set; }//manual
    }
}
