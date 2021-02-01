using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public class FixedAssetRevaluation : TrackUserOperation
    {
        public int FixedAssetRevaluationID { get; set; }
        [Required]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Fixed Asset")]
        public virtual AssetRegistration AssetRegistration { get; set; }
        [Required]
        [Display(Name = "Added Value")]
        public decimal AddedValue { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RevaluationDate { get; set; }
    }
}
