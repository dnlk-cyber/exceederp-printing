using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArStandardCollectionDistribution : VoucherDistribution
    {
        public int ArStandardCollectionDistributionId { get; set; }
        public int ArStandardCollectionId { get; set; }
        public virtual ArStandardCollection ArStandardCollection { get; set; }
    }
}
