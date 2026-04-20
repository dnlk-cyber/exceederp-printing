using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class IfrsDepreciationValidation : VoucherValidation
    {
        public int IfrsDepreciationValidationId { get; set; }
        public int IfrsDepreciationId { get; set; }
        public virtual IfrsDepreciation IfrsDepreciation { get; set; }
    }
}