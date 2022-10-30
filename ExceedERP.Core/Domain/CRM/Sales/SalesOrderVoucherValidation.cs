using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesOrderVoucherValidation : VoucherValidation
    {
        [Key]
        public int SalesOrderVoucherValidationId { get; set; }
        public int SalesOrderVoucherId { get; set; }
        public virtual SalesOrderVoucher SalesOrderVoucher { get; set; }

    }
}
