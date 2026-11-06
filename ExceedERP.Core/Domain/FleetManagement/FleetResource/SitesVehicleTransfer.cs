
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{

    public partial class SitesVehicleTransfer : Operations
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }

        public Guid SitesVehicleTransferID { get; set; }

        [DisplayName("Request No")]
        public string RequestNumber { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentIdentityID { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeID { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameID { get; set; }
        [DisplayName(" Plate No")]
        public Guid? FleetsID { get; set; }
        [DisplayName("Starting Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("To Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<DateTime> ToDate { get; set; }
        [DisplayName("Initial Site")]

        public string InitialSite { get; set; }
        [DisplayName("Destination Site")]

        public string DestinationSite { get; set; }

        public int BranchId { get; set; }

        [DisplayName("OdoMeter")]
        public string OdoMeter { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [NotMapped]
        public string PlateNo { get; set; }
        // Distribution editor selection; the grid id field itself is read-only on the client
        [NotMapped]
        public Guid? RequestId { get; set; }
        //[NotMapped]
        //public string EquipmentTypeId { get; set; }
        [DisplayName("Fuel Amount")]
        public FuelAmount FuelAmount { get; set; }
        [DisplayName("Battery Status")]
        public BatteryStatus BatteryStatus { get; set; }
        [DisplayName("Equipment Condition")]
        public EquipmentCondition EquipmentCondition { get; set; }
        [DisplayName("Transfer Mode")]
        public TransferMode TransferMode { get; set; }
        public HydrolicAmount Hydrolic { get; set; }
        [DisplayName("Battery Ampere")]
        public string BatteryAmpere { get; set; }
        [DisplayName("Battery Quantity")]
        public string BatteryQuantity { get; set; }
        [DisplayName("Transfered Operator")]
        public string TransferedOperator { get; set; }
        public Guid? OperatorId { get; set; }
        public Guid? SecondOperatorId { get; set; }
        [DisplayName("Loaded Plate No")]
        public string LoadedPlateNo { get; set; }
        [DisplayName("Transfer Reason")]
        public string TransferReason { get; set; }
        public virtual Fleets Fleet { get; set; }
        public bool IsOperationApproved { get; set; }
        public string OperationApprvedBy { get; set; }
        public DateTime? OperationApprovalDate { get; set; }
        public bool IsDepartmentApproved { get; set; }
        public string DGMApprovedBy { get; set; }
        public DateTime? DGMApprovalDate { get; set; }
        public TransferStatus? TransferStatus {get;set;}

        // Stage 1: request (equipment type/name only, no specific vehicle yet)
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RequestDate { get; set; }
        [DisplayName("Requested By")]
        public string RequestedBy { get; set; }
        public bool IsRequestChecked { get; set; }
        [DisplayName("Checked By")]
        public string RequestCheckedBy { get; set; }
        public DateTime? RequestCheckedDate { get; set; }
        public bool IsRequestApproved { get; set; }
        [DisplayName("Approved By")]
        public string RequestApprovedBy { get; set; }
        public DateTime? RequestApprovedDate { get; set; }

        // Stage 2: request receival - receiver assigns a specific vehicle
        public bool IsRequestReceived { get; set; }
        [DisplayName("Request Received By")]
        public string RequestReceivedBy { get; set; }
        public DateTime? RequestReceivedDate { get; set; }
        public bool IsDistributionChecked { get; set; }
        [DisplayName("Distribution Checked By")]
        public string DistributionCheckedBy { get; set; }
        public DateTime? DistributionCheckedDate { get; set; }
        public bool IsDistributionApproved { get; set; }
        [DisplayName("Distribution Approved By")]
        public string DistributionApprovedBy { get; set; }
        public DateTime? DistributionApprovedDate { get; set; }

        // Stage 3 (fleet): Accept -> SendForApproval (inherited IsSendForApproval) -> Approve (inherited IsOnlineApproved)
        public bool IsAccepted { get; set; }
        [DisplayName("Accepted By")]
        public string AcceptedBy { get; set; }
        public DateTime? AcceptedTime { get; set; }
    }

    public static class SitesVehicleTransferStatus
    {
        public const string Draft = "Draft";
        public const string RequestChecked = "Request Checked";
        public const string RequestApproved = "Request Approved";
        public const string VehicleAssigned = "Vehicle Assigned";
        public const string DistributionChecked = "Distribution Checked";
        public const string WaitingFleetApproval = "Waiting Fleet Approval";
        public const string Accepted = "Accepted";
        public const string WaitingApproval = "Waiting Approval";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
    public enum TransferStatus
    {
        Draft,

        WaitingOperationManager,

        WaitingDepartmentGM,

        Approved,
    }
}