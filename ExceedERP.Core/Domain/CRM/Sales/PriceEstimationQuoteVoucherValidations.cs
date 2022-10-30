using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class PriceEstimationQuoteVoucherFirstValidation : VoucherValidation
    {
        [Key]
        public int PriceEstimationQuoteVoucherFirstValidationId { get; set; }
        public int PriceEstimationQuoteVoucherId { get; set; }

        public virtual PriceEstimationQuoteVoucher PriceEstimationQuoteVoucher { get; set; }
    }

    public class PriceEstimationQuoteVoucherSecondValidation : VoucherValidation
    {
        [Key]
        public int PriceEstimationQuoteVoucherSecondValidationId { get; set; }
        public int PriceEstimationQuoteVoucherId { get; set; }

        public virtual PriceEstimationQuoteVoucher PriceEstimationQuoteVoucher { get; set; }
    }

    public class PriceEstimationQuoteVoucherValidation  : VoucherValidation
    {
        [Key]
        public int PriceEstimationQuoteVoucherValidationId { get; set; }
        public int PriceEstimationQuoteVoucherId { get; set; }

        public virtual PriceEstimationQuoteVoucher PriceEstimationQuoteVoucher  { get; set; }
    }
}
