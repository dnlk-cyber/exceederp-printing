using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLRecordJournalDetail : TrackUserOperation
    {
        public int GlRecordJournalDetailId { get; set; }
        [Required]
        [Display(Name = "Record JournalID ")]
        public int GlRecordJournalId { get; set; }
        public virtual GlRecordJournal GLRecordJournal { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Range(0.0, 9999999999, ErrorMessage = "Debit can't be negative value")]
        public decimal Debit { get; set; }
        [Range(0.0, 9999999999, ErrorMessage = "Credit can't be negative value")]
        public decimal Credit { get; set; }
        [Display(Name = "Additional Description")]
        public string AdditionalDescription { get; set; }
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
    }
}
