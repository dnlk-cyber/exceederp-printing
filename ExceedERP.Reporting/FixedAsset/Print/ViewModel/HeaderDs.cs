using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System.Drawing;
using ExceedERP.Reporting.Inventory.ViewModel;
using static ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Reporting.FixedAsset.Print.ViewModel
{

    public class MachineryAndVehicleTransferHeader : Organization
    {
        public string TransferNumber { get; set; }
        public string TransferReceiveNumber { get; set; }
        [Display(Name = "Request Document No.")]
        public string RequestNumber { get; set; }
        [DisplayName("Category")]
        public string CategoryCode { get; set; }
        [DisplayName("Sub Category")]
        public string SubCategoryCode { get; set; }
        [DisplayName("Tag")]
        public string Tag { get; set; }
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }
        [Display(Name = "Usage Days")]
        public int UsageDays { get; set; }
        [Display(Name = "Transfer Description")]
        public string TransferDesc { get; set; }
        [Display(Name = "Custodian Before")]
        public string CustodianBeforeTransfer { get; set; }//empid
        [Display(Name = "Location Before")]
        public string LocationBeforeTransfer { get; set; }
        [Display(Name = "Transfer To")]
        public string TransferTo { get; set; }//empid
        [Display(Name = "Case of Transfer")]
        public string TransferReason { get; set; }
        [Display(Name = "Transfer By")]
        public string TransferBy { get; set; }
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
        [Display(Name = "Condition")]
        public string Condition { get; set; }

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
        public string Deliverer { get; set; }


        public string Recipient { get; set; }

        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
    }
    public class MachineryAndVehicleTransferViewModel
    {
        public virtual ICollection<DamagedPart> DamagedParts { get; set; }
        public virtual ICollection<MissedPart> MissedParts { get; set; }
        public virtual ICollection<Accessory> Accessories { get; set; }
    }

    public class MachineryAndVehicleTransferReceiveVm
    {
        public virtual ICollection<DamagedPartReceive> DamagedParts { get; set; }
        public virtual ICollection<MissedPartReceive> MissedParts { get; set; }
        public virtual ICollection<AccessoryReceive> Accessories { get; set; }
    }

    public class FixedAssetTransferHeader : Organization
    {
        public string TransferDescription { get; set; }
        public string TransferNumber { get; set; }
        public string TransferReceiveNumber { get; set; }
        public string TransferType { get; set; }
        public string Deliverer { get; set; }
        public string DelivererPlate { get; set; }
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public string FromEmployee { get; set; }
        public string ToEmployee { get; set; }
        public string StoreRequistionNo { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string StoreKeeperName { get; set; }
        public string Recipient { get; set; }
        public string Remark { get; set; }
        public string TransferDate { get; set; }


        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
    }
    public class FixedAssetTransferItemVm
    {

        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string GrnNo { get; set; }
        public string Size { get; set; }
        public string TagNo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string BatchNo { get; set; }
        public string Manufacturer { get; set; }
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
        public string Remark { get; set; }
    }

    public class FixedAssetReturnHeader : Organization
    {
        public string ReturnNo { get; set; }      
        public string Reference { get; set; }
        public string StoreCode { get; set; }
        public string BranchOrProject { get; set; }
        public string Department { get; set; }
        public string StoreKeeperName { get; set; }
        public string ApproverName { get; set; }
        public string Recipient { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnDate { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
    }
    public class FixedAssetReturnItemVm
    {

        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string GrnNo { get; set; }
        public string Size { get; set; }
        public string TagNo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string SerialNo { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string Condition { get; set; }
        public string PlateNo { get; set; }
    }
    public class FixedAssetIssueHeader : Organization
    {
        public string Reference { get; set; }
        public string IssueNumber { get; set; }
        public string IssueTo { get; set; }
        public string GatePass { get; set; }
        public string StoreRequistionNo { get; set; }
        public string StoreCode { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string StoreKeeperName { get; set; }
        public string Recipient { get; set; }
        public string IssueDate { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
    }
    public class FixedAssetIssueItemVm
    {
        
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string GrnNo { get; set; }
        public string Size { get; set; }
        public string TagNo { get; set; }
        public int RequestedQuantity { get; set; }
        public int ApprovedQuantity { get; set; }
        public int IssuedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public bool IsMachinery { get; set; }
        public string BatchNo { get; set; }
        public string Manufacturer { get; set; }
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
        public string Remark { get; set; }
    }
    public class FixedAssetReceiveHeader : Organization
    {
        public string Reference { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string TenderNo { get; set; }
        public string GoodReciveNo { get; set; }
        public string SupplierInvoice { get; set; }
        public string StoreName { get; set; }
        public string Deliverer { get; set; }
        public string Receiver { get; set; }
        public string Supplier { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
        public string ReceiveDate { get; set; }

        public string Inspector1 { get; set; }
        public string Inspector2 { get; set; }
        public string Inspector3 { get; set; }

    }
    public class FixedAssetRequistionHeader : Organization
    {
        public string StoreRequsitionNo { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Requestor { get; set; }
        public Image RequestorSignature { get; set; }
        public string RequestingDepartment { get; set; }
        public string RequestingStore { get; set; }
        public string ForProject { get; set; }
        public string Remark { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }

        public string CheckedBy { get; set; }
        public DateTime? CheckedDate { get; set; }
        public Image CheckedBySignature { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public Image AuthorizedSignature { get; set; }
        public string StoreName { get; set; }
    }

    public class FixedAssetPurchaseRequistionHeader : InventoryVoucherHeaderVm
    {
        public string PurchaseRequistionNo { get; set; }
        public string StoreRequistionNo { get; set; }
        public string Requestor { get; set; }
        public string ApproverName { get; set; }//sr
        public DateTime ApprovalDate { get; set; }//sr
        public Image ApprovalBySignature { get; set; }

        public string PrApproverName { get; set; }//pr
        public DateTime PrApprovalDate { get; set; }//pr
        public string AuthorizerName { get; set; }
        public DateTime AuthorizationDate { get; set; }
        public Image AuthorizerBySignature { get; set; }
        public string BudgetAuthorizerName { get; set; }
        public DateTime BudgetAuthorizationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string RequestingDate { get; set; }
        public string RequestingDepartment { get; set; }
        public string RequestingStore { get; set; }
        public string RequestingProject { get; set; }
        public string ForProject { get; set; }
        public string PRDate { get; set; }
        public string Model { get; set; }
        public string PlateNo { get; set; }
        public string Remark { get; set; }
    }
}
