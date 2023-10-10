using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaybleInvoiceDistribution : VoucherDistribution
    {
        public ApPaybleInvoiceDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int ApPaybleInvoiceDistributionId { get; set; }
        public int ApPaybleInvoicesId { get; set; }
        public virtual ApPaybleInvoices ApPaybleInvoices { get; set; }
    }
}
