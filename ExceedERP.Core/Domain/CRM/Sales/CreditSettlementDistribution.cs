using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class CreditSettlementDistribution : VoucherDistribution
    {
        [Key]
        public int CreditSettlementDistributionId { get; set; }
        public int CreditSettlementId { get; set; }

        public virtual CreditSettlement CreditSettlement { get; set; }
    }
}
