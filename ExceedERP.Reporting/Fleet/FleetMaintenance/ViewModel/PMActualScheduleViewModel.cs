using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class PMActualScheduleViewModel : Organization
    {

        public string EquipmentType { get; set; }
        public decimal FleetCount { get; set; }
        public string Service { get; set; }
        public decimal Plan { get; set; }
        public decimal Actual { get; set; }
        public decimal Percentage { get; set; }
     
    }
}
