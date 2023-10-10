using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArStandardCollectionDistribution : VoucherDistribution
    {
        public int ArStandardCollectionDistributionId { get; set; }
        public int ArStandardCollectionId { get; set; }
        public virtual ArStandardCollection ArStandardCollection { get; set; }
    }
}
