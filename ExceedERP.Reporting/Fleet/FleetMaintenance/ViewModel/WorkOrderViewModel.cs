using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class WorkOrderViewModel : Organization
    {
        public string EquipmentName { get; set; }
        public string PlateNo { get; set; }
        public int WorkNum { get; set; }
        public double Duration { get; set; }
        public decimal SpareCost { get; set; }
        public decimal TyreCost { get; set; }
        public decimal OilCost { get; set; }
        public decimal BatteryCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal ExternalRepairCost { get; set; }
        public decimal SalvageCost { get; set; }
        public decimal TotalCost { get; set; }

    }
}
