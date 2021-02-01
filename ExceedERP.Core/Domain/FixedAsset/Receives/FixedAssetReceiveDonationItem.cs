using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveDonationItem : TrackUserOperation
    {
        public int FixedAssetReceiveDonationItemID { get; set; }
        public int FixedAssetReceiveDonationId { get; set; }
        public virtual FixedAssetReceiveDonation FixedAssetReceiveDonation { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax Type")]
        [Required]
        public string TaxType { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

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
        [Display(Name = "Unit")]
        public string UOM { get; set; }
        //machine
        [Display(Name = "Machinery (Tick if yes)")]
        public bool IsMachinery { get; set; }
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
    }
}