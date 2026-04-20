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
        [Required]
        [Display(Name = "Depreciation Method")]
        public DepreciationMethod DepreciationMethod { get; set; }
        [Required]
        [Display(Name = "Yearly Straight Line (%)")]
        [Range(typeof(decimal), "0", "100")]
        public decimal StraightLinePercentage { get; set; }
        [Required]
        [Display(Name = "Yearly Diminishing (%)")]
        [Range(typeof(decimal), "0", "100")]
        public decimal PoolingPercentage { get; set; }
        [Required]
        [Display(Name = "IFRS (%)")]
        [Range(typeof(decimal), "0", "100")]
        public decimal IfrsPercentage { get; set; }
        [Required]
        [Display(Name = "Salvage Value")]
        public decimal SalvageValue { get; set; }
        [Display(Name = "Salvage Value Type")]
        public PPEAmountType SalvageValueType { get; set; }
    }
}