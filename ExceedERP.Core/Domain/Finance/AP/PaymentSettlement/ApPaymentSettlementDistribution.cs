using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaymentSettlementDistribution : VoucherDistribution
    {
        public ApPaymentSettlementDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int ApPaymentSettlementDistributionId { get; set; }
        public int ApPaymentSettlementId { get; set; }
        public virtual ApPaymentSettlement ApPaymentSettlement { get; set; }
    }
}
