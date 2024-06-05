using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModels
{
    public class DumpTruckExternalTimesheetHeaderViewModel
    {
        public string project { get; set; }
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string OperatorName { get; set; }
        public string EquipmentName { get; set; }
        public string Project { get; set; }
    }
    public class DumpTruckExternalTimesheetViewModel
    {
        public int SN { get; set; }
        public DateTime Date { get; set; }    
        public string WorkItem { get; set; }       
        public decimal OperationNetWorkingHour { get; set; }        
        public decimal NumberOfTrips { get; set; }        
        public decimal ActualDistance { get; set; }        
        public decimal Volume { get; set; } 
        public decimal RatePerKm { get; set; }
        public decimal TotalHaulage { get; set; }
        public decimal OperationAmount { get; set; }
        public decimal IdleHour { get; set; }        
        public decimal IdleRateperHour { get; set; }        
        public decimal IdleAmount { get; set; }
        public decimal DownHour { get; set; }     
        public decimal TotalAmountBeforVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal MaintenaceCost { get; set; }       
        public decimal TotalCost { get; set; }
        public decimal NetIncome { get; set; }
    }
}
