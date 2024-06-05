using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class MachineryExternalTimeSheetSummeryHeaderVM
    {
        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string OperatorName { get; set; }
        public string Project { get; set; }
        public string EquipmentName { get; set; }
    }
    public class MachineryExternalTimeSheetSummeryVM
    {
        public string PlateNumber { get; set; }
        public DateTime Date { get; set; }
        public string WorkItem { get; set; }
        public decimal OperationNetworkinHour { get; set; }
        public decimal OperationRatePerHour { get; set; }
        public decimal OpertionRevenue { get; set; }
        public decimal IdleHour { get; set; }
        public decimal IdleRatePerHour { get; set; }
        public decimal IdleRevenue { get; set; }
        public decimal DownHour { get; set; }
        public decimal TotalAmountBeforeVat { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal FuelCost { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetAmount { get; set; }
    }
    
}
