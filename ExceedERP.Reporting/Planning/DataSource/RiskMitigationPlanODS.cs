using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Planning.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class RiskMitigationPlanODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RiskMitigationPlanVM> GetRiskMitigationPlanData(DateTime? FromDate, DateTime? ToDate)
        {

            List<RiskMitigationPlanVM> List =  new List<RiskMitigationPlanVM>();
            var riskMitigationPlans = db.RiskMitigationPlans.AsNoTracking().ToList();
            if(FromDate.HasValue)
            {

                riskMitigationPlans = riskMitigationPlans.Where(x => x.PlanDate.Date >= FromDate.Value.Date).ToList();
          
            }
            if (ToDate.HasValue)
            {

                riskMitigationPlans = riskMitigationPlans.Where(x => x.PlanDate.Date <= ToDate.Value.Date).ToList();

            }
            foreach (var riskPlan in riskMitigationPlans)
            {
                var mitigationPlan = new RiskMitigationPlanVM
                {
                    IdentifiedRisk =riskPlan.IdentifiedRisk,
                    RiskLevel = riskPlan.RiskLevel.ToString(),
                    TreatmentMethod = riskPlan.ActionToBeTaken,
                    ActionToBeTaken = riskPlan.IdentifiedRisk,
                    MainResponsibly = riskPlan.MainResponsibly,
                    SupportResponsibly = riskPlan.SupportResponsibly,
                    Opportunity = riskPlan.Opportunity,
                    Time = riskPlan.Time,
                    MonitoringMethod = riskPlan.MonitoringMethod,
                    Evaluation = riskPlan.Evaluation,
                    PlanDate = riskPlan.PlanDate,


                };
                List.Add(mitigationPlan);
            }
            return List;
        }
      
    }       
        
       
    
}
