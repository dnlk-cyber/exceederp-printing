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
    public class ProjectExecutiveSummaryODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectExecutiveSummaryVM> GetProjectExecutiveSummaryData(string planyear,string periodId,string projectTypeId,string projectId,string projectKpiType/*,DateTime? from,DateTime? to*/)
        {

            var models = new List<ProjectExecutiveSummaryVM>();
            
            var projectTypes = db.ConsProjectTypes.ToList();
            if (planyear != null)
            {

                int.TryParse(planyear, out int glFiscalYearId);
                var projectPlans = db.ProjectActuals.Where(x=>x.GlFiscalYearId==glFiscalYearId).ToList();
                if (projectPlans.Count() > 0)
                {
                   var  projecInfiIds  = projectPlans.Select(x=>x.ProjectInformationId).ToList();
                    if (projecInfiIds.Count() > 0)
                    {

                        var prjectTypeIds = db.ProjectInformation.Where(x => projecInfiIds.Contains(x.ProjectTypeId)).Select(x=>x.ProjectTypeId).ToList();
                        if (prjectTypeIds.Count() > 0)
                        {
                            projectTypes = projectTypes.Where(x => prjectTypeIds.Contains(x.ConsProjectTypeId)).ToList();
                        }
                    }
                    if (projectId != null && projectPlans.Count() > 0)
                    {
                        int.TryParse(projectId, out int projectinfoId);
                        projectPlans = projectPlans.Where(x => x.ProjectInformationId == projectinfoId).ToList();

                    }



                    if (projectTypeId != null)
                    {
                        int.TryParse(projectTypeId, out int typeId);
                        var projectIds = db.ProjectInformation.Where(s => s.ProjectTypeId == typeId).Select(x => x.ProjectInformationId);
                        if (projectIds.Count() > 0 && projectPlans.Count() > 0)
                        {
                            projectPlans = projectPlans.Where(s => projectIds.Contains(s.ProjectInformationId)).ToList();

                        }
                        projectTypes = projectTypes.Where(x => x.ConsProjectTypeId == typeId).ToList();
                    }
                }
                if (projectKpiType != null && projectPlans.Count() > 0)
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        projectPlans = projectPlans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                    }
                }
                //if (from.HasValue)
                //{

                //}
                //if (to.HasValue)
                //{

                //}
                var projectExcutiveSummaries = db.ProjectExcutiveSettings.ToList();
                if (projectPlans.Count() > 0)
                {

                    foreach (var plan in projectExcutiveSummaries)
                    {
                    
                        var model = new ProjectExecutiveSummaryVM();
                        var glfiscalYears = db.GLFiscalYears.Find(glFiscalYearId);
                        model.MajorActivities = plan.Category;
                        model.ProjectExecutives = new List<ProjectExecutiveSummaryDetailVM>();
                        var excutiveSummaries = db.ProjectExcutiveSummarySettings.Where(x => x.ProjectExcutiveSettingId == plan.ProjectExcutiveSettingId).ToList();

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
                            //HashSet<int> projectInformationIds = projectPlansss.Count()>0? new HashSet<int>(projectPlansss.Select(x=>x.ProjectInformationId)):null;
                            //HashSet<int> typeIds = new HashSet<int>(db.ProjectInformation.Where(x => projectInformationIds.Contains(x.ProjectInformationId)).Select(x=>x.ProjectTypeId));

                            projectSummary.projectInformation = new List<ProjectInformationExecutiveSummaryDetailVM>();
                            foreach(var proInfo in projectTypes)
                            {
                                var projectPlanDetails = projectPlansss.Where(x => x.ProjectKPIId == excutiveSummary.ProjectKPIId
                                                                                    && x.ProjectSubKPIId == excutiveSummary.ProjectSubKPIId
                                                                                    && x.GlFiscalYearId == glFiscalYearId).ToList();

                                //var projectActuals = db.ProjectActuals.Where(x=>x.ProjectKPIId==excutiveSummary.ProjectKPIId
                                //                                                    &&x.ProjectSubKPIId==excutiveSummary.ProjectSubKPIId
                                //                                                    &&x.GlFiscalYearId==glFiscalYearId).ToList();
                                var projectIds = db.ProjectInformation.Where(s => s.ProjectTypeId == proInfo.ConsProjectTypeId).Select(x => x.ProjectInformationId);

                                if (projectIds.Any()  && projectPlanDetails.Any() )
                                {
                                 
                                    projectPlanDetails = projectPlanDetails.Where(x => projectIds.Contains(x.ProjectInformationId)).ToList();
                                    
                                    var projects = new ProjectInformationExecutiveSummaryDetailVM();
                                    var projectType = db.ConsProjectTypes.Find(proInfo.ConsProjectTypeId);
                                    {
                                        projects.Project = projectType.Name;

                                    }

                                    if (projectPlanDetails.Any())
                                    {
                                       
                                        var projectInfo = db.ProjectInformation.Where(x=> projectIds.Contains(x.ProjectInformationId)).Where(x => x.ProjectTypeId == proInfo.ConsProjectTypeId).ToList();
                                        if (projectInfo.Count() > 0)
                                        {
                                            // projects.Project = projectInfo.Code + " - " + projectInfo.Name;
                                            projects.ContratAmount = projectInfo.Sum(x => x.ContractAmount);
                                            projects.Unit = "Birr .";
                                        }
                                       

                                        var projectActualIds = projectPlanDetails.Select(x => x.ProjectActualId);
                                        var annualPlan = db.ProjectPlans.Where(x=> x.ProjectKPIId == excutiveSummary.ProjectKPIId
                                                                                    && x.ProjectSubKPIId == excutiveSummary.ProjectSubKPIId
                                                                                    && x.GlFiscalYearId == glFiscalYearId);
                                        projects.AnnualPlan = annualPlan.Any() ? annualPlan.Sum(x => x.Plan) : 0;
                                        var projectMonthlyPlans = db.ProjectMonthlyActuals.Where(x => projectActualIds.Contains(x.ProjectActualId)).ToList();
                                        projects.Plan = projectMonthlyPlans.Any()? projectMonthlyPlans.Sum(x => x.PlanValue) : 0;
                                        projects.Actual = projectMonthlyPlans.Any()  ? projectMonthlyPlans.Sum(x => x.ActualValue) : 0; ;
                                        projects.Percent = projects.Plan != 0 ? (projects.Actual / projects.Plan) * 100 : 0;
                                        projects.ToMonthPlan = projects.Plan;
                                        projects.ToMonthActual = projects.Actual;
                                        projects.ToMonthPercent = projects.Percent;
                                        if (periodId != null)
                                        {
                                            int.TryParse(periodId, out int gPeriodId);
                                            var singleprojectMonthlyPlans = projectMonthlyPlans.Where(x => x.GLPeriodId == gPeriodId).ToList();
                                            projects.Plan = singleprojectMonthlyPlans.Count > 0 ? singleprojectMonthlyPlans.Sum(x => x.PlanValue) : 0;
                                            projects.Actual = singleprojectMonthlyPlans.Count() > 0 ? singleprojectMonthlyPlans.Sum(x => x.ActualValue) : 0;
                                            projects.Percent = projects.Plan != 0 ? (projects.Actual / projects.Plan) * 100 : 0;
                                            var period = db.GLPeriods.Find(gPeriodId);
                                            if (period != null)
                                            {
                                                var periodIds = db.GLPeriods.Where(x => x.Index <= period.Index && x.GlFiscalYearId == glFiscalYearId).Select(x => x.GLPeriodId);
                                                if (periodIds.Count() > 0)
                                                {
                                                    projectMonthlyPlans = projectMonthlyPlans.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                                    projects.ToMonthPlan = projectMonthlyPlans.Any() ? projectMonthlyPlans.Sum(x => x.PlanValue) : 0;
                                                    projects.ToMonthActual = projectMonthlyPlans.Count() > 0 ? projectMonthlyPlans.Sum(x => x.ActualValue) : 0;
                                                    projects.ToMonthPercent = projects.ToMonthPlan != 0 ? (projects.ToMonthActual / projects.ToMonthPlan) * 100 : 0;

                                                }

                                            }
                                        }
                                    }
                                    
                                    projectSummary.projectInformation.Add(projects);
                                }
                                
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
        
       
    

