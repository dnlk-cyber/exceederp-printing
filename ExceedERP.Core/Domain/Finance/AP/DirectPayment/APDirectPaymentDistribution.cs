using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApDirectPaymentDistribution : VoucherDistribution
    {
        public ApDirectPaymentDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int ApDirectPaymentDistributionId { get; set; }
        public int ApDirectPaymentId { get; set; }
        public virtual ApDirectPayment ApDirectPayment { get; set; }
    }
}
