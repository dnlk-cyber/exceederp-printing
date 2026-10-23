using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.Budget.DataSource
{

    [DataObject]
    public class BudgetPlanQuarterlyOds
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetPlanByQuarterVM> GetBudgetPlan(string budgetYearId, string type, bool draft)
        {
            var result = new List<BudgetPlanByQuarterVM>();

            if (string.IsNullOrWhiteSpace(budgetYearId))
                return result;

            var costCenterId = db.GlSegmentSetups
                                .Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter)
                                .FirstOrDefault()
                                ?.GLSegmentSetupId ?? 0;

            var costCenterValues = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costCenterId)
                           .AsNoTracking()
                           .ToList()
                           .ToDictionary(b => b.Values, b => b.Description);

            var bCategory = db.BudgetCategorys
                .AsNoTracking()
                .ToList()
                .ToDictionary(b => b.BudgetCategoryID, b => b.Name);

            int fyearId = int.Parse(budgetYearId);

            // Get periods once
            var periods = db.GLPeriods
                .Where(p => p.GlFiscalYearId == fyearId)
                .AsNoTracking()
                .ToList();

            var periodIdsByQuarter = new Dictionary<int, HashSet<int>>
            {
                [1] = periods.Where(p => p.Index >= 1 && p.Index <= 3).Select(p => p.GLPeriodId).ToHashSet(),
                [2] = periods.Where(p => p.Index >= 4 && p.Index <= 6).Select(p => p.GLPeriodId).ToHashSet(),
                [3] = periods.Where(p => p.Index >= 7 && p.Index <= 9).Select(p => p.GLPeriodId).ToHashSet(),
                [4] = periods.Where(p => p.Index >= 10 && p.Index <= 12).Select(p => p.GLPeriodId).ToHashSet()
            };

            // Preload relevant data
            var categories = db.PlanCategorys
                .Where(pc => pc.GlFiscalYearId == fyearId)
                .Where(pc => pc.IsDepartmentOnlineApproved && !pc.IsVoid)
                .Where(pc => draft || pc.IsOnlineApproved)
                .AsNoTracking()
                .ToList();

            if (!string.IsNullOrEmpty(type))
                categories = categories.Where(x => x.BudgetType == type).ToList();

            var categoryIds = categories.Select(c => c.PlanCategoryID).ToList();

            var plans = db.BudgetPlans
                .Where(p => categoryIds.Contains(p.PlanCategoryId))
                .AsNoTracking()
                .ToList();

            var planIds = plans.Select(p => p.BudgetPlanID).ToList();

            var monthlies = db.BudgetPlanMonthlys
                .Where(m => planIds.Contains(m.BudgetDefineID))
                .AsNoTracking()
                .ToList()
                .ToLookup(m => m.BudgetDefineID);

            // Preload all BudgetItems (to avoid Find in loop)
            var allBudgetItemIds = plans.Select(p => int.Parse(p.BudgetItem)).Distinct().ToList();
            var budgetItemsDict = db.BudgetCategoryDetails
                .Where(b => allBudgetItemIds.Contains(b.BudgetCategoryDetailID))
                .ToDictionary(b => b.BudgetCategoryDetailID, b => b.BudgetItem);

            // Group by BudgetType
            foreach (var group in categories.GroupBy(c => c.BudgetType))
            {
                var vm = new BudgetPlanByQuarterVM
                {
                    BudgetType = group.Key,
                    BudgetPlanByQuarterCategoryVMs = new List<BudgetPlanByQuarterCategoryVM>()
                };

                foreach (var cat in group)
                {
                    var catPlans = plans.Where(p => p.PlanCategoryId == cat.PlanCategoryID);

                    foreach (var plan in catPlans)
                    {
                        var bItemId = int.Parse(plan.BudgetItem);
                        var monthlyList = monthlies[plan.BudgetPlanID];

                        decimal q1 = 0, q2 = 0, q3 = 0, q4 = 0;

                        foreach (var month in monthlyList)
                        {
                            if (periodIdsByQuarter[1].Contains(month.GLPeriodId)) q1 += month.CurrentBudget;
                            else if (periodIdsByQuarter[2].Contains(month.GLPeriodId)) q2 += month.CurrentBudget;
                            else if (periodIdsByQuarter[3].Contains(month.GLPeriodId)) q3 += month.CurrentBudget;
                            else if (periodIdsByQuarter[4].Contains(month.GLPeriodId)) q4 += month.CurrentBudget;
                        }

                        vm.BudgetPlanByQuarterCategoryVMs.Add(new BudgetPlanByQuarterCategoryVM
                        {
                            BudgetType = group.Key,
                            BudgetCategory = bCategory.TryGetValue(cat.BudgetCategoryID, out var catName) ? catName : null,
                            BudgetItem = budgetItemsDict.TryGetValue(bItemId, out var itemName) ? itemName : null,
                            CostCenter = plan.CostCenter != null && costCenterValues.TryGetValue(plan.CostCenter, out var costCenter) ? costCenter : null,                            Quarter1 = q1,
                            Quarter2 = q2,
                            Quarter3 = q3,
                            Quarter4 = q4,
                            Total = q1 + q2 + q3 + q4
                        });
                    }
                }

                vm.Quarter1 = vm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter1);
                vm.Quarter2 = vm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter2);
                vm.Quarter3 = vm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter3);
                vm.Quarter4 = vm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter4);
                vm.Total = vm.Quarter1 + vm.Quarter2 + vm.Quarter3 + vm.Quarter4;

                result.Add(vm);
            }

            return result;
        }

        //public List<BudgetPlanByQuarterVM> GetBudgetPlan1(string budgetYearId, string type, bool draft)
        //{

        //    List<BudgetPlanByQuarterVM> list = new List<BudgetPlanByQuarterVM>();


        //    if (!string.IsNullOrEmpty(budgetYearId))
        //    {

        //        int fyearId = int.Parse(budgetYearId);
        //        var periods = db.GLPeriods
        //            .Where(x => x.GlFiscalYearId == fyearId).AsNoTracking().ToList();

        //        var q1 = periods.Where(x => x.Index >= 1 && x.Index <= 3).Select(x => x.GLPeriodId);
        //        var q2 = periods.Where(x => x.Index >= 4 && x.Index <= 6).Select(x => x.GLPeriodId);
        //        var q3 = periods.Where(x => x.Index >= 7 && x.Index <= 9).Select(x => x.GLPeriodId);
        //        var q4 = periods.Where(x => x.Index >= 10 && x.Index <= 12).Select(x => x.GLPeriodId);


        //        var currentYearPlanCategories = db.PlanCategorys
        //            .Where(x => x.IsDepartmentOnlineApproved)
        //            .Where(x => (draft || x.IsOnlineApproved) && !x.IsVoid)
        //            .Where(x => x.GlFiscalYearId == fyearId)
        //            .AsNoTracking()
        //            .ToList();

        //        var currYearBudgetTypes = currentYearPlanCategories.Select(x => x.BudgetType).ToList();

        //        if (!string.IsNullOrEmpty(type))
        //        {
        //            currYearBudgetTypes = currYearBudgetTypes.Where(x => x == type).ToList();
        //        }


        //        foreach (var typ in currYearBudgetTypes)
        //        {
        //            var quarterlyVm = new BudgetPlanByQuarterVM
        //            {
        //                BudgetType = typ,
        //                BudgetPlanByQuarterCategoryVMs = new List<BudgetPlanByQuarterCategoryVM>()
        //            };

        //            var categories = currentYearPlanCategories.Where(x => x.BudgetType == typ).ToList();

        //            foreach (var cat in categories)
        //            {

        //                var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == cat.PlanCategoryID).AsNoTracking().ToList();

        //                foreach (var budgetPlan in plans)
        //                {
        //                    int bItemId = int.Parse(budgetPlan.BudgetItem);
        //                    var monthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID).AsNoTracking().ToList();
        //                    var entity = new BudgetPlanByQuarterCategoryVM
        //                    {

        //                        BudgetCategory = cat.Name,
        //                        BudgetItem = db.BudgetCategoryDetails.Find(bItemId)?.BudgetItem,
        //                        CostCenter = budgetPlan.CostCenter,
        //                        Quarter1 = monthly?.Where(x => q1.Contains(x.GLPeriodId)).Sum(x => x.CurrentBudget) ?? 0,
        //                        Quarter2 = monthly?.Where(x => q2.Contains(x.GLPeriodId)).Sum(x => x.CurrentBudget) ?? 0,
        //                        Quarter3 = monthly?.Where(x => q3.Contains(x.GLPeriodId)).Sum(x => x.CurrentBudget) ?? 0,
        //                        Quarter4 = monthly?.Where(x => q4.Contains(x.GLPeriodId)).Sum(x => x.CurrentBudget) ?? 0

        //                    };
        //                    quarterlyVm.BudgetPlanByQuarterCategoryVMs.Add(entity);
        //                }


        //            }

        //            quarterlyVm.Quarter1 = quarterlyVm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter1);
        //            quarterlyVm.Quarter2 = quarterlyVm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter2);
        //            quarterlyVm.Quarter3 = quarterlyVm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter3);
        //            quarterlyVm.Quarter3 = quarterlyVm.BudgetPlanByQuarterCategoryVMs.Sum(x => x.Quarter3);

        //            list.Add(quarterlyVm);
        //        }

        //    }
        //    return list;

        //}
    }
}
