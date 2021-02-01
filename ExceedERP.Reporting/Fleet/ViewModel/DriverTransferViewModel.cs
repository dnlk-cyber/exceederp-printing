using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class DriverTransferViewModel : Organization
    {
          public string FromOperator { get; set; }
        public string ToOperator { get; set; }
        public Nullable<DateTime> StartingDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string Description { get; set; }

    }
}
