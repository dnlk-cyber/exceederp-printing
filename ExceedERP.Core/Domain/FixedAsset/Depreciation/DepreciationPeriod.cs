using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class DepreciationPeriod : TrackUserOperation
    {
        public int DepreciationPeriodId { get; set; }
        public int Period { get; set; }
        public int FiscalYear { get; set; }
        public string StoreCode { get; set; }
        public DepreciationStatus Status { get; set; }

    }
}