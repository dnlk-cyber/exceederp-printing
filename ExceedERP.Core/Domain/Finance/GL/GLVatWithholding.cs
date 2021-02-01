using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLVatWithholding : TrackUserSettingOperation
    {
        public int GLVatWithholdingID { get; set; }
        [Required]
        [Display(Name = "VAT Withholding Account Code")]
        public string WithholdingAccountCode { get; set; }

        [Display(Name = "Branch")]
        public string Branch { get; set; }
    }
}
