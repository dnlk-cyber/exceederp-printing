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
using Telerik.Reporting;
using static ExceedERP.Reporting.Planning.ViewModel.ActualVM;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ActualSelfCareODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ActualVM GetActualData(string year,string workUnitId)
        {

            ActualVM model = new ActualVM();
            if (year != null)
            {


                int.TryParse(year, out int glFiscalYearId);
                var glfiscalyears = db.GLFiscalYears.Find(glFiscalYearId);
                if (glfiscalyears != null)
                {
                    var actuals = db.Actuals/*.Where(x => x.IsPersonal)*/.FirstOrDefault(s => s.GlFiscalYearId == glfiscalyears.GlFiscalYearId);
                    
                    if (actuals != null )
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
                        model.FiscalYear = glfiscalyears.Name;
                        model.Description = actuals.Description;
                        model.ObjectiveItems = new List<ActualObjectiveItem>();
                        var bscActuals = db.BSCActuals.Where(s => s.ActualId == actuals.ActualId).ToList();
                        if (workUnitId != null)
                        {
                            int.TryParse(workUnitId, out int orgStructureId);
                            bscActuals = bscActuals.Where(x => x.OrgStructureId == orgStructureId).ToList();
                        }
                        foreach (var bscActual in bscActuals)
                        {
                            ActualObjectiveItem objectiveItem = new ActualObjectiveItem();
                            var workUnit = db.MainCompanyStructures.Find(bscActual.OrgStructureId);
                            if (workUnit != null)
                            {
                                objectiveItem.WorkUnit = workUnit.Name;

                            }
                            var objectives = db.Objectives.Find(bscActual.ObjectiveId);
                            if (objectives != null)
                            {
                                objectiveItem.Objective = objectives.Name;
                                objectiveItem.Description = objectives.Description;

                            }
                            objectiveItem.IndicatorItems = new List<IndicatorActualItems>();
                            var actualItems = db.ActualDetails.Where(q => q.BSCActualId == bscActual.BSCActualId&&q.ParentId==0).ToList();
                            foreach (var indicator in actualItems)
                            {
                                var indicatorItem = new IndicatorActualItems();

                                var indicators = db.Indicators.Find(indicator.IndicatorId);
                                if (indicators != null)
                                {
                                    indicatorItem.Indicators = indicators.Name;
                                }
                                indicatorItem.ActualItems = new List<ActualItem>();

                                var plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId ==indicator.BSCActualId&&x.ParentId ==indicator.IndicatorId).ToList();
                                if (indicator.HasSubIndicator)
                                {
                                     plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId && x.ParentId == indicator.IndicatorId).ToList();

                                }
                                else
                                {
                                    plansOfCurrentIndicator = db.ActualDetails.Where(x => x.BSCActualId == indicator.BSCActualId && x.IndicatorId == indicator.IndicatorId).ToList();

                                }
                                foreach (var actual in plansOfCurrentIndicator)
                                {


                                    var actualItem = new ActualItem();
                                    var subIndicators = db.Indicators.Find(actual.IndicatorId);
                                    if (subIndicators != null)
                                    {
                                        actualItem.SubIndicators = subIndicators.Name;

                                    }
                                    actualItem.AnnualPlan = actual.Plan;
                                   

                                    if (indicators != null)
                                    {

                                            actualItem.Indicators = indicators.Name;
                                            actualItem.Unit = indicators.Unit;
                                            actualItem.KPIType = indicators.KPIType.ToString();

                                    }
                                        
                                        actualItem.Description = indicators.Description;
                                        var periods = db.MonthlyActuals.Where(x => x.ActualDetailId == actual.ActualDetailId).ToList();
                                        foreach (var period in periods)
                                        {
                                            decimal[] Month = new decimal[12];
                                            var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == period.MonthlyActualId).ToList();
                                            var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
                                            if (periodIndex != null)
                                            {
                                                switch (periodIndex.Index)
                                                {
                                                    case 1:
                                                        actualItem.July = period.ActualValue;
                                                        actualItem.JulyWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.JulyWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.JulyWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.JulyWeek4 = weeklyActuals.Sum(x => x.WeekFour);

                                                        break;
                                                    case 2:
                                                        
                                                        actualItem.Augest = period.ActualValue;
                                                        actualItem.AugestWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.AugestWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.AugestWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.AugestWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 3:
                                                       
                                                        actualItem.September = period.ActualValue;
                                                        actualItem.SeptemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.SeptemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.SeptemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                       actualItem.SeptemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 4:

                                                        actualItem.October = period.ActualValue;
                                                        actualItem.OctoberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.OctoberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.OctoberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.OctoberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 5:

                                                        actualItem.November = period.ActualValue;
                                                        actualItem.NovemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.NovemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.NovemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.NovemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 6:

                                                        actualItem.December = period.ActualValue;
                                                        actualItem.DecemberWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.DecemberWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.DecemberWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.DecemberWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 7:

                                                        actualItem.January = period.ActualValue;
                                                        actualItem.JanuaryWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.JanuaryWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.JanuaryWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.JanuaryWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 8:

                                                        actualItem.February = period.ActualValue;
                                                        actualItem.FebruaryWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.FebruaryWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.FebruaryWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.FebruaryWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 9:

                                                        actualItem.March = period.ActualValue;
                                                        actualItem.MarchWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.MarchWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.MarchWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.MarchWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 10:

                                                        actualItem.April = period.ActualValue;
                                                        actualItem.AprilWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.AprilWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.AprilWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.AprilWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 11:

                                                        actualItem.May = period.ActualValue;
                                                        actualItem.MayWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.MayWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.MayWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.MayWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;
                                                    case 12:

                                                        actualItem.June = period.ActualValue;
                                                        actualItem.JuneWeek1 = weeklyActuals.Sum(x => x.WeekOne);
                                                        actualItem.JuneWeek2 = weeklyActuals.Sum(x => x.WeekTwo);
                                                        actualItem.JuneWeek3 = weeklyActuals.Sum(x => x.WeekThree);
                                                        actualItem.JuneWeek4 = weeklyActuals.Sum(x => x.WeekFour);
                                                        break;

                                                }

                                            }

                                        }
                                    indicatorItem.ActualItems.Add(actualItem);


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
