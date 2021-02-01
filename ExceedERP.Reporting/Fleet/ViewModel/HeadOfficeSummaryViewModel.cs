using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
        public  class HeadOfficeSummaryViewModel   : Organization
    {

            public string RentIncome { get; set; }
            public string RentIdealCost { get; set; }
            public string SpareCost { get; set; }
            public string Detail { get; set; }
            public decimal Expense { get; set; }
            public decimal Income { get; set; }
            public decimal NetIncome { get; set; }

    }
}
