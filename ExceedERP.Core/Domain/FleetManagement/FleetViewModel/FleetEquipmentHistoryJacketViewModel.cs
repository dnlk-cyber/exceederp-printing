using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetViewModel
{
    public class FleetEquipmentHistoryJacketViewModel 
    {
        public string EquipmentCategoryName { get; set; }
        public string EquipmentTypeName { get; set; }
        public string EquipmentName { get; set; }
        public Guid FleetId { get; set; }
        public string PlateNo { get; set; }
        public string SerialNo { get; set; }

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
