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

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ExecutiveSummaryODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanExecutiveSummaryVM> GetExecutiveSummaryData(string fiscalYearId,string periodId,string workUnitId)
        {

            var  planLists  = new List<PlanExecutiveSummaryVM>();
            if (fiscalYearId != null)
            {
                int.TryParse(fiscalYearId, out int yearId);
                var actual = db.Actuals.Where(x => x.GlFiscalYearId == yearId).FirstOrDefault();
                if (actual != null)
                {
                    var bSCActuals = db.BSCActuals.Where(x => x.ActualId == actual.ActualId);
                  //  var actualDetails = db.ActualDetails.Where(x => x.GetSCActual.ActualId == actual.ActualId).ToList();
                    if(workUnitId != null && bSCActuals.Any())
                    {
                        int.TryParse(workUnitId, out int orgId);
                      //  actualDetails = actualDetails.Where(x => x.GetSCActual.OrgStructureId == orgId).ToList();
                        bSCActuals = bSCActuals.Where(x => x.OrgStructureId == orgId);
                    }
                  
                    foreach (var bSCActual in bSCActuals)
                    {
                        var bscPlanLists = new PlanExecutiveSummaryVM();
                        var objective = db.Objectives.FirstOrDefault(x => x.ObjectiveId == bSCActual.ObjectiveId);
                        if (objective != null)
                        {
                            bscPlanLists.Objective = objective.Name;
                        }
                        var depart = db.MainCompanyStructures.Where(x => x.OrgStructureId == bSCActual.OrgStructureId).FirstOrDefault();
                        if (depart != null)
                        {
                            bscPlanLists.Department = depart.Name;
                        }
                        bscPlanLists.ExecutiveSummaries = new List<ExecutiveSummaryVM>();
                        HashSet<int> indicatorIds = new HashSet<int>(db.ExcutivePlanKPIs.Select(x => x.IndicatorId));

                        if (indicatorIds.Any())
                        {
                            var actualDetails = db.ActualDetails.Where(x => indicatorIds.Contains(x.IndicatorId)
                                                                    &&x.BSCActualId==bSCActual.BSCActualId).ToList();
                            HashSet<int> ActualDetailsIndicatorIds = new HashSet<int>(actualDetails.Select(x => x.IndicatorId));

                            foreach (var actualDetail in actualDetails)
                            {


                                var exData = new ExecutiveSummaryVM();
                                var monthkyAcuals = db.MonthlyActuals.Where(x =>x.ActualDetailId== actualDetail.ActualDetailId).ToList();

                                var indicators = db.Indicators.Find(actualDetail.IndicatorId);
                                if (indicators != null)
                                {
                                    exData.KeyPerformance = indicators.Name;
                                    exData.Unit = indicators.Unit;
                                }
                               
                                //exData.ThisWeekPlan = monthkyAcual.Count() > 0 ? monthkyAcual.Sum(x => x.PlanValue) : 0;
                                //exData.ThisWeekActual = monthkyAcual.Count() > 0 ? monthkyAcual.Sum(x => x.ActualValue) : 0;

                                //exData.ThisWeekPercent = exData.ThisWeekPlan > 0 ? exData.ThisWeekActual / exData.ThisWeekPlan : 0;

                                exData.UpToThisWeekPlan = monthkyAcuals.Count() > 0 ? monthkyAcuals.Sum(x => x.PlanValue) : 0;
                                exData.UPToThisWeekActual = monthkyAcuals.Count() > 0 ? monthkyAcuals.Sum(x => x.ActualValue) : 0;

                                exData.UPToThisWeekPercent = exData.UpToThisWeekPlan > 0 ? exData.UPToThisWeekActual / exData.UpToThisWeekPlan : 0;
                                if (periodId != null)
                                {
                                    int.TryParse(periodId, out int gPeriodId);
                                    var MontlyActualsSingleMonths = monthkyAcuals.Where(x => x.GLPeriodId == gPeriodId).ToList();
                                    exData.ThisWeekPlan = MontlyActualsSingleMonths.Count() > 0 ? MontlyActualsSingleMonths.Sum(x => x.PlanValue) : 0;
                                    exData.ThisWeekActual = MontlyActualsSingleMonths.Count() > 0 ?MontlyActualsSingleMonths.Sum(x => x.ActualValue) : 0;
                                    exData.ThisWeekPercent = exData.ThisWeekPlan != 0 ? (exData.ThisWeekActual / exData.ThisWeekPlan) : 0;
                                    var period = db.GLPeriods.Find(gPeriodId);
                                    if (period != null)
                                    {
                                        var periodIds = db.GLPeriods.Where(x => x.Index <= period.Index && x.GlFiscalYearId == yearId).Select(x => x.GLPeriodId);
                                        if (periodIds.Count() > 0)
                                        {
                                            var planMontlyActuals = monthkyAcuals.Where(x => periodIds.Contains(x.GLPeriodId)).ToList();
                                            exData.UpToThisWeekPlan = planMontlyActuals.Count() > 0 ? planMontlyActuals.Sum(x => x.PlanValue) : 0;
                                            exData.UPToThisWeekActual = planMontlyActuals.Count() > 0 ? planMontlyActuals.Sum(x => x.ActualValue) : 0;
                                            exData.UPToThisWeekPercent = exData.UpToThisWeekPlan > 0 ? (exData.UPToThisWeekActual / exData.UpToThisWeekPlan): 0;

                                        }

                                    }



                                }

                                bscPlanLists.ExecutiveSummaries.Add(exData);
                               
                            }
                            planLists.Add(bscPlanLists);


                        }
                    }
                   

                }
                
            }

            return planLists;
        }
       
    }       
        
       
    
}
