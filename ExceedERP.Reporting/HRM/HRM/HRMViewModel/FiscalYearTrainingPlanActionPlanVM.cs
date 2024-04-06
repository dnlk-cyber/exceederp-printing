using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class FiscalYearTrainingPlanActionPlanVM
    {
        public string TrainingType { get; set; }
        public string WorkUnit { get; set; }
        public string Year { get; set; }
        public string Provider { get; set; }
        public int MaleTrainee { get; set; }
        public int FeMaleTrainee { get; set; }
        public int TotalTrainee { get; set; }
        public string TraingPeriod { get; set; }
        public string TrainingCoductBody { get; set; }
        public string TrainingPlace { get; set; }
        public decimal Cost { get; set; }      
        public decimal FixedBudget { get; set; } 
        //public List<TrainingEmployee> TrainingEmployees { get; set; }

    }
    //public class TrainingEmployee
    //{
    //    public string EmployeeName { get; set; }
    //    public string Branch { get; set; }
    //    public string WorkUnit { get; set; }
    //    public string Status { get; set; }
    //    public decimal CostSingle { get; set; }
    //}

}
