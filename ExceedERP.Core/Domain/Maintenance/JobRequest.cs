using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class JobRequest : OperationWithSecondValidation
    {
        [Key]
        [DisplayName(" Request Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public int? RequestNumber { get; set; }
        public Guid JobRequestId { get; set; }
        public int VeheicleReceptionFormId { get; set; }
        [DisplayName(" Customer")]
        public int? CustomerId { get; set; }
        public bool IsPM { get; set; }
        public JobRequestSource Source { get; set; }
        //[Required]
        [DisplayName(" Equipment (License Plate)")]
        public Guid? EquipmentInformationId { get; set; }
        [DisplayName("Plate No/Serial No")]
        public string LicensePlate { get; set; }
        public string FromBranchId { get; set; }
        [DisplayName(" Equipment Name ")]
        public string EquipmentName { get; set; }
        public string Requester { get; set; }
        [DisplayName(" Operator Name ")]
        public string OperatorName { get; set; }
        [DisplayName(" Fuel Amount ")]
        public string FuelAmount { get; set; }
        public decimal Odometer { get; set; }

        //[DisplayName(" Asset Num ")]
        //public string AssetNum { get; set; }

        //[DisplayName(" Project Name")]
        //public string ProjectName { get; set; }
        public string KMReading { get; set; }
        public string GPSInfo { get; set; }
        public decimal LastKmHr { get; set; }
        public decimal EstmatedCostAmount { get; set; }

        [NotMapped]
        public string Model { get; set; }

        [NotMapped]
        public string EquipmentType { get; set; }

        [Display(Name = "Maintenance Center")]
        public string MaintenanceCenter { get; set; }
        [Display(Name = "Maintenance Center"), Required]
        public Guid? MaintenanceCenterId { get; set; } // Added to track MaintenanceCenterId
        [Display(Name ="Requester")]
        [Required]
        public int RequesterId { get; set; }
        // Added
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Business Unit")]
        public int BusinessUnitId { get; set; }
        [Display(Name ="Org Structure")]
        public int OrgStructureId { get; set; }

        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        public bool HasInsurance { get; set; }
        public bool IsHighPriority { get; set; }
        [Display(Name ="Job Order Status")]
        public JobOrderStatus JobOrderStatus { get; set; }
        [Display(Name ="delivered Date"), DataType(DataType.Date)]
        public DateTime? DeliveredDate { get; set; }
        public bool IsSentBackToReception { get; set; }
        public bool CreateJobOrder { get; set; }
    }
    public class JobRequestVM
    {
        
        public string JobRequestId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime DateApproved { get; set; }
        public int? RequestNumber { get; set; }
        [DisplayName(" Customer")]
        public string CustomerName { get; set; }
        public bool IsPM { get; set; }
        public string Source { get; set; }
        //[Required]
        
        [DisplayName(" Equipment Name ")]
        public string EquipmentName { get; set; }
        public string Requester { get; set; }
        [DisplayName(" Operator Name ")]
        public string OperatorName { get; set; }
        [DisplayName(" Fuel Amount ")]
        public decimal Odometer { get; set; }

        public decimal LastKmHr { get; set; }
        public decimal EstmatedCostAmount { get; set; }
           
        [Display(Name = "Maintenance Center")]
        public string MaintenanceCenter { get; set; }
       
             // Added
        [Display(Name = "Branch")]
        public string BranchName { get; set; }
        [Display(Name = "Business Unit")]
        public string BusinessUnit { get; set; }
        [Display(Name = "Org Structure")]
        public int OrgStructure { get; set; }
        public string JobOrderStatus { get; set; }
        [Display(Name = "delivered Date"), DataType(DataType.Date)]
        public DateTime? DeliveredDate { get; set; }
    }
    public enum JobOrderStatus
    {       
        [Display(Name ="None")]
        None,
        Progress,
        Delivered
    }

    public enum JobRequestSource
    {
        CompanyVehicle,
        ExternalVehicle
    }
}
