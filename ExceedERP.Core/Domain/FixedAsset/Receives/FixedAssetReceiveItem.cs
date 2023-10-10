using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveItem : TrackUserOperation
    {
        public int FixedAssetReceiveItemID { get; set; }
        public int FixedAssetReceiveId { get; set; }
        public virtual FixedAssetReceive FixedAssetReceive { get; set; }       
     
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
        public virtual FixedAssetCategory FixedAssetCategory { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }
        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        public int  FixedAssetNameID { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Tick if Extra Quantity")]
        public bool IsExtra { get; set; }
        [Display(Name = "Extra Quantity")]
        public decimal? ExtraQuantity { get; set; }
        public decimal ExtraAmount { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Warranty Information")]
        public string WarrantyInformation { get; set; }
        [DataType(DataType.MultilineText)]
        public string Specification { get; set; }
        [Display(Name = "This item expires")]
        public bool Expire { get; set; }
        [Display(Name = "Expire Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpireDate { get; set; }
        public decimal Balance { get; set; }
        //machine
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        [Display(Name = "Is Machinery ?")]
        public bool IsMachinery { get; set; }
        public string Manufacturer { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }
        [Display(Name = "Batch No.")]
        public string BatchNo { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Condition")]
        public int FixedAssetConditionId { get; set; }
        public virtual FixedAssetCondition Condition { get; set; }
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
        [Display(Name = "Manufacture Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ManufacturDate { get; set; }
        //only for building
        [Display(Name = "Building (Tick if yes)")]
        public bool IsBuilding { get; set; }
        [Display(Name = "Location of Building")]
        public string Location { get; set; }
        [Display(Name = "Contractor")]
        public string Contractor { get; set; }
        [Display(Name = "Construction Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ConstructionStartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Construction End Date ")]
        public DateTime? ConstructionEndDate { get; set; }
        public bool IsGroup { get; set; }
        public string Remark { get; set; }

    }
}