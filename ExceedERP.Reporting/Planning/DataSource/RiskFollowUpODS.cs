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
    public class RiskFollowUpODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RiskFollowUpVM> GetRiskFellowUpPlanData(DateTime? fromDate, DateTime? toDate)
        {

            List<RiskFollowUpVM> List =  new List<RiskFollowUpVM>();
            var riskFellowUps = db.RiskFollowUps.AsNoTracking().ToList();
            if (fromDate.HasValue)
            {

                riskFellowUps = riskFellowUps.Where(x => x.PlanDate.Date >= fromDate.Value.Date && x.PlanDate.Date <= toDate.Value.Date).ToList();
          
            }
            if (toDate.HasValue)
            {

                riskFellowUps = riskFellowUps.Where(x => x.PlanDate.Date <= toDate.Value.Date).ToList();

            }
            foreach (var fellowPlan in riskFellowUps)
            {
               var riskmitigation = db.RiskMitigationPlans.Find(fellowPlan.RiskMitigationPlanId);
               if(riskmitigation != null){


                    var riskfellowUPlan = new RiskFollowUpVM
                    {
                        IdentifiedRisk = riskmitigation.IdentifiedRisk,
                        Description = fellowPlan.Description,
                        TreatmentMethod = fellowPlan.TreatmentMethod,
                        PlannedActionToBeTaken = fellowPlan.PlannedActionToBeTaken,
                        ActualActionTaken = fellowPlan.ActualActionTaken,
                        CurrentStatus = fellowPlan.CurrentStatus,
                        PlanDate = fellowPlan.PlanDate


                    };
                    List.Add(riskfellowUPlan);
               }
            }
            return List;
        }
      
    }       
        
       
    
}
