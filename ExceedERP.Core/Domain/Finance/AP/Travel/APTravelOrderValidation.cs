using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelOrderValidation : VoucherValidation
    {
        public int ApTravelOrderValidationId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }
       
    }

    public class ApTravelOrderDepartmentValidation : VoucherValidation
    {
        public int ApTravelOrderDepartmentValidationId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }

    }
    public class ApTravelOrderBudgetValidation : VoucherValidation
    {
        public int ApTravelOrderBudgetValidationId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }

    }
}
