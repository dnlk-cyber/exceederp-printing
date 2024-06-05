using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

namespace ExceedERP.Core.Domain.FleetManagement.RentalPayment
{
    public class RentalPaymentRequestValidation : VoucherValidation
    {
        public int RentalPaymentRequestValidationId { get; set; }
        public int RentalPaymentRequestId { get; set; }
        public virtual RentalPaymentRequest RentalPaymentRequest { get; set; }
    }

    public class RentalPaymentRequestDepartmentValidation : VoucherValidation
    {
        public int RentalPaymentRequestDepartmentValidationId { get; set; }
        public int RentalPaymentRequestId { get; set; }
        public virtual RentalPaymentRequest RentalPaymentRequest { get; set; }
    }
    public class RentalPaymentRequestBudgetValidation : VoucherValidation
    {
        public int RentalPaymentRequestBudgetValidationId { get; set; }
        public int RentalPaymentRequestId { get; set; }
        public virtual RentalPaymentRequest RentalPaymentRequest { get; set; }
    }
}
