using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Planning;
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
    public class PlanODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanningVM> GetPlannedData(object[] planyear, string workUnitId, string periodd, bool includeInactive)
        {
            var selectedFiscalYearIds = (planyear ?? new object[0])
                .Select(value =>
                {
                    int fiscalYearId;
                    return int.TryParse(Convert.ToString(value), out fiscalYearId)
                        ? fiscalYearId
                        : 0;
                })
                .Where(fiscalYearId => fiscalYearId > 0)
                .Distinct()
                .ToList();

            var fiscalYears = db.GLFiscalYears.AsQueryable();
            if (selectedFiscalYearIds.Any())
            {
                fiscalYears = fiscalYears.Where(x => selectedFiscalYearIds.Contains(x.GlFiscalYearId));
            }
            else
            {
                var fiscalYearIdsWithPlans = db.Plans.Select(x => x.GlFiscalYearId).Distinct();
                fiscalYears = fiscalYears.Where(x => fiscalYearIdsWithPlans.Contains(x.GlFiscalYearId));
            }

            var fiscalYearIds = fiscalYears
                .OrderBy(x => x.DateFrom)
                .Select(x => x.GlFiscalYearId)
                .ToList();

            return fiscalYearIds
                .Select(fiscalYearId => GetPlannedData(fiscalYearId, workUnitId, periodd, includeInactive))
                .Where(model => model != null)
                .ToList();
        }

        private PlanningVM GetPlannedData(int fiscalYearId, string workUnitId, string periodd, bool includeInactive)
        {
            var model = new PlanningVM();

            // Preload all required data with fewer DB calls
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            var plan = db.Plans.FirstOrDefault(s => s.GlFiscalYearId == fiscalYearId);
            var fiscalYear = db.GLFiscalYears.Find(fiscalYearId);

            if (plan == null)
                return null;

            // Load logo
            if (organization != null)
            {
                var logo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.LOGO);

                model.LogoFileName = logo != null ? ByteArrayToImage(logo.BinaryFile) : null;

                model.Date = XAPI.EthiopicDateTime.GetEthiopicDate(
                    DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                model.Name = organization.TradeName;
                model.AmharicName = organization.AmharicName;
                model.Abbreviation = organization.Abbreviation;
            }

            model.FiscalYear = fiscalYear?.Name;
            model.Description = plan.Description;
            model.ObjectiveItems = new List<ObjectiveItem>();

            // Load all BSC plans
            var bscPlans = db.BSCPlans
                .Where(s => s.PlanId == plan.PlanId)
                .ToList();

            // Filter by workUnitId if needed
            if (int.TryParse(workUnitId, out int orgStructureId))
            {
                var placement = db.Placements.FirstOrDefault(x =>
                    x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId);

                if (placement != null)
                {
                    var childOrgs = db.MainCompanyStructures
                        .Where(x => x.Parent == orgStructureId)
                        .Select(x => x.OrgStructureId)
                        .ToList();

                    if (childOrgs.Any())
                    {
                        bscPlans = bscPlans
                            .Where(x => childOrgs.Contains(x.OrgStructureId) ||
                                        x.OrgStructureId == orgStructureId)
                            .ToList();
                    }
                    else
                    {
                        bscPlans = bscPlans
                            .Where(x => x.OrgStructureId == orgStructureId)
                            .ToList();
                    }
                }
            }

            // Regular reports print active indicators only; SelfCare can explicitly include all.
            var indicatorQuery = db.Indicators.AsQueryable();
            if (!includeInactive)
            {
                indicatorQuery = indicatorQuery.Where(x => x.IsActive);
            }

            var indicatorLookup = indicatorQuery.ToDictionary(x => x.IndicatorId);
            var objectiveLookup = db.Objectives.ToDictionary(x => x.ObjectiveId);
            var structureLookup = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            // Preload all Planning Details
            var bscPlanIds = bscPlans.Select(b => b.BSCPlanId).ToList();

            var planningDetailsQuery = db.PlanningDetails
                .Where(x => bscPlanIds.Contains(x.BSCPlanId));
            if (!includeInactive)
            {
                planningDetailsQuery = planningDetailsQuery.Where(x =>
                    db.Indicators.Any(indicator =>
                        indicator.IndicatorId == x.IndicatorId && indicator.IsActive));
            }

            var allPlanningDetails = planningDetailsQuery.ToList();

            // Preload Monthly Plans
            var planningDetailIds = allPlanningDetails
                .Select(p => p.PlanningDetailId)
                .ToList();

            var allMonthlyPlans = db.MonthlyPlans
                .Where(x => planningDetailIds.Contains(x.PlanningDetailId))
                .ToList();

            // Preload Weekly Plans
            var monthlyPlanIds = allMonthlyPlans
                .Select(m => m.MonthlyPlanId)
                .ToList();

            var allWeeklyPlans = db.WeeklyPlans
                .Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId))
                .ToList();


            // Preload GL Periods
            var periodLookup = db.GLPeriods.ToDictionary(x => x.GLPeriodId);

            // Optional period filter
            int? filterPeriodIndex = null;
            int perId;
            GLPeriod selectedPeriod;
            if (int.TryParse(periodd, out perId) && periodLookup.TryGetValue(perId, out selectedPeriod))
                filterPeriodIndex = selectedPeriod.Index;

            // ========== MAIN LOOP ==========

            foreach (var bsc in bscPlans)
            {
                var objItem = new ObjectiveItem();
                structureLookup.TryGetValue(bsc.OrgStructureId, out var workUnit);
                if (workUnit != null)
                    objItem.WorkUnit = workUnit.Name;

                if (objectiveLookup.TryGetValue(bsc.ObjectiveId, out var objective))
                {
                    objItem.Objective = objective.Name;
                    objItem.Description = objective.Description;
                }

                objItem.IndicatorItems = new List<IndicatorItems>();
                var childrenByParent = allPlanningDetails
                    .Where(x => x.BSCPlanId == bsc.BSCPlanId)
                    .ToLookup(x => x.ParentId);

                // Get top-level indicators
                var topIndicators = allPlanningDetails
                    .Where(x => x.BSCPlanId == bsc.BSCPlanId && x.ParentId == 0)
                    .ToList();

                foreach (var ind in topIndicators)
                {
                    var indicatorItem = new IndicatorItems();
                    if (indicatorLookup.TryGetValue(ind.IndicatorId, out var indInfo))
                    {
                        indicatorItem.Indicators = indInfo.Name;
                        indicatorItem.Weight = indInfo.Weight;
                    }

                    indicatorItem.PlanItems = new List<PlanItem>();

                    foreach (var row in GetPlanRows(ind, childrenByParent))
                    {
                        var p = row.Item1;
                        var planItem = new PlanItem { IndicatorLevel = row.Item2 };

                        // Sub indicator name
                        if (indicatorLookup.TryGetValue(p.IndicatorId, out var subInd))
                        {
                            planItem.SubIndicators = subInd.Name;
                            planItem.Weight = subInd.Weight;
                        }

                        planItem.Baseline = p.Baseline;
                        planItem.AnualPlan = p.Plan;
                        planItem.Target = p.Target;
                        planItem.Unit = p.Unit;
                        planItem.KPIType = p.KPIType.ToString();
                        planItem.Description = indInfo?.Description;

                        // Monthly plans
                        var months = allMonthlyPlans
                            .Where(x => x.PlanningDetailId == p.PlanningDetailId &&
                                        (!filterPeriodIndex.HasValue ||
                                         (periodLookup.ContainsKey(x.GLPeriodId) &&
                                          periodLookup[x.GLPeriodId].Index == filterPeriodIndex.Value)))
                            .ToList();

                        foreach (var m in months)
                        {
                            if (!periodLookup.TryGetValue(m.GLPeriodId, out var periodIndex))
                                continue;

                            var weekly = allWeeklyPlans
                                .Where(x => x.MonthlyPlanId == m.MonthlyPlanId)
                                .ToList();

                            decimal sum1 = weekly.Any()?weekly.Sum(x => x.WeekOne):0m;
                            decimal sum2 = weekly.Any() ? weekly.Sum(x => x.WeekTwo):0m;
                            decimal sum3 = weekly.Any() ? weekly.Sum(x => x.WeekThree):0m;
                            decimal sum4 = weekly.Any() ? weekly.Sum(x => x.WeekFour):0m;

                            // Fill month into planItem dynamically
                            SetMonthValue(planItem, periodIndex.Index, m.Value,
                                          sum1, sum2, sum3, sum4);
                        }

                        // Percentage KPIs: show the average of monthly quantities at the end of the row
                        if (planItem.KPIType == "Percentage" || (planItem.Unit != null && planItem.Unit.Contains("%")))
                        {
                            var monthlyValues = allMonthlyPlans
                                .Where(x => x.PlanningDetailId == p.PlanningDetailId)
                                .ToList();
                            if (monthlyValues.Any())
                            {
                                planItem.MonthlyAverage = monthlyValues.Average(x => x.Value);
                            }
                        }

                        indicatorItem.PlanItems.Add(planItem);
                    }

                    if (includeInactive || indicatorItem.PlanItems.Any())
                    {
                        objItem.IndicatorItems.Add(indicatorItem);
                    }
                }

                if (includeInactive || objItem.IndicatorItems.Any())
                {
                    model.ObjectiveItems.Add(objItem);
                }
            }

            var percentageAverages = model.ObjectiveItems
                .SelectMany(o => o.IndicatorItems)
                .SelectMany(i => i.PlanItems)
                .Where(x => x.MonthlyAverage.HasValue)
                .Select(x => x.MonthlyAverage.Value)
                .ToList();
            model.PercentageAverage = percentageAverages.Any() ? percentageAverages.Average() : 0m;

            return model;
        }

        private static IEnumerable<Tuple<PlanningDetail, int>> GetPlanRows(
            PlanningDetail root, ILookup<int, PlanningDetail> childrenByParent)
        {
            var children = childrenByParent[root.IndicatorId].ToList();
            if (!children.Any())
            {
                if (!root.HasSubIndicator)
                    yield return Tuple.Create(root, 0);
                yield break;
            }

            var visited = new HashSet<int> { root.PlanningDetailId };
            var pending = new Stack<Tuple<PlanningDetail, int>>(
                children.AsEnumerable().Reverse().Select(child => Tuple.Create(child, 1)));
            while (pending.Count > 0)
            {
                var row = pending.Pop();
                if (!visited.Add(row.Item1.PlanningDetailId))
                    continue;

                yield return row;
                foreach (var child in childrenByParent[row.Item1.IndicatorId].Reverse())
                    pending.Push(Tuple.Create(child, row.Item2 + 1));
            }
        }

        private void SetMonthValue(PlanItem planItem, int index, decimal value,
                            decimal w1, decimal w2, decimal w3, decimal w4)
        {
            switch (index)
            {
                case 1: planItem.July = value; planItem.JulyWeek1 = w1; planItem.JulyWeek2 = w2; planItem.JulyWeek3 = w3; planItem.JulyWeek4 = w4; break;
                case 2: planItem.Augest = value; planItem.AugestWeek1 = w1; planItem.AugestWeek2 = w2; planItem.AugestWeek3 = w3; planItem.AugestWeek4 = w4; break;
                case 3: planItem.September = value; planItem.SeptemberWeek1 = w1; planItem.SeptemberWeek2 = w2; planItem.SeptemberWeek3 = w3; planItem.SeptemberWeek4 = w4; break;
                case 4: planItem.October = value; planItem.OctoberWeek1 = w1; planItem.OctoberWeek2 = w2; planItem.OctoberWeek3 = w3; planItem.OctoberWeek4 = w4; break;
                case 5: planItem.November = value; planItem.NovemberWeek1 = w1; planItem.NovemberWeek2 = w2; planItem.NovemberWeek3 = w3; planItem.NovemberWeek4 = w4; break;
                case 6: planItem.December = value; planItem.DecemberWeek1 = w1; planItem.DecemberWeek2 = w2; planItem.DecemberWeek3 = w3; planItem.DecemberWeek4 = w4; break;
                case 7: planItem.January = value; planItem.JanuaryWeek1 = w1; planItem.JanuaryWeek2 = w2; planItem.JanuaryWeek3 = w3; planItem.JanuaryWeek4 = w4; break;
                case 8: planItem.February = value; planItem.FebruaryWeek1 = w1; planItem.FebruaryWeek2 = w2; planItem.FebruaryWeek3 = w3; planItem.FebruaryWeek4 = w4; break;
                case 9: planItem.March = value; planItem.MarchWeek1 = w1; planItem.MarchWeek2 = w2; planItem.MarchWeek3 = w3; planItem.MarchWeek4 = w4; break;
                case 10: planItem.April = value; planItem.AprilWeek1 = w1; planItem.AprilWeek2 = w2; planItem.AprilWeek3 = w3; planItem.AprilWeek4 = w4; break;
                case 11: planItem.May = value; planItem.MayWeek1 = w1; planItem.MayWeek2 = w2; planItem.MayWeek3 = w3; planItem.MayWeek4 = w4; break;
                case 12: planItem.June = value; planItem.JuneWeek1 = w1; planItem.JuneWeek2 = w2; planItem.JuneWeek3 = w3; planItem.JuneWeek4 = w4; break;
            }
        }
        private static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }

        //public PlanningVM GetPlannedData(string planyear,string workUnitId,string periodd)
        //{

        //    PlanningVM model = new PlanningVM();
        //    if(!string.IsNullOrEmpty(planyear))
        //    {


        //        int.TryParse(planyear, out int glFiscalYearId);

        //        var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

        //        var plans = db.Plans.FirstOrDefault(s=>s.GlFiscalYearId== glFiscalYearId);
        //        if (plans != null )
        //        {
        //            if (Organization != null)
        //            {
        //                var logo =
        //                db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
        //                                                   a.ReferenceType == AttachmentType.LOGO);
        //                if (logo != null)
        //                {
        //                    model.LogoFileName = ByteArrayToImage(logo.BinaryFile);
        //                }

        //                model.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        //                model.Name = Organization.TradeName;
        //                model.AmharicName = Organization.AmharicName;
        //                model.Abbreviation = Organization.Abbreviation;
        //            }
        //            var glfiscalYears = db.GLFiscalYears.Find(glFiscalYearId);
        //            if(glfiscalYears != null)
        //            {
        //                model.FiscalYear = glfiscalYears.Name;
        //            }

        //            model.Description = plans.Description;
        //            model.ObjectiveItems = new List<ObjectiveItem>();
        //            var bscPlans = db.BSCPlans.Where(s=>s.PlanId==plans.PlanId).ToList();
        //            if(workUnitId != null)
        //            {
        //                int.TryParse(workUnitId, out int orgStructureId);
        //                var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId).FirstOrDefault();
        //                if (placement != null)
        //                {
        //                    var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId).Select(x => x.OrgStructureId);
        //                    if (orgstructureIds.Count() > 0)
        //                    {
        //                        bscPlans = db.BSCPlans.Where(x => x.PlanId == plans.PlanId && orgstructureIds.Contains(x.OrgStructureId) || x.OrgStructureId == orgStructureId).Distinct().ToList();

        //                    }
        //                    else
        //                    {
        //                        bscPlans = bscPlans.Where(x => x.OrgStructureId == orgStructureId).ToList();
        //                    }

        //                }

        //            }
        //            foreach (var bscPlan in bscPlans)
        //            {
        //                ObjectiveItem objectiveItem = new ObjectiveItem();
        //                var workUnit = db.MainCompanyStructures.Find(bscPlan.OrgStructureId);
        //                if(workUnit != null)
        //                {
        //                    objectiveItem.WorkUnit = workUnit.Name;

        //                }
        //                var objectives = db.Objectives.Find(bscPlan.ObjectiveId);
        //                if (objectives != null)
        //                {
        //                    objectiveItem.Objective = objectives.Name;
        //                    objectiveItem.Description = objectives.Description;

        //                }
        //                objectiveItem.IndicatorItems = new List<IndicatorItems>();
        //                var plannings = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlan.BSCPlanId&& x.ParentId==0 ).ToList();

        //                foreach (var indicator in plannings)
        //                {
        //                    var indicatorItem = new IndicatorItems();

        //                    var indicators = db.Indicators.Find(indicator.IndicatorId);
        //                    if (indicators != null)
        //                    {
        //                        indicatorItem.Indicators = indicators.Name;
        //                    }
        //                    indicatorItem.PlanItems = new List<PlanItem>();

        //                    // plannings = plannings.Where(x => indicatorIds.Contains(x.IndicatorId)).ToList();
        //                    var plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId==indicator.BSCPlanId&&x.ParentId ==indicator.IndicatorId).ToList();
        //                    if (indicator.HasSubIndicator)
        //                    {
        //                        plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId == indicator.BSCPlanId && x.ParentId == indicator.IndicatorId).ToList();

        //                    }
        //                    else
        //                    {
        //                        plansOfCurrentIndicator = db.PlanningDetails.Where(x => x.BSCPlanId == indicator.BSCPlanId && x.IndicatorId == indicator.IndicatorId).ToList();

        //                    }
        //                    foreach (var planning in plansOfCurrentIndicator)
        //                    {

        //                        var planItem = new PlanItem();
        //                        var subIndicators = db.Indicators.Find(planning.IndicatorId);
        //                        if (subIndicators != null)
        //                        {
        //                            planItem.SubIndicators = subIndicators.Name;

        //                        }

        //                        planItem.Baseline = planning.Baseline;
        //                        planItem.AnualPlan = planning.Plan;
        //                        planItem.Target = planning.Target;
        //                        planItem.Unit = planning.Unit;
        //                        planItem.KPIType = planning.KPIType.ToString();
        //                        planItem.Description = indicators.Description;


        //                        var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == planning.PlanningDetailId).ToList();
        //                        if (!string.IsNullOrEmpty(periodd) && int.TryParse(periodd, out int periodId))
        //                            periods = periods.Where(x => x.GLPeriodId == periodId).ToList();
        //                        foreach (var period in periods)
        //                        {
        //                            var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == period.MonthlyPlanId&& x.MonthlyDetail.GLPeriodId==period.GLPeriodId).ToList();
        //                            var periodIndex = db.GLPeriods.Find(period.GLPeriodId);
        //                            if (periodIndex != null)
        //                            {


        //                                switch (periodIndex.Index)
        //                                {
        //                                    case 1:
        //                                        planItem.July = period.Value;
        //                                        planItem.JulyWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.JulyWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.JulyWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.JulyWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 2:
        //                                        planItem.Augest = period.Value;
        //                                        planItem.AugestWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.AugestWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.AugestWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.AugestWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 3:
        //                                        planItem.September = period.Value;
        //                                        planItem.SeptemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.SeptemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.SeptemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.SeptemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 4:
        //                                        planItem.October = period.Value;
        //                                        planItem.OctoberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.OctoberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.OctoberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.OctoberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 5:
        //                                        planItem.November = period.Value;
        //                                        planItem.NovemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.NovemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.NovemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.NovemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 6:
        //                                        planItem.December = period.Value;
        //                                        planItem.DecemberWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.DecemberWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.DecemberWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.DecemberWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 7:
        //                                        planItem.January = period.Value;
        //                                        planItem.JanuaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.JanuaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.JanuaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.JanuaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 8:
        //                                        planItem.February = period.Value;
        //                                        planItem.FebruaryWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.FebruaryWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.FebruaryWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.FebruaryWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 9:
        //                                        planItem.March = period.Value;
        //                                        planItem.MarchWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.MarchWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.MarchWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.MarchWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 10:
        //                                        planItem.April = period.Value;
        //                                        planItem.AprilWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.AprilWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.AprilWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.AprilWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 11:
        //                                        planItem.May = period.Value;
        //                                        planItem.MayWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.MayWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.MayWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.MayWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;
        //                                    case 12:
        //                                        planItem.June = period.Value;
        //                                        planItem.JuneWeek1 = weeklyPlans.Sum(x => x.WeekOne);
        //                                        planItem.JuneWeek2 = weeklyPlans.Sum(x => x.WeekTwo);
        //                                        planItem.JuneWeek3 = weeklyPlans.Sum(x => x.WeekThree);
        //                                        planItem.JuneWeek4 = weeklyPlans.Sum(x => x.WeekFour);
        //                                        break;

        //                                }
        //                            }

        //                        }
        //                        indicatorItem.PlanItems.Add(planItem);

        //                    }
        //                    objectiveItem.IndicatorItems.Add(indicatorItem);


        //                }


        //                model.ObjectiveItems.Add(objectiveItem);
        //            }
        //        }

        //    }
        //    return model;
        //}
        //public static Image ByteArrayToImage(byte[] byteArrayIn)
        //{
        //    if (byteArrayIn != null)
        //    {
        //        MemoryStream ms = new MemoryStream(byteArrayIn);
        //        Image returnImage = Image.FromStream(ms);
        //        return returnImage;
        //    }
        //    return null;

        //}
    }



}
