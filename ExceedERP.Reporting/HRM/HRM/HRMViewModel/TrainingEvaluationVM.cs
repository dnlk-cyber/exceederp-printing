using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class TrainingEvaluationCategory
    {
        public string Category { get; set; }
        public List<TrainingEvaluationVM> TrainingEvaluations { get; set; }
    }
    public class TrainingEvaluationVM
    {
        public string Criteria { get; set; }
        public string Excellent { get; set; }
        public string VeryGood { get; set; }
        public string Good { get; set; }
        public string Low { get; set; }
        public string VeryLow { get; set; }
    }
}
