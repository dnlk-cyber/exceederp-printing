using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ProbationMeasureVM
    {
        public string EmpName { get; set; }
        public string JobTitle { get; set; }
        public string Workunit { get; set; }
        public string HireDate { get; set; }
        public string ProbationEndDate { get; set; }
        public List<ProbationMeasureDetailVM> ProbationMeasureDetail { get; set; }
        public List<EmployementTypeVM> EmployementType { get; set; }
    }
}


    
    
    