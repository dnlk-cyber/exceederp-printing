using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class WorkandCostSummary
    {
        public string TrainingType { get; set; }
        public string WorkUnit { get; set; }
        public string TrainingTime { get; set; }
        public int TraineeQuantity { get; set; }
        public decimal TotalCost { get; set; }  
        public string Provider { get; set; }
    }
}
