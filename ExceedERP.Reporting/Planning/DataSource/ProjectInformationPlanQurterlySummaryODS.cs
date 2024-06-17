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

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectInformationPlanQurterlySummaryODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectInformationPlanQuarterlySummaryVM> GetProjectInformationPlanSummaryData(string planyear,string periodId,string projectTypeId,string kpiId,string subKpiId,string quarter,string projectKpiType)
        {

            var models = new List<ProjectInformationPlanQuarterlySummaryVM>();
            var projectPlans = db.ProjectActuals.ToList();

            if (planyear != null)
            {

                int.TryParse(planyear, out int glFiscalYearId);
                projectPlans = projectPlans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();


                if (projectTypeId != null)
                {
                   
                }
                if(kpiId != null && projectPlans.Count() > 0)
                {
                    int.TryParse(kpiId, out int KPIId);
                    projectPlans = projectPlans.Where(s => s.ProjectKPIId == KPIId).ToList();

                }
                if (subKpiId != null && projectPlans.Count() > 0)
                {
                    int.TryParse(subKpiId, out int subKPIId);
                    projectPlans = projectPlans.Where(s => s.ProjectSubKPIId == subKPIId).ToList();

                }
                if (projectKpiType != null && projectPlans.Count() > 0)
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
                    
                    var model = new ProjectInformationPlanQuarterlySummaryVM();
                    var projectInfo = db.ProjectInformation.Find(project.ProjectInformationId);
                    if (projectInfo != null)
                    {
                        model.ProjectName = projectInfo.Code + "-" + projectInfo.Name;
                        model.ContratAmount = projectInfo.ContractAmount;
                        model.Unit = "Birr";
                    }
                    var annualProjectPlans = db.ProjectPlans.Where(x => x.ProjectInformationId == project.ProjectInformationId
                                                                 && x.GlFiscalYearId == project.GlFiscalYearId).ToList();

                    model.AnnualPlan =annualProjectPlans.Count()>0? annualProjectPlans.Sum(x=>x.Plan):0;
                    var projectMontlyActuals = db.ProjectMonthlyActuals.Where(x=>x.ProjectActualId==project.ProjectActualId).ToList();
                    model.Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                    model.Actual = projectMontlyActuals.Count()>0? projectMontlyActuals.Sum(x => x.ActualValue) : 0; ;
                    model.Percent = model.Plan !=0? (model.Actual/ model.Plan)*100:0;

                    if (periodId != null)
                    {
                        int.TryParse(periodId, out int gPeriodId);
                        var projectMontlyActualPeriods = projectMontlyActuals.Where(x => x.GLPeriodId == gPeriodId).ToList();
                        model.Plan = projectMontlyActualPeriods.Count()>0? projectMontlyActualPeriods.Sum(x => x.PlanValue):0;
                        model.Actual = projectMontlyActualPeriods.Count() > 0 ? projectMontlyActualPeriods.Sum(x => x.ActualValue) : 0;
                        model.Percent = model.Plan != 0 ? (model.Actual / model.Plan) * 100 : 0;
                    }
                    if(quarter != null)
                    {
                        switch (quarter)
                        {
                            case "Quarter 1":
                                var periodIds = db.GLPeriods.Where(x => x.Index <= 3 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodIds.Count() > 0)
                                {
                                    projectMontlyActuals = projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                    model.Quarter1Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                                    model.Quarter1Actual = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.ActualValue) : 0;
                                    model.Quarter1Percent = model.Quarter1Plan != 0 ? (model.Quarter1Actual / model.Quarter1Plan) * 100 : 0;

                                }
                                break;
                            case "Quarter 2":
                                var periodId2s = db.GLPeriods.Where(x => x.Index > 3 && x.Index <= 6 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId2s.Count() > 0)
                                {
                                    projectMontlyActuals = projectMontlyActuals.Where(x => periodId2s.Contains(x.GLPeriodId)).ToList();
                                    model.Quarter2Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                                    model.Quarter2Actual = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.ActualValue) : 0;
                                    model.Quarter2Percent = model.Quarter2Plan != 0 ? (model.Quarter2Actual / model.Quarter2Plan) * 100 : 0;

                                }
                                break;
                            case "Quarter 3":
                                var periodId3s = db.GLPeriods.Where(x => x.Index > 6 && x.Index <= 9 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId3s.Count() > 0)
                                {
                                    model.Quarter3Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodId3s.Contains(x.GLPeriodId)).Sum(x => x.PlanValue) : 0;
                                    model.Quarter3Actual = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodId3s.Contains(x.GLPeriodId)).Sum(x => x.ActualValue) : 0;
                                    model.Quarter3Percent = model.Quarter3Plan != 0 ? (model.Quarter3Actual / model.Quarter3Plan) * 100 : 0;

                                }
                                break;
                            case "Quarter 4":
                                var periodId4s = db.GLPeriods.Where(x => x.Index > 9 && x.Index <= 12 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId4s.Count() > 0)
                                {
                                    model.Quarter4Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodId4s.Contains(x.GLPeriodId)).Sum(x => x.PlanValue) : 0;
                                    model.Quarter4Actual = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodId4s.Contains(x.GLPeriodId)).Sum(x => x.ActualValue) : 0;
                                    model.Quarter4Percent = model.Quarter4Plan != 0 ? (model.Quarter4Actual / model.Quarter4Plan) * 100 : 0;

                                }
                                break;


                        }


                    }
                    else 
                    {

                        var periodIdds = db.GLPeriods.Where(x => x.GlFiscalYearId == project.GlFiscalYearId);
                        for(var index =1;  index<= 12; index += 3)
                        {
                            if (index >= 1 && index <= 3)
                            {
                                var periodIds = db.GLPeriods.Where(x => x.Index <= 3 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodIds.Count() > 0)
                                {
                                    var projectMontlyActual1s = projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                    model.Quarter1Plan = projectMontlyActual1s.Count() > 0 ? projectMontlyActual1s.Sum(x => x.PlanValue) : 0;
                                    model.Quarter1Actual = projectMontlyActual1s.Count() > 0 ? projectMontlyActual1s.Sum(x => x.ActualValue) : 0;
                                    model.Quarter1Percent = model.Quarter1Plan != 0 ? (model.Quarter1Actual / model.Quarter1Plan) * 100 : 0;

                                }
                            }
                            if (index > 3 && index <= 6)
                            {
                                var periodId2s = db.GLPeriods.Where(x => x.Index > 3 && x.Index <= 6 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId2s.Count() > 0)
                                {
                                    var projectMontlyActual2s = projectMontlyActuals.Where(x => periodId2s.Contains(x.GLPeriodId)).ToList();
                                    model.Quarter2Plan = projectMontlyActual2s.Count() > 0 ? projectMontlyActual2s.Sum(x => x.PlanValue) : 0;
                                    model.Quarter2Actual = projectMontlyActual2s.Count() > 0 ? projectMontlyActual2s.Sum(x => x.ActualValue) : 0;
                                    model.Quarter2Percent = model.Quarter2Plan != 0 ? (model.Quarter2Actual / model.Quarter2Plan) * 100 : 0;

                                }
                            }
                            if (index >6 && index <= 9)
                            {
                                var periodId3s = db.GLPeriods.Where(x => x.Index > 6 && x.Index <= 9 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId3s.Count() > 0)
                                {
                                    var projectMontlyActual3s = projectMontlyActuals.Where(x => periodId3s.Contains(x.GLPeriodId)).ToList();
                                    model.Quarter3Plan = projectMontlyActual3s.Count() > 0 ? projectMontlyActual3s.Sum(x => x.PlanValue) : 0;
                                    model.Quarter3Actual = projectMontlyActual3s.Count() > 0 ? projectMontlyActual3s.Sum(x => x.ActualValue) : 0;
                                    model.Quarter3Percent = model.Quarter3Plan != 0 ? (model.Quarter3Actual / model.Quarter3Plan) * 100 : 0;

                                }
                            }
                            if (index >9 && index <= 12)
                            {
                                var periodId4s = db.GLPeriods.Where(x => x.Index > 9 && x.Index <= 12 && x.GlFiscalYearId == project.GlFiscalYearId).Select(x => x.GLPeriodId);
                                if (periodId4s.Count() > 0)
                                {
                                    var projectMontlyActual4s = projectMontlyActuals.Where(x => periodId4s.Contains(x.GLPeriodId));
                                    model.Quarter4Plan = projectMontlyActual4s.Count() > 0 ? projectMontlyActual4s.Where(x => periodId4s.Contains(x.GLPeriodId)).Sum(x => x.PlanValue) : 0;
                                    model.Quarter4Actual = projectMontlyActual4s.Count() > 0 ? projectMontlyActual4s.Where(x => periodId4s.Contains(x.GLPeriodId)).Sum(x => x.ActualValue) : 0;
                                    model.Quarter4Percent = model.Quarter4Plan != 0 ? (model.Quarter4Actual / model.Quarter4Plan) * 100 : 0;

                                }
                            }
                            //model.ToMonthPlan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).Sum(x => x.PlanValue) : 0;
                            //model.ToMonthActual = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).Sum(x => x.ActualValue) : 0;
                            //model.ToMonthPercent = model.ToMonthPlan != 0 ? (model.ToMonthActual / model.ToMonthPlan) * 100 : 0;
                            //index += 3;
                        }

                    }
                    models.Add(model);

                }







            }
            
            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

