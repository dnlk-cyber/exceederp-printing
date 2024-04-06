using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class TrainingNeedRequestVM
    {
        public string TrainingType { get; set; }
        public string WorkUnit { get; set; }
        public string Year { get; set; }
        public string Provider { get; set; }
        public string TrainingCoductBody { get; set; }
        public string TraingPeriod { get; set; }
        public string Trainee { get; set; }
        public int TotalTrainee { get; set; }

        public List<TrainingNeedRequestDetailVM> TrainingNeedRequests { get; set; }
    }
   public class TrainingNeedRequestDetailVM
   {
        public string Employee { get; set; }
        public string Prerequest { get; set; }
        public string EmployeePerformanceGap { get; set; }
        public string TrainingNeedForEmployeePerformanceGap { get; set; }
        public string TheNeedForTrainingRelatedToInistitution { get; set; }
        public string ExpectedResultFromTraining { get; set; }
    }

}
