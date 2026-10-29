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
using static ExceedERP.Reporting.Planning.ViewModel.ProjectPlanAndActualVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectPlanAndActualODSODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectPlanAndActualVM> GetProjectActualData(string year,string businessUnitCode, string project,string projectObjectiveId,string projectKpiType)
        {


            var models = new List<ProjectPlanAndActualVM>();
            var plans = db.ProjectActuals.ToList();

            if (year != null)
            {

                int.TryParse(year, out int glFiscalYearId);
                plans = plans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
            }
            if (businessUnitCode != null && plans.Count() > 0)
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
            if (project != null&&plans.Any())
            {
                int.TryParse(project, out int projectId);
                plans = plans.Where(s => s.ProjectInformationId == projectId).ToList();
            }
            if (projectObjectiveId != null && plans.Any())
            {
                int.TryParse(projectObjectiveId, out int objectiveId);
                plans = plans.Where(s => s.ProjectObjectiveId == objectiveId).ToList();
            }
            if (projectKpiType != null && plans.Any() )
            {
                var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                if (kpiIds.Any())
                {
                    plans = plans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                }
            }
            foreach (var plan in plans)
            {
                var model = new ProjectPlanAndActualVM();


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
                model.ActualItems = new List<ProjectPlanActualItem>();
                var periods = db.ProjectMonthlyActuals.Where(q => q.ProjectActualId == plan.ProjectActualId).ToList();
                var actualItem = new ProjectPlanActualItem();
                decimal yearlyActual = 0;
                decimal yearlyPlan = 0;
                decimal toDateActual = 0;
                decimal toDatePlan = 0;
                foreach (var period in periods)
                {
                    var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
                    if (periodIndex != null)
                    {

                        yearlyActual += period.ActualValue;
                        yearlyPlan += period.PlanValue;
                        if (period.ActualValue > 0)
                        {
                            toDateActual += period.ActualValue;
                            toDatePlan += period.PlanValue;
                        }
                        switch (periodIndex.Index)
                        {
                            case 1:
                                actualItem.July = period.ActualValue;
                                actualItem.JulyPlan = period.PlanValue;
                                actualItem.JulyPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 2:

                                actualItem.Augest = period.ActualValue; 
                                actualItem.AugestPlan = period.PlanValue;
                                actualItem.AugestPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 3:

                               actualItem.September = period.ActualValue;
                               actualItem.SeptemberPlan = period.PlanValue;
                               actualItem.SeptemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 4:

                                actualItem.October = period.ActualValue;
                                actualItem.OctoberPlan = period.PlanValue;
                                actualItem.OctoberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 5:

                                actualItem.November = period.ActualValue;
                                actualItem.NovemberPlan = period.PlanValue;
                                actualItem.NovemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 6:

                                actualItem.December = period.ActualValue;
                                actualItem.DecemberPlan = period.PlanValue;
                                actualItem.DecemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 7:

                                actualItem.January = period.ActualValue;
                                actualItem.JanuaryPlan = period.PlanValue;
                                actualItem.JanuaryPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 8:

                                actualItem.February = period.ActualValue;
                                actualItem.FebruaryPlan = period.PlanValue;
                                actualItem.FebruaryPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 9:

                                actualItem.March = period.ActualValue;
                                actualItem.MarchPlan = period.PlanValue;
                                actualItem.MarchPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 10:

                                actualItem.April = period.ActualValue;
                                actualItem.AprilPlan = period.PlanValue;
                                actualItem.AprilPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 11:

                                actualItem.May = period.ActualValue;
                                actualItem.MayPlan = period.PlanValue;
                                actualItem.MayPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;
                            case 12:

                                actualItem.June = period.ActualValue;
                                actualItem.JunePlan = period.PlanValue;
                                actualItem.JunePercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                break;

                        }

                    }


                }
                actualItem.ToDatePlan = toDatePlan;
                actualItem.ToDateActual = toDateActual;
                actualItem.YearlyPlan = yearlyPlan-toDatePlan;
                actualItem.YearlyActual = yearlyActual-toDateActual;
                actualItem.ToDate = (toDatePlan>0) ? (100 * toDateActual) /toDatePlan: 0;
                actualItem.Yearly = (yearlyPlan - toDatePlan > 0) ? (100 * (yearlyActual-toDateActual)) / (yearlyPlan-toDatePlan) : 0;
                //actualItem.Yearly = 100- ((100 * toDateActual) / toDatePlan);
                model.ActualItems.Add(actualItem);
                models.Add(model);
            }
            
            return models;

        }
      
      

    }
}
