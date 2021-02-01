using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class DownTimeViewModel : Organization
    {
                public string EquipmentType { get; set; }
                public string Model { get; set; }
                public string PlateNo { get; set; }
                public DateTime? DownDate { get; set; }
                public string ReasonForPending { get; set; }
                public string DownDuration { get; set; }
                public decimal DownCost { get; set; }
    }
}
