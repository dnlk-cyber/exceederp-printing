using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Planning.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class QuarterlyPlanODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuarterlyPlanVM> GetQuarterlyPlan(object[] planyear, string workUnitId)
        {
            var result = new List<QuarterlyPlanVM>();
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

            var plans = db.Plans.AsQueryable();
            if (selectedFiscalYearIds.Any())
            {
                plans = plans.Where(x => selectedFiscalYearIds.Contains(x.GlFiscalYearId));
            }

            var planIds = plans.Select(x => x.PlanId).ToList();
            var fiscalYearIds = plans.Select(x => x.GlFiscalYearId).Distinct().ToList();
            if (!planIds.Any())
            {
                return result;
            }

            var bscPlans = db.BSCPlans
                .Where(x => planIds.Contains(x.PlanId))
                .ToList();

            if (int.TryParse(workUnitId, out var selectedWorkUnitId))
            {
                var workUnitIds = new HashSet<int> { selectedWorkUnitId };
                var isHead = db.Placements.Any(x =>
                    x.isCurrent && x.IsHead && x.OrgStructureId == selectedWorkUnitId);

                if (isHead)
                {
                    var childWorkUnitIds = db.MainCompanyStructures
                        .Where(x => x.Parent == selectedWorkUnitId)
                        .Select(x => x.OrgStructureId)
                        .ToList();

                    foreach (var childWorkUnitId in childWorkUnitIds)
                    {
                        workUnitIds.Add(childWorkUnitId);
                    }
                }

                bscPlans = bscPlans
                    .Where(x => workUnitIds.Contains(x.OrgStructureId))
                    .ToList();
            }

            var bscPlanIds = bscPlans.Select(x => x.BSCPlanId).ToList();
            var planningDetails = db.PlanningDetails
                .Where(x => bscPlanIds.Contains(x.BSCPlanId))
                .ToList();
            var planningDetailIds = planningDetails
                .Select(x => x.PlanningDetailId)
                .ToList();
            var monthlyPlans = db.MonthlyPlans
                .Where(x => planningDetailIds.Contains(x.PlanningDetailId))
                .ToList();

            var objectiveLookup = db.Objectives.ToDictionary(x => x.ObjectiveId);
            var indicatorLookup = db.Indicators.ToDictionary(x => x.IndicatorId);
            var workUnitLookup = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var periodIndexLookup = db.GLPeriods
                .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                .ToDictionary(x => x.GLPeriodId, x => x.Index);

            var serialNumber = 1;
            foreach (var bscPlan in bscPlans.OrderBy(x => x.OrgStructureId).ThenBy(x => x.ObjectiveId))
            {
                workUnitLookup.TryGetValue(bscPlan.OrgStructureId, out var workUnit);
                objectiveLookup.TryGetValue(bscPlan.ObjectiveId, out var objective);

                var topLevelDetails = planningDetails
                    .Where(x => x.BSCPlanId == bscPlan.BSCPlanId && x.ParentId == 0)
                    .ToList();

                foreach (var topLevelDetail in topLevelDetails)
                {
                    indicatorLookup.TryGetValue(topLevelDetail.IndicatorId, out var indicator);

                    var detailRows = planningDetails.Where(x =>
                        x.BSCPlanId == bscPlan.BSCPlanId &&
                        (topLevelDetail.HasSubIndicator
                            ? x.ParentId == topLevelDetail.IndicatorId
                            : x.IndicatorId == topLevelDetail.IndicatorId))
                        .ToList();

                    foreach (var detailRow in detailRows)
                    {
                        indicatorLookup.TryGetValue(detailRow.IndicatorId, out var subIndicator);

                        var model = new QuarterlyPlanVM
                        {
                            SerialNumber = serialNumber++,
                            WorkUnit = workUnit?.Name,
                            Objective = objective?.Name,
                            Indicator = indicator?.Name,
                            SubIndicator = topLevelDetail.HasSubIndicator ? subIndicator?.Name : null,
                            Description = indicator?.Description,
                            Unit = detailRow.Unit,
                            KPIType = detailRow.KPIType.ToString(),
                            Baseline = detailRow.Baseline,
                            AnnualPlan = detailRow.Plan,
                            Target = detailRow.Target
                        };

                        var detailMonthlyPlans = monthlyPlans
                            .Where(x => x.PlanningDetailId == detailRow.PlanningDetailId)
                            .ToList();

                        foreach (var monthlyPlan in detailMonthlyPlans)
                        {
                            if (!periodIndexLookup.TryGetValue(monthlyPlan.GLPeriodId, out var periodIndex))
                            {
                                continue;
                            }

                            if (periodIndex >= 1 && periodIndex <= 3)
                            {
                                model.QuarterOne += monthlyPlan.Value;
                            }
                            else if (periodIndex <= 6)
                            {
                                model.QuarterTwo += monthlyPlan.Value;
                            }
                            else if (periodIndex <= 9)
                            {
                                model.QuarterThree += monthlyPlan.Value;
                            }
                            else if (periodIndex <= 12)
                            {
                                model.QuarterFour += monthlyPlan.Value;
                            }
                        }

                        result.Add(model);
                    }
                }
            }

            return result;
        }
    }
}
