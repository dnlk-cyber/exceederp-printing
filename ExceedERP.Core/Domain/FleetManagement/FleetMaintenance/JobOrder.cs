

using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{

    using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class JobOrder : OperationWithSecondValidation
    {
        public JobOrder()
        {


        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public int? JobOrderNum { get; set; }


        public Guid JobOrderID { get; set; }


        [DisplayName(" Equipment")]
        public Guid FleetsID { get; set; }

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
        [Required]
        [DisplayName(" Project ")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }

        public decimal Odometer { get; set; }
        public string Site { get; set; }
        public string FailureReason { get; set; }
        public string AdvanceMeasure { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentType { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public string CurrentFuel { get; set; }
        public string Status { get; set; }
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

        public virtual Fleets Fleets { get; set; }



    }
}
