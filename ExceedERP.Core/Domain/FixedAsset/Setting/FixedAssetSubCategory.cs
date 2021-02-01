using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetSubCategory : TrackUserSettingOperation
    {
        public int FixedAssetSubCategoryID { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string CategoryCode { get; set; }
        [Required]
        [Display(Name = "Sub Category Name")]
        public string SubCategoryName { get; set; }
        [Required]
        [Display(Name = "Sub Category Code")]
        public string SubCategoryCode { get; set; }
        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; }
        public bool IsGroup { get; set; }
        public virtual ICollection<FixedAssetSubCategoryName> Names { get; set; }
    }
    public class FixedAssetSubCategoryName : TrackUserSettingOperation
    {
        public int FixedAssetSubCategoryNameId { get; set; }
        public int FixedAssetSubCategoryID { get; set; }
        public virtual FixedAssetSubCategory SubCategory { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}