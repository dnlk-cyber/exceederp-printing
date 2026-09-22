using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class DumpTruckTimeSheet : OperationsCostTimeSheet
    {
        public int DumpTruckTimeSheetId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid? EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid? EquipmentNameId { get; set; }
        public decimal LabourCost { get; set; }
        public string FuelType { get; set; }
        public int? RentalAgreementNum { get; set; }
        public string OilType { get; set; }
        [NotMapped]
        public bool IsECWC { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal OverTime { get; set; }
        public decimal BatteryCost { get; set; }
        [Required]
        [Column(TypeName = "DATE")]
        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time Start")]
        public DateTime? MorningTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Morning Time End")]
        public DateTime? MorningTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time Start")]
        public DateTime? AfternoonTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Afternoon Time End")]
        public DateTime? AfternoonTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time Start")]
        public DateTime? NightTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Night Time End")]
        public DateTime? NightTimeEnd { get; set; }
        //[Required]
        public decimal Volume { get; set; }
        public UnitSpecification? Unit { get; set; }
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
        public decimal TotalTime { get; set; }

        //[Required]
        [DisplayName("Actual Distance")]
        public decimal ActualDistance { get; set; }
        //[Required]
        [DisplayName("No.of Trip")]
        public int NumberOfTrip { get; set; }
        //[Required]
        [DisplayName("Total Haulage (m3)")]
        public decimal TotalHaulage { get; set; }
        [Required]
        [DisplayName("Fuel Consumption(Lit)")]
        public decimal FuelConsumption { get; set; }

       // [Required]
       // [DisplayName("Oil Consumption(Lit)")]
        public decimal OilConsumption { get; set; }

        //[Required]
        [DisplayName("Grease Consumption(Kg)")]
        public decimal GreaseConsumption { get; set; }
        public decimal GreaseCost { get; set; }
        [Required]
        [DisplayName("Idle Hour")]
        public decimal IdleHour { get; set; }
        
        [DisplayName("Reason for Idle")]
        public string ReasonForIdle { get; set; }

        [Required]
        [DisplayName("Down Hour")]
        public decimal DownHour { get; set; }     
        [DisplayName("Reason for Down")]
        public string ReasonForDown { get; set; }
        [Required]
        [DisplayName("Rental Rate")]
        public decimal RentalRate { get; set; }
        [Required]
        [DisplayName("Idle Rental Rate")]
        public decimal IdealRentalRate { get; set; }

        public Guid FleetsID { get; set; } // 5

        public virtual Fleets Fleet { get; set; }
        [Display(Name ="Net Working Hour")]
        public decimal NetworkingHour { get; set; } // Added
        [Display(Name ="Fuel Rate")]
        public decimal FuelRate { get; set; } // Added
        [Display(Name = "Oil Cost")]
        public decimal OilCost { get; set; } //Added
        [Display(Name = "Maintenance Cost")]
        public decimal MaintenanceCost { get; set; } //
        [Display(Name = "Tire Cost")]
        public decimal TireCost { get; set; } // 
        public decimal Salary { get; set; } // 
        public decimal Perdium { get; set; } // 
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Business Unit")]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "Operator")]
        public string ExternalOperatorName { get; set; }
        [Display(Name = "Operator")]
        public Guid? OperatorId { get; set; }
        [Display(Name = "External Operator")]
        public string ExternalOperator { get; set; }
        public DTLoadingStatus LoadingStatus { get; set; }
        public decimal InitialKMR { get; set; }
        public decimal FinalKMR { get; set; }
        public decimal DifferenceKMR { get; set; }
        public decimal FuelKMR { get; set; }
        public decimal? MaintenanceFuel { get; set; }
        public DownReasonList? DownReason { get; set; }
        public IdleReasonList? IdleReason { get; set; }
    }
    
    public enum IdleReasonList
    {
        
        Activity_Dependency,
        Weather_Condition,
        Equipment_Dependency,
        Fuel_Lubricant_Shortage,
        Material_Problem,
        Due_to_Mobilization_Shortage_of_Water,
        Sunday,
        Holiday,
        National_Issue,
        Rain,
        Right_Off_Way_Problem,
        Miscellaneous_Factor,
        Social_Problem,
        Operators,
        WeatherCondition,


    }

    public enum DownReasonList
    {
        Waiting_Part,
        Under_Maintenance,
        Wainting_ForInspection,
        Wainting_ForManPower,
        Wainting_ForTools,
    }

    public enum DTLoadingStatus
    {
        UnClasified = 0,
        WithLoad = 1,
        WithOutLoad = 2
    }
    public static class EnumExtension
{
    public static List<SelectListItem> EnumToSelectList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = Convert.ToInt32(e).ToString()
            }).ToList();
    }
}
    public class TimeSheetConsumptionItem:TrackUserSettingOperation
    {
        public int TimeSheetConsumptionItemId { get; set; }
        public int? DumpTruckTimeSheetId { get; set; }
        public int? DryLoadingTimeSheetId { get; set; }
        public int? LightVehicleTimeSheetId { get; set; }
        public int? MachineryWorkingTimeSheetId { get; set; }
        public int? WagonDrillTimesheetId { get; set; }
        public decimal OilConsumption { get; set; }
        public decimal OilCost { get; set; }
        public string OilType { get; set; }
    }
    public class DumpTruckTimeSheetDetail
    {
        public int DumpTruckTimeSheetDetailId { get; set; }
        [Display(Name ="Loading From"), Required]
        public string LoadingLocationFrom { get; set; }
        [Display(Name ="Loading To"), Required]
        public string LoadingLocationTo { get; set; }
        [Display(Name ="No of Trips"), Required]
        public int NumberOfTrip { get; set; }
        public DumpTruckTypeOfWork DumpTruckTypeOfWork { get; set; }
        [Display(Name ="Type of Work"), Required]
        public string TypeOfWork { get; set; }
        [Display(Name ="Actual Distance"), Required]
        public decimal ActualDistance { get; set; }
        [Display(Name ="Total Haulage")]
        public decimal TotalHaulage { get; set; }
        [Display(Name ="Capacity (m3/quintal)")]
        public decimal Capacity { get; set; }
        public int DumpTruckTimeSheetId { get; set; }
        [Display(Name ="Rental Rate")]
        public decimal RentalRate { get; set; }
    }
}
