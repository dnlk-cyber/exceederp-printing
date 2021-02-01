using System.ComponentModel;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class FixedAssetCountVm
    {
        public int FixedAssetCountItemId { get; set; }
        public int FixedAssetCountId { get; set; }
        public virtual FixedAssetCount FixedAssetCount { get; set; }
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }
      
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }

        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }

        [DisplayName("Physical Qty.")]
        public decimal Count { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Unit")]
        public string Uom { get; set; }
    }
}