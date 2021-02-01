using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLTaxRate : TrackUserOperation
    {
        public int GLTaxRateID { get; set; }
        [Required]
        [Display(Name = "Tax Name")]
        public string TaxName { get; set; }
        [Required]
        [Display(Name = "Tax Rate (%)")]
        public decimal Rate { get; set; }
        [Display(Name = "AR")]
        public bool ApplyToAr { get; set; }
        [Display(Name = "AP")]
        public bool ApplyToAp { get; set; }
        [Display(Name = "Withholding")]
        public bool ApplyToWithholding { get; set; }
        [Display(Name = "AR Account Code")]
        public string ArAccountCode { get; set; }
        [Display(Name = "AP Account Code")]
        public string ApAccountCode { get; set; }
        [Display(Name = "Withholding Account Code")]
        public string WithholdingAccountCode { get; set; }
        [Display(Name = "Enable ?")]
        public bool Enable { get; set; }
        public decimal CalculatedRate { get; set; }
    }
}
