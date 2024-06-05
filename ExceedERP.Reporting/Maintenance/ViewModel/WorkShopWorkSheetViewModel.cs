using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel
{
    public class WorkShopWorkSheetViewModel : Organization
    {
        public string Customer { get; set; }
        public string TimeIn { get; set; }
        public string OrderNo { get; set; }
        public string VehicleType { get; set; }
        public string vehicleModel { get; set; }
        public string PlateNo { get; set; }

        public decimal Mileage { get; set; }
        public string Owner { get; set; }
        public virtual ICollection<WorkSheetViewModel> WorkSheetViewModels { get; set; }

    }
    public class WorkSheetViewModel
    {
        public int No { get; set; }
        public string Description { get; set; }
        public string SubCategoryCode { get; set; }
        public ServiceGroup ServiceGroup { get; set; }
        public string Mechanic { get; set; }
        public string ActionCode { get; set; }
        public decimal RegularTime { get; set; }
        public decimal OverTime { get; set; }
        public decimal LaborCost { get; set; }
        }
}
