using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation
{
    public class FleetAnnualPlan : Operations
    {
        [Key]
        public int FleetAnnualPlanId { get; set; }
        public Guid EquipmentCategoryId { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public Guid EquipmentNameId { get; set; }
        public Guid FleetId { get; set; }

    }
    public class FleetAnnualPlanDetail : TrackUserSettingOperation 
    {
        [Key]
        public int FleetAnnualPlanDetailId { get; set; }
        public int NumberOfEquipmentInMonth { get; set; }
        public int WorkingHourPerDay { get; set; }
        public int WorkingDaysInMonth { get; set; }
        public int WorkingHourPerMonth { get; set; }
        public decimal InefficiencyPercentile { get; set; }
        public decimal EfficiencyPercentile { get; set; }
        public decimal HourForInefficiencyPerMonth { get; set; }
        public decimal ExpectedEffectiveWorkingHrPerMonth { get; set; }
        public decimal ChargingRate { get; set; }
        public decimal ExpectedMonthlyIncome { get; set; }
        public decimal TyreCost { get; set; }
        public decimal LiterPerHour { get; set; }
        public decimal TotalLiter { get; set; }
        public decimal FuelCostPerLiter { get; set; }
        public decimal TotalFuelCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal Perduim { get; set; }
        public decimal Salary { get; set; }
        public decimal TotalExpectedExpense { get; set; }
        public decimal ExpectedTotalProfit { get; set; }

        //Fk
        public int FleetAnnualPlanId { get; set; }
        public FleetAnnualPlan FleetAnnualPlan { get; set; }

    }
    
}
