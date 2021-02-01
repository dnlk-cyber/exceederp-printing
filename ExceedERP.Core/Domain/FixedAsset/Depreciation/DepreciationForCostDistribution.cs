using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class DepreciationForCostDistribution : VoucherDistribution
    {
        public int DepreciationForCostDistributionId { get; set; }
        public int DepreciationForCostId { get; set; }
        public virtual DepreciationForCost DepreciationForCost { get; set; }
    }
}