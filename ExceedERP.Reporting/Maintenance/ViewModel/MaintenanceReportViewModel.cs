using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel 
{
    public class MaintenanceReportViewModel : Organization
    {
        public string Customer { get; set; }
        public string PlateNo { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal SparePartCost { get; set; }
    }
    public class MaintenanceDurationViewModel : Organization
    {
        public string Customer { get; set; }
        public string PlateNo { get; set; }
        public string CustomerRequestDate { get; set; }
  
        public string CustomerRequestDuration { get; set; }
        public string JobOrderDate { get; set; }
        public string JobOrderDuration { get; set; }
        public string ServiceCenterDate { get; set; }
        public string ServiceCenterDuration { get; set; }
        public string WorkShopDate { get; set; }
        public string WorkShopDuration { get; set; }
        public string TotalDurationDate { get; set; }
        public string TotalDurationDays { get; set; }
    }
}
