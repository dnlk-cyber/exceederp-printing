using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public  class CreditSettlementValidation : VoucherValidation
    {
        [Key]
        public int CreditSettlementValidationId { get; set; }
        public int CreditSettlementId { get; set; }

        public virtual CreditSettlement CreditSettlement { get; set; }

    }
}
