using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLRecordJournalValidation : TrackUserOperation
    {
        public int GlRecordJournalValidationId { get; set; }
        public int GlRecordJournalId { get; set; }
        public virtual GlRecordJournal GLRecordJournal { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
