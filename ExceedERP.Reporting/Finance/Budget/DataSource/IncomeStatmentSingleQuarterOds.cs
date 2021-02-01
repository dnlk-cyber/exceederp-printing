using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.BugetGAAP;
using ExceedERP.Reporting.Finance.Budget.ViewModel;
using ExceedERP.Helpers.Budget;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.Core.Domain.Finance.Report.GAAP;

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class IncomeStatmentSingleQuarterOds : List<FinancialStatementSingleQuarterVm>
    {
        public ExceedDbContext Db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementSingleQuarterVm> GetIncomeStatmentDataSource(string year,  string quarter)
        {
            ReportHelper reportHelper = new ReportHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

            var balance = new List<FinancialStatementSingleQuarterVm>();
            List<GLLedgerPostingLines> yearBegPeriodBalance = new List<GLLedgerPostingLines>();
            List<GLLedgerPostingLines> quarterBegPeriodBalance = new List<GLLedgerPostingLines>();
            List<GLLedgerPostingLines> quarterEndPeriodBalance = new List<GLLedgerPostingLines>();

            List<BudgetPlanMonthly> budgetAnnualMonthlies = new List<BudgetPlanMonthly>();
            List<BudgetPlanMonthly> budgetQuarterMonthlies = new List<BudgetPlanMonthly>();

            var operationIncomeList = new List<FinancialStatementSingleQuarterVm>();
            var otherIncomeList = new List<FinancialStatementSingleQuarterVm>();
            var costofGoodSoldExpenseList = new List<FinancialStatementSingleQuarterVm>();
            var sellingAndDistributionExpenseList = new List<FinancialStatementSingleQuarterVm>();
            var administrativeAndGeneralExpenseList = new List<FinancialStatementSingleQuarterVm>();

            if (!string.IsNullOrWhiteSpace(year) && !string.IsNullOrWhiteSpace(quarter))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var gaapIncomeStatement = Db.GaapIncomeStatements.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPTwo);

                if (gaapIncomeStatement != null && fiscalYear != null)
                {
                    int fiscalyearId = fiscalYear.GlFiscalYearId;

                    int yearBegPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(fiscalyearId);

                    int quarterBegPeriodId = 0;
                    int quarterEndPeriodId = 0;

                    switch (quarter)
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


                    int firstPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,1);
                    int thirdPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,3);
                    int fourthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,4);
                    int sixthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,6);
                    int seventhPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,7);
                    int ninthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,9);
                    int tenthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,10);
                    int twelvethPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearId,12);

                    var expensesBalances = gaapIncomeStatement.Expenses.ToList();
                    var revenuesBalances = gaapIncomeStatement.Revenues.ToList();

                    var allBalnces = reportHelper.GetAccountBalanceByPeriodRange(fiscalyearId, yearBegPeriodId, quarterEndPeriodId);

                    yearBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == yearBegPeriodId).ToList();
                    quarterBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterBegPeriodId).ToList();
                    quarterEndPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterEndPeriodId).ToList();



                    //#region Budget Modifed 1/25/18
                    //var periodRangeBalnce = budgetReportHelper.GetPeriodRangePlanBalance(fiscalyearId, firstPeriodId, lastPeriodId);
                    //var firstQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.First);
                    //var secondQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Second);
                    //var thirdQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Third);
                    //var fourthQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Fourth);
                    
                    //decimal PlanDiffFirst = 0;
                    //decimal PlanDiffSecond = 0;
                    //decimal PlanDiffThird = 0;
                    //decimal PlanDiffFourth = 0;
                    //#endregion


                    #region operation
                    #region operational income header


                    var operatingIncomeHedaer = new FinancialStatementSingleQuarterVm
                    {
                        Description = "OPERATING INCOME",
                        IsHeader = true

                    };
                    balance.Add(operatingIncomeHedaer);

                    #endregion

                    #region operational income

                    var operationIncomes = revenuesBalances.Where(x => x.Category == IncomeStatementRevenueCategory.OperatingIncome);

                    foreach (var revenuesBalance in operationIncomes)
                    {
                        var ranges = revenuesBalance.GaapIncomeStatementOperationRevenueRanges.ToList();

                        decimal beginning = 0;
                        decimal quarterActual = 0;
                        decimal quarterBudget = 0;
                        decimal quarterPercentage = 0;
                        decimal quarterVariance = 0;


                        decimal totalActual = 0;
                        decimal totalBudget = 0;
                        decimal totalPercentage = 0;
                        decimal totalVariance = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            yearBegPeriodBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
                            quarterActual = quarterActual + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(yearBegPeriodBalance, range.AccountFrom, range.AccountTo)
                                      - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
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

                            quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.AccountFrom, range.AccountTo);
                            totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        quarterVariance = quarterBudget - quarterActual;
                        quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                        totalVariance = totalBudget - totalActual;
                        totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;

                        var opIncomeRecord = new FinancialStatementSingleQuarterVm
                        {
                            Description = revenuesBalance.Description,
                            Begining = beginning,
                            QuarterPlan = quarterBudget,
                            QuarterActual = quarterActual,
                            QuarterPercentage = quarterPercentage,
                            QuarterVariance = quarterVariance,

                            TotalBudget = totalBudget,
                            TotalActual = totalActual,
                            TotalPercentage = totalPercentage,
                            TotalVariance = totalVariance

                        };

                        operationIncomeList.Add(opIncomeRecord);
                    }
                    balance.AddRange(operationIncomeList);
                    #region total op revenue
                    var total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL OPERATING INCOME",
                        QuarterActual = operationIncomeList.Sum(x => x.QuarterActual),
                        QuarterPlan = operationIncomeList.Sum(x => x.QuarterPlan),
                        QuarterVariance = operationIncomeList.Sum(x => x.QuarterVariance),
                        TotalActual = operationIncomeList.Sum(x => x.TotalActual),
                        TotalBudget = operationIncomeList.Sum(x => x.TotalBudget),
                        TotalVariance = operationIncomeList.Sum(x => x.TotalVariance),                        
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region cost of service sold
                    #region cost of service sold header


                    var costofGoodSoldExpenseHeader = new FinancialStatementSingleQuarterVm
                    {
                        Description = "COST OF SERVICE SOLD",
                        IsHeader = true

                    };
                    balance.Add(costofGoodSoldExpenseHeader);

                    #endregion

                    var costOfGoodSoldExpenses = expensesBalances.Where(x => x.Category == IncomeStatementExpenseCategory.CostOfGoodSold);

                    foreach (var expensesBalance in costOfGoodSoldExpenses)
                    {

                        var ranges = expensesBalance.GaapIncomeStatementOperationExpenseRanges.ToList();
                        decimal beginning = 0;
                        decimal quarterActual = 0;
                        decimal quarterBudget = 0;
                        decimal quarterPercentage = 0;
                        decimal quarterVariance = 0;


                        decimal totalActual = 0;
                        decimal totalBudget = 0;
                        decimal totalPercentage = 0;
                        decimal totalVariance = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            yearBegPeriodBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
                            quarterActual = quarterActual + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(yearBegPeriodBalance, range.AccountFrom, range.AccountTo)
                                      - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
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

                            quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.AccountFrom, range.AccountTo);
                            totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        quarterVariance = quarterBudget - quarterActual;
                        quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                        totalVariance = totalBudget - totalActual;
                        totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;

                        var costOfGoodSoldExpense = new FinancialStatementSingleQuarterVm
                        {
                            Description = expensesBalance.Description,
                            Begining = beginning,
                            QuarterPlan = quarterBudget,
                            QuarterActual = quarterActual,
                            QuarterPercentage = quarterPercentage,
                            QuarterVariance = quarterVariance,

                            TotalBudget = totalBudget,
                            TotalActual = totalActual,
                            TotalPercentage = totalPercentage,
                            TotalVariance = totalVariance

                        };

                        costofGoodSoldExpenseList.Add(costOfGoodSoldExpense);
                    }

                    balance.AddRange(costofGoodSoldExpenseList);

                    #region total Cost of Service Sold
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL COST OF SERVICE SOLD",
                        QuarterPlan = costofGoodSoldExpenseList.Sum(x => x.QuarterPlan),
                        QuarterActual = costofGoodSoldExpenseList.Sum(x => x.QuarterActual),
                        QuarterVariance = costofGoodSoldExpenseList.Sum(x => x.QuarterVariance),
                        TotalBudget = costofGoodSoldExpenseList.Sum(x => x.TotalBudget),
                        TotalActual = costofGoodSoldExpenseList.Sum(x => x.TotalActual),
                        TotalVariance = costofGoodSoldExpenseList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #endregion
                    #region gross profit
                    decimal gProfitQuarterBudget = operationIncomeList.Sum(x => x.QuarterPlan) - costofGoodSoldExpenseList.Sum(x => x.QuarterPlan);
                    decimal gProfitQuarterActual = operationIncomeList.Sum(x => x.QuarterActual) - costofGoodSoldExpenseList.Sum(x => x.QuarterActual);
                    decimal gProfitQuarterVariance = operationIncomeList.Sum(x => x.QuarterVariance) - costofGoodSoldExpenseList.Sum(x => x.QuarterVariance);

                    decimal gProfitTotalActual = operationIncomeList.Sum(x => x.TotalActual) - costofGoodSoldExpenseList.Sum(x => x.TotalActual);
                    decimal gProfitTotalBudget = operationIncomeList.Sum(x => x.TotalBudget) - costofGoodSoldExpenseList.Sum(x => x.TotalBudget);
                    decimal gProfitTotalVariance = operationIncomeList.Sum(x => x.TotalVariance) - costofGoodSoldExpenseList.Sum(x => x.TotalVariance);

                    #region total op revenue
                    var grossProfit = new FinancialStatementSingleQuarterVm
                    {
                        Description = "GROSS PROFIT",
                        Begining = 0,
                        QuarterPlan = gProfitQuarterBudget,
                        QuarterActual = gProfitQuarterActual,
                        QuarterPercentage = gProfitQuarterVariance,
                        QuarterVariance = 0,

                        TotalBudget = gProfitTotalBudget,
                        TotalActual = gProfitTotalActual,
                        TotalPercentage = 0,
                        TotalVariance = gProfitTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(grossProfit);
                    #endregion

                    #region selling and dist expense
                    #region selling and dist header


                    var sellingAndDistExpenseHeader = new FinancialStatementSingleQuarterVm
                    {
                        Description = "SELLING AND DISTRIBUTION EXPENSE",
                        IsHeader = true

                    };
                    balance.Add(sellingAndDistExpenseHeader);

                    #endregion

                    var sellingAndAdminExpenses = expensesBalances.Where(x => x.Category == IncomeStatementExpenseCategory.SellingAndDistributionExpense);

                    foreach (var expensesBalance in sellingAndAdminExpenses)
                    {

                        var ranges = expensesBalance.GaapIncomeStatementOperationExpenseRanges.ToList();
                        decimal beginning = 0;
                        decimal quarterActual = 0;
                        decimal quarterBudget = 0;
                        decimal quarterPercentage = 0;
                        decimal quarterVariance = 0;


                        decimal totalActual = 0;
                        decimal totalBudget = 0;
                        decimal totalPercentage = 0;
                        decimal totalVariance = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            yearBegPeriodBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
                            quarterActual = quarterActual + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(yearBegPeriodBalance, range.AccountFrom, range.AccountTo)
                                      - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
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

                            quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.AccountFrom, range.AccountTo);
                            totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        quarterVariance = quarterBudget - quarterActual;
                        quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                        totalVariance = totalBudget - totalActual;
                        totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;

                        var sellingAndDistExpense = new FinancialStatementSingleQuarterVm
                        {
                            Description = expensesBalance.Description,
                            Begining = beginning,
                            QuarterPlan = quarterBudget,
                            QuarterActual = quarterActual,
                            QuarterPercentage = quarterPercentage,
                            QuarterVariance = quarterVariance,

                            TotalBudget = totalBudget,
                            TotalActual = totalActual,
                            TotalPercentage = totalPercentage,
                            TotalVariance = totalVariance

                        };

                        sellingAndDistributionExpenseList.Add(sellingAndDistExpense);
                    }

                    balance.AddRange(sellingAndDistributionExpenseList);

                    #region total selling and dist expense
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL SELLING AND DISTRIBUTION EXPENSE",
                        Begining = sellingAndDistributionExpenseList.Sum(x => x.Begining),
                        QuarterActual = sellingAndDistributionExpenseList.Sum(x => x.QuarterActual),
                        QuarterPlan = sellingAndDistributionExpenseList.Sum(x => x.QuarterPlan),
                        QuarterPercentage = sellingAndDistributionExpenseList.Sum(x => x.QuarterPercentage),
                        QuarterVariance = sellingAndDistributionExpenseList.Sum(x => x.QuarterVariance),
                        TotalBudget = sellingAndDistributionExpenseList.Sum(x => x.TotalBudget),
                        TotalActual = sellingAndDistributionExpenseList.Sum(x => x.TotalActual),
                        TotalVariance = sellingAndDistributionExpenseList.Sum(x => x.TotalVariance),
                        TotalPercentage = sellingAndDistributionExpenseList.Sum(x => x.TotalPercentage),
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region administrative and general expense
                    #region administrative and general header


                    var adminAndGeneralExpenseHeader = new FinancialStatementSingleQuarterVm
                    {
                        Description = "ADMINISTRATION AND GENERAL EXPENSE",
                        IsHeader = true

                    };
                    balance.Add(adminAndGeneralExpenseHeader);

                    #endregion

                    var adminAndGenExpenses = expensesBalances.Where(x => x.Category == IncomeStatementExpenseCategory.AdministrationAndGeneralExpense);

                    foreach (var expensesBalance in adminAndGenExpenses)
                    {

                        var ranges = expensesBalance.GaapIncomeStatementOperationExpenseRanges.ToList();
                        decimal beginning = 0;
                        decimal quarterActual = 0;
                        decimal quarterBudget = 0;
                        decimal quarterPercentage = 0;
                        decimal quarterVariance = 0;


                        decimal totalActual = 0;
                        decimal totalBudget = 0;
                        decimal totalPercentage = 0;
                        decimal totalVariance = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            yearBegPeriodBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
                            quarterActual = quarterActual + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(yearBegPeriodBalance, range.AccountFrom, range.AccountTo)
                                      - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
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

                            quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.AccountFrom, range.AccountTo);
                            totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        quarterVariance = quarterBudget - quarterActual;
                        quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                        totalVariance = totalBudget - totalActual;
                        totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;

                        var adminAndGeneralExpense = new FinancialStatementSingleQuarterVm
                        {
                            Description = expensesBalance.Description,
                            Begining = beginning,
                            QuarterPlan = quarterBudget,
                            QuarterActual = quarterActual,
                            QuarterPercentage = quarterPercentage,
                            QuarterVariance = quarterVariance,

                            TotalBudget = totalBudget,
                            TotalActual = totalActual,
                            TotalPercentage = totalPercentage,
                            TotalVariance = totalVariance

                        };

                        administrativeAndGeneralExpenseList.Add(adminAndGeneralExpense);
                    }

                    balance.AddRange(administrativeAndGeneralExpenseList);

                    #region total admin and general expense
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL ADMINISTRATION AND GENERAL EXPENSE",

                        Begining = administrativeAndGeneralExpenseList.Sum(x => x.Begining),
                        QuarterPlan = administrativeAndGeneralExpenseList.Sum(x => x.QuarterPlan),
                        QuarterActual = administrativeAndGeneralExpenseList.Sum(x => x.QuarterActual),
                        QuarterVariance = administrativeAndGeneralExpenseList.Sum(x => x.QuarterVariance),
                        QuarterPercentage = administrativeAndGeneralExpenseList.Sum(x => x.QuarterPercentage),
                        TotalActual = administrativeAndGeneralExpenseList.Sum(x => x.TotalActual),
                        TotalBudget = administrativeAndGeneralExpenseList.Sum(x => x.TotalBudget),
                        TotalPercentage = administrativeAndGeneralExpenseList.Sum(x => x.TotalPercentage),
                        TotalVariance = administrativeAndGeneralExpenseList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region total operating expense

                    decimal totalOperatingexpenseQuarterBudget = sellingAndDistributionExpenseList.Sum(x => x.QuarterPlan) + administrativeAndGeneralExpenseList.Sum(x => x.QuarterPlan);
                    decimal totalOperatingexpenseQuarterActual = sellingAndDistributionExpenseList.Sum(x => x.QuarterActual) + administrativeAndGeneralExpenseList.Sum(x => x.QuarterActual);
                    decimal totalOperatingexpenseQuarterVariance = sellingAndDistributionExpenseList.Sum(x => x.QuarterVariance) + administrativeAndGeneralExpenseList.Sum(x => x.QuarterVariance);
                    decimal totalOperatingexpenseQuarterPercentage = sellingAndDistributionExpenseList.Sum(x => x.QuarterPercentage) + administrativeAndGeneralExpenseList.Sum(x => x.QuarterPercentage);

                    decimal totalOperatingexpenseTotalPlan = sellingAndDistributionExpenseList.Sum(x => x.TotalBudget) + administrativeAndGeneralExpenseList.Sum(x => x.TotalBudget);
                    decimal totalOperatingexpenseTotalActual = sellingAndDistributionExpenseList.Sum(x => x.TotalActual) + administrativeAndGeneralExpenseList.Sum(x => x.TotalActual);
                    decimal totalOperatingexpenseTotalVariance = sellingAndDistributionExpenseList.Sum(x => x.TotalVariance) + administrativeAndGeneralExpenseList.Sum(x => x.TotalVariance);
                    decimal totalOperatingexpenseTotalPercentage = sellingAndDistributionExpenseList.Sum(x => x.TotalPercentage) + administrativeAndGeneralExpenseList.Sum(x => x.TotalPercentage);
                  
                    #region total operating expense
                    var totalOperatingexpense = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL OPERATING EXPENSE",
                        QuarterPlan = totalOperatingexpenseQuarterBudget,                       
                        QuarterActual = totalOperatingexpenseQuarterActual,
                        QuarterVariance = totalOperatingexpenseQuarterVariance,
                        QuarterPercentage = totalOperatingexpenseQuarterPercentage,
                        TotalBudget = totalOperatingexpenseTotalPlan,
                        TotalActual = totalOperatingexpenseTotalActual,
                        TotalVariance = totalOperatingexpenseTotalVariance,
                        TotalPercentage = totalOperatingexpenseTotalPercentage,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(totalOperatingexpense);
                    #endregion

                    #region total operating income/loss

                    decimal totalOperatingInComeorLossQuarterBudget = gProfitQuarterBudget - totalOperatingexpenseQuarterBudget;                  
                    decimal totalOperatingInComeorLossQuarterActual = gProfitQuarterActual - totalOperatingexpenseQuarterActual;
                    decimal totalOperatingInComeorLossQuarterVariance = gProfitQuarterVariance - totalOperatingexpenseQuarterVariance;

                    decimal totalOperatingInComeorLossTotalActual = gProfitTotalActual - totalOperatingexpenseTotalActual;
                    decimal totalOperatingInComeorLossTotalBudget= gProfitTotalBudget - totalOperatingexpenseTotalPlan;
                    decimal totalOperatingInComeorLossTotalVariance = gProfitTotalVariance - totalOperatingexpenseTotalVariance;

                    #region operating income/loss
                    var totalOperatingincomeorloss = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL OPERATING INCOME/LOSS",
                        QuarterActual = totalOperatingInComeorLossQuarterActual,
                        QuarterPlan = totalOperatingInComeorLossQuarterBudget,
                        QuarterVariance = totalOperatingInComeorLossQuarterVariance,
                        TotalActual = totalOperatingInComeorLossTotalActual,
                        TotalBudget = totalOperatingInComeorLossTotalBudget,
                        TotalVariance = totalOperatingInComeorLossTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(totalOperatingincomeorloss);
                    #endregion

                    #region other income

                    #region other income header


                    var otherIncomeHeader = new FinancialStatementSingleQuarterVm
                    {
                        Description = "OTHER INCOME",
                        IsHeader = true

                    };
                    balance.Add(otherIncomeHeader);

                    #endregion

                    var otherIncomes = revenuesBalances.Where(x => x.Category == IncomeStatementRevenueCategory.OtherIncome);

                    foreach (var revenuesBalance in otherIncomes)
                    {
                        var ranges = revenuesBalance.GaapIncomeStatementOperationRevenueRanges.ToList();

                        decimal beginning = 0;
                        decimal quarterActual = 0;
                        decimal quarterBudget = 0;
                        decimal quarterPercentage = 0;
                        decimal quarterVariance = 0;


                        decimal totalActual = 0;
                        decimal totalBudget = 0;
                        decimal totalPercentage = 0;
                        decimal totalVariance = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            yearBegPeriodBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(quarterEndPeriodBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
                            quarterActual = quarterActual + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(yearBegPeriodBalance, range.AccountFrom, range.AccountTo)
                                      - reportHelper.GetAccountBegBalancesWithAllranges(quarterBegPeriodBalance, range.AccountFrom, range.AccountTo);
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

                            quarterBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetQuarterMonthlies, range.AccountFrom, range.AccountTo);
                            totalBudget += budgetReportHelper.GetQuarterPlanByAccountRange(budgetAnnualMonthlies, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        quarterVariance = quarterBudget - quarterActual;
                        quarterPercentage = (quarterActual / quarterBudget.DivideByZeroHelper()) * 100;

                        totalVariance = totalBudget - totalActual;
                        totalPercentage = (totalActual / totalBudget.DivideByZeroHelper()) * 100;

                        var otherIncomeRecord = new FinancialStatementSingleQuarterVm
                        {
                            Description = revenuesBalance.Description,
                            Begining = beginning,
                            QuarterPlan = quarterBudget,
                            QuarterActual = quarterActual,
                            QuarterPercentage = quarterPercentage,
                            QuarterVariance = quarterVariance,

                            TotalBudget = totalBudget,
                            TotalActual = totalActual,
                            TotalPercentage = totalPercentage,
                            TotalVariance = totalVariance
                        };

                        otherIncomeList.Add(otherIncomeRecord);
                    }
                    balance.AddRange(otherIncomeList);
                    #region total other income
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL OTHER INCOME",
                        QuarterActual = otherIncomeList.Sum(x => x.QuarterActual),
                        QuarterPlan = otherIncomeList.Sum(x => x.QuarterPlan),
                        QuarterVariance = otherIncomeList.Sum(x => x.QuarterVariance),
                        QuarterPercentage = otherIncomeList.Sum(x => x.QuarterPercentage),
                        TotalActual = otherIncomeList.Sum(x => x.TotalActual),
                        TotalBudget = otherIncomeList.Sum(x => x.TotalBudget),
                        TotalVariance = otherIncomeList.Sum(x => x.TotalVariance),
                        TotalPercentage = otherIncomeList.Sum(x => x.TotalPercentage),
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region total operating income before tax
                    decimal taxableIncomeQuarterBudget = totalOperatingInComeorLossQuarterBudget + otherIncomeList.Sum(x => x.QuarterPlan);
                    decimal taxableIncomeQuarterActual = totalOperatingInComeorLossQuarterActual + otherIncomeList.Sum(x => x.QuarterActual);
                    decimal taxableIncomeQuarterVariance = totalOperatingInComeorLossQuarterVariance + otherIncomeList.Sum(x => x.QuarterVariance);

                    decimal taxableIncomeTotalActual = totalOperatingInComeorLossTotalActual + otherIncomeList.Sum(x => x.TotalActual);
                    decimal taxableIncomeTotalBudget = totalOperatingInComeorLossTotalBudget + otherIncomeList.Sum(x => x.TotalBudget);
                    decimal taxableIncomeTotalVariance = totalOperatingInComeorLossTotalVariance + otherIncomeList.Sum(x => x.TotalVariance);
                   
                    #region taxable income
                    var taxableIncome = new FinancialStatementSingleQuarterVm
                    {
                        Description = "TOTAL OPERATING INCOME/LOSS BEFORE INCOME TAX",
                        QuarterActual = taxableIncomeQuarterActual,
                        QuarterPlan = taxableIncomeQuarterBudget,
                        QuarterVariance = taxableIncomeQuarterVariance,
                        TotalActual = taxableIncomeTotalActual,
                        TotalBudget = taxableIncomeTotalBudget,
                        TotalVariance = taxableIncomeTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(taxableIncome);
                    #endregion

                    #region profit tax

                   var profittaxRate = Db.FinanceSettings.FirstOrDefault();
                    decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;


                    if(profittaxRate!= null)
                    {
                           rate = profittaxRate.ProfitTaxCalculatedRate;

                    }
                   
                    #region profit tax
                    decimal profitTaxPlanQaurterActual = taxableIncomeQuarterActual * rate;
                    decimal profitTaxPlanQuarterBudget= taxableIncomeQuarterBudget * rate;
                    decimal profitTaxPlanQuarterVariance = taxableIncomeQuarterVariance * rate;

                    decimal profitTaxPlanTotalActual = taxableIncomeTotalActual * rate;
                    decimal profitTaxPlanTotalBudget= taxableIncomeTotalBudget * rate;
                    decimal profitTaxPlanTotalVariance = taxableIncomeTotalVariance * rate;

                    var profittax = new FinancialStatementSingleQuarterVm
                    {
                        Description = "PROFIT TAX",
                        QuarterActual = profitTaxPlanQaurterActual,
                        QuarterPlan = profitTaxPlanQuarterBudget,
                        QuarterVariance = profitTaxPlanQuarterVariance,
                        TotalActual = profitTaxPlanTotalActual,
                        TotalBudget = profitTaxPlanTotalBudget,
                        TotalVariance = profitTaxPlanTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(profittax);
                    #endregion

                    #region profit after tax

                    #region profit after tax
                    decimal profitAfterTaxQuarterActual = taxableIncomeQuarterBudget - profitTaxPlanQaurterActual;
                    decimal profitAfterTaxQuarterBudget= taxableIncomeQuarterBudget - profitTaxPlanQuarterBudget;
                    decimal profitAfterTaxQuarterVariance = taxableIncomeQuarterVariance - profitTaxPlanQuarterVariance;

                    decimal profitAfterTaxTotalBudget = taxableIncomeTotalBudget - profitTaxPlanTotalBudget;
                    decimal profitAfterTaxTotalActual = taxableIncomeTotalActual - profitTaxPlanTotalActual;
                    decimal profitAfterTaxTotalVariance = taxableIncomeTotalVariance - profitTaxPlanTotalVariance;

                    var profitaftertax = new FinancialStatementSingleQuarterVm
                    {
                        Description = "NET INCOME AFTER TAX",
                        QuarterActual = profitAfterTaxQuarterActual,
                        QuarterPlan = profitAfterTaxQuarterBudget,
                        QuarterVariance = profitAfterTaxQuarterVariance,

                        TotalBudget = profitAfterTaxTotalBudget,
                        TotalActual = profitAfterTaxTotalActual,
                        TotalVariance = profitAfterTaxTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(profitaftertax);
                    #endregion

                }
            }

            return balance;
        }
    }

}