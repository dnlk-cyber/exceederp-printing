using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class FiscalYearTrainingPlanActionPlanODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FiscalYearTrainingPlanActionPlanVM> GetTrainingActionPlan(string id, string yearId)
        {
            List<FiscalYearTrainingPlanActionPlanVM> Lists = new List<FiscalYearTrainingPlanActionPlanVM>();

            var courseTrakings = db.CourseTrackings.ToList();
            var trainingPlans = db.TrainingPlans.ToList();

            if (yearId != null)
            {
                int.TryParse(yearId, out int fiscalyearld);
                trainingPlans = trainingPlans.Where(x => x.GlFiscalYearId == fiscalyearld).ToList();
            }
            if (id != null)
            {
                int.TryParse(id, out int workUnitId);

                trainingPlans = trainingPlans.Where(x => x.WorkUnit == workUnitId).ToList();
            }

            foreach (var trainingPlan in trainingPlans)
            {
                var TrainingActionPlan = new FiscalYearTrainingPlanActionPlanVM();
                var courseList = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive&& l.LookupId==trainingPlan.Course).FirstOrDefault();              
                if (courseList != null)
                {
                    TrainingActionPlan.TrainingType = courseList.Description;

                }
                var targetGroup = db.Lookups.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive&&l.LookupId==trainingPlan.LookupId).FirstOrDefault();
                if (targetGroup !=null)
                {
                    TrainingActionPlan.TrainingCoductBody = targetGroup.Description;
                }
                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.WorkUnit);
                if (mainCompany != null)
                {
                  TrainingActionPlan.WorkUnit = mainCompany.Name;
                }

                var providerlist = db.Companies.FirstOrDefault(x => x.MainCompanyId == trainingPlan.MainCompanyId);
                if (providerlist != null)
                {
                    TrainingActionPlan.Provider = providerlist.TradeName;
                }
                TrainingActionPlan.Year = trainingPlan.GlFiscalYear.Name;
                TrainingActionPlan.TraingPeriod = trainingPlan.GlFiscalYear.Name;
                TrainingActionPlan.TrainingPlace = "";
                TrainingActionPlan.MaleTrainee = 0;
                TrainingActionPlan.FeMaleTrainee = 0;
                TrainingActionPlan.TotalTrainee = 0;
                TrainingActionPlan.Cost = 0;
                TrainingActionPlan.FixedBudget = 0;

               

                Lists.Add(TrainingActionPlan);

            }
            
            return Lists;
        }
    }
}
