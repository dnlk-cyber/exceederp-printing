using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetGroup : TrackUserSettingOperation
    {
        public int FixedAssetGroupID { get; set; }
        [Required]
        [Display(Name = "Asset Group")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        //[Required]
        //[Display(Name = "AnnualDepreciation Method")]
        //public DepreciationMethod DepreciationMethod { get; set; }
        [Required]
        [Display(Name = "Yearly Straight Line (%)")]
        [Range(typeof(float), "0", "100")]
        public float StraightLinePercentage { get; set; }
        [Required]
        [Display(Name = "Yearly Pooling (%)")]
        [Range(typeof(float), "0", "100")]
        public float PoolingPercentage { get; set; }
        [Required]
        [Display(Name = "Salvage Value")]
        public decimal SalvageValue { get; set; }
    }
}