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

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class PlanSelfCareODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PlanningVM GetPlannedData(string planyear,string workUnitId)
        {

            PlanningVM model = new PlanningVM();
            if(planyear != null)
            {


                int.TryParse(planyear, out int glFiscalYearId);
                
                var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

                var plans = db.Plans/*.Where(x=>x.IsPersonal)*/.FirstOrDefault(s=>s.GlFiscalYearId== glFiscalYearId);
                if (plans != null )
                {
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
                    var glfiscalYears = db.GLFiscalYears.Find(glFiscalYearId);
                    if(glfiscalYears != null)
                    {
                        model.FiscalYear = glfiscalYears.Name;
                    }
                   
                    model.Description = plans.Description;
                    model.ObjectiveItems = new List<ObjectiveItem>();
                    var bscPlans = db.BSCPlans.Where(s=>s.PlanId==plans.PlanId).ToList();
                    if(workUnitId != null)
                    {
                        int.TryParse(workUnitId, out int orgStructureId);
                        bscPlans = bscPlans.Where(x => x.OrgStructureId == orgStructureId).ToList();
                    }
                    foreach (var bscPlan in bscPlans)
                    {
                        ObjectiveItem objectiveItem = new ObjectiveItem();
                        var workUnit = db.MainCompanyStructures.Find(bscPlan.OrgStructureId);
                        if(workUnit != null)
                        {
                            objectiveItem.WorkUnit = workUnit.Name;

                        }
                        var objectives = db.Objectives.Find(bscPlan.ObjectiveId);
                        if (objectives != null)
                        {
                            objectiveItem.Objective = objectives.Name;
                            objectiveItem.Description = objectives.Description;

                        }
                        objectiveItem.IndicatorItems = new List<IndicatorItems>();
                        var plannings = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlan.BSCPlanId&& x.ParentId==0 ).ToList();
                        foreach(var indicator in plannings)
                        {
                            var indicatorItem = new IndicatorItems();

                            var indicators = db.Indicators.Find(indicator.IndicatorId);
                            if (indicators != null)
                            {
                                indicatorItem.Indicators = indicators.Name;
                            }
                            indicatorItem.PlanItems = new List<PlanItem>();

                            // plannings = plannings.Where(x => indicatorIds.Contains(x.IndicatorId)).ToList();
                            var plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId==indicator.BSCPlanId&&x.ParentId ==indicator.IndicatorId).ToList();
                            if (indicator.HasSubIndicator)
                            {
                                plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId == indicator.BSCPlanId && x.ParentId == indicator.IndicatorId).ToList();

                            }
                            else
                            {
                                plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId == indicator.BSCPlanId && x.IndicatorId == indicator.IndicatorId).ToList();

                            }
                            foreach (var planning in plansOfCurrentIndicator)
                            {

                                var planItem = new PlanItem();
                                var subIndicators = db.Indicators.Find(planning.IndicatorId);
                                if (subIndicators != null)
                                {
                                    planItem.SubIndicators = subIndicators.Name;

                                }
                                if (indicators != null)
                                {
                                    planItem.Unit = indicators.Unit;
                                    planItem.KPIType = indicators.KPIType.ToString();
                                    planItem.Description = indicators.Description;
                                }

                                planItem.Baseline = planning.Baseline;
                                planItem.AnualPlan = planning.Plan;
                                planItem.Target = planning.Target;
                               
                                var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == planning.PlanningDetailId).ToList();
                                    foreach (var period in periods)
                                    {
                                        var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == period.MonthlyPlanId).ToList();
                                        var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
                                        if (periodIndex != null)
                                        {


                                            switch (periodIndex.Index)
                                            {
                                                case 1:
                                                    planItem.July = period.Value;
                                                    planItem.JulyWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.JulyWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.JulyWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.JulyWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 2:
                                                    planItem.Augest = period.Value;
                                                    planItem.AugestWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.AugestWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.AugestWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.AugestWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 3:
                                                    planItem.September = period.Value;
                                                    planItem.SeptemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.SeptemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.SeptemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.SeptemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 4:
                                                    planItem.October = period.Value;
                                                    planItem.OctoberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.OctoberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.OctoberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.OctoberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 5:
                                                    planItem.November = period.Value;
                                                    planItem.NovemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.NovemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.NovemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.NovemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 6:
                                                    planItem.December = period.Value;
                                                    planItem.DecemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.DecemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.DecemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.DecemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 7:
                                                    planItem.January = period.Value;
                                                    planItem.JanuaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.JanuaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.JanuaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.JanuaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 8:
                                                    planItem.February = period.Value;
                                                    planItem.FebruaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.FebruaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.FebruaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.FebruaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 9:
                                                    planItem.March = period.Value;
                                                    planItem.MarchWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.MarchWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.MarchWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.MarchWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 10:
                                                    planItem.April = period.Value;
                                                    planItem.AprilWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.AprilWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.AprilWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.AprilWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 11:
                                                    planItem.May = period.Value;
                                                    planItem.MayWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.MayWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.MayWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.MayWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;
                                                case 12:
                                                    planItem.June = period.Value;
                                                    planItem.JuneWeek1 = weeklyPlans.Sum(x => x.WeekOne);
                                                    planItem.JuneWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
                                                    planItem.JuneWeek3 = weeklyPlans.Sum(x => x.WeekThree);
                                                    planItem.JuneWeek4 = weeklyPlans.Sum(x => x.WeekFour);
                                                    break;

                                            }
                                        }
                                   
                                    }
                                    indicatorItem.PlanItems.Add(planItem);




                            }
                            objectiveItem.IndicatorItems.Add(indicatorItem);


                        }


                        model.ObjectiveItems.Add(objectiveItem);
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
