using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaymentDistribution : VoucherDistribution
    {
        public ApPaymentDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int ApPaymentDistributionId { get; set; }
        public int ApPaymentId { get; set; }
        public virtual ApPayment ApPayment { get; set; }
    }
}
