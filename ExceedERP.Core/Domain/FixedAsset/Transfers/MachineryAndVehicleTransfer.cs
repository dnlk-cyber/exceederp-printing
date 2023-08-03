using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using static ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
   
    public class MachineryAndVehicleTransfer : Operations
    {
        public int MachineryAndVehicleTransferId { get; set; }
        [Display(Name = "Request Document No.")]
        public string RequestNumber { get; set; }
        public TransferType Type { get; set; }
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }      
        [Required]
        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransferDate { get; set; }
        [Display(Name = "Usage Days")]
        public int UsageDays { get; set; }
        [Display(Name = "Transfer Description")]
        public string TransferDesc { get; set; }
       
        [Display(Name = "Location Before")]
        public string LocationBeforeTransfer { get; set; }
        [Display(Name = "Custodian Before")]
        public int? CustodianBeforeTransfer { get; set; }//empid
        [Display(Name = "Transfer To")]
        public int? TransferTo { get; set; }//empid
        [Display(Name = "From Store")]
        public string FromStoreCode { get; set; }
        [Display(Name = "To Store")]
        public string ToStoreCode { get; set; }
        [Display(Name = "From Branch")]
        public int? FromBranchId { get; set; }
        [Display(Name = "To Branch")]
        public int? ToBranchId { get; set; }
        [Display(Name = "Case of Transfer")]
        public string TransferReason { get; set; }
        [Display(Name = "Transfer By")]
        public TransferBy TransferBy { get; set; }
        [Display(Name = "Driver")]
        public string DriverName { get; set; }
        [Display(Name = "Transfer Plate No.")]
        public string TransferPlateNo { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        [Display(Name = "Bar Code No.")]
        public string BarCodeNo { get; set; }
        [Display(Name = "KM/ Hour Reading")]
        public string KmHourReading { get; set; }

        [DisplayName("Fuel Volume")]
        public string FuelVolume { get; set; }
        [DisplayName("Hydraulic Oil Volume")]
        public string HydraulicOilVolume { get; set; }
        [DisplayName("Battery Ampere")]
        public string BatteryAmpere { get; set; }
        [DisplayName("Battery Status")]
        public BatteryStatus BatteryStatus { get; set; }
        [DisplayName("Battery QTY")]
        public int BatteryQuantity { get; set; }
        [DisplayName("Unit Price")]
        public decimal UPrice { get; set; }
        public string Unit { get; set; }
        public virtual ICollection<DamagedPart> DamagedParts { get; set; }
        public virtual ICollection<MissedPart> MissedParts { get; set; }
        public virtual ICollection<Accessory> Accessories { get; set; }
        public virtual ICollection<MachineryAndVehicleTransferValidation> MachineryAndVehicleTransferValidations { get; set; }
    }
    public class DamagedPart : TrackUserSettingOperation
    {
        public int DamagedPartId { get; set; }
        public int MachineryAndVehicleTransferId { get; set; }
        public virtual MachineryAndVehicleTransfer Transfer { get; set; }
        [DisplayName("Damaged Part")]
        public string DamagePart { get; set; }
    }
    public class MissedPart : TrackUserSettingOperation
    {
        public int MissedPartId { get; set; }
        public int MachineryAndVehicleTransferId { get; set; }
        public virtual MachineryAndVehicleTransfer Transfer { get; set; }
        [DisplayName("Missed Part")]
        public string MissingPart { get; set; }
    }
    public class Accessory : TrackUserSettingOperation
    {
        public int AccessoryId { get; set; }
        public int MachineryAndVehicleTransferId { get; set; }
        public virtual MachineryAndVehicleTransfer Transfer { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("QTY")]
        public int Quantity { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
    }
    public class MachineryAndVehicleTransferValidation : TrackUserSettingOperation
    {
        public int MachineryAndVehicleTransferValidationId { get; set; }
        public int MachineryAndVehicleTransferId { get; set; }
        public virtual MachineryAndVehicleTransfer MachineryAndVehicleTransfer { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
