using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Requisitions
{
    public class FixedAssetRequisitionItem : TrackUserSettingOperation
    {
        public int FixedAssetRequisitionItemId { get; set; }
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
        public virtual FixedAssetCategory FixedAssetCategory { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }
        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Size { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Chasis No.")]
        public string ChasisNo { get; set; }
        public string UoM { get; set; }
        [Display(Name = "Requested QTY")]
        public int RequestedQuantity { get; set; }
        [Display(Name = "Approved QTY")]
        public int ApprovedQuantity { get; set; }
        [Display(Name = "Issued QTY")]
        public int IssuedQuantity { get; set; }
        [Display(Name = "Purchase QTY")]
        public int PurchaseQuantity { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Generate PR")]
        public bool GeneratePurchaseReq { get; set; }
        public bool IsGroup { get; set; }
    }
}