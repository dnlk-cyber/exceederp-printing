using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARSetupTaxRate : TrackUserSettingOperation
    {
        public int ARSetupTaxRateID { get; set; }
        [Required]
        [Display(Name = "Tax Type")]
        public string Tax { get; set; }
        [Required]
        [Display(Name = "Tax Rate (%)")]
        public decimal Rate { get; set; }
    }
}
