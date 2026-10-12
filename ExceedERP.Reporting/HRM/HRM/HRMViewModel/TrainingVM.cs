using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class TrainingVM
    {
        public string TypeOfTraining  { get; set; }
        public string Unit { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal Plan { get; set; }
        public decimal Excution { get; set; }
        public decimal TodatePlan { get; set; }
        public decimal TodateExcution { get; set; }
        public decimal TrainingCost { get; set; }
       
      
    }
}
