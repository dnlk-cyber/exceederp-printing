using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesQuoteVoucherFirstValidation: VoucherValidation
    {
        [Key]
        public int SalesQuoteVoucherFirstValidationId { get; set; }
        public int SalesQuoteVoucherId { get; set; }

        public virtual SalesQuoteVoucher SalesQuoteVoucher { get; set; }
    }
    public class SalesQuoteVoucherSecondValidation: VoucherValidation
    {
        [Key]
        public int SalesQuoteVoucherSecondValidationId { get; set; }
        public int SalesQuoteVoucherId { get; set; }

        public virtual SalesQuoteVoucher SalesQuoteVoucher { get; set; }
    }
    public class SalesQuoteVoucherValidation : VoucherValidation
    {
        [Key]
        public int SalesQuoteVoucherValidationId { get; set; }
        public int SalesQuoteVoucherId { get; set; }

        public virtual SalesQuoteVoucher SalesQuoteVoucher { get; set; }
    }
}
