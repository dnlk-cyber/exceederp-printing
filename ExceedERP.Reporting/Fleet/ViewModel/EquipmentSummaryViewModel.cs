using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class EquipmentSummaryViewModel  : Organization
    {
        public string Location { get; set; }
        public string EquipmentType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string RentalRate { get; set; }
        public string PlateNumber { get; set; }
        public decimal WorkingHour { get; set; }
        public decimal IdealHour { get; set; }
        public decimal IdealCost { get; set; }
        public decimal DownTime { get; set; }
        public decimal RentalDownTime { get; set; }
        public decimal WorkingCost { get; set; }
        public decimal OwnedNo { get; set; }
        public decimal RentalNo { get; set; }
        public decimal RentalWorkingHour { get; set; }
        public decimal RentalIdealHour { get; set; }
        public decimal RentalWorkingCost { get; set; }
        public decimal RentalIdealCost { get; set; }

        public string FuelType { get; set; }
        public string RentalFuelType { get; set; }
        public decimal RentalFuelConsumptionQty { get; set; }
        public decimal RentalFuelConsumptionCost { get; set; }
        public decimal FuelConsumptionQty { get; set; }
        public decimal FuelConsumptionCost { get; set; }

        public decimal  Total { get; set; }
    }
}
