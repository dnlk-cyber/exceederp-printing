using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class VoucherTerm : TrackUserOperation
    {
        public int VoucherTermID { get; set; }
        public int VoucherDefinitionID { get; set; }
        public string Voucher { get; set; }
        [Required]
        public int TermID { get; set; }
        public virtual Term Term { get; set; }
        [Required]
        public string Value { get; set; }
        public string Remark { get; set; }

    }
}
