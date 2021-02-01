using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class EquipmentPlanEvaluationViewModel
    {
        public string EquipmentType { get; set; }
        public decimal FleetSize { get; set; }
        public string Type { get; set; }
        public decimal Actual { get; set; }
        public decimal Plan { get; set; } 
    }
}
