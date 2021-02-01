using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
    public class AssetTransferReceiveItem : TrackUserSettingOperation
    {
        public int AssetTransferReceiveItemId { get; set; }
        public int AssetTransferReceiveId { get; set; }
        public virtual AssetTransferReceive TransferReceive { get; set; }
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }

        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }
        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Grn No.")]
        public string GrnNo { get; set; }
        public string Size { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        [DisplayName("QTY")]
        public int Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        public string Remark { get; set; }
    }
}