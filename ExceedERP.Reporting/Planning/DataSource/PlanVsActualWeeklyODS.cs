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
using static ExceedERP.Reporting.Planning.ViewModel.ActualAndPlanningVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class PlanVsActualWeeklyODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ActualAndPlanningVM GetPlanvsActualWeeklyData(string year, string preiodId,string workUnitId)
        {

            ActualAndPlanningVM model = new ActualAndPlanningVM();
            if (year != null && preiodId != null)
            {

                int.TryParse(year, out int glFiscalYearId);
                int.TryParse(preiodId, out int glPeriodId);
                var glfiscalyear = db.GLFiscalYears.Find(glFiscalYearId);
                if (glfiscalyear != null)
                {

                   
                    var actuall = db.Actuals.FirstOrDefault(s => s.GlFiscalYearId == glfiscalyear.GlFiscalYearId);
                    
                    if(actuall != null)
                    {
                        model.Description = actuall.Description;
                        model.FiscalYear = glfiscalyear.Name;
                        model.ObjectiveItems = new List<BothObjectiveItem>();
                        var bscPlans = db.BSCActuals.Where(s => s.ActualId == actuall.ActualId).ToList();
                        if (workUnitId != null)
                        {
                            int.TryParse(workUnitId, out int orgStructureId);
                            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId).FirstOrDefault();
                            if (placement != null)
                            {
                                var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId).Select(x => x.OrgStructureId);
                                if (orgstructureIds.Count() > 0)
                                {
                                    bscPlans = db.BSCActuals.Where(x => x.ActualId == actuall.ActualId && orgstructureIds.Contains(x.OrgStructureId) || x.OrgStructureId == orgStructureId).Distinct().ToList();

                                }
                                else
                                {
                                    bscPlans = bscPlans.Where(x => x.OrgStructureId == orgStructureId).ToList();
                                }

                            }
                        }
                        foreach (var bscPlan in bscPlans)
                        {
                            BothObjectiveItem objectiveItem = new BothObjectiveItem();
                            var workUnit = db.MainCompanyStructures.Find(bscPlan.OrgStructureId);
                            if (workUnit != null)
                            {
                                objectiveItem.WorkUnit = workUnit.Name;

                            }
                            var objectives = db.Objectives.Find(bscPlan.ObjectiveId);
                            if (objectives != null)
                            {
                                objectiveItem.Objective = objectives.Name;
                                objectiveItem.Description = objectives.Description;

                            }
                            objectiveItem.IndicatorItems = new List<IndicatorPlanActualItems>();
                            var plannings = db.ActualDetails.Where(q => q.BSCActualId == bscPlan.BSCActualId&&q.ParentId==0);
                            foreach (var indicator in plannings)
                            {
                                var indicatorItem = new IndicatorPlanActualItems();

                                var indicators = db.Indicators.Find(indicator.IndicatorId);
                                if (indicators != null)
                                {
                                    indicatorItem.Indicators = indicators.Name;
                                }
                                indicatorItem.ActualVsPlanItems = new List<ActualVsPlanMontlyItems>();

                                var plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId&&x.ParentId ==indicator.IndicatorId).ToList();
                                if (indicator.HasSubIndicator)
                                {
                                    plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId && x.ParentId == indicator.IndicatorId).ToList();
                                }
                                else
                                {
                                    plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId && x.IndicatorId == indicator.IndicatorId).ToList();

                                }

                                foreach (var planning in plansOfCurrentIndicator)
                                {

                                   var planItem = new ActualVsPlanMontlyItems();
                                    var subIndicators = db.Indicators.Find(planning.IndicatorId);
                                    if (subIndicators != null)
                                    {
                                        planItem.SubIndicators = subIndicators.Name;
                                        planItem.KPIType = planning.KPIType.ToString();
                                        planItem.Description = subIndicators.Description;


                                    }

                                    planItem.AnnulPlan = planning.Plan;
                                    
                                    var monthlyActualDatas = db.MonthlyActuals.Where(x => x.ActualDetailId == planning.ActualDetailId&&x.GLPeriodId==glPeriodId).ToList();
                                    var monthlyPlanDatas = db.MonthlyPlans.Where(x => x.PlanningDetailId == planning.PlanningDetailId&&x.GLPeriodId==glPeriodId).ToList();
                                    planItem.MonthActual = monthlyActualDatas.Count()>0? monthlyActualDatas.Sum(x=>x.ActualValue):0;
                                    planItem.MonthPlan = monthlyPlanDatas.Count()>0? monthlyPlanDatas.Sum(x=>x.Value):0;
                                    planItem.MonthPercent = (planItem.MonthPlan > 0) ? (100 * planItem.MonthActual) / planItem.MonthPlan : 0;
                                    planItem.WeeklyActualVsPlanItems = new List<ActualVsPlanWeeklItems>();

                                    foreach (var pair in monthlyActualDatas.Zip(monthlyPlanDatas, (actual, plan) => new { Item1 = actual, Item2 = plan }))
                                    {
                                        var WeeklyModel = new ActualVsPlanWeeklItems();
                                        var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == pair.Item1.MonthlyActualId).ToList();
                                        var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == pair.Item2.MonthlyPlanId).ToList();

                                        WeeklyModel.ActualWeek1 = weeklyActuals.Count()>0? weeklyActuals.Sum(x=>x.WeekOne):0;
                                        WeeklyModel.ActualWeek2 = weeklyActuals.Count()>0? weeklyActuals.Sum(x=>x.WeekTwo):0;
                                        WeeklyModel.ActualWeek3 = weeklyActuals.Count()>0? weeklyActuals.Sum(x=>x.WeekThree):0;
                                        WeeklyModel.ActualWeek4 = weeklyActuals.Count()>0? weeklyActuals.Sum(x=>x.WeekFour):0;
                                        WeeklyModel.PlanWeek1 = weeklyPlans.Count()>0? weeklyPlans.Sum(x=>x.WeekOne):0;
                                        WeeklyModel.PlanWeek2 = weeklyPlans.Count()>0? weeklyPlans.Sum(x=>x.WeekTwo):0;
                                        WeeklyModel.PlanWeek3 = weeklyPlans.Count()>0? weeklyPlans.Sum(x=>x.WeekThree):0;
                                        WeeklyModel.PlanWeek4 = weeklyPlans.Count()>0? weeklyPlans.Sum(x=>x.WeekFour):0;
                                        WeeklyModel.PercentWeek1 = (WeeklyModel.PlanWeek1 > 0) ? (100 * WeeklyModel.ActualWeek1) / WeeklyModel.PlanWeek1 : 0;
                                        WeeklyModel.PercentWeek2 = (WeeklyModel.PlanWeek2 > 0) ? (100 * WeeklyModel.ActualWeek2) / WeeklyModel.PlanWeek2 : 0;
                                        WeeklyModel.PercentWeek3 = (WeeklyModel.PlanWeek3 > 0) ? (100 * WeeklyModel.ActualWeek3) / WeeklyModel.PlanWeek3 : 0;
                                        WeeklyModel.PercentWeek4 = (WeeklyModel.PlanWeek4 > 0) ? (100 * WeeklyModel.ActualWeek4) / WeeklyModel.PlanWeek4 : 0;
                                        planItem.WeeklyActualVsPlanItems.Add(WeeklyModel);


                                    }
                                  

                                    indicatorItem.ActualVsPlanItems.Add(planItem);

                                }

                                objectiveItem.IndicatorItems.Add(indicatorItem);

                            }

                            model.ObjectiveItems.Add(objectiveItem);

                        }


                    }
                }


            }

            return model;
        }
       
    }       
        
       
    
}
