using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Procurement.Supplier;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class DryLoadingTimeSheet : OperationsCostTimeSheet
    {
        public int DryLoadingTimeSheetId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid? EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid? EquipmentNameId { get; set; }
        [DisplayName("Supplier Name")]
        public int? ProcurementSupplierID { get; set; }
        [Required]
        [DisplayName("Work Item")]
        public string WorkItem { get; set; }
        [Required]
        [DisplayName("Working Hour")]
        public decimal WorkingHour { get; set; }
        [Display(Name = "Down Hour")]
        public decimal Downhour { get; set; }

        [Display(Name = "Down Reason")]
        public string DownReason { get; set; }
        [Display(Name = "Lubricant Consumption")]
        public decimal OilConsumption {get;set;}
        [Required]
        [DisplayName("Trip Start Place")]
        public string TripStartPlace { get; set; }
        [Required]
        [DisplayName("Trip Arrival Place")]
        public string TriplArrivalPlace { get; set; }
        [Column(TypeName = "DATE")]
        [Required]
        [DisplayName("Trip Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TripStartDate { get; set; }
        [Column(TypeName = "DATE")]
        [Required]
        [DisplayName("Trip Arrival Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TripArrivalDate { get; set; }
        [Required]
        [DisplayName("Road Type (Asphalt(Km))")]
        public decimal Asphalt { get; set; }
        [Required]
        [DisplayName("Road Type (Gravel(Km))")]
        public decimal Gravel { get; set; }
        [Required]
        [DisplayName("Total Km Travelled")]
        public decimal TotalKmTravelled { get; set; }
        [Required]
        [DisplayName("Fuel(Lit)/")]
        public decimal FuelInLiter { get; set; }
        [Required]
        [DisplayName("Per Diem")]
        public decimal PerDiem { get; set; }
        
        [Required]
        [DisplayName("Idle Hour")]
        public decimal IdleHour { get; set; }
    
        [DisplayName("Idle Rate")]
        public decimal IdleRate { get; set; }
        [Required]
        [DisplayName("Reason For Idle")]
        public string ReasonForIdle { get; set; }

        [Display(Name ="Equipment Name")]
        public Guid FleetsID { get; set; }

        public virtual Fleets Fleet { get; set; }
        public virtual ProcurementSupplier ProcurementSupplier { get; set; }

       
        public decimal NetworkingHour { get; set; } // Added
        [Display(Name = "Fuel Rate")]
        public decimal FuelRate { get; set; } // Added
        [Display(Name = "Lubricant Cost")]
        public decimal LubricantCost { get; set; } //Added
        [Display(Name = "Maintenance Cost")]
        public decimal MaintenanceCost { get; set; } //
        [Display(Name = "Tire Cost")]
        public decimal TireCost { get; set; } // 
        [Display(Name = "Salary")]
        public decimal Salary { get; set; } // 
        [Display(Name = "Perdium")]
        public decimal Perdium { get; set; } // 


        public FleetMesuringCategory Category { get; set; }

        [Display(Name = "Quantity")]
        public int NoOfTripe { get; set; } // 


        [Display(Name = "Asphalt Rate")]
        public decimal AsphaltRate { get; set; } // 
        [Display(Name = "Gravel Rate")]
        public decimal GravelRate { get; set; } // 
        [Display(Name = "Loaded Type")]
        public string LoadedType { get; set; } // 
        [Display(Name = "Loading/Unloading Cost")]
        public decimal LoadingUnloadingCost { get; set; } //

        [Display(Name ="Rental Rate")]
        public decimal RentalRate { get; set; }

        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Business Unit")]
        public int? BusinessUnitId { get; set; }

        [Display(Name = "Operator")]
        public Guid? OperatorId { get; set; }
        [Display(Name ="External Operator")]
        public string ExternalOperator { get; set; }
        
    }
    public enum FleetMesuringCategory
    {
        Trips,
        Quintals,
        Liters
    }
}
