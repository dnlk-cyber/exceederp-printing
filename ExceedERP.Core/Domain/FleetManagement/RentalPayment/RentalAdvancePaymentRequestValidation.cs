using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

namespace ExceedERP.Core.Domain.FleetManagement.RentalPayment
{
    public class RentalAdvancePaymentRequestValidation : VoucherValidation
    {
        public int RentalAdvancePaymentRequestValidationId { get; set; }
        public int RentalAdvancePaymentRequestId { get; set; }
        public virtual RentalAdvancePaymentRequest RentalAdvancePaymentRequest { get; set; }
    }

    public class RentalAdvancePaymentRequestDepartmentValidation : VoucherValidation
    {
        public int RentalAdvancePaymentRequestDepartmentValidationId { get; set; }
        public int RentalAdvancePaymentRequestId { get; set; }
        public virtual RentalAdvancePaymentRequest RentalAdvancePaymentRequest { get; set; }
    }
    public class RentalAdvancePaymentRequestBudgetValidation : VoucherValidation
    {
        public int RentalAdvancePaymentRequestBudgetValidationId { get; set; }
        public int RentalAdvancePaymentRequestId { get; set; }
        public virtual RentalAdvancePaymentRequest RentalAdvancePaymentRequest { get; set; }
    }
   
}
