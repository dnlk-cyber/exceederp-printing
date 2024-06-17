using ExceedERP.Core.Domain.Common;
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
using Telerik.Reporting;
using static ExceedERP.Reporting.Planning.ViewModel.ProjectActualVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectActualODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectActualVM> GetProjectActualData(string year, string businessUnitCode, string project,string projectObjectiveId,string projectKpiType)
        {


            var models = new List<ProjectActualVM>();
            var plans = db.ProjectActuals.ToList();

            if (year != null)
            {

                int.TryParse(year, out int glFiscalYearId);
                plans = plans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
            }
            if (businessUnitCode != null && plans.Count() > 0)
            {

                


            }
            if (project != null&& plans.Count() > 0)
            {
                int.TryParse(project, out int projectId);
                plans = plans.Where(s => s.ProjectInformationId == projectId).ToList();
            }
            if (projectObjectiveId != null && plans.Count() > 0)
            {
                int.TryParse(projectObjectiveId, out int objectiveId);
                plans = plans.Where(s => s.ProjectObjectiveId == objectiveId).ToList();
            }
            if (projectKpiType != null && plans.Count() > 0)
            {
                var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x=>x.ProjectKPIId);
                if (kpiIds.Any())
                {
                    plans = plans.Where(x =>kpiIds.Contains(x.ProjectKPIId)).ToList();

                }
            }
            foreach (var plan in plans)
            {
                var model = new ProjectActualVM();


                var glfiscalYears = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if (glfiscalYears != null)
                {
                    model.FiscalYear = glfiscalYears.Name;
                }
                var projectInfo = db.ProjectInformation.Find(plan.ProjectInformationId);
                if (projectInfo != null)
                {
                    model.Project = projectInfo.Code + "-" + projectInfo.Name;
                }
                var objective = db.ProjectObjectives.Find(plan.ProjectObjectiveId);
                if (objective != null)
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
              
                model.Description = plan.Description;
                model.ActualItems = new List<ProjectActualItem>();
                var periods = db.ProjectMonthlyActuals.Where(q => q.ProjectActualId == plan.ProjectActualId).ToList();
                var actualItem = new ProjectActualItem();
                foreach (var period in periods)
                {
                    var weeklyActuals = db.ProjectWeeklyActuals.Where(x => x.ProjectMonthlyActualId == period.ProjectMonthlyActualId).ToList();

                    var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
                    if (periodIndex != null)
                    {


                        switch (periodIndex.Index)
                        {
                            case 1:
                                actualItem.July = period.ActualValue;
                                actualItem.JulyWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.JulyWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.JulyWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.JulyWeek4 = weeklyActuals.Sum(x => x.WeekFour);

                                break;
                            case 2:
                                
                                actualItem.Augest = period.ActualValue;
                                actualItem.AugestWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.AugestWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.AugestWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.AugestWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 3:
                                
                                actualItem.September = period.ActualValue;
                                actualItem.SeptemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.SeptemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.SeptemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.SeptemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 4:
                                
                                actualItem.October = period.ActualValue;
                                actualItem.OctoberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.OctoberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.OctoberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.OctoberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 5:

                                actualItem.November = period.ActualValue;
                                actualItem.NovemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.NovemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.NovemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.NovemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 6:

                                actualItem.December = period.ActualValue;
                                actualItem.DecemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.DecemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.DecemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.DecemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 7:
                                
                                actualItem.January = period.ActualValue;
                                actualItem.JanuaryWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.JanuaryWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.JanuaryWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.JanuaryWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 8:
                                
                                actualItem.February = period.ActualValue;
                                actualItem.FebruaryWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.FebruaryWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.FebruaryWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.FebruaryWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 9:
                                
                                actualItem.March = period.ActualValue;
                                actualItem.MarchWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.MarchWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.MarchWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.MarchWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 10:
                                
                                actualItem.April = period.ActualValue;
                                actualItem.AprilWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.AprilWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.AprilWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.AprilWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 11:
                               
                                actualItem.May = period.ActualValue;
                                actualItem.MayWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.MayWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.MayWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.MayWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;
                            case 12:
                               
                                actualItem.June = period.ActualValue;
                                actualItem.JuneWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                actualItem.JuneWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                actualItem.JuneWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                actualItem.JuneWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                break;

                        }
                    }

                }
                
                model.ActualItems.Add(actualItem);
                models.Add(model);
            }


            return models;

        }
      
      

    }
}
