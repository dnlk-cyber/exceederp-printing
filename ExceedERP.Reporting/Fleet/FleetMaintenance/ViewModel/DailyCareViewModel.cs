using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class DailyCareViewModel : Organization
    {
        public string OperatorName { get; set; }
        public string EquipmentType { get; set; }
        public string SerialNo { get; set; }
        public string Reading { get; set; }
        public string AssetNo { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Care { get; set; }
        public string Mon { get; set; }
        public string MonRemark { get; set; }

        public string Tus { get; set; }
        public string TusRemark { get; set; }
        public string Wed { get; set; }
        public string WedRemark { get; set; }
        public string Thu { get; set; }
        public string ThuRemark { get; set; }
        public string Fri { get; set; }
        public string FriRemark { get; set; }
        
    }
}
