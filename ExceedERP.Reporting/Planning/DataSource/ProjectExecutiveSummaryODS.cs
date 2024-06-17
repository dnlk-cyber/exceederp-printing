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
        public List<ProjectExecutiveSummaryVM> GetProjectExecutiveSummaryData(string planyear,string periodId,string projectTypeId,string projectId,string projectKpiType)
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

                       
                    }
                    if (projectId != null && projectPlans.Count() > 0)
                    {
                        int.TryParse(projectId, out int projectinfoId);
                        projectPlans = projectPlans.Where(x => x.ProjectInformationId == projectinfoId).ToList();

                    }



                    if (projectTypeId != null)
                    {
                        
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
        
       
    

