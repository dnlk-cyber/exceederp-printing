using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class DeprciationReverse : TrackUserOperation
    {
        public int DeprciationReverseId { get; set; }
        [Required]
        [Display(Name = "Fixed Asset")]
        public int RegistrationID { get; set; }
        public string StoreCode { get; set; }
        public string Period { get; set; }
        public string FiscalYear { get; set; }
        public decimal BookValue { get; set; }
    }
}