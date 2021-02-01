
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
        [DisplayName(" Equipment")]
        public Guid FleetsID { get; set; }
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
        [Required]
        public string InitialSite { get; set; }
        [DisplayName("Destination Site")]
        [Required]
        public string DestinationSite { get; set; }
         [DisplayName("OdoMeter")]
        public string OdoMeter { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [NotMapped]
        public string PlateNo { get; set; }
        [NotMapped]
        public string EquipmentTypeId { get; set; }
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
        [DisplayName("Loaded Plate No")]
        public string LoadedPlateNo { get; set; }
        [DisplayName("Transfer Reason")]
        public string TransferReason { get; set; }
        public virtual Fleets Fleet { get; set; }

    }
}