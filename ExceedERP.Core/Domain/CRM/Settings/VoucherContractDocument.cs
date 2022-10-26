using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class VoucherContractDocument : TrackUserOperation
    {
        [Key]
        public int VoucherContractDocumentId { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string Remark { get; set; }

        [Display(Name ="Voucher")]
        public int VoucherId { get; set; }
        public VoucherType VoucherType { get; set; }

        //public IEnumerable<HttpPostedFileBase> files { get; set; }

    }
}
