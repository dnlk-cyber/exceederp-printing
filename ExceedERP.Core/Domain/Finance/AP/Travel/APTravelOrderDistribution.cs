using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelOrderDistribution : VoucherDistribution
    {
        public int ApTravelOrderDistributionId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }
    }
}
