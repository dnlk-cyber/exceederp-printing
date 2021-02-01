using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class JobRequestViewModel
    {


        public int? JobOrderNum { get; set; }
        public string Type { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNum { get; set; }
        public string MaintenanceCenter { get; set; }
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }

    }
}
