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
using static ExceedERP.Reporting.Planning.ViewModel.PlanningVM;
using static ExceedERP.Reporting.Planning.ViewModel.ProjectPlanVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectPlanODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectPlanVM> GetProjectPlanData(string planyear,string businessUnitCode, string project,string projectObjectiveId, string projectKpiType)
        {

            var models = new List<ProjectPlanVM>();
            var plans = db.ProjectPlans.ToList();

            if (planyear != null)
            {

                int.TryParse(planyear, out int glFiscalYearId);
                 plans = plans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();

            }
            if (businessUnitCode != null && plans.Count()>0)
            {

                var projectTypeIds = db.ConsProjectTypes.Where(x => x.BusinessUnit == businessUnitCode).Select(x => x.ConsProjectTypeId);
                if (projectTypeIds.Count() > 0)
                {
                    var projectIds = db.ProjectInformation.Where(x => projectTypeIds.Contains(x.ProjectTypeId)).Select(x => x.ProjectInformationId);
                    if (projectIds.Count() > 0)
                    {
                        plans = plans.Where(x => projectIds.Contains(x.ProjectInformationId)).ToList();

                    }

                }


            }

            if(project != null && plans.Count()>0)
            {
                int.TryParse(project, out int projectId);
                plans = plans.Where(s => s.ProjectInformationId == projectId).ToList();
            }
            if (projectObjectiveId != null&& plans.Count()>0)
            {
                int.TryParse(projectObjectiveId, out int objectiveId);
                plans = plans.Where(s => s.ProjectObjectiveId == objectiveId).ToList();
            }
            if (projectKpiType != null && plans.Count() > 0)
            {
                var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                if (kpiIds.Any())
                {
                    plans = plans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                }
            }

            foreach (var plan in plans)
            {
                var model = new ProjectPlanVM();
                var glfiscalYears = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if(glfiscalYears != null)
                {
                    model.FiscalYear = glfiscalYears.Name;
                }
                var projectInfo = db.ProjectInformation.Find(plan.ProjectInformationId);
                if(projectInfo != null)
                {
                    model.Project = projectInfo.Code+"-"+projectInfo.Name;
                }
                var objective = db.ProjectObjectives.Find(plan.ProjectObjectiveId);
                if(objective != null)
                {
                    model.Objective = objective.ProjectObjectiveName;
                }
                var kpIs = db.ProjectKPIs.Find(plan.ProjectKPIId);
                if (projectInfo != null)
                {
                    model.KPI = kpIs.KPIName;

                }
                var subKpIs = db.ProjectSubKPIs.Find(plan.ProjectSubKPIId);
                if (projectInfo != null)
                {
                    model.SubKPIId = subKpIs.SubKPIName;

                }
                model.Target = plan.Target;
                model.Baseline = plan.Baseline;
                model.Description = plan.Description;                
                model.PlanItems = new List<ProjectPlanItem>();
                var periods = db.ProjectMonthlyPlans.Where(q => q.ProjectPlanId == plan.ProjectPlanId).ToList();
                var planItem = new ProjectPlanItem();
                foreach (var period in periods)
                {
                    var weeklyPlans = db.ProjectWeeklyPlans.Where(x => x.ProjectMonthlyPlanId == period.ProjectMonthlyPlanId).ToList();
                    var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
                    if (periodIndex != null)
                    {


                        switch (periodIndex.Index)
                        {
                            case 1:
                                planItem.July = period.Value;
                                planItem.JulyWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.JulyWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.JulyWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.JulyWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 2:
                                planItem.Augest = period.Value;
                                planItem.AugestWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.AugestWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.AugestWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.AugestWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 3:
                                planItem.September = period.Value;
                                planItem.SeptemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.SeptemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.SeptemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.SeptemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 4:
                                planItem.October = period.Value;
                                planItem.OctoberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.OctoberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.OctoberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.OctoberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 5:
                                planItem.November = period.Value;
                                planItem.NovemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.NovemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.NovemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.NovemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 6:
                                planItem.December = period.Value;
                                planItem.DecemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.DecemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.DecemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.DecemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 7:
                                planItem.January = period.Value;
                                planItem.JanuaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.JanuaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.JanuaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.JanuaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 8:
                                planItem.February = period.Value;
                                planItem.FebruaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.FebruaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.FebruaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.FebruaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 9:
                                planItem.March = period.Value;
                                planItem.MarchWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.MarchWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.MarchWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.MarchWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 10:
                                planItem.April = period.Value;
                                planItem.AprilWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.AprilWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.AprilWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.AprilWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 11:
                                planItem.May = period.Value;
                                planItem.MayWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.MayWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.MayWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.MayWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;
                            case 12:
                                planItem.June = period.Value;
                                planItem.JuneWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                planItem.JuneWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                planItem.JuneWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                planItem.JuneWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                break;

                        }
                    }

                }

                model.PlanItems.Add(planItem);
                models.Add(model);
            }


            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

