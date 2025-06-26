using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetViewModel
{
    public class DryLoadingTimesheetHistoryVM
    {
      
        public int DryLoadingTimeSheetId { get; set; }
        [Display(Name = "Equipment Category")]
        public string EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public string EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public string EquipmentNameId { get; set; }
        [DisplayName("Supplier Name")]
        public string ProcurementSupplierID { get; set; }
        public string TimeSheetStatus { get; set; }
      
        [DisplayName("Work Item")]
        public string WorkItem { get; set; }
       
        [DisplayName("Working Hour")]
        public decimal WorkingHour { get; set; }
        [Display(Name = "Down Hour")]
        public decimal Downhour { get; set; }

        [Display(Name = "Down Reason")]
        public string DownReason { get; set; }
        [Display(Name = "Lubricant Consumption")]
        public decimal OilConsumption { get; set; }
       
        [DisplayName("Trip Start Place")]
        public string TripStartPlace { get; set; }
      
        [DisplayName("Trip Arrival Place")]
        public string TriplArrivalPlace { get; set; }
       
        [DisplayName("Trip Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TripStartDate { get; set; }
       
        [DisplayName("Trip Arrival Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TripArrivalDate { get; set; }
        
        [DisplayName("Road Type (Asphalt(Km))")]
        public decimal Asphalt { get; set; }

        [DisplayName("Road Type (Gravel(Km))")]
        public decimal Gravel { get; set; }
   
        [DisplayName("Total Km Travelled")]
        public decimal TotalKmTravelled { get; set; }
     
        [DisplayName("Fuel(Lit)/")]
        public decimal FuelInLiter { get; set; }
    
        [DisplayName("Per Diem")]
        public decimal PerDiem { get; set; }

     
        [DisplayName("Idle Hour")]
        public decimal IdleHour { get; set; }
      
        [DisplayName("Reason For Idle")]
        public string ReasonForIdle { get; set; }

        [Display(Name = "Equipment Name")]
        public string FleetsID { get; set; }    

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
        public string Category { get; set; }

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

        [Display(Name = "Rental Rate")]
        public decimal RentalRate { get; set; }

        [Display(Name = "Branch")]
        public string BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public string BusinessUnitId { get; set; }

        [Display(Name = "Operator")]
        public string OperatorId { get; set; }
        [Display(Name = "External Operator")]
        public string ExternalOperator { get; set; }

    }
  
 public class DumpTruckTimeSheetViewModel
    {
        public int DumpTruckTimeSheetId { get; set; }
        [Display(Name = "Equipment Category")]
        public string EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public string EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public string EquipmentNameId { get; set; }
      
        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        //[Required]
        [DisplayName("Type Of Work Excuted")]
        public string TypeOfWorkExcuted { get; set; }
        [DisplayName("Supplier Name")]
        public int? ProcurementSupplierID { get; set; }
        //[Required]
        [DisplayName("Loading Location From")]
        public string LoadingLocationFrom { get; set; }
        //[Required]nt
        [DisplayName("Loading Location To")]
        public string LoadingLOactionTo { get; set; }
        //[Required]
        [DisplayName("Capacity (m3/ quintal)")]
        public decimal Capacity { get; set; }
        //[Required]
        [DisplayName("Actual Distance")]
        public decimal ActualDistance { get; set; }
        //[Required]
        [DisplayName("No.of Trip")]
        public int NumberOfTrip { get; set; }
        //[Required]
        [DisplayName("Total Haulage (m3)")]
        public decimal TotalHaulage { get; set; }
       
        [DisplayName("Fuel Consumption(Lit)")]
        public decimal FuelConsumption { get; set; }
        
        [DisplayName("Oil Consumption(Lit)")]
        public decimal OilConsumption { get; set; }
      
        [DisplayName("Grease Consumption(Kg)")]
        public decimal GreaseConsumption { get; set; }

       
        [DisplayName("Idle Hour")]
        public decimal IdleHour { get; set; }
        [DisplayName("Reson for Idle")]
        public string ReasonForIdle { get; set; }

        [DisplayName("Down Hour")]
        public decimal DownHour { get; set; }
        [DisplayName("Reason for Down")]
        public string ReasonForDown { get; set; }
        [DisplayName("Rental Rate")]
        public decimal RentalRate { get; set; }
        [DisplayName("Idle Rental Rate")]
        public decimal IdealRentalRate { get; set; }

        public string FleetsID { get; set; } // 5

        
        [Display(Name = "Net Working Hour")]
        public decimal NetworkingHour { get; set; } // Added
        [Display(Name = "Fuel Rate")]
        public decimal FuelRate { get; set; } // Added
        [Display(Name = "Lubricant Cost")]
        public decimal LubricantCost { get; set; } //Added
        [Display(Name = "Maintenance Cost")]
        public decimal MaintenanceCost { get; set; } //
        [Display(Name = "Tire Cost")]
        public decimal TireCost { get; set; } // 
        public decimal Salary { get; set; } // 
        public decimal Perdium { get; set; } // 

        [Display(Name = "Branch")]
        public string BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public string BusinessUnitId { get; set; }
        [Display(Name = "Operator")]
        public string OperatorId { get; set; }
        [Display(Name = "External Operator")]
        public string ExternalOperator { get; set; }
    }
    public class LightVehicleTimeSheetViewModel
    {

    } public class MachineryWorkingTimeSheetViewModel
    {

    } public class WagonDrillTimeSheetViewModel
    {

    }

    public class EquipmentDistributionViewModel
    {
        
        public string OperatorName { get; set; } //
        public decimal StartOdometer { get; set; }
        public decimal EndOdometer { get; set; }
        public decimal NetOdometer { get; set; }
        public int Quantity { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; } //
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } //
        public string Destination { get; set; } //
        
        public string RequestNumber { get; set; }
        public Guid AssignedEqiupmentId { get; set; }

        public decimal TotalCost { get; set; }
    }
    public class MaintenanceViewModel
    {
        public string MaintenanceCenter { get; set; }
        public string JobNumber { get; set; }
        public decimal MaitenanceServiceCost { get; set; }
        public decimal SparePartsCost { get; set; }
        public decimal AdditionalCost { get; set; }
        public string ServiceType { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class BoloRegistrationHistoryVM
    {
        public int FleetBoloRegistrationId { get; set; }
        public string EquipmentCategoryId { get; set; }
        public string EquipmentTypeId { get; set; }
        public string EquipmentNameId { get; set; }

        [Display(Name = "Plate Number")]
        public string FleetId { get; set; }[Display(Name = "Plate Number")]
        public string SerialNo { get; set; }

        [DataType(DataType.Date), Display(Name = "Bolo Registration Date")]
        public DateTime BoloRegistrationDate { get; set; }
        [DataType(DataType.Date), Display(Name = "Bolo Expiration Date")]
        public DateTime BoloExpireDate { get; set; }
        public string Remark { get; set; }
    }
    public class PurchasingCostVM
    {
        public string FleetId { get; set; }
        [DisplayName("Purchasing Cost   ")]
        public decimal PurchasingCost { get; set; }
       
    }
    public class FuelConsumptionViewModel
    {
        public Guid FleetId { get; set; }
        public decimal TotalFuelFromStore { get; set; }
        public decimal TotalFuelFromTravelOrder { get; set; }
        public decimal TotalFuelConsumption { get; set; }
    }
    public class FleetEquipmentHistoryJacketViewModel 
    {
        public string EquipmentCategoryName { get; set; }
        public string EquipmentTypeName { get; set; }
        public string EquipmentName { get; set; }
        public string PurchasingCost { get; set; }
        public Guid FleetId { get; set; }
        public string PlateNo { get; set; }
        public string PlateNumber { get; set; }
        public string SerialNo { get; set; }

    }
    public class VehicleDistributionVM
    {public int Num { get; set; }
        public Guid VehicleDistributionID { get; set; }

        [Required]
        [DisplayName(" Request")]
        public Guid RequestID { get; set; }
        public enum CompanyRequestType { Company }
        public enum RequestType { Rent, Company }
        public string OperatorSelector { get; set; }
        public RequestType Type { get; set; }
        [DisplayName("Plate Number")]
        public string PlateNumber { get; set; }
        [DisplayName("Manufacturing Year")]
        public string ManufacturingYear { get; set; }

        [DisplayName(" Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName(" End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }
        public string Status { get; set; }
        public string OperatorName { get; set; }
        [Required]
        [DisplayName("  Branch ")]
        public string FromBranchName { get; set; }
        [Required]
        [DisplayName("  Work Unit ")]
        public string FromStructureName { get; set; }

        [DisplayName(" Branch ")]
        public string RequestingBranchName { get; set; }
        [DisplayName(" Employee Name")]
        public int EmployeeId { get; set; }
        public string ReasonForTravel { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [DisplayName("Assignment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AssignmentDate { get; set; }
        public bool IsAssigned { get; set; }
        [DisplayName(" Work Unit ")]
        public string RequestingStructureName { get; set; }
        public string Reference { get; set; }
        [DisplayName(" # Distribution  ")]
        public int? RequestNumber { get; set; }
        public Nullable<int> DistrubtionNumber { get; set; }
        public decimal Duration { get; set; }
        public string NumberHourKM { get; set; }

        public Nullable<int> AssignedQuantity { get; set; }
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        [DisplayName("Equipment Type")]
        public string EquipmentType { get; set; }
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        public string Destination { get; set; }
        public decimal NumberofPersonston { get; set; }
        public string Remarks { get; set; }

    }
    public class FleetInsuranceViewModel 
    {
        public int FleetInsuranceId { get; set; }
        [Display(Name ="Insurance Company Name")]
        public string InsuranceCompanyName { get; set; }
        [Display(Name ="Insurance Type")]
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        [Display(Name ="Due Date"), DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        [Display(Name = "Third Party")]
        public decimal ThirdPartyInsurance { get; set; }
        [Display(Name = "Comprehensive")]
        public decimal ComprehensiveInsurance { get; set; }
        [Display(Name ="Premium Cost")]
        public decimal PremiumCost { get; set; }
        [Display(Name = "Policy No")]
        public string PolicyNo { get; set; }
        public string Description { get; set; }
    }
    public class FleetAcidentViewModel
    {
        public Guid AccidentId { get; set; }
        public string LevelOfDamage { get; set; }
        public string CauseOfAccident { get; set; }
        public decimal VehicleDistanceFromRoadEdge { get; set; }
        public string PolicyIdNo { get; set; }
        public string AssetDamage { get; set; }
        public string Accountablebody { get; set; }
        public string ThirdPartyDamage { get; set; }
        public decimal VehicleSpeed { get; set; }
        public string InjuredPersons { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfOccurrence { get; set; }
        [DataType(DataType.Date)]
        public DateTime TimeOfAccident { get; set; }
        public string AccidentLocation { get; set; }
        public string OperatorName { get; set; }
    }

    public class TimesheetVm
    {
        public TimeSheetType TimeSheetType { get; set; }
        public int FleetTimeSheetId { get; set; }
        public string OperatorName { get; set; }
        public string BranchName { get; set; }
        public string BusinessUnitName { get; set; }
        public string SerialNo { get; set; }
        public decimal NetWorkingHour { get; set; }
        public decimal OperationRateForMeter { get; set; }
        public decimal NoOfHolesDrilled { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal IdleRateHour { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public decimal DrilledDepthOfHole { get; set; }
        public decimal IdleAmount { get; set; }
        public string WorkItem { get; set; }
        public decimal IdleHour { get; set; }
        public decimal DownTime { get; set; }
        public decimal OperationAmount { get; set; }
        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime MorningTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime MorningTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AfternoonTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AfternoonTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NightTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NightTimeEnd { get; set; }
        public decimal HoleDepth { get; set; }
        public decimal GreaseConsumption { get; set; }
        public decimal FuelConsumption { get; set; }
        public decimal DownHours { get; set; }
        public string IdleReason { get; set; }
        public string WorkType { get; set; }

        public decimal IdleHours { get; set; }
        public string DownReason { get; set; }
        public string Remark { get; set; }
        public double TotalWorkingHour { get; set; }
        public string PlateNo { get; set; }
        public string Branch { get; set; }
        public string LoadingLocationFrom { get; set; }
        public decimal ActualDistance { get; set; }
        
        public string ReasonForDown { get; set; }
        public decimal IdleRentalRate { get; set; }
        public string LoadingLocationTo { get; set; }
        public int NoOfTrip { get; set; }
        public decimal OilConsumption { get; set; }
        public string ReasonForIdle { get; set; }
        public string TypeOfWorkExcuted { get; set; }
       
        public decimal TotalHaulage { get; set; }
        
        public decimal DownHour { get; set; }
        public decimal Capacity { get; set; }
        [DataType(DataType.Date)]
        public DateTime TripStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime TripArrivalDate { get; set; }
        public string TripStartPlace { get; set; }
        public decimal TotalKmTravelled { get; set; }
        public string TripArrivalPlace { get; set; }
       
        public decimal WorkingHour { get; set; }
        
        public decimal RoadType { get; set; }
        public FleetMesuringCategory CategoryType { get; set; }
        public decimal Quantity { get; set; }
        public string LoadedType { get; set; }
        public decimal AsphaltKm { get; set; }
        public decimal GravelKm { get; set; }
    }

    public class CostSheetVm
    {
        public TimeSheetType TimeSheetType { get; set; }
        public string OperatorName { get; set; }
        public string BranchName { get; set; }
        public string BusinessUnitName { get; set; }
        public int CostSheetId { get; set; }      
        public string WorkType { get; set; }
        public string DeparturePlace { get; set; }
        public string ArrivalPlace { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndDate { get; set; }
        public FleetMesuringCategory Category { get; set; }
        public int NoOfTripe { get; set; }
        public decimal AsphaltTraveled { get; set; }
        public decimal GravelTraveled { get; set; }
        public decimal AsphaltRate { get; set; }
        public decimal GravelRate { get; set; }
        public string LoadedType { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal LubricantCost { get; set; }
        
        public decimal TireCost { get; set; }
        public decimal LoadingUnloadingCost { get; set; }
        public decimal Perdium { get; set; }
        public decimal Salary { get; set; }
        public decimal GreaseRate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetIncome { get; set; }
        public int SN { get; set; }
        public string PlateNo { get; set; }
        public string Branch { get; set; }
        public DateTime Date { get; set; }
        public string WorkItem { get; set; }
        public decimal OperationNetWorkingHour { get; set; }
        public decimal NumberOfTrips { get; set; }
        public decimal ActualDistance { get; set; }
        public decimal Capacity { get; set; } 
        public decimal OperationRateperHour { get; set; }
        public decimal OperationRevenue { get; set; }
        public decimal IdleHour { get; set; }
        public decimal IdleRateperHour { get; set; }
        public decimal IdleRevenue { get; set; }
        
        public decimal DownHour { get; set; }
        
        public decimal TotalHaulage { get; set; }
        
        public decimal OperationNetworkinHour { get; set; }
        public decimal OperationRatePerHour { get; set; }
        public decimal OpertionRevenue { get; set; }
        
        public decimal IdleRatePerHour { get; set; }
        
        public decimal MaintenanceCost { get; set; }
        
        public decimal OperationDrilledHoleDepth { get; set; }
        public decimal NumberOfHolesDrilled { get; set; }
        public decimal OperationRateForMeter { get; set; }
        public decimal OperationAmount { get; set; }
        
        public decimal IdleRateHour { get; set; }
        public decimal IdleAmount { get; set; }
        public decimal DownTime { get; set; }
        public decimal TotalAmountBeforeVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        
        public decimal ServiceCharge { get; set; }
        
        public decimal NetAmount { get; set; }
        public decimal Revenue { get; set; }
        

    }

    public class TyreAndBatteryViewModel
    {
        public Guid TyreAndBatteryId { get; set; }
        public string PlateNumber { get; set; }
        public string SerialNumber { get; set; }
        public string OperatorName { get; set; }
        public string Brand { get; set; }
        public FleetBatteryOrTyreType IssueType { get; set; }
        public string TireSize { get; set; }
        
        public decimal UnitPrice { get; set; }
        public string Status { get; set; }
    }
}
