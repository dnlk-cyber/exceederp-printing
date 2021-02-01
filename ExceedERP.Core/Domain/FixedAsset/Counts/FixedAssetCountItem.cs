using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class FixedAssetCountItem: TrackUserSettingOperation
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
        [DisplayName("Record Qty.")]
        public decimal PreviousBalance { get; set; }
        [DisplayName("Physical Count")]
        public decimal Count { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public bool Updated { get; set; }
        [DisplayName("Cost")]
        public decimal Price { get; set; }
        public string Location { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
    }
}