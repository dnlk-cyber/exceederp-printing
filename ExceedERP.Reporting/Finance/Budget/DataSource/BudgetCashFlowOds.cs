using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.Budget.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Data.Entity;
using System;

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BudgetCashFlowOds : IDisposable
    {
        private readonly ExceedDbContext Db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetCashFlowVm> GetBudgetCashFlow(string year)
        {
            var balance = new List<BudgetCashFlowVm>();

            if (string.IsNullOrWhiteSpace(year)) return balance;

            int yearId = int.Parse(year);
            var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == yearId)
                                      .ToDictionary(x => x.Index, x => x.GLPeriodId);

            var currYearPlancatIds = Db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.IsOnlineApproved && !x.IsVoid)
                                                     .Select(x => x.PlanCategoryID)
                                                     .ToList();

            var currentBudgetPlans = Db.BudgetPlans
                .Where(x => currYearPlancatIds.Contains(x.PlanCategoryId))
                .GroupBy(x => x.BudgetCode)
                .ToDictionary(g => g.Key, g => g.Select(x => x.BudgetPlanID).ToList());

            var revenueAccounts = Db.GLChartOfAccountAccounts
                .Where(x => x.AccountType == GlAccountTypes.Revenue)
                .Select(x => new { x.Values, x.Description })
                .ToList();

            var expenseAccounts = Db.GLChartOfAccountAccounts
                .Where(x => x.AccountType == GlAccountTypes.Expense)
                .Select(x => new { x.Values, x.Description })
                .ToList();

            var budgetPlanMonthly = Db.BudgetPlanMonthlys
                .Where(x => periods.Values.Contains(x.GLPeriodId))
                .GroupBy(x => new { x.BudgetDefineID, x.GLPeriodId })
                .ToDictionary(g => g.Key, g => g.Sum(x => x.CurrentBudget));

            var record = new BudgetCashFlowVm();

            foreach (var r in revenueAccounts)
            {
                if (!currentBudgetPlans.TryGetValue(r.Values, out List<int> budgetPlanIds))
                    continue;

                var inflow = new BudgetCashInFlowVm
                {
                    AccountCode = r.Description
                };

                for (int i = 1; i <= 12; i++)
                {
                    if (periods.TryGetValue(i, out int periodId))
                    {
                        decimal totalBudget = budgetPlanIds
                            .Sum(planId => budgetPlanMonthly.TryGetValue(new { BudgetDefineID = planId, GLPeriodId = periodId }, out decimal budget) ? budget : 0);

                        inflow.SetPeriodBudget(i, totalBudget);
                    }
                }
                record.InFlowVm.Add(inflow);
            }

            foreach (var e in expenseAccounts)
            {
                if (!currentBudgetPlans.TryGetValue(e.Values, out List<int> budgetPlanIds))
                    continue;

                var outflow = new BudgetCashOutFlowVm
                {
                    AccountCode = e.Description
                };

                for (int i = 1; i <= 12; i++)
                {
                    if (periods.TryGetValue(i, out int periodId))
                    {
                        decimal totalBudget = budgetPlanIds
                            .Sum(planId => budgetPlanMonthly.TryGetValue(new { BudgetDefineID = planId, GLPeriodId = periodId }, out decimal budget) ? budget : 0);

                        outflow.SetPeriodBudget(i, totalBudget);
                    }
                }
                record.OutFlowVm.Add(outflow);
            }

            var diff = new BudgetCasFlowDiffVm()
            {
                Period1 = record.InFlowVm.Sum(x => x.Period1) - record.OutFlowVm.Sum(x => x.Period1),
                Period2 = record.InFlowVm.Sum(x => x.Period2) - record.OutFlowVm.Sum(x => x.Period2),
                Period3 = record.InFlowVm.Sum(x => x.Period3) - record.OutFlowVm.Sum(x => x.Period3),
                Period4 = record.InFlowVm.Sum(x => x.Period4) - record.OutFlowVm.Sum(x => x.Period4),
                Period5 = record.InFlowVm.Sum(x => x.Period5) - record.OutFlowVm.Sum(x => x.Period5),
                Period6 = record.InFlowVm.Sum(x => x.Period6) - record.OutFlowVm.Sum(x => x.Period6),
                Period7 = record.InFlowVm.Sum(x => x.Period7) - record.OutFlowVm.Sum(x => x.Period7),
                Period8 = record.InFlowVm.Sum(x => x.Period8) - record.OutFlowVm.Sum(x => x.Period8),
                Period9 = record.InFlowVm.Sum(x => x.Period9) - record.OutFlowVm.Sum(x => x.Period9),
                Period10 = record.InFlowVm.Sum(x => x.Period10) - record.OutFlowVm.Sum(x => x.Period10),
                Period11 = record.InFlowVm.Sum(x => x.Period11) - record.OutFlowVm.Sum(x => x.Period11),
                Period12 = record.InFlowVm.Sum(x => x.Period12) - record.OutFlowVm.Sum(x => x.Period12)
            };
            record.Diff.Add(diff);

            balance.Add(record);
            return balance;
        }


        public List<BudgetCashFlowVm> GetBudgetCashFlow_1(string year)
        {
            var balance = new List<BudgetCashFlowVm>();         

            if (!string.IsNullOrWhiteSpace(year))
            {
                int yearId = int.Parse(year);
                var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == yearId);
                var currYearPlancatIds = Db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.IsOnlineApproved == true && !x.IsVoid).Select(x => x.PlanCategoryID).ToList();
                var CurrentBudgetPlans = Db.BudgetPlans.Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).ToList();
                
                var revenue = Db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Revenue).Select(x=> new { Value=x.Values,Desc=x.Description }).AsNoTracking().ToList();
                var expense = Db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Expense).Select(x => new { Value = x.Values, Desc = x.Description }).AsNoTracking().ToList();
                var record = new BudgetCashFlowVm();
                foreach(var r in revenue)
                {
                    var planIds = CurrentBudgetPlans.Where(x => x.BudgetCode == r.Value).Select(x => x.BudgetPlanID).ToList();
                    if (planIds.Any())
                    {
                        int P1 = periods.Where(x => x.Index == 1).FirstOrDefault().GLPeriodId;
                        var inflow = new BudgetCashInFlowVm
                        {
                            AccountCode = /*r.Value+" "+*/r.Desc,
                            Period1 = Db.BudgetPlanMonthlys
                            .Where(x => x.GLPeriodId == P1 && planIds.Contains(x.BudgetDefineID))
                            .Select(x => (decimal?)x.CurrentBudget) // Cast to nullable decimal
                            .DefaultIfEmpty(0)
                            .Sum() ?? 0,
                        };

                        record.InFlowVm.Add(inflow);
                    }

                }
                
                foreach (var e in expense)
                {
                    var planIds = CurrentBudgetPlans.Where(x => x.BudgetCode == e.Value).Select(x => x.BudgetPlanID).ToList();
                    if (planIds.Any())
                    {
                        int P1 = periods.Where(x => x.Index == 1).FirstOrDefault().GLPeriodId;
                        var outflow = new BudgetCashOutFlowVm
                        {
                            AccountCode = /*r.Value+" "+*/e.Desc,
                            Period1 = Db.BudgetPlanMonthlys
                            .Where(x => x.GLPeriodId == P1 && planIds.Contains(x.BudgetDefineID))
                            .Select(x => (decimal?)x.CurrentBudget) // Cast to nullable decimal
                            .DefaultIfEmpty(0)
                            .Sum() ?? 0,
                        };

                        record.OutFlowVm.Add(outflow);
                    }

                }
                balance.Add(record);

            }
            return balance;
        }
        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}
