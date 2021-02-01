using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PettyCash
{
    public class ApPettyCashDistribution : VoucherDistribution
    {
        public int ApPettyCashDistributionId { get; set; }
        public int ApPettyCashId { get; set; }
        public virtual ApPettyCash PettyCash { get; set; }
    }
}
