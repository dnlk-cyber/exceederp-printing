using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class ProjectSummary
    {
        public string ProjectName { get; set; }

        public string EquipmentType { get; set; }
        public decimal RentalWorkingCost { get; set; }
        public decimal CompanyWorkingCost { get; set; }
        public decimal RentalWorkingTime { get; set; }
        public decimal CompanyWorkingTime { get; set; }
        public decimal FuelConsumtion { get; set; }
        public decimal SpareCost { get; set; }
     
    }
}
