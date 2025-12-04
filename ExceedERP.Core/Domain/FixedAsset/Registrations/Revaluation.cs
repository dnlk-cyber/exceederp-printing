using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public enum AssetRevualutionType
    {
        IFRS,
        Tax,
    }
    public class FixedAssetRevaluation : TrackUserOperation
    {
        public int FixedAssetRevaluationID { get; set; }
        [Required]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Fixed Asset")]
        public virtual AssetRegistration AssetRegistration { get; set; }
        [DisplayName("RevaluationType")]
        public AssetRevualutionType RevaluationType { get; set; }
        [Required]
        [DisplayName("New Book Value")]
        public decimal BookValue { get; set; }//new book value       
        public decimal UsefulLifeYears { get; set; }//new years of useful life  

        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RevaluationDate { get; set; }
    }
}
