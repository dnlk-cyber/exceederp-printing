using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLLedgerPosting : TrackUserOperation
    {
        public int GLLedgerPostingID { get; set; }
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
        public string BusinessUnit { get; set; }
        public string Segment1 { get; set; }
        public string Segment2 { get; set; }
        public string Segment3 { get; set; }
        public string Segment4 { get; set; }
        public string Segment5 { get; set; }
        public string Segment6 { get; set; }
        public string Segment7 { get; set; }
        public string Segment8 { get; set; }
        public virtual ICollection<GLLedgerPostingLines> GLLedgerPostingLines { get; set; }
    }
}
