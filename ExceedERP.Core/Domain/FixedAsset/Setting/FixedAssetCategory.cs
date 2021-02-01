using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetCategory : TrackUserSettingOperation
    {
        public int FixedAssetCategoryID { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name = "Category Code")]
        public string CategoryCode { get; set; }
        [Required]
        [Display(Name = "Asset Group")]
        public int FixedAssetGroupID { get; set; }
        //[Required]
        [Display(Name = "AnnualDepreciation Category")]
        public string DepreciationCategory { get; set; }
        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; }
        [Display(Name = "Fixed Asset Class")]
        public FixedAssetClass FixedAssetClass { get; set; }

    }
}