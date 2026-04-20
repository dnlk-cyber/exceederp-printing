using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class AnnualDepreciationValidation : VoucherValidation
    {
        public int AnnualDepreciationValidationId { get; set; }
        public int AnnualDepreciationId { get; set; }
        public virtual AnnualDepreciation AnnualDepreciation { get; set; }
    }
}