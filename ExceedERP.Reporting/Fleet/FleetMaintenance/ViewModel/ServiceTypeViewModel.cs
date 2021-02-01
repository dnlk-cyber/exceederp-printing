using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class ServiceTypeViewModel : Organization
    {
        public string ServiceName { get; set; }
        public decimal? STD { get; set; }
        public string Equipment { get; set; }
        public string Section { get; set; }
        public decimal ActualTime { get; set; }
        public decimal Efficency { get; set; }
        public string TotalCost { get; set; }
    }



    public class ServiceSpareViewModel
    {


        public Guid ServiceSpareID { get; set; }
        public Guid ServiceID { get; set; }
        public string InventoryItemId { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal UsedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string ItemName { get; set; }
        public string PlateNumber { get; set; }
        public string Location { get; set; }


    }
}
