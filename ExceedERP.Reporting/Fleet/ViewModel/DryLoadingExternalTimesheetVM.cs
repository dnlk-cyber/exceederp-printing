using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class DryLoadingExternalTimesheetHeaderVM
    {

        public string OwnerName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime DurationFrom { get; set; }
        public DateTime DurationTo { get; set; }
        public string Duration { get; set; }
        public string Period { get; set; }
        public string PlateNumber { get; set; }
        public string Project { get; set; }
        public string EqipmentName { get; set; }
        public string EquipmentName { get; set; }
        public string OperatorName { get; set; }

    }
    public class DryLoadingExternalTimesheetViewModel
    {
        public int Sn { get; set; }
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
        public decimal TotalAmountBeforeVAT { get; set; }
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
