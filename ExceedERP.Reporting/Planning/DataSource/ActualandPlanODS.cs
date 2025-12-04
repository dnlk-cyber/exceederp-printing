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
    public class ActualAndPlanODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ActualAndPlanningVM GetBothData(string year,string workUnitId)
        {

            ActualAndPlanningVM model = new ActualAndPlanningVM();
            if (year != null)
            {

                int.TryParse(year, out int glFiscalYearId);
                var glfiscalyear = db.GLFiscalYears.Find(glFiscalYearId);
                if (glfiscalyear != null)
                {
                    var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

                    if (Organization != null)
                    {
                        var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                                           a.ReferenceType == AttachmentType.LOGO);
                        if (logo != null)
                        {
                            model.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                        }

                        model.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                        model.Name = Organization.TradeName;
                        model.AmharicName = Organization.AmharicName;
                        model.Abbreviation = Organization.Abbreviation;
                    }
                    var plans = db.Actuals.FirstOrDefault(s => s.GlFiscalYearId == glFiscalYearId);
                    
                    if(plans != null)
                    {
                        model.Description = plans.Description;
                        model.FiscalYear = glfiscalyear.Name;
                        model.ObjectiveItems = new List<BothObjectiveItem>();
                        var bscPlans = db.BSCActuals.Where(s => s.ActualId == plans.ActualId).ToList();
                        if (!string.IsNullOrEmpty(workUnitId))
                        {
                            int.TryParse(workUnitId, out int orgStructureId);
                            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId).FirstOrDefault();
                            if (placement != null)
                            {
                                var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId).Select(x => x.OrgStructureId);
                                if (orgstructureIds.Count() > 0)
                                {
                                    bscPlans = bscPlans.Where(x=>orgstructureIds.Contains(x.OrgStructureId) || x.OrgStructureId == orgStructureId).Distinct().ToList();

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
                                indicatorItem.ActualPlanItems = new List<ActualPlanItems>();

                                var plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == bscPlan.BSCActualId&&x.ParentId ==indicator.IndicatorId).ToList();
                                if (indicator.HasSubIndicator)
                                {
                                    plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == bscPlan.BSCActualId && x.ParentId == indicator.IndicatorId&&x.ParentId !=0).ToList();
                                }
                                else
                                {
                                    plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId && x.IndicatorId == indicator.IndicatorId).ToList();

                                }

                                foreach (var planning in plansOfCurrentIndicator)
                                {

                                   var planItem = new ActualPlanItems();
                                    var subIndicators = db.Indicators.Find(planning.IndicatorId);
                                    if (subIndicators != null)
                                    {
                                        planItem.SubIndicators = subIndicators.Name;
                                        planItem.KPIType = planning.KPIType.ToString();
                                        planItem.Description = subIndicators.Description;


                                    }

                                    planItem.AnnulPlan = planning.Plan;
                                    
                                    var periods = db.MonthlyActuals.Where(x => x.ActualDetailId == planning.ActualDetailId).ToList();
                                    decimal yearlyActual = 0;
                                    decimal yearlyPlan = 0;
                                    decimal toDateActual = 0;
                                    decimal toDatePlan = 0;
                                    foreach (var period in periods)
                                    {

                                        //decimal[] Month = new decimal[12];
                                        yearlyActual += period.ActualValue;
                                        yearlyPlan += period.PlanValue;
                                        if (period.ActualValue > 0)
                                        {
                                            toDateActual += period.ActualValue;
                                            toDatePlan += period.PlanValue;
                                        }
                                        var periodIndex = db.GLPeriods.FirstOrDefault(x=>x.GlFiscalYearId==glFiscalYearId&&x.GLPeriodId==period.GLPeriodId);
                                        if (periodIndex != null)
                                        {
                                            switch (periodIndex.Index)
                                            {
                                                case 1:
                                                    planItem.July = period.ActualValue;
                                                    planItem.JulyPlan = period.PlanValue;
                                                    planItem.JulyPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;

                                                    break;
                                                case 2:

                                                    planItem.Augest = period.ActualValue;
                                                    planItem.AugestPlan = period.PlanValue;
                                                    planItem.AugestPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 3:

                                                    planItem.September = period.ActualValue;
                                                    planItem.SeptemberPlan = period.PlanValue;
                                                    planItem.SeptemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 4:

                                                    planItem.October = period.ActualValue;
                                                    planItem.OctoberPlan = period.PlanValue;
                                                    planItem.OctoberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 5:

                                                    planItem.November = period.ActualValue;
                                                    planItem.NovemberPlan = period.PlanValue;
                                                    planItem.NovemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 6:

                                                    planItem.December = period.ActualValue;
                                                    planItem.DecemberPlan = period.PlanValue;
                                                    planItem.DecemberPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 7:

                                                    planItem.January = period.ActualValue;
                                                    planItem.JanuaryPlan = period.PlanValue;
                                                    planItem.JanuaryPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 8:

                                                    planItem.February = period.ActualValue;
                                                    planItem.FebruaryPlan = period.PlanValue;
                                                    planItem.FebruaryPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 9:

                                                    planItem.March = period.ActualValue;
                                                    planItem.MarchPlan = period.PlanValue;
                                                    planItem.MarchPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 10:

                                                    planItem.April = period.ActualValue;
                                                    planItem.AprilPlan = period.PlanValue;
                                                    planItem.AprilPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 11:

                                                    planItem.May = period.ActualValue;
                                                    planItem.MayPlan = period.PlanValue;
                                                    planItem.MayPercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;
                                                case 12:

                                                    planItem.June = period.ActualValue;
                                                    planItem.JunePlan = period.PlanValue;
                                                    planItem.JunePercent = (period.PlanValue > 0) ? (100 * period.ActualValue) / period.PlanValue : 0;
                                                    break;


                                            }

                                        }


                                    }
                                    planItem.ToDatePlan = toDatePlan;
                                    planItem.ToDateActual = toDateActual;
                                    planItem.YearlyPlan = yearlyPlan-toDatePlan;
                                    planItem.YearlyActual = yearlyActual-toDateActual;
                                    planItem.ToDate = (toDateActual > 0 && toDatePlan > 0) ? (toDateActual) / toDatePlan : 0;
                                    planItem.Yearly = (yearlyPlan - toDatePlan > 0) ? (yearlyActual - toDateActual) / (yearlyPlan - toDatePlan) : 0;

                                    indicatorItem.ActualPlanItems.Add(planItem);

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
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }
    }       
        
       
    
}
