using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Planning.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class QuickFixPlanODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuickFixPlanVM> GetQuickFixPlans(string fiscalYearId, string upToQuarter)
        {
            using (var db = new ExceedDbContext())
            {
                var query = db.QuickFixPlans.AsQueryable();

                int selectedFiscalYear;
                if (int.TryParse(fiscalYearId, out selectedFiscalYear))
                {
                    query = query.Where(x => x.GlFiscalYearId == selectedFiscalYear);
                }

                int selectedQuarter;
                if (int.TryParse(upToQuarter, out selectedQuarter))
                {
                    query = query.Where(x => x.UpToQuarter == selectedQuarter);
                }

                var quickFixPlans = query
                    .OrderBy(x => x.GlFiscalYearId)
                    .ThenBy(x => x.PlanId)
                    .ToList();

                var planIds = quickFixPlans.Select(x => x.PlanId).Distinct().ToList();
                var fiscalYearIds = quickFixPlans.Select(x => x.GlFiscalYearId).Distinct().ToList();
                var plans = db.Plans
                    .Where(x => planIds.Contains(x.PlanId))
                    .ToDictionary(x => x.PlanId);
                var fiscalYears = db.GLFiscalYears
                    .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                    .ToDictionary(x => x.GlFiscalYearId);

                var result = new List<QuickFixPlanVM>();
                var serialNumber = 1;

                foreach (var item in quickFixPlans)
                {
                    plans.TryGetValue(item.PlanId, out var plan);
                    fiscalYears.TryGetValue(item.GlFiscalYearId, out var fiscalYear);

                    result.Add(new QuickFixPlanVM
                    {
                        SerialNumber = serialNumber++,
                        FiscalYear = fiscalYear == null ? null : fiscalYear.Name,
                        MajorActivity = plan == null ? null : plan.Description,
                        AnnualPlan = item.AnnualPlan,
                        UpToQuarter = GetQuarterName(item.UpToQuarter),
                        PlanUpToQuarter = item.PlanUpToQuarter,
                        Actual = item.Actual,
                        PreviousPlanRemainingQuarter = item.PreviousPlanRemainingQuarter,
                        RevisedPlanRemainingQuarter = item.RevisedPlanRemainingQuarter
                    });
                }

                return result;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuickFixQuarterVM> GetQuarters()
        {
            return new List<QuickFixQuarterVM>
            {
                new QuickFixQuarterVM { Value = 1, Text = "Quarter 1" },
                new QuickFixQuarterVM { Value = 2, Text = "Quarter 2" },
                new QuickFixQuarterVM { Value = 3, Text = "Quarter 3" }
            };
        }

        private static string GetQuarterName(int quarter)
        {
            return "Quarter " + quarter;
        }
    }
}
