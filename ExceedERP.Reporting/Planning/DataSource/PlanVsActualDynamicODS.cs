using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Planning.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class PlanVsActualDynamicODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuarterOptionVM> GetQuarterOptions()
        {
            return new List<QuarterOptionVM>
            {
                new QuarterOptionVM { Number = 1, Label = "Quarter 1" },
                new QuarterOptionVM { Number = 2, Label = "Quarter 2" },
                new QuarterOptionVM { Number = 3, Label = "Quarter 3" },
                new QuarterOptionVM { Number = 4, Label = "Quarter 4" }
            };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanVsActualQuarterlyVM> GetPlanVsActualDynamic(string planyear, string workUnitId, object[] quarters)
        {
            var result = new List<PlanVsActualQuarterlyVM>();
            if (!int.TryParse(planyear, out var fiscalYearId))
            {
                return result;
            }

            // no quarters selected means "all quarters", matching the static Plan Vs Actual report
            var selectedQuarters = ParseQuarters(quarters);
            if (selectedQuarters.Count == 0)
            {
                selectedQuarters = new List<int> { 1, 2, 3, 4 };
            }

            var actual = db.Actuals.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId);
            if (actual == null)
            {
                return result;
            }

            var bscActuals = db.BSCActuals
                .Where(x => x.ActualId == actual.ActualId)
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

                bscActuals = bscActuals
                    .Where(x => workUnitIds.Contains(x.OrgStructureId))
                    .ToList();
            }

            var bscActualIds = bscActuals.Select(x => x.BSCActualId).ToList();
            var actualDetails = db.ActualDetails
                .Where(x => bscActualIds.Contains(x.BSCActualId))
                .ToList();
            var actualDetailIds = actualDetails.Select(x => x.ActualDetailId).ToList();
            var monthlyActuals = db.MonthlyActuals
                .Where(x => actualDetailIds.Contains(x.ActualDetailId))
                .ToList();

            var objectiveLookup = db.Objectives.ToDictionary(x => x.ObjectiveId);
            var indicatorLookup = db.Indicators.ToDictionary(x => x.IndicatorId);
            var workUnitLookup = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var periodIndexLookup = db.GLPeriods
                .Where(x => x.GlFiscalYearId == fiscalYearId)
                .ToDictionary(x => x.GLPeriodId, x => x.Index);

            var serialNumber = 1;
            foreach (var bscActual in bscActuals.OrderBy(x => x.OrgStructureId).ThenBy(x => x.ObjectiveId))
            {
                workUnitLookup.TryGetValue(bscActual.OrgStructureId, out var workUnit);
                objectiveLookup.TryGetValue(bscActual.ObjectiveId, out var objective);

                var topLevelDetails = actualDetails
                    .Where(x => x.BSCActualId == bscActual.BSCActualId && x.ParentId == 0)
                    .ToList();

                foreach (var topLevelDetail in topLevelDetails)
                {
                    indicatorLookup.TryGetValue(topLevelDetail.IndicatorId, out var indicator);

                    var detailRows = actualDetails.Where(x =>
                        x.BSCActualId == bscActual.BSCActualId &&
                        (topLevelDetail.HasSubIndicator
                            ? x.ParentId == topLevelDetail.IndicatorId
                            : x.IndicatorId == topLevelDetail.IndicatorId))
                        .ToList();

                    foreach (var detailRow in detailRows)
                    {
                        indicatorLookup.TryGetValue(detailRow.IndicatorId, out var rowIndicator);

                        var quarterPlan = new decimal[4];
                        var quarterActual = new decimal[4];
                        var detailMonthlyActuals = monthlyActuals
                            .Where(x => x.ActualDetailId == detailRow.ActualDetailId)
                            .ToList();

                        foreach (var monthlyActual in detailMonthlyActuals)
                        {
                            if (!periodIndexLookup.TryGetValue(monthlyActual.GLPeriodId, out var periodIndex) ||
                                periodIndex < 1 || periodIndex > 12)
                            {
                                continue;
                            }

                            var quarterIndex = (periodIndex - 1) / 3;
                            quarterPlan[quarterIndex] += monthlyActual.PlanValue;
                            quarterActual[quarterIndex] += monthlyActual.ActualValue;
                        }

                        // "Annual" columns represent the total of only the selected quarters
                        var selectedPlan = selectedQuarters.Sum(q => quarterPlan[q - 1]);
                        var selectedActual = selectedQuarters.Sum(q => quarterActual[q - 1]);

                        result.Add(new PlanVsActualQuarterlyVM
                        {
                            SerialNumber = serialNumber++,
                            WorkUnit = workUnit?.Name,
                            Objective = objective?.Name,
                            Indicator = indicator?.Name,
                            SubIndicator = topLevelDetail.HasSubIndicator ? rowIndicator?.Name : null,
                            Description = rowIndicator?.Description ?? indicator?.Description,
                            Unit = detailRow.Unit,
                            KPIType = detailRow.KPIType.ToString(),
                            Baseline = detailRow.Baseline,
                            Target = detailRow.Target,
                            AnnualPlan = selectedPlan,
                            AnnualActual = selectedActual,
                            AnnualPercent = GetPercent(selectedPlan, selectedActual),
                            QuarterOnePlan = quarterPlan[0],
                            QuarterOneActual = quarterActual[0],
                            QuarterOnePercent = GetPercent(quarterPlan[0], quarterActual[0]),
                            QuarterTwoPlan = quarterPlan[1],
                            QuarterTwoActual = quarterActual[1],
                            QuarterTwoPercent = GetPercent(quarterPlan[1], quarterActual[1]),
                            QuarterThreePlan = quarterPlan[2],
                            QuarterThreeActual = quarterActual[2],
                            QuarterThreePercent = GetPercent(quarterPlan[2], quarterActual[2]),
                            QuarterFourPlan = quarterPlan[3],
                            QuarterFourActual = quarterActual[3],
                            QuarterFourPercent = GetPercent(quarterPlan[3], quarterActual[3])
                        });
                    }
                }
            }

            return result;
        }

        private static List<int> ParseQuarters(object[] quarters)
        {
            if (quarters == null || quarters.Length == 0)
            {
                return new List<int>();
            }

            return quarters
                .Select(x => x != null && int.TryParse(x.ToString(), out var value) ? value : 0)
                .Where(x => x >= 1 && x <= 4)
                .Distinct()
                .ToList();
        }

        private static decimal GetPercent(decimal plan, decimal actual)
        {
            return plan == 0 ? 0 : 100 * actual / plan;
        }
    }
}
