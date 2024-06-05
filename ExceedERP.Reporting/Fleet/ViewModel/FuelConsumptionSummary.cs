using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class FuelConsumptionSummary   : Organization
    {

       public string ProjectName { get; set; }
       public string FuelType { get; set; }
       public string FuelItem { get; set; }
       public decimal Amount { get; set; }   

       public decimal TotalCost { get; set; }

       public decimal RentalAmount { get; set; }
       public decimal RentalFuelType { get; set; }
       public decimal RentalTotalCost { get; set; }

       public decimal Total { get; set; }
    }
}
