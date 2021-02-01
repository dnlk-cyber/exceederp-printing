using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class IndexingCount
    {
        [Key]
        public string IndexCode { get; set; }
        public string SubCategoryCode { get; set; }
        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }

        public int CurrentValue { get; set; }
    }
}