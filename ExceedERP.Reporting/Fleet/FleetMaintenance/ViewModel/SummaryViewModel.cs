using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
        public  class SummaryViewModel : Organization
    {
            public string Type { get; set; }
            public decimal TotalEquipment { get; set; }
            public decimal TotalTireCost { get; set; }
            public decimal TotalSpareCost { get; set; }
            public decimal TotalSalvageCost { get; set; }
            public decimal TotalOilLubricantCost { get; set; }
            public decimal TotalOtherCost { get; set; }
            public decimal TotalBatteryCost { get; set; }
            public decimal TotalLabourCost { get; set; }
            public decimal TotalNoCrew { get; set; }
            public decimal TotalCost { get; set; }
            public decimal TotalHr { get; set; }
            public decimal ExternalCost { get; set; }
    }
}
