using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class TimeSheetViewModel : Organization
    {

        public Guid TimeSheetID { get; set; }
        public decimal TotalIdleTime { get; set; }
        public decimal TotalWorkingTime { get; set; }
        public decimal TotalWorkingCost { get; set; }
        public decimal TotalIdealCost { get; set; }
        public string CustomerName { get; set; }
        public string TypeOfWork { get; set; }


   

    }
}
