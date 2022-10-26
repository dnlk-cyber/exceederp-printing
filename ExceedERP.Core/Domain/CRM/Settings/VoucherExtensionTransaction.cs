using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class VoucherExtensionTransaction : TrackUserOperation
    {
        public int VoucherExtensionTransactionID { get; set; }

        public string Voucher { get; set; }

        [Required]
        public string VoucherExtension { get; set; }
        [Required]
        public string Number { get; set; }

        public string Remark { get; set; }

    }
}
