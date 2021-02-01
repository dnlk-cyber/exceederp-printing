using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class EquipmentUtilizationSummary
    {

        public string Type { get; set; } 
       public string Description { get; set; }
       public decimal IdealExpense { get; set; }
       public decimal WorkingExpense { get; set; }
       public decimal TotalExpense { get; set; }
       public decimal IdealIncome { get; set; }
       public decimal WorkingIncome { get; set; }
       public decimal TotalIncome { get; set; }

        
    }
}
