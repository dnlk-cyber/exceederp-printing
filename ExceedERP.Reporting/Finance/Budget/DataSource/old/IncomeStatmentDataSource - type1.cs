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

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class IncomeStatmentGaapOne : List<FinancialStatementQuarterlyVm>
    {
        public ExceedDbContext Db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementQuarterlyVm> GetIncomeStatmentDataSource(string year)
        {
            ReportHelper reportHelper = new ReportHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();

            var balance = new List<FinancialStatementQuarterlyVm>();
            var operationRevenueList = new List<FinancialStatementQuarterlyVm>();
            var nonOperationRevenueList = new List<FinancialStatementQuarterlyVm>();
            var operationExpenseList = new List<FinancialStatementQuarterlyVm>();
            var nonOperationExpenseList = new List<FinancialStatementQuarterlyVm>();

            if (!string.IsNullOrWhiteSpace(year))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var gaapIncomeStatement = Db.GaapIncomeStatements.FirstOrDefault();

                if (gaapIncomeStatement != null && fiscalYear != null)
                {
                    int fiscalyearid = fiscalYear.GlFiscalYearId;
                    var expensesBalances = gaapIncomeStatement.Expenses.ToList();
                    var revenuesBalances = gaapIncomeStatement.Revenues.ToList();

                    var firstMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 1));
                    var thirdMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 3));

                    var forthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 4));
                    var sixthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 6));

                    var seventhMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 7));
                    var ninthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 9));

                    var tenthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 10));
                    var twelvethMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 12));



                    #region Budget Modifed 1/25/18
                    var firstQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.First);
                    var secondQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Second);
                    var thirdQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Third);
                    var fourthQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Fourth);
                    #endregion


                    #region operation
                    #region operational revenue header


                    var assetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Operation Revenue",
                        IsHeader = true

                    };
                    balance.Add(assetHedaer);

                    #endregion

                    #region operational revenue

                    var operationRevenues = revenuesBalances.Where(x =>x.Category == IncomeStatementRevenueCategory.OperationalRevenue);

                    foreach (var revenuesBalance in operationRevenues)
                    {
                        var ranges = revenuesBalance.GaapIncomeStatementOperationRevenueRanges.ToList();

                        decimal beginning = 0;
                        decimal actualFirstQuarter = 0;
                        decimal actualSecondQuarter = 0;
                        decimal actualThirdQuarter = 0;
                        decimal actualFourthQuarter = 0;

                        decimal planFirstQuarter = 0;
                        decimal planSecondQuarter = 0;
                        decimal planThirdQuarter = 0;
                        decimal planFourthQuarter = 0;

                        decimal percentageFirstQuarter = 0;
                        decimal percentageSecondQuarter = 0;
                        decimal percentageThirdQuarter = 0;
                        decimal percentageFourthQuarter = 0;

                        decimal actualTotal = 0;
                        decimal planTotal = 0;
                        decimal percentageTotal = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            firstMonthBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(thirdMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(firstMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFirstQuarter = actualFirstQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(sixthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(forthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualSecondQuarter = actualSecondQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(ninthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(seventhMonthBalance, range.AccountFrom, range.AccountTo);
                            actualThirdQuarter = actualThirdQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(twelvethMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(tenthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFourthQuarter = actualFourthQuarter + netChange;


                            #region Budget Modifed 1/25/18
                            planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            #endregion

                        }

                        percentageFirstQuarter = ((actualFirstQuarter - planFirstQuarter) / planFirstQuarter.DivideByZeroHelper()) * 100;
                        percentageSecondQuarter = ((actualSecondQuarter - planSecondQuarter) / planSecondQuarter.DivideByZeroHelper()) * 100;
                        percentageThirdQuarter = ((actualThirdQuarter - planThirdQuarter) / planThirdQuarter.DivideByZeroHelper()) * 100;
                        percentageFourthQuarter = ((actualFourthQuarter - planFourthQuarter) / planFourthQuarter.DivideByZeroHelper()) * 100;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var oprevenueRecord = new FinancialStatementQuarterlyVm
                        {
                            Description = revenuesBalance.Description,
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      Description = AssetBalance.,
                            First = actualFirstQuarter,
                            Second = actualSecondQuarter,
                            Third = actualThirdQuarter,
                            Fourth = actualFourthQuarter,

                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            PercentageTotal = percentageTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        operationRevenueList.Add(oprevenueRecord);
                    }
                    balance.AddRange(operationRevenueList);
                    #region total op revenue
                    var total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Operation Revenue",
                        PlanFirst = operationRevenueList.Sum(x=>x.PlanFirst),
                        PlanSecond = operationRevenueList.Sum(x => x.PlanSecond),
                        PlanThird = operationRevenueList.Sum(x => x.PlanThird),
                        PlanFourth = operationRevenueList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = operationRevenueList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = operationRevenueList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = operationRevenueList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = operationRevenueList.Sum(x => x.DiffrenceFourth),
                        First = operationRevenueList.Sum(x => x.First),
                        Second = operationRevenueList.Sum(x => x.Second),
                        Third = operationRevenueList.Sum(x => x.Third),
                        Fourth = operationRevenueList.Sum(x => x.Fourth),
                        ActualTotal = operationRevenueList.Sum(x => x.ActualTotal),
                        PlanTotal = operationRevenueList.Sum(x => x.PlanTotal),
                        PercentageTotal = operationRevenueList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader=true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region operational expense
                    #region operational expense header


                    var operationExpenseHeader = new FinancialStatementQuarterlyVm
                    {
                        Description = "Operation Expense",
                        IsHeader = true

                    };
                    balance.Add(operationExpenseHeader);

                    #endregion

                    var operationExpenses = expensesBalances.Where(x => x.Category == IncomeStatementExpenseCategory.OperationalExpense);

                    foreach (var expensesBalance in operationExpenses)
                    {

                        var ranges = expensesBalance.GaapIncomeStatementOperationExpenseRanges.ToList();
                        decimal beginning = 0;
                        decimal actualFirstQuarter = 0;
                        decimal actualSecondQuarter = 0;
                        decimal actualThirdQuarter = 0;
                        decimal actualFourthQuarter = 0;

                        decimal planFirstQuarter = 0;
                        decimal planSecondQuarter = 0;
                        decimal planThirdQuarter = 0;
                        decimal planFourthQuarter = 0;

                        decimal percentageFirstQuarter = 0;
                        decimal percentageSecondQuarter = 0;
                        decimal percentageThirdQuarter = 0;
                        decimal percentageFourthQuarter = 0;

                        decimal actualTotal = 0;
                        decimal planTotal = 0;
                        decimal percentageTotal = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            firstMonthBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(thirdMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(firstMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFirstQuarter = actualFirstQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(sixthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(forthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualSecondQuarter = actualSecondQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(ninthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(seventhMonthBalance, range.AccountFrom, range.AccountTo);
                            actualThirdQuarter = actualThirdQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(twelvethMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(tenthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFourthQuarter = actualFourthQuarter + netChange;


                            #region Budget Modifed 1/25/18
                            planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        percentageFirstQuarter = ((actualFirstQuarter - planFirstQuarter) / planFirstQuarter.DivideByZeroHelper()) * 100;
                        percentageSecondQuarter = ((actualSecondQuarter - planSecondQuarter) / planSecondQuarter.DivideByZeroHelper()) * 100;
                        percentageThirdQuarter = ((actualThirdQuarter - planThirdQuarter) / planThirdQuarter.DivideByZeroHelper()) * 100;
                        percentageFourthQuarter = ((actualFourthQuarter - planFourthQuarter) / planFourthQuarter.DivideByZeroHelper()) * 100;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var operationExpense=new FinancialStatementQuarterlyVm
                        {
                            Description = expensesBalance.Description,
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      Description = AssetBalance.,
                            First = actualFirstQuarter,
                            Second = actualSecondQuarter,
                            Third = actualThirdQuarter,
                            Fourth = actualFourthQuarter,

                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            PercentageTotal = percentageTotal,

                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        operationExpenseList.Add(operationExpense);
                    }

                    balance.AddRange(operationExpenseList);

                    #region total op expense
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Operation Expense",
                        PlanFirst = operationExpenseList.Sum(x => x.PlanFirst),
                        PlanSecond = operationExpenseList.Sum(x => x.PlanSecond),
                        PlanThird = operationExpenseList.Sum(x => x.PlanThird),
                        PlanFourth = operationExpenseList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = operationExpenseList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = operationExpenseList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = operationExpenseList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = operationExpenseList.Sum(x => x.DiffrenceFourth),
                        First = operationExpenseList.Sum(x => x.First),
                        Second = operationExpenseList.Sum(x => x.Second),
                        Third = operationExpenseList.Sum(x => x.Third),
                        Fourth = operationExpenseList.Sum(x => x.Fourth),
                        ActualTotal = operationExpenseList.Sum(x => x.ActualTotal),
                        PlanTotal = operationExpenseList.Sum(x => x.PlanTotal),
                        PercentageTotal = operationExpenseList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #endregion
                    #region gross profit
                    decimal gProfitPlanFirst = operationRevenueList.Sum(x => x.PlanFirst)- operationExpenseList.Sum(x => x.PlanFirst);
                    decimal gProfitPlanSecond = operationRevenueList.Sum(x => x.PlanSecond)- operationExpenseList.Sum(x => x.PlanSecond);
                    decimal gProfitPlanThird = operationRevenueList.Sum(x => x.PlanThird)- operationExpenseList.Sum(x => x.PlanThird);
                    decimal gProfitPlanFourth = operationRevenueList.Sum(x => x.PlanFourth)- operationExpenseList.Sum(x => x.PlanFourth);
                    decimal gProfitDiffrenceFirst = operationRevenueList.Sum(x => x.DiffrenceFirst)- operationExpenseList.Sum(x => x.DiffrenceFirst);
                    decimal gProfitDiffrenceSecond = operationRevenueList.Sum(x => x.DiffrenceSecond)- operationExpenseList.Sum(x => x.DiffrenceSecond);
                    decimal gProfitDiffrenceThird = operationRevenueList.Sum(x => x.DiffrenceThird)- operationExpenseList.Sum(x => x.DiffrenceThird);
                    decimal gProfitDiffrenceFourth = operationRevenueList.Sum(x => x.DiffrenceFourth) - operationExpenseList.Sum(x => x.DiffrenceFourth);
                    decimal gProfitFirst = operationRevenueList.Sum(x => x.First) - operationExpenseList.Sum(x => x.First);
                    decimal gProfitSecond = operationRevenueList.Sum(x => x.Second)- operationExpenseList.Sum(x => x.Second);
                    decimal gProfitThird = operationRevenueList.Sum(x => x.Third)- operationExpenseList.Sum(x => x.Third);
                    decimal gProfitFourth = operationRevenueList.Sum(x => x.Fourth)- operationExpenseList.Sum(x => x.Fourth);
                    decimal gProfitActualTotal = operationRevenueList.Sum(x => x.ActualTotal)- operationExpenseList.Sum(x => x.ActualTotal);
                    decimal gProfitPlanTotal = operationRevenueList.Sum(x => x.PlanTotal)- operationExpenseList.Sum(x => x.PlanTotal);
                    decimal gProfitPercentageTotal = operationRevenueList.Sum(x => x.PercentageTotal)- operationExpenseList.Sum(x => x.PercentageTotal);

                    #region total op revenue
                    var grossProfit = new FinancialStatementQuarterlyVm
                    {
                        Description = "Gross Profit",
                        PlanFirst = gProfitPlanFirst,
                        PlanSecond = gProfitPlanSecond,
                        PlanThird = gProfitPlanThird,
                        PlanFourth = gProfitPlanFourth,
                        DiffrenceFirst = gProfitDiffrenceFirst,
                        DiffrenceSecond = gProfitDiffrenceSecond,
                        DiffrenceThird = gProfitDiffrenceThird,
                        DiffrenceFourth = gProfitDiffrenceFourth,
                        First = gProfitFirst,
                        Second = gProfitSecond,
                        Third = gProfitThird,
                        Fourth = gProfitFourth,
                        ActualTotal = gProfitActualTotal,
                        PlanTotal = gProfitPlanTotal,
                        PercentageTotal = gProfitPercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(grossProfit);
                    #endregion
                    #region non operation

                    #region non operational revenue

                    #region operational expense header


                    var nonOperationRevenueHeader = new FinancialStatementQuarterlyVm
                    {
                        Description = "Non Operation Revenue",
                        IsHeader = true

                    };
                    balance.Add(nonOperationRevenueHeader);

                    #endregion

                    var nonOperationRevenues = revenuesBalances.Where(x => x.Category == IncomeStatementRevenueCategory.NonOperationalRevenue);

                    foreach (var revenuesBalance in nonOperationRevenues)
                    {
                        var ranges = revenuesBalance.GaapIncomeStatementOperationRevenueRanges.ToList();

                        decimal beginning = 0;
                        decimal actualFirstQuarter = 0;
                        decimal actualSecondQuarter = 0;
                        decimal actualThirdQuarter = 0;
                        decimal actualFourthQuarter = 0;

                        decimal planFirstQuarter = 0;
                        decimal planSecondQuarter = 0;
                        decimal planThirdQuarter = 0;
                        decimal planFourthQuarter = 0;

                        decimal percentageFirstQuarter = 0;
                        decimal percentageSecondQuarter = 0;
                        decimal percentageThirdQuarter = 0;
                        decimal percentageFourthQuarter = 0;

                        decimal actualTotal = 0;
                        decimal planTotal = 0;
                        decimal percentageTotal = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            firstMonthBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(thirdMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(firstMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFirstQuarter = actualFirstQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(sixthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(forthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualSecondQuarter = actualSecondQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(ninthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(seventhMonthBalance, range.AccountFrom, range.AccountTo);
                            actualThirdQuarter = actualThirdQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(twelvethMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(tenthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFourthQuarter = actualFourthQuarter + netChange;


                            #region Budget Modifed 1/25/18
                            planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            #endregion

                        }

                        percentageFirstQuarter = ((actualFirstQuarter - planFirstQuarter) / planFirstQuarter.DivideByZeroHelper()) * 100;
                        percentageSecondQuarter = ((actualSecondQuarter - planSecondQuarter) / planSecondQuarter.DivideByZeroHelper()) * 100;
                        percentageThirdQuarter = ((actualThirdQuarter - planThirdQuarter) / planThirdQuarter.DivideByZeroHelper()) * 100;
                        percentageFourthQuarter = ((actualFourthQuarter - planFourthQuarter) / planFourthQuarter.DivideByZeroHelper()) * 100;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var oprevenueRecord = new FinancialStatementQuarterlyVm
                        {
                            Description = revenuesBalance.Description,
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      Description = AssetBalance.,
                            First = actualFirstQuarter,
                            Second = actualSecondQuarter,
                            Third = actualThirdQuarter,
                            Fourth = actualFourthQuarter,

                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            PercentageTotal = percentageTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        nonOperationRevenueList.Add(oprevenueRecord);
                    }
                    balance.AddRange(nonOperationRevenueList);
                    #region total non op revenue
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Non Operation Revenue",
                        PlanFirst = nonOperationRevenueList.Sum(x => x.PlanFirst),
                        PlanSecond = nonOperationRevenueList.Sum(x => x.PlanSecond),
                        PlanThird = nonOperationRevenueList.Sum(x => x.PlanThird),
                        PlanFourth = nonOperationRevenueList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = nonOperationRevenueList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = nonOperationRevenueList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = nonOperationRevenueList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = nonOperationRevenueList.Sum(x => x.DiffrenceFourth),
                        First = nonOperationRevenueList.Sum(x => x.First),
                        Second = nonOperationRevenueList.Sum(x => x.Second),
                        Third = nonOperationRevenueList.Sum(x => x.Third),
                        Fourth = nonOperationRevenueList.Sum(x => x.Fourth),
                        ActualTotal = nonOperationRevenueList.Sum(x => x.ActualTotal),
                        PlanTotal = nonOperationRevenueList.Sum(x => x.PlanTotal),
                        PercentageTotal = nonOperationRevenueList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region non operational expense
                    #region operational expense header


                    var nonOperationExpenseHeader = new FinancialStatementQuarterlyVm
                    {
                        Description = "Non Operation Expense",
                        IsHeader = true

                    };
                    balance.Add(nonOperationExpenseHeader);

                    #endregion

                    var nonOperationExpenses = expensesBalances.Where(x => x.Category == IncomeStatementExpenseCategory.NonOperationalExpense);

                    foreach (var expensesBalance in nonOperationExpenses)
                    {

                        var ranges = expensesBalance.GaapIncomeStatementOperationExpenseRanges.ToList();
                        decimal beginning = 0;
                        decimal actualFirstQuarter = 0;
                        decimal actualSecondQuarter = 0;
                        decimal actualThirdQuarter = 0;
                        decimal actualFourthQuarter = 0;

                        decimal planFirstQuarter = 0;
                        decimal planSecondQuarter = 0;
                        decimal planThirdQuarter = 0;
                        decimal planFourthQuarter = 0;

                        decimal percentageFirstQuarter = 0;
                        decimal percentageSecondQuarter = 0;
                        decimal percentageThirdQuarter = 0;
                        decimal percentageFourthQuarter = 0;

                        decimal actualTotal = 0;
                        decimal planTotal = 0;
                        decimal percentageTotal = 0;

                        foreach (var range in ranges)
                        {
                            decimal netChange = 0;

                            beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                            firstMonthBalance, range.AccountFrom, range.AccountTo);

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(thirdMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(firstMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFirstQuarter = actualFirstQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(sixthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(forthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualSecondQuarter = actualSecondQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(ninthMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(seventhMonthBalance, range.AccountFrom, range.AccountTo);
                            actualThirdQuarter = actualThirdQuarter + netChange;

                            netChange = reportHelper.GetAccountEndBalancesWithAllranges(twelvethMonthBalance, range.AccountFrom, range.AccountTo)
                                        - reportHelper.GetAccountBegBalancesWithAllranges(tenthMonthBalance, range.AccountFrom, range.AccountTo);
                            actualFourthQuarter = actualFourthQuarter + netChange;


                            #region Budget Modifed 1/25/18
                            planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.AccountFrom, range.AccountTo);
                            #endregion
                        }

                        percentageFirstQuarter = ((actualFirstQuarter - planFirstQuarter) / planFirstQuarter.DivideByZeroHelper()) * 100;
                        percentageSecondQuarter = ((actualSecondQuarter - planSecondQuarter) / planSecondQuarter.DivideByZeroHelper()) * 100;
                        percentageThirdQuarter = ((actualThirdQuarter - planThirdQuarter) / planThirdQuarter.DivideByZeroHelper()) * 100;
                        percentageFourthQuarter = ((actualFourthQuarter - planFourthQuarter) / planFourthQuarter.DivideByZeroHelper()) * 100;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var operationExpense = new FinancialStatementQuarterlyVm
                        {
                            Description = expensesBalance.Description,
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      Description = AssetBalance.,
                            First = actualFirstQuarter,
                            Second = actualSecondQuarter,
                            Third = actualThirdQuarter,
                            Fourth = actualFourthQuarter,

                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            PercentageTotal = percentageTotal,

                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        nonOperationExpenseList.Add(operationExpense);
                    }

                    balance.AddRange(nonOperationExpenseList);

                    #region total op expense
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Non Operation Expense",

                        PlanFirst = nonOperationExpenseList.Sum(x => x.PlanFirst),
                        PlanSecond = nonOperationExpenseList.Sum(x => x.PlanSecond),
                        PlanThird = nonOperationExpenseList.Sum(x => x.PlanThird),
                        PlanFourth = nonOperationExpenseList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = nonOperationExpenseList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = nonOperationExpenseList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = nonOperationExpenseList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = nonOperationExpenseList.Sum(x => x.DiffrenceFourth),
                        First = nonOperationExpenseList.Sum(x => x.First),
                        Second = nonOperationExpenseList.Sum(x => x.Second),
                        Third = nonOperationExpenseList.Sum(x => x.Third),
                        Fourth = nonOperationExpenseList.Sum(x => x.Fourth),
                        ActualTotal = nonOperationExpenseList.Sum(x => x.ActualTotal),
                        PlanTotal = nonOperationExpenseList.Sum(x => x.PlanTotal),
                        PercentageTotal = nonOperationExpenseList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #endregion

                    #region gtaxable inclome
                    decimal taxableIncomePlanFirst =gProfitPlanFirst+ nonOperationRevenueList.Sum(x => x.PlanFirst) - nonOperationExpenseList.Sum(x => x.PlanFirst);
                    decimal taxableIncomePlanSecond =gProfitPlanSecond+ nonOperationRevenueList.Sum(x => x.PlanSecond) - nonOperationExpenseList.Sum(x => x.PlanSecond);
                    decimal taxableIncomePlanThird = gProfitPlanThird+nonOperationRevenueList.Sum(x => x.PlanThird) - nonOperationExpenseList.Sum(x => x.PlanThird);
                    decimal taxableIncomePlanFourth =gProfitPlanFourth+ nonOperationRevenueList.Sum(x => x.PlanFourth) - nonOperationExpenseList.Sum(x => x.PlanFourth);
                    decimal taxableIncomeDiffrenceFirst =gProfitDiffrenceFirst+ nonOperationRevenueList.Sum(x => x.DiffrenceFirst) - nonOperationExpenseList.Sum(x => x.DiffrenceFirst);
                    decimal taxableIncomeDiffrenceSecond = gProfitDiffrenceSecond + nonOperationRevenueList.Sum(x => x.DiffrenceSecond) - nonOperationExpenseList.Sum(x => x.DiffrenceSecond);
                    decimal taxableIncomeDiffrenceThird = gProfitDiffrenceThird + nonOperationRevenueList.Sum(x => x.DiffrenceThird) - nonOperationExpenseList.Sum(x => x.DiffrenceThird);
                    decimal taxableIncomeDiffrenceFourth = gProfitDiffrenceFourth + nonOperationRevenueList.Sum(x => x.DiffrenceFourth) - nonOperationExpenseList.Sum(x => x.DiffrenceFourth);
                    decimal taxableIncomeFirst = gProfitFirst+nonOperationRevenueList.Sum(x => x.First) - nonOperationExpenseList.Sum(x => x.First);
                    decimal taxableIncomeSecond = gProfitSecond + nonOperationRevenueList.Sum(x => x.Second) - nonOperationExpenseList.Sum(x => x.Second);
                    decimal taxableIncomeThird = gProfitThird + nonOperationRevenueList.Sum(x => x.Third) - nonOperationExpenseList.Sum(x => x.Third);
                    decimal taxableIncomeFourth = gProfitFourth + nonOperationRevenueList.Sum(x => x.Fourth) - nonOperationExpenseList.Sum(x => x.Fourth);
                    decimal taxableIncomeActualTotal = gProfitActualTotal + nonOperationRevenueList.Sum(x => x.ActualTotal) - nonOperationExpenseList.Sum(x => x.ActualTotal);
                    decimal taxableIncomePlanTotal = gProfitPlanTotal + nonOperationRevenueList.Sum(x => x.PlanTotal) - nonOperationExpenseList.Sum(x => x.PlanTotal);
                    decimal taxableIncomePercentageTotal = gProfitPercentageTotal + nonOperationRevenueList.Sum(x => x.PercentageTotal) - nonOperationExpenseList.Sum(x => x.PercentageTotal);

                    #region taxable income
                    var taxableIncome = new FinancialStatementQuarterlyVm
                    {
                        Description = "Taxable Income",
                        PlanFirst = taxableIncomePlanFirst,
                        PlanSecond = taxableIncomePlanSecond,
                        PlanThird = taxableIncomePlanThird,
                        PlanFourth = taxableIncomePlanFourth,
                        DiffrenceFirst = taxableIncomeDiffrenceFirst,
                        DiffrenceSecond = taxableIncomeDiffrenceSecond,
                        DiffrenceThird = taxableIncomeDiffrenceThird,
                        DiffrenceFourth = taxableIncomeDiffrenceFourth,
                        First = taxableIncomeFirst,
                        Second = taxableIncomeSecond,
                        Third = taxableIncomeThird,
                        Fourth = taxableIncomeFourth,
                        ActualTotal = taxableIncomeActualTotal,
                        PlanTotal = taxableIncomePlanTotal,
                        PercentageTotal = taxableIncomePercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(taxableIncome);
                    #endregion

                    #region profit tax

                    var profittaxRate = Db.FinanceSettings.FirstOrDefault();
                    decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;

                    #region profit tax
                    decimal profitTaxPlanFirst = taxableIncomePlanFirst* rate;
                    decimal profitTaxPlanSecond = taxableIncomePlanSecond * rate;
                    decimal profitTaxPlanThird = taxableIncomePlanThird * rate;
                    decimal profitTaxPlanFourth = taxableIncomePlanFourth * rate;
                    decimal profitTaxDiffrenceFirst = taxableIncomeDiffrenceFirst * rate;
                    decimal profitTaxDiffrenceSecond = taxableIncomeDiffrenceSecond * rate;
                    decimal profitTaxDiffrenceThird = taxableIncomeDiffrenceThird * rate;
                    decimal profitTaxDiffrenceFourth = taxableIncomeDiffrenceFourth * rate;
                    decimal profitTaxFirst = taxableIncomeFirst * rate;
                    decimal profitTaxSecond = taxableIncomeSecond * rate;
                    decimal profitTaxThird = taxableIncomeThird * rate;
                    decimal profitTaxFourth = taxableIncomeFourth * rate;
                    decimal profitTaxActualTotal = taxableIncomeActualTotal * rate;
                    decimal profitTaxPlanTotal = taxableIncomePlanTotal * rate;
                    decimal profitTaxPercentageTotal = taxableIncomePercentageTotal * rate;

                    var profittax = new FinancialStatementQuarterlyVm
                    {
                        Description = "Profit Tax",
                        PlanFirst = profitTaxPlanFirst,
                        PlanSecond = profitTaxPlanSecond,
                        PlanThird = profitTaxPlanThird,
                        PlanFourth = profitTaxPlanFourth,
                        DiffrenceFirst = profitTaxDiffrenceFirst,
                        DiffrenceSecond = profitTaxDiffrenceSecond,
                        DiffrenceThird = profitTaxDiffrenceThird,
                        DiffrenceFourth = profitTaxDiffrenceFourth,
                        First = profitTaxFirst,
                        Second = profitTaxSecond,
                        Third = profitTaxThird,
                        Fourth = profitTaxFourth,
                        ActualTotal = profitTaxActualTotal,
                        PlanTotal = profitTaxPlanTotal,
                        PercentageTotal = profitTaxPercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(profittax);
                    #endregion

                    #region profit after tax

                    #region profit after tax
                    decimal profitAfterTaxPlanFirst = taxableIncomePlanFirst -profitTaxPlanFirst;
                    decimal profitAfterTaxPlanSecond = taxableIncomePlanSecond - profitTaxPlanSecond;
                    decimal profitAfterTaxPlanThird = taxableIncomePlanThird - profitTaxPlanThird;
                    decimal profitAfterTaxPlanFourth = taxableIncomePlanFourth - profitTaxPlanFourth;
                    decimal profitAfterTaxDiffrenceFirst = taxableIncomeDiffrenceFirst - profitTaxDiffrenceFirst;
                    decimal profitAfterTaxDiffrenceSecond = taxableIncomeDiffrenceSecond - profitTaxDiffrenceSecond;
                    decimal profitAfterTaxDiffrenceThird = taxableIncomeDiffrenceThird - profitTaxDiffrenceThird;
                    decimal profitAfterTaxDiffrenceFourth = taxableIncomeDiffrenceFourth - profitTaxDiffrenceFourth;
                    decimal profitAfterTaxFirst = taxableIncomeFirst - profitTaxFirst;
                    decimal profitAfterTaxSecond = taxableIncomeSecond - profitTaxSecond;
                    decimal profitAfterTaxThird = taxableIncomeThird - profitTaxThird;
                    decimal profitAfterTaxFourth = taxableIncomeFourth - profitTaxFourth;
                    decimal profitAfterTaxActualTotal = taxableIncomeActualTotal - profitTaxActualTotal;
                    decimal profitAfterTaxPlanTotal = taxableIncomePlanTotal - profitTaxPlanTotal;
                    decimal profitAfterTaxPercentageTotal = taxableIncomePercentageTotal - profitTaxPercentageTotal;

                    var profitaftertax = new FinancialStatementQuarterlyVm
                    {
                        Description = "Profit After Tax",
                        PlanFirst = profitAfterTaxPlanFirst,
                        PlanSecond = profitAfterTaxPlanSecond,
                        PlanThird = profitAfterTaxPlanThird,
                        PlanFourth = profitAfterTaxPlanFourth,
                        DiffrenceFirst = profitAfterTaxDiffrenceFirst,
                        DiffrenceSecond = profitAfterTaxDiffrenceSecond,
                        DiffrenceThird = profitAfterTaxDiffrenceThird,
                        DiffrenceFourth = profitAfterTaxDiffrenceFourth,
                        First = profitAfterTaxFirst,
                        Second = profitAfterTaxSecond,
                        Third = profitAfterTaxThird,
                        Fourth = profitAfterTaxFourth,
                        ActualTotal = profitAfterTaxActualTotal,
                        PlanTotal = profitAfterTaxPlanTotal,
                        PercentageTotal = profitAfterTaxPercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
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