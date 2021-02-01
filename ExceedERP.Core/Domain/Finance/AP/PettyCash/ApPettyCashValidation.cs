using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PettyCash
{
    public class ApPettyCashValidation : VoucherValidation
    {
        public int ApPettyCashValidationId { get; set; }
        public int ApPettyCashId { get; set; }
        public virtual ApPettyCash PettyCash { get; set; }
       
    }
}
