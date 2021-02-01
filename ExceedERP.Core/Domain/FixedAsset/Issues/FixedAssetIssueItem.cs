using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Issues
{
    public class FixedAssetIssueItem : TrackUserOperation
    {
        public int FixedAssetIssueItemID { get; set; }
        public int FixedAssetIssueID { get; set; }
        public virtual FixedAssetIssue FixedAssetIssue { get; set; }
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }

        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Grn No.")]
        public string GrnNo { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        [Display(Name = "User Card")]
        public bool SendToUserCard { get; set; }
        [Display(Name = "Requested QTY")]
        public int RequestedQuantity { get; set; }
        [Display(Name = "Approved QTY")]
        public int ApprovedQuantity { get; set; }
        [Display(Name = "Issued QTY")]
        public int IssuedQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Is Machinery ?")]
        public bool IsMachinery { get; set; }
        [Display(Name = "Batch No.")]
        public string BatchNo { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Engine Model No.")]
        public string EngineModelNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) S.No.")]
        public string EngineElectricalNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) Model No.")]
        public string EngineElectricalModelNo { get; set; }
        public bool IsGroup { get; set; }
        public string Remark { get; set; }
    }
}