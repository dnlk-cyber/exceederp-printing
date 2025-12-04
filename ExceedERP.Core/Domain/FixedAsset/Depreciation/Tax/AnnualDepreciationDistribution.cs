using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class AnnualDepreciationDistribution : VoucherDistribution
    {
        public int AnnualDepreciationDistributionId { get; set; }
        public int AnnualDepreciationId { get; set; }
        public virtual AnnualDepreciation AnnualDepreciation { get; set; }
    }
}