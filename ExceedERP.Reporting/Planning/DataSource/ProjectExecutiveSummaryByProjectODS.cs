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
    public class ProjectExecutiveSummaryByProjectODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectExecutiveSummaryVM> GetProjectExecutiveSummaryData(string planyear,string periodId,string projectTypeId,string projectId,string projectKpiType)
        {

            var models = new List<ProjectExecutiveSummaryVM>();
            
            var projectTypes = db.ConsProjectTypes.ToList();
            if (!string.IsNullOrEmpty(planyear))
            {

                int.TryParse(planyear, out int glFiscalYearId);
                var projectPlans = db.ProjectActuals.Where(x=>x.GlFiscalYearId==glFiscalYearId).ToList();
                if(!string.IsNullOrEmpty(projectId) && projectPlans.Count() > 0)
                {
                    int.TryParse(projectId, out int projectinfoId);
                    projectPlans = projectPlans.Where(x => x.ProjectInformationId == projectinfoId).ToList();
                }
                
                if (!string.IsNullOrEmpty( projectTypeId))
                {
                    
                }
                if (!string.IsNullOrEmpty(projectKpiType) && projectPlans.Any())
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        projectPlans = projectPlans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                    }
                }

                var projectExcutiveSummaries = db.ProjectExcutiveSettings.ToList();
                if (projectExcutiveSummaries.Any()&& !string.IsNullOrEmpty(projectKpiType))
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        var projectExcutiveSettingIds = db.ProjectExcutiveSummarySettings.Where(x => kpiIds.Contains(x.ProjectKPIId)).Select(x=>x.ProjectExcutiveSettingId);
                        if (projectExcutiveSettingIds.Any())
                        {
                            projectExcutiveSummaries = projectExcutiveSummaries.Where(x => projectExcutiveSettingIds.Contains(x.ProjectExcutiveSettingId)).ToList();
                        }
                    }
                }
              
                if (projectPlans.Any())
                {

                    foreach (var plan in projectExcutiveSummaries)
                    {
                    
                        var model = new ProjectExecutiveSummaryVM();
                        var glfiscalYears = db.GLFiscalYears.Find(glFiscalYearId);
                        model.MajorActivities = plan.Category;
                        model.ProjectExecutives = new List<ProjectExecutiveSummaryDetailVM>();

                        var excutiveSummaries = db.ProjectExcutiveSummarySettings.Where(x => x.ProjectExcutiveSettingId == plan.ProjectExcutiveSettingId).ToList();
                        if (projectKpiType != null && excutiveSummaries.Any())
                        {
                            var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType).Select(x => x.ProjectKPIId);
                            if (kpiIds.Any())
                            {
                                excutiveSummaries = excutiveSummaries.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                            }
                        }

                        foreach (var excutiveSummary in excutiveSummaries)
                        {
                            var projectSummary = new ProjectExecutiveSummaryDetailVM();
                            var kpIs = db.ProjectKPIs.Find(excutiveSummary.ProjectKPIId);
                            if (kpIs != null)
                            {
                                projectSummary.MajorActivities = kpIs.KPIName;

                            }
                            var subKpIs = db.ProjectSubKPIs.Find(excutiveSummary.ProjectSubKPIId);
                            if (subKpIs != null && kpIs != null)
                            {
                                projectSummary.MajorActivities = kpIs.KPIName + "-" + subKpIs.SubKPIName;
                                projectSummary.Unit = subKpIs.Unit;
                            }
                           
                            var  projectPlansss = projectPlans.Where(x => x.ProjectKPIId == excutiveSummary.ProjectKPIId && x.ProjectSubKPIId == excutiveSummary.ProjectSubKPIId).ToList();
                            projectSummary.projectInformation = new List<ProjectInformationExecutiveSummaryDetailVM>();
                            foreach(var proInfo in projectPlansss)
                            {

                                
                                var projects = new ProjectInformationExecutiveSummaryDetailVM();
                            
                                var projectInfo = db.ProjectInformation.Where(x => x.ProjectInformationId == proInfo.ProjectInformationId).FirstOrDefault();
                                if (projectInfo !=null)
                                {
                                    projects.Project = projectInfo.Code + "-" + projectInfo.Name;
                                    projects.ContratAmount = projectInfo.ContractAmount;
                                    projects.Unit = "Birr .";
                                }
                                  
                                projects.AnnualPlan = db.ProjectPlans.Where(x => x.ProjectKPIId == excutiveSummary.ProjectKPIId
                                                                                    && x.ProjectInformationId == excutiveSummary.ProjectSubKPIId
                                                                                    && x.ProjectSubKPIId == proInfo.ProjectInformationId
                                                                                    && x.GlFiscalYearId == glFiscalYearId).ToList().Sum(x=>x.Plan);

                                var projectMonthlyPlans = db.ProjectMonthlyActuals.Where(x => x.ProjectActualId==proInfo.ProjectActualId).ToList();
                                projects.Plan = projectMonthlyPlans.Count() > 0 ? projectMonthlyPlans.Sum(x => x.PlanValue) : 0;
                                
                                projects.Actual = projectMonthlyPlans.Count() > 0 ? projectMonthlyPlans.Sum(x => x.ActualValue) : 0; ;
                                projects.Percent = projects.Plan != 0 ? (projects.Actual / projects.Plan) * 100 : 0;
                                projects.ToMonthPlan = projects.Plan;
                                projects.ToMonthActual = projects.Actual;
                                projects.ToMonthPercent = projects.Percent;
                                if (periodId != null)
                                {
                                    int.TryParse(periodId, out int gPeriodId);
                                    var singleprojectMonthlyPlans = projectMonthlyPlans.Where(x => x.GLPeriodId == gPeriodId).ToList();
                                    projects.Plan = singleprojectMonthlyPlans.Count>0?singleprojectMonthlyPlans.Sum(x => x.PlanValue):0;
                                    projects.Actual = singleprojectMonthlyPlans.Count() > 0 ? singleprojectMonthlyPlans.Sum(x => x.ActualValue) : 0;
                                    projects.Percent = projects.Plan != 0 ? (projects.Actual / projects.Plan) * 100 : 0;
                                    var period = db.GLPeriods.Find(gPeriodId);
                                    if (period != null)
                                    {
                                        var periodIds = db.GLPeriods.Where(x => x.Index <= period.Index && x.GlFiscalYearId == glFiscalYearId).Select(x => x.GLPeriodId);
                                        if (periodIds.Count() > 0)
                                        {
                                            projectMonthlyPlans = projectMonthlyPlans.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                            projects.ToMonthPlan = projectMonthlyPlans.Count() > 0 ? projectMonthlyPlans.Sum(x => x.PlanValue) : 0;
                                            projects.ToMonthActual = projectMonthlyPlans.Count() > 0 ? projectMonthlyPlans.Sum(x => x.ActualValue) : 0;
                                            projects.ToMonthPercent = projects.ToMonthPlan != 0 ? (projects.ToMonthActual / projects.ToMonthPlan) * 100 : 0;

                                        }

                                    }



                                }
                                
                           

                            projectSummary.projectInformation.Add(projects);


                            }
                            projectSummary.ContratAmount = projectSummary.projectInformation.Sum(x => x.ContratAmount);
                            projectSummary.AnnualPlan = projectSummary.projectInformation.Sum(x => x.AnnualPlan);
                            projectSummary.Actual = projectSummary.projectInformation.Sum(x => x.Actual);
                            projectSummary.Plan = projectSummary.projectInformation.Sum(x => x.Plan);
                            projectSummary.Percent = projectSummary.projectInformation.Sum(x => x.Percent);
                            projectSummary.ToMonthPlan = projectSummary.projectInformation.Sum(x => x.ToMonthPlan);
                            projectSummary.ToMonthActual = projectSummary.projectInformation.Sum(x => x.ToMonthActual);
                            projectSummary.ToMonthPlan = projectSummary.projectInformation.Sum(x => x.ToMonthPlan);
                            projectSummary.ToMonthPercent = projectSummary.ToMonthPlan>0 ? (projectSummary.ToMonthActual/ projectSummary.ToMonthPlan)*100:0;
                            model.ProjectExecutives.Add(projectSummary);
                      

                        }
                        model.ContratAmount = model.ProjectExecutives.Sum(x => x.ContratAmount);
                        model.AnnualPlan = model.ProjectExecutives.Sum(x => x.AnnualPlan);
                        model.Actual = model.ProjectExecutives.Sum(x => x.Actual);
                        model.Plan = model.ProjectExecutives.Sum(x => x.Plan);
                        model.Percent = model.ProjectExecutives.Sum(x => x.Percent);
                        model.ToMonthPlan = model.ProjectExecutives.Sum(x => x.ToMonthPlan);
                        model.ToMonthActual = model.ProjectExecutives.Sum(x => x.ToMonthActual);
                        model.ToMonthPercent = model.ProjectExecutives.Sum(x => x.ToMonthPercent);
                        models.Add(model);
                    }

                }
            }
            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

