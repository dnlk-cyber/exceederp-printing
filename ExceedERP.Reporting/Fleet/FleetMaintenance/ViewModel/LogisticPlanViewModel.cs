using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel
{
    public class LogisticPlanViewModel :  Organization
    {
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public string Model { get; set; }
        public string Location { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal PlanCost { get; set; }
        public decimal ActualCost { get; set; }
        public string Service { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal WareHouseQuantity { get; set; }
        public decimal PurchaseOrderQuantity { get; set; }
      
 

    }
}
