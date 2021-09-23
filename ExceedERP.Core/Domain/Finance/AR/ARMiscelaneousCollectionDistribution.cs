using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARMiscelaneousCollectionDistribution : VoucherDistribution
    {
        public int ARMiscelaneousCollectionDistributionID { get; set; }
        public int ARMiscelaneousCollectionID { get; set; }
        public virtual ARMiscelaneousCollection ARMiscelaneousCollection { get; set; }
    }
}
