using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel
{
    public class MaintenanceSummaryViewModel : Organization
    {
        public string JobNo { get; set; }
        public string vehicleTypeModel { get; set; }
        public string PlateNo { get; set; }
        public string Owner { get; set; }
        public string EnteredDate { get; set; }
        public string Status { get; set; }
        public virtual ICollection<ServiceTypeViewModel> ServiceTypeViewModels { get; set; }

    }
    public class ServiceTypeViewModel
    {
        public string RepairingStartDate { get; set; }
        public string Service { get; set; }
        public decimal AT { get; set; }
        public decimal ST { get; set; }
        public decimal AtStDifferencePercent { get; set; }
        public decimal AtStDifferenceNumber { get; set; }
        public string InvestigatedBy { get; set; }

        public virtual ICollection<DownTimeViewModel> DownTimeViewModels { get; set; }

    }
    public class DownTimeViewModel
    {
        public decimal DW { get; set; }
        public string DWReason { get; set; }
    }
}
