using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
