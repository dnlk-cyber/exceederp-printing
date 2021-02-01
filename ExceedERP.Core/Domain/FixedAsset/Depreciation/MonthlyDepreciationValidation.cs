using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class MonthlyDepreciationValidation: VoucherValidation
    {
        public int MonthlyDepreciationValidationId { get; set; }
        public int MonthlyDepreciationId { get; set; }
        public virtual MonthlyDepreciation MonthlyDepreciation { get; set; }
    }
}