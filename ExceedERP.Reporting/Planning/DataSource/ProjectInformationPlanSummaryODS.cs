using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
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
using static ExceedERP.Reporting.Planning.ViewModel.PlanningVM;
using static ExceedERP.Reporting.Planning.ViewModel.ProjectPlanVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectInformationPlanSummaryODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectInformationPlanSummaryVM> GetProjectInformationPlanSummaryData(string planyear,string periodId,string projectTypeId,string kpiId,string subKpiId,string projectKpiType)
        {

            var models = new List<ProjectInformationPlanSummaryVM>();
            var projectPlans = db.ProjectActuals.ToList();

            if (planyear != null)
            {

                int.TryParse(planyear, out int glFiscalYearId);
                projectPlans = projectPlans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();


                if (projectTypeId != null)
                {
                    int.TryParse(projectTypeId, out int typeId);
                    var projectIds  = db.ProjectInformation.Where(s => s.ProjectTypeId == typeId).Select(x=>x.ProjectInformationId);
                    if (projectIds.Count() > 0)
                    {
                        projectPlans = projectPlans.Where(s => projectIds.Contains(s.ProjectInformationId)).ToList();

                    }
                }
                if(kpiId != null)
                {
                    int.TryParse(kpiId, out int KPIId);
                    projectPlans = projectPlans.Where(s => s.ProjectKPIId == KPIId).ToList();

                }
                if (subKpiId != null)
                {
                    int.TryParse(subKpiId, out int subKPIId);
                    projectPlans = projectPlans.Where(s => s.ProjectSubKPIId == subKPIId).ToList();

                }
                if (projectKpiType != null && projectPlans.Any())
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        projectPlans = projectPlans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                    }
                }
                var projectInformationPlans = projectPlans.ToList().DistinctByField(x => x.ProjectInformationId);
                foreach (var project in projectInformationPlans)
                {
                    
                    var model = new ProjectInformationPlanSummaryVM();
                    var projectInfo = db.ProjectInformation.Find(project.ProjectInformationId);
                    if (projectInfo != null)
                    {
                        model.ProjectName = projectInfo.Code + "-" + projectInfo.Name;
                        model.ContratAmount = projectInfo.ContractAmount;
                        model.Unit = "Birr";
                    }
                    
                    model.AnnualPlan = db.ProjectPlans.Where(x=>x.ProjectInformationId==project.ProjectInformationId
                                                                  &&x.GlFiscalYearId==project.GlFiscalYearId).Sum(x=>x.Plan);
                    var projectMontlyActuals = db.ProjectMonthlyActuals.Where(x=>x.ProjectActualId==project.ProjectActualId).ToList();
                    model.Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                    model.Actual = projectMontlyActuals.Count()>0? projectMontlyActuals.Sum(x => x.ActualValue) : 0; ;
                    model.Percent = model.Plan !=0? (model.Actual/ model.Plan)*100:0;
                    model.ToMonthPlan = model.Plan;
                    model.ToMonthActual = model.Actual;
                    model.ToMonthPercent = model.Percent;
                    if (periodId != null)
                    {
                        int.TryParse(periodId, out int gPeriodId);
                        model.Plan = projectMontlyActuals.Where(x=>x.GLPeriodId==gPeriodId).Sum(x => x.PlanValue);
                        model.Actual = projectMontlyActuals.Count()>0? projectMontlyActuals.Where(x =>x.GLPeriodId==gPeriodId).Sum(x => x.ActualValue):0;
                        model.Percent = model.Plan != 0 ? (model.Actual / model.Plan) * 100 : 0;
                        var period = db.GLPeriods.Find(gPeriodId);
                        if(period !=null)
                        {
                            var periodIds = db.GLPeriods.Where(x => x.Index < period.Index&&x.GlFiscalYearId==glFiscalYearId).Select(x => x.GLPeriodId);
                            if (periodIds.Count() > 0)
                            {
                                model.ToMonthPlan = projectMontlyActuals.Count()>0? projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).Sum(x => x.PlanValue):0;
                                model.ToMonthActual = projectMontlyActuals.Count()>0? projectMontlyActuals.Where(x =>periodIds.Contains(x.GLPeriodId)).Sum(x => x.ActualValue):0;
                                model.ToMonthPercent = model.ToMonthPlan != 0 ? (model.ToMonthActual / model.ToMonthPlan) * 100 : 0;

                            }

                        }



                    }
                    
                    models.Add(model);
                }
            }
            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

