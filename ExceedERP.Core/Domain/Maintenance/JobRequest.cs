using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class JobRequest : Operations
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

        public decimal LastKmHr { get; set; }

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
