using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BudgetAccountSheduleSingleQuarterOds : List<AccountSheduleSingleQuarterVm>
    {
        ExceedDbContext db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();
        ReportHelper reportHelper = new ReportHelper();
        BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
        FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountSheduleSingleQuarterVm> GetAccountShedule(string year, string schedule, string branch, string rule, string periodFrom, string periodTo,string quarter)
        {

            List<AccountSheduleSingleQuarterVm> acccountSchedules = new List<AccountSheduleSingleQuarterVm>();
            #region sub category variables
            decimal grand_beginning = 0;
            decimal grand_quarterActual = 0;
            decimal grand_quarterBudget = 0;
            decimal grand_quarterPercentage = 0;
            decimal grand_quarterVariance = 0;

            decimal grand_totalActual = 0;
            decimal grand_totalBudget = 0;
            decimal grand_totalPercentage = 0;
            decimal grand_totalVariance = 0;
            #endregion

            if (!String.IsNullOrWhiteSpace(year)&& !String.IsNullOrWhiteSpace(quarter)&& !String.IsNullOrWhiteSpace(schedule))
            {
                List<GLLedgerPostingLines> yearBegPeriodBalance = new List<GLLedgerPostingLines>();
                List<GLLedgerPostingLines> quarterBegPeriodBalance = new List<GLLedgerPostingLines>();
                List<GLLedgerPostingLines> quarterEndPeriodBalance = new List<GLLedgerPostingLines>();

                List<BudgetPlanMonthly> budgetAnnualMonthlies = new List<BudgetPlanMonthly>();
                List<BudgetPlanMonthly> budgetQuarterMonthlies = new List<BudgetPlanMonthly>();

                var budgetSchedules = db.BudgetAccountShedules
                    .Where(x => x.BudgetAccountSheduleCategoryId.ToString() == schedule).FirstOrDefault();//cuz one schedule should be there once
                var budgetScheduleline = db.BudgetAccountSheduleLines.ToList();
                var budgetSchedulelineAccount = db.BudgetLineSheduleAccounts.ToList();
                Branch legerBranch = new Branch();
                bool isBranch = false;
                if (!String.IsNullOrWhiteSpace(branch))
                {
                    legerBranch = db.Branch.FirstOrDefault(x => x.BranchId.ToString() == branch);
                    if (legerBranch != null)
                    {
                        isBranch = true;
                    }
                }

                int did = int.Parse(year);
                var fiscalYear = db.GLFiscalYears.Find(did);
                int fiscalyearId = fiscalYear.GlFiscalYearId;

                int yearBegPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(fiscalyearId);                

                int quarterBegPeriodId = 0;
                int quarterEndPeriodId = 0;

                switch(quarter)
                {
                    case "1":
                        quarterBegPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 1);
                        quarterEndPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 3);                        
                        break;
                    case "2":
                        quarterBegPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 4);
                        quarterEndPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 6);
                        break;
                    case "3":
                        quarterBegPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 7);
                        quarterEndPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 9);
                        break;
                    case "4":
                        quarterBegPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 10);
                        quarterEndPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalyearId, 12);
                        break;
                }

                var allBalnces = reportHelper.GetAccountBalanceByPeriodRange(fiscalyearId, yearBegPeriodId, quarterEndPeriodId);
                yearBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == yearBegPeriodId).ToList();
                quarterBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterBegPeriodId).ToList();
                quarterEndPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterEndPeriodId).ToList();               
              

                //if (!string.IsNullOrEmpty(rule))
                //{
                //    budgetSchedules = budgetSchedules.Where(x => x.Index.ToString() == rule).ToList();
                //}

                if (budgetSchedules!=null)
                {                   

                    var budgetScheduleSubCategories = db.BudgetAccountSheduleSubCategories
                                                        .Where(x => x.BudgetAccountSheduleId == budgetSchedules.BudgetAccountSheduleId).ToList();

                    foreach (var subCat in budgetScheduleSubCategories)
                    {
                        #region sub category variables
                        decimal cat_beginning = 0;
                        decimal cat_quarterActual = 0;
                        decimal cat_quarterBudget = 0;
                        decimal cat_quarterPercentage = 0;
                        decimal cat_quarterVariance = 0;

                        decimal cat_totalActual = 0;
                        decimal cat_totalBudget = 0;
                        decimal cat_totalPercentage = 0;
                        decimal cat_totalVariance = 0;
                        #endregion
                        #region sub category header
                        acccountSchedules.Add(new AccountSheduleSingleQuarterVm
                        {
                            AccountDesc = subCat.Name.ToUpper(),
                            Branch = legerBranch.Name,
                            IsHead = true
                        });
                        #endregion
                        var budgetSchedulesLines = budgetScheduleline.Where(x => x.BudgetAccountSheduleSubCategoryId == subCat.BudgetAccountSheduleSubCategoryId);

                        foreach (var budgetSchedulesLine in budgetSchedulesLines)
                        {
                            var budgetSchedulesLineAccounts = budgetSchedulelineAccount.Where(x => x.BudgetAccountSheduleLineId == budgetSchedulesLine.BudgetAccountSheduleLineId);
                            decimal beginning = 0;
                            decimal quarterActual = 0;
                            decimal quarterBudget = 0;
                            decimal quarterPercentage = 0;
                            decimal quarterVariance = 0;

                            decimal totalActual = 0;
                            decimal totalBudget = 0;
                            decimal totalPercentage = 0;
                            decimal totalVariance = 0;


                            foreach (var range in budgetSchedulesLineAccounts)
                            {
                                decimal netChange = 0;
                                if (isBranch)
                                {
                                    beginning = beginning + reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(
                                                    yearBegPeriodBalance, range.FromAccountCode, range.ToAccountCode,
                                                    legerBranch.LocationAccount);


                                    netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(quarterEndPeriodBalance, range.FromAccountCode, range.ToAccountCode,
                                                    legerBranch.LocationAccount)
                                                - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(quarterBegPeriodBalance, range.FromAccountCode, range.ToAccountCode,
                                                    legerBranch.LocationAccount);

                                    quarterActual = quarterActual + netChange;

                                    netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(quarterEndPeriodBalance, range.FromAccountCode, range.ToAccountCode,
                                                    legerBranch.LocationAccount)
                                                - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(yearBegPeriodBalance, range.FromAccountCode, range.ToAccountCode,
                                                    legerBranch.LocationAccount);

                                    totalActual = totalActual + netChange;

                                    #region Budget with branch

                                    budgetAnnualMonthlies = budgetReportHelper.GetPeriodRangePlanBalance(fiscalyearId, yearBegPeriodId, quarterEndPeriodId);
                                    switch (quarter)
                                    {
                                        case "1":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.First);
                                            break;
                                        case "2":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Second);
                                            break;
                                        case "3":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Third);
                                            break;
                                        case "4":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Fourth);
                                            break;
                                    }

                                    quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.FromAccountCode, range.ToAccountCode);
                                    totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.FromAccountCode, range.ToAccountCode);
                                    #endregion

                                }
                                else
                                {
                                    beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                                    yearBegPeriodBalance, range.FromAccountCode, range.ToAccountCode);

                                    netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.FromAccountCode, range.ToAccountCode)
                                                - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.FromAccountCode, range.ToAccountCode);
                                    quarterActual = quarterActual + netChange;

                                    netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.FromAccountCode, range.ToAccountCode)
                                                - reportHelper.GetAccountBegBalancesWithAllranges(yearBegPeriodBalance, range.FromAccountCode, range.ToAccountCode);
                                    totalActual = totalActual + netChange;


                                    #region Budget 

                                    budgetAnnualMonthlies = budgetReportHelper.GetPeriodRangePlanBalance(fiscalyearId, yearBegPeriodId, quarterEndPeriodId);
                                    switch (quarter)
                                    {
                                        case "1":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.First);
                                            break;
                                        case "2":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Second);
                                            break;
                                        case "3":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Third);
                                            break;
                                        case "4":
                                            budgetQuarterMonthlies = budgetReportHelper.ParseQuarterBalanceFromPeriod(budgetAnnualMonthlies, fiscalyearId, BudgetEnums.BudgetQuarter.Fourth);
                                            break;
                                    }

                                    quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.FromAccountCode, range.ToAccountCode);
                                    totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.FromAccountCode, range.ToAccountCode);
                                    #endregion
                                }
                            }

                            quarterVariance = quarterBudget - quarterActual;
                            quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                            totalVariance = totalBudget - totalActual;
                            totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;



                            acccountSchedules.Add(new AccountSheduleSingleQuarterVm
                            {

                                AccountDesc = budgetSchedulesLine.Name,
                                Branch = legerBranch.Name,
                                Begining = beginning,
                                QuarterPlan = quarterBudget,
                                QuarterActual = quarterActual,
                                QuarterPercentage = quarterPercentage,
                                QuarterVariance = quarterVariance,

                                TotalBudget = totalBudget,
                                TotalActual = totalActual,
                                TotalPercentage = totalPercentage,
                                TotalVariance = totalVariance

                            });

                           
                            #region sub sum
                            cat_beginning = cat_beginning+beginning;
                            cat_quarterActual = cat_quarterActual+quarterActual;
                            cat_quarterBudget = cat_quarterBudget+quarterBudget;
                            cat_quarterPercentage = 0;
                            cat_quarterVariance = cat_quarterVariance+quarterVariance;

                            cat_totalActual = cat_totalActual+totalActual;
                            cat_totalBudget = cat_totalBudget+totalBudget;
                            cat_totalPercentage = 0;
                            cat_totalVariance = cat_totalVariance+totalVariance;
                            #endregion
                        }

                        #region sub category footer...summary
                        acccountSchedules.Add(new AccountSheduleSingleQuarterVm
                        {
                            AccountDesc = $"{subCat.Name.ToUpper()}  TOTAL",
                            Branch = legerBranch.Name,
                            Begining = cat_beginning,
                            QuarterPlan = cat_quarterBudget,
                            QuarterActual = cat_quarterActual,
                            QuarterPercentage = cat_quarterPercentage,
                            QuarterVariance = cat_quarterVariance,
                            TotalBudget = cat_totalBudget,
                            TotalActual = cat_totalActual,
                            TotalPercentage = cat_totalPercentage,
                            TotalVariance = cat_totalVariance,
                            IsHead = true
                        });
                        #endregion
                        #region grand sum
                        grand_beginning = grand_beginning + cat_beginning;
                        grand_quarterActual = grand_quarterActual + cat_quarterActual;
                        grand_quarterBudget = grand_quarterBudget + cat_quarterBudget;
                        grand_quarterPercentage = 0;
                        grand_quarterVariance = grand_quarterVariance + cat_quarterVariance;

                        grand_totalActual = grand_totalActual + cat_totalActual;
                        grand_totalBudget = grand_totalBudget + cat_totalBudget;
                        grand_totalPercentage = 0;
                        grand_totalVariance = grand_totalVariance + cat_totalVariance;
                        #endregion
                    }

                    #region grand total
                    acccountSchedules.Add(new AccountSheduleSingleQuarterVm
                    {
                        AccountDesc = "GRAND TOTAL",
                        Branch = legerBranch.Name,
                        Begining = grand_beginning,
                        QuarterPlan = grand_quarterBudget,
                        QuarterActual = grand_quarterActual,
                        QuarterPercentage = grand_quarterPercentage,
                        QuarterVariance = grand_quarterVariance,
                        TotalBudget = grand_totalBudget,
                        TotalActual = grand_totalActual,
                        TotalPercentage = grand_totalPercentage,
                        TotalVariance = grand_totalVariance,
                        IsHead = true
                    });
                    #endregion
                }
            }
            return acccountSchedules;

        }
    }
}
