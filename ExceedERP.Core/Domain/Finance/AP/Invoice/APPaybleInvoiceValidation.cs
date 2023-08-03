using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaybleInvoiceValidation : TrackUserOperation
    {
        public int ApPaybleInvoiceValidationId { get; set; }
        public int ApPaybleInvoicesId { get; set; }
        public virtual ApPaybleInvoices APPaybleInvoices { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
