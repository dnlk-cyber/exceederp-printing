using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class IfrsDepreciationDistribution : VoucherDistribution
    {
        public int IfrsDepreciationDistributionId { get; set; }
        public int IfrsDepreciationId { get; set; }
        public virtual IfrsDepreciation IfrsDepreciation { get; set; }
    }
}