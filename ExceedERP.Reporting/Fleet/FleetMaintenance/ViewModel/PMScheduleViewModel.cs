using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class PMScheduleViewModel : Organization
    {

        public string EquipmentType { get; set; }
        public string SerialNo { get; set; }
        public string Type { get; set; }
        public string Reading { get; set; }
        public DateTime LastDate { get; set; }
        public string LastKMHR { get; set; }
        public string AssetNo { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }
        public string Service { get; set; }


    }
}
