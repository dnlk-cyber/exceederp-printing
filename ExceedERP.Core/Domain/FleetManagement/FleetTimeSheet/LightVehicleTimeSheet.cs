using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class LightVehicleTimeSheet : OperationsCostTimeSheet
    {
        [Key]
        public int LightVehicleTimeSheetId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid? EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid? EquipmentNameId { get; set; }
        public decimal TotalTime { get; set; }
        public int? RentalAgreementNum { get; set; }
        public Guid FleetsID { get; set; }  
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "DATE")]
        [Display(Name = "Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time Start")]
        public DateTime MorningTimeStart { get; set; } = new DateTime(2000, 1, 1, 1, 0, 0);
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time End")]
        public DateTime MorningTimeEnd { get; set; } = new DateTime(2000, 1, 1, 1, 0, 0);
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time Start")]
        public DateTime AfternoonTimeStart { get; set; } = new DateTime(2000, 1, 1, 7, 0, 0);
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time End")]
        public DateTime AfternoonTimeEnd { get; set; } = new DateTime(2000, 1, 1, 7, 0, 0);
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time Start")]
        public DateTime NightTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time End")]
        public DateTime NightTimeEnd { get; set; }
        public decimal InitialKMR { get; set; }
        public decimal FinalKMR { get; set; }
        public decimal DifferenceKMR { get; set; }
        public decimal FuelKMR { get; set; }
        [NotMapped]
        public bool IsECWC { get; set; }

        [Display(Name = "Number Of Holes")]
        public decimal NumberOfHoles { get; set; }
        [Display(Name = "Hole Depth")]
        public decimal HoleDepth { get; set; }
        [Display(Name = "Fuel(Lit) Consumption")]
        public decimal FuelConsumption { get; set; }
        [Display(Name = "Fuel Rate")]
        public decimal FuelRate { get; set; }
        [Display(Name = "Rental Rate")]
        public decimal RentalRate { get; set; }
        [Display(Name = "Ideal Rental Rate(%)")]
        public decimal IdealRentalRate { get; set; }
        [Display(Name = " Grease(kg) Consumption")]
        public decimal GreaseConsumption { get; set; }
        public decimal GreaseCost { get; set; }
        [Display(Name = "Idle Hours")]
        public decimal IdleHours { get; set; }
        [Display(Name = "Idle Reason")]
        public string IdleReason { get; set; }
        [Display(Name = "Down Hours")]
        public decimal DownHours { get; set; }
        [Display(Name = "Down Reason")]
        public string DownReason { get; set; }
        [Display(Name = "Down Hour Rate")]
        public decimal DownHourRate { get; set; }
        [Display(Name = "Net Hour")]
        public decimal NetHour { get; set; }
        public string WorkType { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Supplier Name")]
        public int? ProcurementSupplierID { get; set; }
        [NotMapped]
        public decimal OperationRevenue { get; set; }
        public decimal OperationAmount { get; set; }
        public decimal IdleRate { get; set; }
        public decimal IdleAmount { get; set; }
        public decimal TotalAmountBeforeVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        [Display(Name = "Fuel Cost")]
        public decimal FuelCost { get; set; }
        [Display(Name = "Service Recharge(%)")]
        public decimal ServiceRecharge { get; set; }

        [Display(Name = "Maintenance Cost")]
        public decimal MaintenanceCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetAmount { get; set; }

        [Display(Name = "Tire Cost")]
        public decimal TireCost { get; set; } // 
        public decimal Salary { get; set; } // 
        public decimal LabourCost { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal OverTime { get; set; }
        public decimal BatteryCost { get; set; }
        public decimal Perdium { get; set; } // 

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "Operator")]
        public string ExternalOperatorName { get; set; }
        [Display(Name = "Operator")]
        public Guid? OperatorId { get; set; }
        [Display(Name = "Grease Rate")]
        public decimal GreaseRate { get; set; }

        public string FuelType { get; set; }
        public string OilType { get; set; }
        public decimal OilCost {get;set;}
        public decimal OilConsumption { get; set; }
        public decimal? MaintenanceFuel { get; set; }
        public DownReasonList? ReasonForDown { get; set; }
        public IdleReasonList? ReasonForIdle { get; set; }
    }
    public class MaintenanceCostDetail : TrackUserSettingOperation
    {
        public int MaintenanceCostDetailId { get; set; }
        public int TimeSheetId { get; set; }
        public string RepairTypeId { get; set; }
        public string MechanicName { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "SparePartCost cannot be negative")]
        public decimal SparePartCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "OtherCost cannot be negative")]
        public decimal OtherCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "GreaseConsumption cannot be negative")]
        public decimal GreaseConsumption { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Miscellaneous cannot be negative")]
        public decimal MiscellaneousCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "GreaseCost cannot be negative")]
        public decimal GreaseCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "ServiceCost cannot be negative")]
        public decimal ServiceCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "LabourCost cannot be negative")]
        public decimal LabourCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Fuel Consumption cannot be negative")]        
        public decimal FuelConsumption { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Fuel Cost cannot be negative")]        
        public decimal FuelCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "TimeTaken cannot be negative")]        
        public decimal TimeTaken { get; set; }
        public bool IsLightVehicle { get; set; }
        public bool IsMachinery { get; set; }
        public bool IsDumpTruck { get; set; }
        public bool IsWagonDrill { get; set; }
        public bool IsDryLoading { get; set; }
        public decimal Cost { get; set; }
        }
}
