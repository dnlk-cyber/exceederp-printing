

using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{

    using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public  class JobOrder : OperationWithSecondValidation
    {
        public JobOrder()
        {
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public int? JobOrderNum { get; set; }

        public int? CustomerId;

        public Guid JobOrderID { get; set; }
        public string ToExternalJobOrderId { get; set; }
        public string ServiceCenterJobId { get; set; }

        [DisplayName(" Is Rework?")]
        public bool IsRework { get; set; }
        [DisplayName(" Equipment")]
        public Guid FleetsID { get; set; }
        public string KMReading { get; set; }
        public string GPSInfo { get; set; }
        [DisplayName(" Equipment (License Plate)")]
        public Guid EquipmentInformationId { get; set; }


        [DisplayName("Type ")]
        public string Type { get; set; }
        [DisplayName("Equipment")]
        public string EquipmentName { get; set; }
        [DisplayName(" Operator Name ")]
        public string OperatorName { get; set; }
        [DisplayName(" Asset Num ")]
        public string AssetNum { get; set; }
        [DisplayName(" Is External ")]
        public bool IsExternal { get; set; }
        [DisplayName("Maintenance Center")]
        public string MaintenanceCenter { get; set; }
        [DisplayName("Maintenance Center")]
        public Guid? MaintenanceCenterId { get; set; } // Added To Track MaintenanceCenterId
        //[Required]
        [DisplayName(" Project ")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }

        public decimal Odometer { get; set; }
        public string Site { get; set; }
        public string FailureReason { get; set; }
        public string AdvanceMeasure { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentType { get; set; }
        //public Guid EquipmentTypeId { get; set; }
        public string CurrentFuel { get; set; }
        public int? Referance { get; set; }

        [DisplayName("Date Issued  ")]
        public Nullable<DateTime> DateIssued { get; set; }
        [DisplayName("Date Completed   ")]
        public Nullable<DateTime> DateCompleted { get; set; }
        [DisplayName("Requested By")]
        public string InspectedBy { get; set; }

        public bool IsPM { get; set; }
        [DisplayName(" Reason For Job ")]
        [DataType(DataType.MultilineText)]
        public string InspectorComments { get; set; }


        [NotMapped]
        public decimal Efficiency { get; set; }
        [NotMapped]
        [DisplayName("Spare Cost")]
        public decimal SpareCost { get; set; }
        [NotMapped]
        [DisplayName("Tyre Cost")]
        public decimal TyreCost { get; set; }
        [NotMapped]
        [DisplayName("Oil Lubricant Cost")]
        public decimal OilLubricantCost { get; set; }
        [NotMapped]
        [DisplayName("Other Cost")]
        public decimal OtherCost { get; set; }
        [NotMapped]
        [DisplayName("Salvage Cost")]
        public decimal SalvageCost { get; set; }
        [NotMapped]
        [DisplayName(" Labor Cost ")]
        public decimal LabourCost { get; set; }
        [NotMapped]
        [DisplayName(" Battery Cost ")]
        public decimal BattreyCost { get; set; }
        [NotMapped]
        [DisplayName(" Component Cost ")]
        public decimal ComponentCost { get; set; }
        [NotMapped]
        [DisplayName(" Progress ")]
        public string Progress { get; set; }
        public string LicensePlate { get; set; }
        public string RejectionReason { get; set; }
        public bool IsSentToWorkShop { get; set; }
        public DateTime? IsSentToWorkShopTime { get; set; }
        public bool IsSubmittedToServiceCenter { get; set; }
        //public DateTime? IsSubmittedToServiceCenterTime { get; set; }
        public DateTime? OnlineAssignedTime { get; set; }
        public DateTime? OnlineSubmittedTime { get; set; }
        public string WorkShopRejectedReason { get; set; }
        public ServiceCenterStatus ServiceCenterStatus { get; set; }
        //public virtual Fleets Fleets { get; set; }
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Business Unit")]
        public int BusinessUnitId { get; set; }
        [Display(Name ="Org Structure")]
        public int OrgStructureId { get; set; }
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        [Display(Name = "Requester")]
        public int RequesterId { get; set; }
        [Display(Name ="Is Insured")]
        public bool IsInsured { get; set; }
        public string Description { get; set; } 
    }

    public enum ServiceCenterStatus
    {
        AssignedToWorkShop,
        SubmittedToServiceCenter,
        Rejected,
        Approved
    }
    public class JobOrderVM 
    {
       

        public Guid JobOrderId { get; set; }
        public Guid FleetId { get; set; }
        [DisplayName(" Is Rework?")]
        public bool IsRework { get; set; }
        
        public bool IsExternal { get; set; }
        [DisplayName("Maintenance Center")]
        public string MaintenanceCenter { get; set; }
        public string Status { get; set; }
      
        [DisplayName(" Project ")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        public decimal Odometer { get; set; }
        public int Num { get; set; }
        public string Site { get; set; }
        public string FailureReason { get; set; }
        public string AdvanceMeasure { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentType { get; set; }
       
        [DisplayName("Date Issued  ")]
        public DateTime? DateCreated { get; set; }
        public DateTime? DateClosed { get; set; }
        [DisplayName("Date Completed   ")]
        public DateTime? DateCompleted { get; set; }
        [DisplayName("Requested By")]
        public string InspectedBy { get; set; }
       
        public string InspectorComments { get; set; }
        public string LicensePlate { get; set; }
        public string RejectionReason { get; set; }
        public bool IsSentToWorkShop { get; set; } 
        public DateTime? IsSentToWorkShopTime { get; set; }
        public bool IsSubmittedToServiceCenter { get; set; }
        //public DateTime? IsSubmittedToServiceCenterTime { get; set; }
        public DateTime? OnlineAssignedTime { get; set; }
        public DateTime? OnlineSubmittedTime { get; set; }
        public string WorkShopRejectedReason { get; set; }
        //public virtual Fleets Fleets { get; set; }
        [Display(Name ="Branch")]
        public string BranchName { get; set; }
        [Display(Name ="Business Unit")]
        public string BusinessUnit { get; set; }
        [Display(Name ="Org Structure")]
        public string OrgStructure { get; set; }
        public string Description { get; set; } 
        public string SeriveGroup { get; set; }
        public string RequisitionDate { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
    }
}
