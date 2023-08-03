using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{

    public enum EarningTypes
    {
        RetainedEarning,
        ProfitTax
    }
    public class GlEarningLedger : TrackUserOperation
    {
        public int GlEarningLedgerId { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Account Type")]
        public EarningTypes Type { get; set; }
        public string AccountSegment { get; set; }
        public string BranchSegment { get; set; }
        public string CostCenterSegment { get; set; }
        public string SubAccountSegment { get; set; }
        public string ChannelSegment { get; set; }
        public string BusinessUnitSegment { get; set; }
        public string Segment1 { get; set; }
        public string Segment2 { get; set; }
        public string Segment3 { get; set; }
        public string Segment4 { get; set; }
        public string Segment5 { get; set; }
        public string Segment6 { get; set; }
        public string Segment7 { get; set; }
        public string Segment8 { get; set; }
        public virtual ICollection<GlEarningLedgerLine> GlEarningLedgerLines { get; set; }
    }

    public class GlEarningLedgerLine : TrackUserOperation
    {
        public int GlEarningLedgerLineId { get; set; }
        public int GlEarningLedgerId { get; set; }
        public virtual GlEarningLedger GlEarningLedger { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal YearlyAmount { get; set; }
    }
}
