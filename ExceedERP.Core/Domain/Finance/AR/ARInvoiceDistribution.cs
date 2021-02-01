using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArInvoiceDistribution : VoucherDistribution
    {
        public int ArInvoiceDistributionId { get; set; }
        public int ArInvoiceId { get; set; }
        public virtual ArInvoice ArInvoice { get; set; }
    }
}
