using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Planning.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class StrategicPlanReportODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StrategicPlanLookupVM> GetStrategicPlans()
        {
            using (var db = new ExceedDbContext())
            {
                var plans = db.StrategicPlans.OrderBy(x => x.FromGlFiscalYear.DateFrom).ToList();
                var fiscalYearIds = plans
                    .SelectMany(x => new[] { x.FromGlFiscalYearId, x.ToGlFiscalYearId })
                    .Distinct()
                    .ToList();
                var fiscalYears = db.GLFiscalYears
                    .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                    .ToDictionary(x => x.GlFiscalYearId, x => x.Name);

                return plans.Select(plan => new StrategicPlanLookupVM
                {
                    StrategicPlanId = plan.StrategicPlanId,
                    DisplayName = GetPlanPeriod(plan.FromGlFiscalYearId, plan.ToGlFiscalYearId, fiscalYears)
                        + (string.IsNullOrWhiteSpace(plan.Description) ? string.Empty : " - " + plan.Description)
                }).ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StrategicPlanReportVM> GetStrategicPlanReport(string strategicPlanId)
        {
            using (var db = new ExceedDbContext())
            {
                var plansQuery = db.StrategicPlans.AsQueryable();
                int selectedPlanId;
                if (int.TryParse(strategicPlanId, out selectedPlanId))
                {
                    plansQuery = plansQuery.Where(x => x.StrategicPlanId == selectedPlanId);
                }

                var plans = plansQuery.OrderBy(x => x.FromGlFiscalYear.DateFrom).ToList();
                var planIds = plans.Select(x => x.StrategicPlanId).ToList();
                if (!planIds.Any())
                {
                    return new List<StrategicPlanReportVM>();
                }

                var fiscalYearIds = plans
                    .SelectMany(x => new[] { x.FromGlFiscalYearId, x.ToGlFiscalYearId })
                    .Concat(db.AnnualGoals
                        .Where(x => planIds.Contains(x.StrategicPlanId))
                        .Select(x => x.GlFiscalYearId))
                    .Distinct()
                    .ToList();
                var fiscalYears = db.GLFiscalYears
                    .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                    .ToDictionary(x => x.GlFiscalYearId, x => x.Name);
                var strategicGoals = db.StrategicGoals
                    .Where(x => planIds.Contains(x.StrategicPlanId))
                    .OrderBy(x => x.StrategicGoalId)
                    .ToList()
                    .GroupBy(x => x.StrategicPlanId)
                    .ToDictionary(x => x.Key, x => x.ToList());
                var annualGoals = db.AnnualGoals
                    .Where(x => planIds.Contains(x.StrategicPlanId))
                    .OrderBy(x => x.GlFiscalYear.DateFrom)
                    .ToList()
                    .GroupBy(x => x.StrategicPlanId)
                    .ToDictionary(x => x.Key, x => x.ToList());
                var objectives = db.Objectives
                    .Where(x => x.StrategicPlanId.HasValue && planIds.Contains(x.StrategicPlanId.Value))
                    .OrderBy(x => x.ObjectiveId)
                    .ToList()
                    .GroupBy(x => x.StrategicPlanId.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());
                var workUnitIds = objectives.Values
                    .SelectMany(x => x)
                    .Select(x => x.OrgStructureId)
                    .Distinct()
                    .ToList();
                var workUnits = db.MainCompanyStructures
                    .Where(x => workUnitIds.Contains(x.OrgStructureId))
                    .ToDictionary(x => x.OrgStructureId, x => x.Name);

                var result = new List<StrategicPlanReportVM>();
                var serialNumber = 1;
                foreach (var plan in plans)
                {
                    var period = GetPlanPeriod(
                        plan.FromGlFiscalYearId,
                        plan.ToGlFiscalYearId,
                        fiscalYears);
                    var hasItems = false;

                    if (strategicGoals.ContainsKey(plan.StrategicPlanId))
                    {
                        foreach (var goal in strategicGoals[plan.StrategicPlanId])
                        {
                            result.Add(CreateRow(serialNumber++, period, plan.Description,
                                "Strategic Goal", null, goal.Name, null, null,
                                goal.Description, goal.Remark));
                            hasItems = true;
                        }
                    }

                    if (annualGoals.ContainsKey(plan.StrategicPlanId))
                    {
                        foreach (var goal in annualGoals[plan.StrategicPlanId])
                        {
                            string fiscalYear;
                            fiscalYears.TryGetValue(goal.GlFiscalYearId, out fiscalYear);
                            result.Add(CreateRow(serialNumber++, period, plan.Description,
                                "Annual Goal", fiscalYear, goal.Name, null, null,
                                goal.Description, goal.Remark));
                            hasItems = true;
                        }
                    }

                    if (objectives.ContainsKey(plan.StrategicPlanId))
                    {
                        foreach (var objective in objectives[plan.StrategicPlanId])
                        {
                            string workUnit;
                            workUnits.TryGetValue(objective.OrgStructureId, out workUnit);
                            result.Add(CreateRow(serialNumber++, period, plan.Description,
                                "Objective", null, objective.Name, workUnit, objective.Weight,
                                objective.Description, objective.Remark));
                            hasItems = true;
                        }
                    }

                    if (!hasItems)
                    {
                        result.Add(CreateRow(serialNumber++, period, plan.Description,
                            "Strategic Plan", null, null, null, null, null, null));
                    }
                }

                return result;
            }
        }

        private static StrategicPlanReportVM CreateRow(
            int serialNumber,
            string planPeriod,
            string keyResultArea,
            string itemType,
            string fiscalYear,
            string name,
            string workUnit,
            decimal? weight,
            string description,
            string remark)
        {
            return new StrategicPlanReportVM
            {
                SerialNumber = serialNumber,
                PlanPeriod = planPeriod,
                KeyResultArea = keyResultArea,
                ItemType = itemType,
                FiscalYear = fiscalYear,
                Name = name,
                WorkUnit = workUnit,
                Weight = weight,
                Description = description,
                Remark = remark
            };
        }

        private static string GetPlanPeriod(
            int fromFiscalYearId,
            int toFiscalYearId,
            IDictionary<int, string> fiscalYears)
        {
            string fromFiscalYear;
            string toFiscalYear;
            fiscalYears.TryGetValue(fromFiscalYearId, out fromFiscalYear);
            fiscalYears.TryGetValue(toFiscalYearId, out toFiscalYear);

            return (fromFiscalYear ?? fromFiscalYearId.ToString()) + " - "
                + (toFiscalYear ?? toFiscalYearId.ToString());
        }
    }
}
