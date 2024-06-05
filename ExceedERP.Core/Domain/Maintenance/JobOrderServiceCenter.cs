using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class JobOrderServiceCenter : OperationWithSecondValidation
    {
        [Key]
        public int JobOrderServiceCenterId { get; set; }

        // foreign key for tracking Num in JobOrder
        public int Num { get; set; }
        public int? JobOrderNum { get; set; }

        public int? CustomerId;

        public Guid JobOrderID { get; set; }

        [DisplayName(" Is Rework?")]
        public bool IsRework { get; set; }
        [DisplayName(" Equipment")]
        public Guid FleetsID { get; set; }

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
       
        [DisplayName(" Project ")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }

        public decimal Odometer { get; set; }
        public string Site { get; set; }
        public string FailureReason { get; set; }
        public string AdvanceMeasure { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentType { get; set; }
        
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
        public bool IsSubmittedToServiceCenter { get; set; }
        public DateTime? OnlineAssignedTime { get; set; }
        public DateTime? OnlineSubmittedTime { get; set; }
        public string WorkShopRejectedReason { get; set; }
        public ServiceCenterStatus ServiceCenterStatus { get; set; }
        

    }
    //public class JobOrderServiceCenterDetail : OperationWithSecondValidation
    //{
    //    [Key]
    //    public int JobOrderServiceCenterDetailId { get; set; }
    //    public int JobOrderServiceCenterId { get; set; }
        
    //    //public Guid? JobOrderDetailID { get; set; }
    //    //public Guid? JobOrderID { get; set; }
    //    [DisplayName("Vehicle Category")]
    //    public string SubCategoryCode { get; set; }
    //    [DisplayName("Service Group")]
    //    public ServiceGroup ServiceGroup { get; set; }
    //    public Guid? ServiceID { get; set; }
    //    public string ServiceName { get; set; }


    //    [DisplayName("Cause")]
    //    public Guid JobCausesId { get; set; }
    //    [DisplayName("Service Name")]
    //    public Guid RepairTypeID { get; set; }
    //    public RepairType RepairType { get; set; }
    //    public ApprovalStatuses ApprovalStatus { get; set; }
    //    [DisplayName("Start Date")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    public Nullable<DateTime> StartingDate { get; set; }
    //    [DisplayName("End Date")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

    //    public Nullable<DateTime> ToDate { get; set; }


    //    public bool IsApproved { get; set; }
    //    [NotMapped]
    //    public string Section { get; set; }
    //    [NotMapped]
    //    public string Cause { get; set; }
    //    [NotMapped]
    //    public string MaintenanceCenter { get; set; }
    //    [NotMapped]
    //    public decimal Efficency { get; set; }
    //    public decimal Odometer { get; set; }
    //    //public virtual Service Service { get; set; }
    //    public virtual JobCauses JobCauses { get; set; }

    //    //nav to ServiceCenter
    //    [Display(Name = "Service Center")]
    //    public int? ServiceCenterId { get; set; } // new ft added
    //    public Guid? ServiceCenterJobId { get; set; } // new ft added

    //}

}
