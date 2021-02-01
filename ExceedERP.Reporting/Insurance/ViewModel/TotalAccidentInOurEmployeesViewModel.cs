using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
   public class TotalAccidentInOurEmployeesViewModel
    {
        public string AccidentType { get; set; }
        public int AccidentsInpreviousYear { get; set; }
        public int AccidentsInCurrentYear { get; set; }
        public string Difference { get; set; }
    }
}
