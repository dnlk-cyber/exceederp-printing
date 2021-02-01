using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArInvoiceValidation : TrackUserOperation
    {
        public int ArInvoiceValidationId { get; set; }
        public int ArInvoiceId { get; set; }
        public virtual ArInvoice ArInvoice { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
