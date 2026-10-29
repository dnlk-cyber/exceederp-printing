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
    public class ProjectSummaryBySectorsODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectSummaryBySectorsVM> GetProjectExecutiveSummaryData(string planyear,string periodId,string Sector,string projectTypeId,string projectKpiType)
        {

            var models = new List<ProjectSummaryBySectorsVM>();
            if (planyear != null)
            {
                var projectTypes = db.ConsProjectTypes.ToList();

                int.TryParse(planyear, out int glFiscalYearId);
                var projectActuals = db.ProjectActuals.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
                var projectPlans = db.ProjectPlans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
                if (Sector != null && projectTypes.Count() > 0)
                {
                    projectTypes = projectTypes.Where(x => x.BusinessUnit == Sector).ToList();
                }

                if (projectTypeId != null && projectTypes.Count() > 0)
                {
                    int.TryParse(projectTypeId, out int typeId);

                    projectTypes = projectTypes.Where(x => x.ConsProjectTypeId == typeId).ToList();
                }
                if (projectKpiType != null && projectActuals.Count() > 0)
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        projectActuals = projectActuals.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();
                        projectPlans = projectPlans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                    }
                }
                var unitIds = new HashSet<string>(projectTypes.Select(d => d.BusinessUnit));

                foreach (var unitId in unitIds)
                {
                    var modelUnit = new ProjectSummaryBySectorsVM();
                    var CopyprojectTypes = projectTypes.Where(x => x.BusinessUnit == unitId).ToList();
                    var businessUnit = db.BusinessUnits.FirstOrDefault(x => x.Accountcode == unitId);

                    if (businessUnit != null)
                    {
                        modelUnit.ProjectSector = businessUnit.Accountcode + "-" + businessUnit.Name;
                    }
                    modelUnit.projectSummaryBySectorDetails = new List<ProjectSummaryBySectorDetailVM>();
                    foreach (var projectType in CopyprojectTypes)
                    {
                        var model = new ProjectSummaryBySectorDetailVM();
                        //var businessUnit = db.BusinessUnits.FirstOrDefault(x => x.Accountcode == projectType.BusinessUnit);
                        model.ProjectType = projectType.BusinessUnit + "-" + projectType.Name;
                        
                        var projectInfos = db.ProjectInformation.Where(x => x.ProjectTypeId == projectType.ConsProjectTypeId).ToList();

                        model.ContratAmount = projectInfos.Count() > 0 ? projectInfos.Sum(x => x.ContractAmount) : 0;
                        var projectInformationIds = projectInfos.Select(x => x.ProjectInformationId);
                        if (projectInformationIds.Count() > 0 && (projectActuals.Count() > 0 || projectPlans.Count() > 0))
                        {
                            if (projectPlans.Count() > 0)
                            {
                                projectPlans = projectPlans.Where(x => projectInformationIds.Contains(x.ProjectInformationId)).ToList();

                            }
                            if (projectActuals.Count() > 0)
                            {
                                projectActuals = projectActuals.Where(x => projectInformationIds.Contains(x.ProjectInformationId)).ToList();

                            }



                            model.AnnualPlan = projectPlans.Count() > 0 ? projectPlans.Sum(x => x.Plan) : 0;
                            if (projectActuals.Count() > 0)
                            {

                                var ProjectActualIds = projectActuals.Select(x => x.ProjectActualId);
                                if (ProjectActualIds.Count() > 0 || projectPlans.Count() > 0)
                                {


                                    var projectMontlyActuals = ProjectActualIds.Count() > 0 ? db.ProjectMonthlyActuals.Where(x => ProjectActualIds.Contains(x.ProjectActualId)).ToList() : null;
                                    model.Plan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                                    model.Excution = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.ActualValue) : 0; ;
                                    model.Percent = model.Plan != 0 ? (model.Excution / model.Plan) * 100 : 0;
                                    model.ToMonthPlan = model.Plan;
                                    model.ToMonthExcution = model.Excution;
                                    model.ToMonthPercent = model.Percent;
                                    model.ExecuteSinceCommencement = model.Excution;
                                    model.ContractPerformance = model.ContratAmount;
                                    model.WeeklyExpenses = 0;
                                    model.ProfitOrLossOfTheWeek = 0;
                                    model.ExpensesUpToThisWeek = 0;
                                    model.ProfitOrLossUpToWeek = 0;
                                    if (periodId != null && projectMontlyActuals.Count() > 0)
                                    {
                                        int.TryParse(periodId, out int gPeriodId);
                                        var projectMontlyActualsSingleMonths = projectMontlyActuals.Where(x => x.GLPeriodId == gPeriodId&&x.Actual.GlFiscalYearId==glFiscalYearId).ToList();
                                        model.Plan = projectMontlyActualsSingleMonths.Count() > 0 ? projectMontlyActualsSingleMonths.Sum(x => x.PlanValue) : 0;
                                        model.Excution = projectMontlyActualsSingleMonths.Count() > 0 ? projectMontlyActualsSingleMonths.Sum(x => x.ActualValue) : 0;
                                        model.Percent = model.Plan != 0 ? (model.Excution / model.Plan) * 100 : 0;
                                        var period = db.GLPeriods.Find(gPeriodId);
                                        if (period != null)
                                        {
                                            var periodIds = db.GLPeriods.Where(x => x.Index < period.Index && x.GlFiscalYearId == glFiscalYearId).Select(x => x.GLPeriodId);
                                            if (periodIds.Count() > 0)
                                            {
                                                projectMontlyActuals = projectMontlyActuals.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                                model.ToMonthPlan = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.PlanValue) : 0;
                                                model.ToMonthExcution = projectMontlyActuals.Count() > 0 ? projectMontlyActuals.Sum(x => x.ActualValue) : 0;
                                                model.ToMonthPercent = model.ToMonthPlan != 0 ? (model.ToMonthExcution / model.ToMonthPlan) * 100 : 0;

                                            }

                                        }



                                    }
                                }
                            }


                            modelUnit.projectSummaryBySectorDetails.Add(model);
                        }

                    }
                    modelUnit.ContratAmount = modelUnit.projectSummaryBySectorDetails.Sum(x=>x.ContratAmount);
                    modelUnit.AnnualPlan = modelUnit.projectSummaryBySectorDetails.Sum(x=>x.AnnualPlan);
                    modelUnit.Plan = modelUnit.projectSummaryBySectorDetails.Sum(x=>x.Plan);
                    modelUnit.Excution = modelUnit.projectSummaryBySectorDetails.Sum(x => x.Excution);
                    modelUnit.Percent = modelUnit.Plan != 0 ? (modelUnit.Excution / modelUnit.Plan) * 100 : 0;
                    modelUnit.ToMonthPlan = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ToMonthPlan);
                    modelUnit.ToMonthExcution = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ToMonthExcution);
                    modelUnit.ToMonthPercent = modelUnit.ToMonthPlan != 0 ? (modelUnit.ToMonthExcution / modelUnit.ToMonthPlan) * 100 : 0;
                    modelUnit.ExecuteSinceCommencement = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ExecuteSinceCommencement);
                    modelUnit.ContractPerformance = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ContractPerformance);
                    modelUnit.WeeklyExpenses = modelUnit.projectSummaryBySectorDetails.Sum(x => x.WeeklyExpenses);
                    modelUnit.ProfitOrLossOfTheWeek = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ProfitOrLossOfTheWeek);
                    modelUnit.ExpensesUpToThisWeek = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ExpensesUpToThisWeek);
                    modelUnit.ProfitOrLossUpToWeek = modelUnit.projectSummaryBySectorDetails.Sum(x => x.ProfitOrLossUpToWeek);
                    models.Add(modelUnit);
                }
            }
            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

