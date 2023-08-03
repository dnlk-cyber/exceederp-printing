using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApMiscellaneousPaymentDistribution : VoucherDistribution
    {
        public ApMiscellaneousPaymentDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int ApMiscellaneousPaymentDistributionId { get; set; }
        public int ApMiscellaneousPaymentId { get; set; }
        public virtual ApMiscellaneousPayment ApMiscellaneousPayment { get; set; }
    }
}
