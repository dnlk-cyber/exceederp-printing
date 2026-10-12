using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class AssignmentSummaryVM
    {
        public string AssignmentType { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total => Male + Female;
    }

}
