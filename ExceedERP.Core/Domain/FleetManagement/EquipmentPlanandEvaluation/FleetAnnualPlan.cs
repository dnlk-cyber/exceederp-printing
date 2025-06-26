using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation
{
    public class FleetAnnualPlan : Operations
    {
        [Key]
        public int FleetAnnualPlanId { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        [Display(Name = "Plate/Serial No")]
        public Guid FleetId { get; set; }
        [Display(Name ="Fiscal Year")]
        public int GlFiscalYearId { get; set; } // GlFiscalYear

        public EquipmentGrade EquipmentGrade { get; set; } 

    }
    public enum EquipmentGrade
    {
        Excellent,
        Very_Good,
        Good,
        Poor
    }
    public class FleetAnnualPlanDetail : TrackUserSettingOperation 
    {
        [Key]
        public int FleetAnnualPlanDetailId { get; set; }
        public int NumberOfEquipmentInMonth { get; set; }
        public int WorkingHourPerDay { get; set; }
        public int WorkingDaysInMonth { get; set; } 
        public int WorkingHourPerMonth { get; set; } // = WorkingHourPerDay * WorkingDaysInMonth
        public decimal InefficiencyPercentile { get; set; }
        public decimal EfficiencyPercentile { get; set; }
        public decimal HourForInefficiencyPerMonth { get; set; } // = WorkingHourPerMonth * InefficiencyPercentile
        public decimal ExpectedEffectiveWorkingHrPerMonth { get; set; } // = WorkingHourPerMonth * EfficiencyPercentile
        public decimal ChargingRate { get; set; }
        public decimal ExpectedMonthlyIncome { get; set; } // = ExpectedEffectiveWorkingHrPerMonth * ChargingRate
        public decimal TyreCost { get; set; }
        public decimal LiterPerHour { get; set; }
        public decimal TotalLiter { get; set; } // = ExpectedEffectiveWorkingHrPerMonth * LiterPerHour
        public decimal FuelCostPerLiter { get; set; }  
        public decimal TotalFuelCost { get; set; } // = FuelCostPerLiter * FuelCostPerLiter
        public decimal MaintenanceCost { get; set; } 
        public decimal Perduim { get; set; }
        public decimal Salary { get; set; }
        public decimal TotalExpectedExpense { get; set; } // = TyreCost + TotalFuelCost + MaintenanceCost + Perduim + Salary
        public decimal ExpectedTotalProfit { get; set; } // = ExpectedMonthlyIncome - TotalExpectedExpense

        //Fk
        public int FleetAnnualPlanId { get; set; }
        public FleetAnnualPlan FleetAnnualPlan { get; set; }
        [Display(Name ="Period")]
        public int GlPeriodId { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
    }
    
}
