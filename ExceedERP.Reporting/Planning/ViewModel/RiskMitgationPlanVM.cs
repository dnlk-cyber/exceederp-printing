using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class RiskMitigationPlanVM
    {
        
        public string IdentifiedRisk { get; set; }
        public string RiskLevel { get; set; }
        public string TreatmentMethod { get; set; }
        public string ActionToBeTaken { get; set; }
        public string MainResponsibly { get; set; }
        public string SupportResponsibly { get; set; }
        public string Opportunity { get; set; }
        public string Time { get; set; }
        public string MonitoringMethod { get; set; }
        public string Evaluation { get; set; }      
        public DateTime PlanDate { get; set; }
    }    
   
}
