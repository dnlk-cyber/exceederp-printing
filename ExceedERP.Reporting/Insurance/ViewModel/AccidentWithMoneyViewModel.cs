using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
   public class AccidentWithMoneyViewModel
    {
        public string ProjectName { get; set; }
        public int VehicleAccidentPreviosYear { get; set; }
        public int VehicleAccidentCurrentYear { get; set; }
        public decimal MoneyPrevious { get; set; }
        public decimal MoneyCurrent { get; set; }
        public string Difference { get; set; }
        public string PreviousYear { get; set; }
        public string CurrentYear { get; set; }
    }
}
