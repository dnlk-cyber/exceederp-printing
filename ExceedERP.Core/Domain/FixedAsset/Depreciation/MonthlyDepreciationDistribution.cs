using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class MonthlyDepreciationDistribution : VoucherDistribution
    {
        public int MonthlyDepreciationDistributionId { get; set; }
        public int MonthlyDepreciationId { get; set; }
        public virtual MonthlyDepreciation MonthlyDepreciation { get; set; }
    }
}