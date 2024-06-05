using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel
{
    public class MaintenanceJobRequestViewModel : Organization
    {
        public int? RequestNumber { get; set; }
        public string JobRequest { get; set; }
        public string CustomerName { get; set; }

        public string IsPM { get; set; }
        public string Source { get; set; }
        public string EquipmentInformation { get; set; }
        public string LicensePlate { get; set; }
        public string EquipmentName { get; set; }

        public string Requester { get; set; }
        public string OperatorName { get; set; }
        public string FuelAmount { get; set; }

        public decimal Odometer { get; set; }
        public decimal LastKmHr { get; set; }
        public string Model { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentCategory { get; set; }
        public string MaintenanceCenter { get; set; }
    }
}
