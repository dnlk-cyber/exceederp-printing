using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Returns
{
    public class FixedAssetReturnItem : TrackUserOperation
    {
        public int FixedAssetReturnItemId { get; set; }
        public int FixedAssetReturnId { get; set; }
        public virtual FixedAssetReturn FixedAssetReturn { get; set; }
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }
        
        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration Asset { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        public string Remark { get; set; }
        public bool IsGroup { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}