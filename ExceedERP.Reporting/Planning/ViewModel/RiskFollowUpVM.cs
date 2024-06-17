using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class RiskFollowUpVM
    {

        public DateTime PlanDate { get; set; }
        public string IdentifiedRisk { get; set; }
        public string Description { get; set; }
        public string TreatmentMethod { get; set; }
        public string PlannedActionToBeTaken { get; set; }
        public string ActualActionTaken { get; set; }
        public string CurrentStatus { get; set; }
    }
}
