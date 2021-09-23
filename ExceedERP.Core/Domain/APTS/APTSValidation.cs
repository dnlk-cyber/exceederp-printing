using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.APTS
{
    public class APTSValidation : VoucherValidation
    {
        public int APTSValidationId { get; set; }
        public int DrugReceivalModel19Id { get; set; }
        public virtual DrugReceivalModel19 DrugReceivalModel19 { get; set; }
    
    }
}
