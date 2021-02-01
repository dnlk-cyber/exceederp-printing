using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
   
    public class GlSecondayLedger : TrackUserOperation
    {
        public int GlSecondayLedgerId { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Account Type")]
        public GlAccountTypes AccountType { get; set; }
        public string AccountSegment { get; set; }
        public string BranchSegment { get; set; }
        public string CostCenterSegment { get; set; }
        public string SubAccountSegment { get; set; }
        public string ChannelSegment { get; set; }
        public string Segment1 { get; set; }
        public string Segment2 { get; set; }
        public string Segment3 { get; set; }
        public string Segment4 { get; set; }
        public string Segment5 { get; set; }
        public string Segment6 { get; set; }
        public string Segment7 { get; set; }
        public string Segment8 { get; set; }
        public virtual ICollection<GlSecondayLedgerLine> GlSecondayLedgerLines { get; set; }
    }

    public class GlSecondayLedgerLine : TrackUserOperation
    {
        public int GlSecondayLedgerLineId { get; set; }
        public int GlSecondayLedgerId { get; set; }
        public virtual GlSecondayLedger GlSecondayLedger { get; set; }
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
        public JournalSources PostingSource { get; set; }
    }
}
