using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class TrainingOneByeOneViewModel
    {
        public string Training { get; set; }
        public string Year { get; set; }
        public string Provider { get; set; }
        public decimal Cost { get; set; }      
        public int TotalEmployee { get; set; } 
        public List<TrainingEmployee> TrainingEmployees { get; set; }

    }
    public class TrainingEmployee
    {
        public string EmployeeName { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string Status { get; set; }
        public decimal CostSingle { get; set; }
    }

}
