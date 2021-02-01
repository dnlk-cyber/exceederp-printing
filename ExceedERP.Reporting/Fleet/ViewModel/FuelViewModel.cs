using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class FuelViewModel : Organization
    {

       public string FuelType { get; set; }
       public string OperatorName { get; set; }
        public  int InitialKmHr { get; set; }
       
        public decimal FinalKmHr { get; set; }

        public DateTime? Date { get; set; }
 
        public int PlanKmHr { get; set; }
    
        public  decimal FuelFilledAmount { get; set; }
    
        public string FuelSource { get; set; }
        public string Type { get; set; }
       
        public decimal UnitPrice { get; set; }
       
        public string ReceiptNo { get; set; }
        public string FuelSourceDetail { get; set; }
        
        public decimal TotalFuelcost { get; set; }

        public string Description { get; set; }
    }
}
