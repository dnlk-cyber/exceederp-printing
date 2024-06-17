using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class ProjectPlanVsActualWeeklyODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectPlanVsActualWeeklyVM> GetProjectActualData(string year, string businessUnitCode, string project, string projectObjectiveId, string preiodId, string projectKpiType)
        {


            var models = new List<ProjectPlanVsActualWeeklyVM>();
            var projectActuals = db.ProjectActuals.ToList();
            if (!string.IsNullOrEmpty(year))
            {

                int.TryParse(year, out int glFiscalYearId);
                projectActuals = projectActuals.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
            }

            if (!string.IsNullOrEmpty(businessUnitCode) && projectActuals.Any())
            {

                

            }
            if (!string.IsNullOrEmpty(project) && projectActuals.Any())
            {
                int.TryParse(project, out int projectId);
                projectActuals = projectActuals.Where(s => s.ProjectInformationId == projectId).ToList();
            }
            if (!string.IsNullOrEmpty(projectObjectiveId) && projectActuals.Any())
            {
                int.TryParse(projectObjectiveId, out int objectiveId);
                projectActuals = projectActuals.Where(s => s.ProjectObjectiveId == objectiveId).ToList();
            }
            if (!string.IsNullOrEmpty(projectKpiType) && projectActuals.Any())
            {
                var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                if (kpiIds.Any())
                {
                    projectActuals = projectActuals.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                }
            }
            var projectActualByFiscalYears = projectActuals.DistinctByField(x => x.GlFiscalYearId).ToList();
            foreach (var plan in projectActualByFiscalYears)
            {
                var model = new ProjectPlanVsActualWeeklyVM();
                var glfiscalYears = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if (glfiscalYears != null)
                {
                    model.FiscalYear = glfiscalYears.Name;
                }
                model.ActualItems = new List<ProjectPlanVsActualKPIVM>();
                var AllProjects = projectActuals.Where(x => x.GlFiscalYearId == plan.GlFiscalYearId).ToList();

                foreach (var projectplan in AllProjects)
                {

                    var ModelKpI = new ProjectPlanVsActualKPIVM();
                    var projectInfo = db.ProjectInformation.Find(projectplan.ProjectInformationId);
                    if (projectInfo != null)
                    {
                        ModelKpI.Project = projectInfo.Code + "-" + projectInfo.Name;
                    }
                    var objective = db.ProjectObjectives.Find(projectplan.ProjectObjectiveId);
                    if (objective != null)
                    {
                        ModelKpI.Objective = objective.ProjectObjectiveName;
                    }
                    var kpIs = db.ProjectKPIs.Find(projectplan.ProjectKPIId);
                    if (kpIs != null)
                    {
                        ModelKpI.KPI = kpIs.KPIName;
                    }
                    var subKpIs = db.ProjectSubKPIs.Find(projectplan.ProjectSubKPIId);
                    if (subKpIs != null)
                    {
                        ModelKpI.SubKPIId = subKpIs.SubKPIName;
                    }
                    ModelKpI.Description = projectplan.Description;
                    ModelKpI.ProjectActualVsPlanMOnthlyItems = new List<ProjectActualVsPlanMontlyKPIItems>();

                    var projectMonthlyActuals = db.ProjectMonthlyActuals.Where(x => x.ProjectActualId == projectplan.ProjectActualId
                                                                                    &&x.Actual.ProjectInformationId== projectplan.ProjectInformationId
                                                                                    &&x.Actual.ProjectObjectiveId== projectplan.ProjectObjectiveId
                                                                                    &&x.Actual.ProjectKPIId== projectplan.ProjectKPIId
                                                                                    &&x.Actual.ProjectSubKPIId== projectplan.ProjectSubKPIId
                                                                                    &&x.Actual.GlFiscalYearId== projectplan.GlFiscalYearId).ToList();
                    if (!string.IsNullOrEmpty(preiodId))
                    {
                        int.TryParse(preiodId, out int glPeriodId);
                        projectMonthlyActuals = projectMonthlyActuals.Where(x => x.GLPeriodId == glPeriodId).ToList();

                    }
                    foreach (var projectMonth in projectMonthlyActuals)
                    {
                        var actualItem = new ProjectActualVsPlanMontlyKPIItems();
                        var period = db.GLPeriods.Find(projectMonth.GLPeriodId);
                        if(period != null)
                        {
                            actualItem.Month = period.Name;
                        }
                        actualItem.MonthPlan = projectMonth.PlanValue;
                        actualItem.MonthActual = projectMonth.ActualValue;
                        actualItem.MonthPercent = projectMonth.PlanValue > 0 ? (projectMonth.ActualValue / projectMonth.PlanValue) * 100 : 0;
                        actualItem.ProjectWeeklyActualVsPlanItems = new List<ProjectActualVsPlanWeeklyKPIItems>();
                        var weeklyActuals = db.ProjectWeeklyActuals.Where(x => x.MonthlyActualDetails.ProjectActualId == projectMonth.ProjectActualId &&
                                                                               x.MonthlyActualDetails.Actual.ProjectInformationId == projectMonth.Actual.ProjectInformationId
                                                                               && x.MonthlyActualDetails.Actual.GlFiscalYearId == projectMonth.Actual.GlFiscalYearId
                                                                               && x.MonthlyActualDetails.Actual.ProjectObjectiveId == projectMonth.Actual.ProjectObjectiveId
                                                                               && x.MonthlyActualDetails.Actual.ProjectKPIId == projectMonth.Actual.ProjectKPIId
                                                                               && x.MonthlyActualDetails.Actual.ProjectSubKPIId == projectMonth.Actual.ProjectSubKPIId
                                                                               && x.MonthlyActualDetails.GLPeriodId==projectMonth.GLPeriodId).ToList();

                        var weeklyPlans = db.ProjectWeeklyPlans.Where(x => x.Monthlyplandetal.GetPlan.ProjectInformationId == projectMonth.Actual.ProjectInformationId
                                                                               && x.Monthlyplandetal.GetPlan.GlFiscalYearId == projectMonth.Actual.GlFiscalYearId
                                                                               && x.Monthlyplandetal.GetPlan.ProjectObjectiveId == projectMonth.Actual.ProjectObjectiveId
                                                                               && x.Monthlyplandetal.GetPlan.ProjectKPIId == projectMonth.Actual.ProjectKPIId
                                                                               && x.Monthlyplandetal.GetPlan.ProjectSubKPIId == projectMonth.Actual.ProjectSubKPIId
                                                                               && x.Monthlyplandetal.GLPeriodId == projectMonth.GLPeriodId).ToList();
                        if(weeklyActuals.Any()|| weeklyPlans.Any())
                        {

                            var actualVsPlanWeekly = new ProjectActualVsPlanWeeklyKPIItems();
                            actualVsPlanWeekly.ActualWeek1 = weeklyActuals.Any()? weeklyActuals.Sum(x=>x.WeekOne):0;
                            actualVsPlanWeekly.ActualWeek2 = weeklyActuals.Any() ? weeklyActuals.Sum(x => x.WeekTwo ):0;
                            actualVsPlanWeekly.ActualWeek3 = weeklyActuals.Any() ? weeklyActuals.Sum(x => x.WeekThree):0 ;
                            actualVsPlanWeekly.ActualWeek4 = weeklyActuals.Any() ? weeklyActuals.Sum(x => x.WeekFour):0;
                            actualVsPlanWeekly.PlanWeek1 = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekOne) : 0;
                            actualVsPlanWeekly.PlanWeek2 = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekTwo) : 0;
                            actualVsPlanWeekly.PlanWeek3 = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekThree) : 0;
                            actualVsPlanWeekly.PlanWeek4 = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekFour) : 0;
                            actualVsPlanWeekly.PercentWeek1 = (actualVsPlanWeekly.PlanWeek1 > 0) ? (100 * actualVsPlanWeekly.ActualWeek1) / actualVsPlanWeekly.PlanWeek1 : 0;
                            actualVsPlanWeekly.PercentWeek2 = (actualVsPlanWeekly.PlanWeek2 > 0) ? (100 * actualVsPlanWeekly.ActualWeek2) / actualVsPlanWeekly.PlanWeek2 : 0;
                            actualVsPlanWeekly.PercentWeek3 = (actualVsPlanWeekly.PlanWeek3 > 0) ? (100 * actualVsPlanWeekly.ActualWeek3) / actualVsPlanWeekly.PlanWeek3 : 0;
                            actualVsPlanWeekly.PercentWeek4 = (actualVsPlanWeekly.PlanWeek4 > 0) ? (100 * actualVsPlanWeekly.ActualWeek4) / actualVsPlanWeekly.PlanWeek4 : 0;
                            actualItem.ProjectWeeklyActualVsPlanItems.Add(actualVsPlanWeekly);
                        }
                        actualItem.ActualWeek1=  actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.ActualWeek1);
                        actualItem.ActualWeek2=  actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.ActualWeek2);
                        actualItem.ActualWeek3=  actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.ActualWeek3);
                        actualItem.ActualWeek4=  actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.ActualWeek4);
                        actualItem.PlanWeek1 =   actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.PlanWeek1);
                        actualItem.PlanWeek2 =   actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.PlanWeek2);
                        actualItem.PlanWeek3 =   actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.PlanWeek3);
                        actualItem.PlanWeek4 =   actualItem.ProjectWeeklyActualVsPlanItems.Sum(x=>x.PlanWeek4);
                        actualItem.PercentWeek1= actualItem.PlanWeek1 !=0? (actualItem.ActualWeek1/ actualItem.PlanWeek1)*100:0;
                        actualItem.PercentWeek2 = actualItem.PlanWeek2 != 0 ? (actualItem.ActualWeek2 / actualItem.PlanWeek2) * 100 : 0; ;
                        actualItem.PercentWeek3 = actualItem.PlanWeek3 != 0 ? (actualItem.ActualWeek3 / actualItem.PlanWeek3) * 100 : 0; ;
                        actualItem.PercentWeek4 = actualItem.PlanWeek4 != 0 ? (actualItem.ActualWeek4 / actualItem.PlanWeek4) * 100 : 0; ;

                        ModelKpI.ProjectActualVsPlanMOnthlyItems.Add(actualItem);

                    }

                    ModelKpI.ActualWeek1 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.ActualWeek1);
                    ModelKpI.ActualWeek2 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.ActualWeek2);
                    ModelKpI.ActualWeek3 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.ActualWeek3);
                    ModelKpI.ActualWeek4 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.ActualWeek4);
                    ModelKpI.PlanWeek1 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.PlanWeek1);
                    ModelKpI.PlanWeek2 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.PlanWeek2);
                    ModelKpI.PlanWeek3 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.PlanWeek3);
                    ModelKpI.PlanWeek4 = ModelKpI.ProjectActualVsPlanMOnthlyItems.Sum(x => x.PlanWeek4);
                    ModelKpI.PercentWeek1 = ModelKpI.PlanWeek1 != 0 ? (ModelKpI.ActualWeek1 / ModelKpI.PlanWeek1) * 100 : 0;
                    ModelKpI.PercentWeek2 = ModelKpI.PlanWeek2 != 0 ? (ModelKpI.ActualWeek2 / ModelKpI.PlanWeek2) * 100 : 0; ;
                    ModelKpI.PercentWeek3 = ModelKpI.PlanWeek3 != 0 ? (ModelKpI.ActualWeek3 / ModelKpI.PlanWeek3) * 100 : 0; ;
                    ModelKpI.PercentWeek4 = ModelKpI.PlanWeek4 != 0 ? (ModelKpI.ActualWeek4 / ModelKpI.PlanWeek4) * 100 : 0; ;

                    model.ActualItems.Add(ModelKpI);
                   
                }


                models.Add(model);
            }
            return models;


        }
    }

}
