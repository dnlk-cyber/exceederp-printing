using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class WagonDrillTimesheetHeaderViewModel
    {
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string OperatorName { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string Project { get; set; }
        
    }
    public class WagonDrillTimesheetViewModel
    {
        public DateTime Date { get; set; }       
        public string WorkItem { get; set; }       
        public decimal OperationNetWorkingHour { get; set; }       
        public decimal OperationDrilledHoleDepth { get; set; }        
        public decimal NumberOfHolesDrilled { get; set; }       
        public decimal OperationRateForMeter { get; set; }       
        public decimal OperationAmount { get; set; }        
        public decimal IdleHour { get; set; }
        public decimal IdleRateHour { get; set; }       
        public decimal IdleAmount { get; set; }       
        public decimal DownTime { get; set; }
        public decimal TotalAmountBeforeVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal Perdium { get; set; }
        public decimal Salary { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal TireCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Revenue { get; set; }

    }
}
