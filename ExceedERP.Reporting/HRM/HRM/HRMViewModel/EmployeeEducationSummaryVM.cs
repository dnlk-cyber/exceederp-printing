using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeEducationSummaryVM
    {
        public string EmployeeType { get; set; }
        public string Male { get; set; }
        public string Female { get; set; }
        public decimal TotalMale { get; set; }
        public decimal TotalFemale { get; set; }
        public decimal TotalEmployee { get; set; }

    }

}
