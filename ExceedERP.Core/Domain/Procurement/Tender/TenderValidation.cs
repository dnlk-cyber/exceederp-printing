using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public class TenderValidation : VoucherValidation
    {
        public int TenderValidationID { get; set; }
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
    }
}
