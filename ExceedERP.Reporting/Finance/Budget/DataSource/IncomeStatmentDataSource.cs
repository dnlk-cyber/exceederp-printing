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
    public class IncomeStatmentDataSource : List<FinancialStatementQuarterlyVm>
    {
        public ExceedDbContext Db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementQuarterlyVm> GetIncomeStatmentDataSource(string year, string periodFrom, string periodTo)
        {
            ReportHelper reportHelper = new ReportHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

            var balance = new List<FinancialStatementQuarterlyVm>();
            var operationIncomeList = new List<FinancialStatementQuarterlyVm>();
            var otherIncomeList = new List<FinancialStatementQuarterlyVm>();
            var costofGoodSoldExpenseList = new List<FinancialStatementQuarterlyVm>();
            var sellingAndDistributionExpenseList = new List<FinancialStatementQuarterlyVm>();
            var administrativeAndGeneralExpenseList = new List<FinancialStatementQuarterlyVm>();

            if (!string.IsNullOrWhiteSpace(year))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var gaapIncomeStatement = Db.GaapIncomeStatements.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPTwo);

                if (gaapIncomeStatement != null && fiscalYear != null)
                {
                    int fiscalyearId = fiscalYear.GlFiscalYearId;
                    int firstPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(fiscalyearId);
                    int lastPeriodId= _fiscalYearHelper.GetFiscalYearLastPeriod(fiscalyearId);

                    if (!string.IsNullOrWhiteSpace(periodFrom) && !string.IsNullOrWhiteSpace(periodTo))
                    {
                        firstPeriodId=int.Parse(periodFrom);
                        lastPeriodId = int.Parse(periodTo);
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

                    var allBalnces = reportHelper.GetAccountBalanceByPeriodRange(fiscalyearId, firstPeriodId, lastPeriodId);

                    

                    var firstMonthBalance = allBalnces.Where(x => x.GLPeriodId == firstPeriod).ToList();
                    var thirdMonthBalance = allBalnces.Where(x => x.GLPeriodId == thirdPeriod).ToList();

                    var forthMonthBalance = allBalnces.Where(x => x.GLPeriodId == fourthPeriod).ToList();
                    var sixthMonthBalance = allBalnces.Where(x => x.GLPeriodId == sixthPeriod).ToList();

                    var seventhMonthBalance = allBalnces.Where(x => x.GLPeriodId == seventhPeriod).ToList();
                    var ninthMonthBalance = allBalnces.Where(x => x.GLPeriodId == ninthPeriod).ToList();

                    var tenthMonthBalance = allBalnces.Where(x => x.GLPeriodId == tenthPeriod).ToList();
                    var twelvethMonthBalance = allBalnces.Where(x => x.GLPeriodId == twelvethPeriod).ToList();



                    #region Budget Modifed 1/25/18
                    var periodRangeBalnce = budgetReportHelper.GetPeriodRangePlanBalance(fiscalyearId, firstPeriodId, lastPeriodId);
                    var firstQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.First);
                    var secondQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Second);
                    var thirdQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Third);
                    var fourthQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Fourth);
                    
                    decimal PlanDiffFirst = 0;
                    decimal PlanDiffSecond = 0;
                    decimal PlanDiffThird = 0;
                    decimal PlanDiffFourth = 0;
                    #endregion


                    #region operation
                    #region operational income header


                    var operatingIncomeHedaer = new FinancialStatementQuarterlyVm
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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var opIncomeRecord = new FinancialStatementQuarterlyVm
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
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        operationIncomeList.Add(opIncomeRecord);
                    }
                    balance.AddRange(operationIncomeList);
                    #region total op revenue
                    var total = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL OPERATING INCOME",
                        PlanFirst = operationIncomeList.Sum(x => x.PlanFirst),
                        PlanSecond = operationIncomeList.Sum(x => x.PlanSecond),
                        PlanThird = operationIncomeList.Sum(x => x.PlanThird),
                        PlanFourth = operationIncomeList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = operationIncomeList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = operationIncomeList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = operationIncomeList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = operationIncomeList.Sum(x => x.DiffrenceFourth),
                        First = operationIncomeList.Sum(x => x.First),
                        Second = operationIncomeList.Sum(x => x.Second),
                        Third = operationIncomeList.Sum(x => x.Third),
                        Fourth = operationIncomeList.Sum(x => x.Fourth),
                        ActualTotal = operationIncomeList.Sum(x => x.ActualTotal),
                        PlanTotal = operationIncomeList.Sum(x => x.PlanTotal),
                        PercentageTotal = operationIncomeList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region cost of service sold
                    #region cost of service sold header


                    var costofGoodSoldExpenseHeader = new FinancialStatementQuarterlyVm
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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var costOfGoodSoldExpense = new FinancialStatementQuarterlyVm
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
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        costofGoodSoldExpenseList.Add(costOfGoodSoldExpense);
                    }

                    balance.AddRange(costofGoodSoldExpenseList);

                    #region total Cost of Service Sold
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL COST OF SERVICE SOLD",
                        PlanFirst = costofGoodSoldExpenseList.Sum(x => x.PlanFirst),
                        PlanSecond = costofGoodSoldExpenseList.Sum(x => x.PlanSecond),
                        PlanThird = costofGoodSoldExpenseList.Sum(x => x.PlanThird),
                        PlanFourth = costofGoodSoldExpenseList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = costofGoodSoldExpenseList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = costofGoodSoldExpenseList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = costofGoodSoldExpenseList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = costofGoodSoldExpenseList.Sum(x => x.DiffrenceFourth),
                        First = costofGoodSoldExpenseList.Sum(x => x.First),
                        Second = costofGoodSoldExpenseList.Sum(x => x.Second),
                        Third = costofGoodSoldExpenseList.Sum(x => x.Third),
                        Fourth = costofGoodSoldExpenseList.Sum(x => x.Fourth),
                        ActualTotal = costofGoodSoldExpenseList.Sum(x => x.ActualTotal),
                        PlanTotal = costofGoodSoldExpenseList.Sum(x => x.PlanTotal),
                        PercentageTotal = costofGoodSoldExpenseList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
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
                    decimal gProfitPlanFirst = operationIncomeList.Sum(x => x.PlanFirst) - costofGoodSoldExpenseList.Sum(x => x.PlanFirst);
                    decimal gProfitPlanSecond = operationIncomeList.Sum(x => x.PlanSecond) - costofGoodSoldExpenseList.Sum(x => x.PlanSecond);
                    decimal gProfitPlanThird = operationIncomeList.Sum(x => x.PlanThird) - costofGoodSoldExpenseList.Sum(x => x.PlanThird);
                    decimal gProfitPlanFourth = operationIncomeList.Sum(x => x.PlanFourth) - costofGoodSoldExpenseList.Sum(x => x.PlanFourth);
                    decimal gProfitDiffrenceFirst = operationIncomeList.Sum(x => x.DiffrenceFirst) - costofGoodSoldExpenseList.Sum(x => x.DiffrenceFirst);
                    decimal gProfitDiffrenceSecond = operationIncomeList.Sum(x => x.DiffrenceSecond) - costofGoodSoldExpenseList.Sum(x => x.DiffrenceSecond);
                    decimal gProfitDiffrenceThird = operationIncomeList.Sum(x => x.DiffrenceThird) - costofGoodSoldExpenseList.Sum(x => x.DiffrenceThird);
                    decimal gProfitDiffrenceFourth = operationIncomeList.Sum(x => x.DiffrenceFourth) - costofGoodSoldExpenseList.Sum(x => x.DiffrenceFourth);
                    decimal gProfitFirst = operationIncomeList.Sum(x => x.First) - costofGoodSoldExpenseList.Sum(x => x.First);
                    decimal gProfitSecond = operationIncomeList.Sum(x => x.Second) - costofGoodSoldExpenseList.Sum(x => x.Second);
                    decimal gProfitThird = operationIncomeList.Sum(x => x.Third) - costofGoodSoldExpenseList.Sum(x => x.Third);
                    decimal gProfitFourth = operationIncomeList.Sum(x => x.Fourth) - costofGoodSoldExpenseList.Sum(x => x.Fourth);
                    decimal gProfitActualTotal = operationIncomeList.Sum(x => x.ActualTotal) - costofGoodSoldExpenseList.Sum(x => x.ActualTotal);
                    decimal gProfitPlanTotal = operationIncomeList.Sum(x => x.PlanTotal) - costofGoodSoldExpenseList.Sum(x => x.PlanTotal);
                    decimal gProfitPercentageTotal = operationIncomeList.Sum(x => x.PercentageTotal) - costofGoodSoldExpenseList.Sum(x => x.PercentageTotal);

                    #region total op revenue
                    var grossProfit = new FinancialStatementQuarterlyVm
                    {
                        Description = "GROSS PROFIT",
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
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(grossProfit);
                    #endregion

                    #region selling and dist expense
                    #region selling and dist header


                    var sellingAndDistExpenseHeader = new FinancialStatementQuarterlyVm
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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var sellingAndDistExpense = new FinancialStatementQuarterlyVm
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
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        sellingAndDistributionExpenseList.Add(sellingAndDistExpense);
                    }

                    balance.AddRange(sellingAndDistributionExpenseList);

                    #region total selling and dist expense
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL SELLING AND DISTRIBUTION EXPENSE",
                        PlanFirst = sellingAndDistributionExpenseList.Sum(x => x.PlanFirst),
                        PlanSecond = sellingAndDistributionExpenseList.Sum(x => x.PlanSecond),
                        PlanThird = sellingAndDistributionExpenseList.Sum(x => x.PlanThird),
                        PlanFourth = sellingAndDistributionExpenseList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceFourth),
                        First = sellingAndDistributionExpenseList.Sum(x => x.First),
                        Second = sellingAndDistributionExpenseList.Sum(x => x.Second),
                        Third = sellingAndDistributionExpenseList.Sum(x => x.Third),
                        Fourth = sellingAndDistributionExpenseList.Sum(x => x.Fourth),
                        ActualTotal = sellingAndDistributionExpenseList.Sum(x => x.ActualTotal),
                        PlanTotal = sellingAndDistributionExpenseList.Sum(x => x.PlanTotal),
                        PercentageTotal = sellingAndDistributionExpenseList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region administrative and general expense
                    #region administrative and general header


                    var adminAndGeneralExpenseHeader = new FinancialStatementQuarterlyVm
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



                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var adminAndGeneralExpense = new FinancialStatementQuarterlyVm
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
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        administrativeAndGeneralExpenseList.Add(adminAndGeneralExpense);
                    }

                    balance.AddRange(administrativeAndGeneralExpenseList);

                    #region total admin and general expense
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL ADMINISTRATION AND GENERAL EXPENSE",

                        PlanFirst = administrativeAndGeneralExpenseList.Sum(x => x.PlanFirst),
                        PlanSecond = administrativeAndGeneralExpenseList.Sum(x => x.PlanSecond),
                        PlanThird = administrativeAndGeneralExpenseList.Sum(x => x.PlanThird),
                        PlanFourth = administrativeAndGeneralExpenseList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceFourth),
                        First = administrativeAndGeneralExpenseList.Sum(x => x.First),
                        Second = administrativeAndGeneralExpenseList.Sum(x => x.Second),
                        Third = administrativeAndGeneralExpenseList.Sum(x => x.Third),
                        Fourth = administrativeAndGeneralExpenseList.Sum(x => x.Fourth),
                        ActualTotal = administrativeAndGeneralExpenseList.Sum(x => x.ActualTotal),
                        PlanTotal = administrativeAndGeneralExpenseList.Sum(x => x.PlanTotal),
                        PercentageTotal = administrativeAndGeneralExpenseList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region total operating expense

                    decimal totalOperatingexpensePlanFirst = sellingAndDistributionExpenseList.Sum(x => x.PlanFirst) + administrativeAndGeneralExpenseList.Sum(x => x.PlanFirst);
                    decimal totalOperatingexpensePlanSecond = sellingAndDistributionExpenseList.Sum(x => x.PlanSecond) + administrativeAndGeneralExpenseList.Sum(x => x.PlanSecond);
                    decimal totalOperatingexpensePlanThird = sellingAndDistributionExpenseList.Sum(x => x.PlanThird) + administrativeAndGeneralExpenseList.Sum(x => x.PlanThird);
                    decimal totalOperatingexpensePlanFourth = sellingAndDistributionExpenseList.Sum(x => x.PlanFourth) + administrativeAndGeneralExpenseList.Sum(x => x.PlanFourth);
                    decimal totalOperatingexpenseDiffrenceFirst = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceFirst) + administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceFirst);
                    decimal totalOperatingexpenseDiffrenceSecond = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceSecond) + administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceSecond);
                    decimal totalOperatingexpenseDiffrenceThird = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceThird) + administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceThird);
                    decimal totalOperatingexpenseDiffrenceFourth = sellingAndDistributionExpenseList.Sum(x => x.DiffrenceFourth) + administrativeAndGeneralExpenseList.Sum(x => x.DiffrenceFourth);
                    decimal totalOperatingexpenseFirst = sellingAndDistributionExpenseList.Sum(x => x.First) + administrativeAndGeneralExpenseList.Sum(x => x.First);
                    decimal totalOperatingexpenseSecond = sellingAndDistributionExpenseList.Sum(x => x.Second) + administrativeAndGeneralExpenseList.Sum(x => x.Second);
                    decimal totalOperatingexpenseThird = sellingAndDistributionExpenseList.Sum(x => x.Third) + administrativeAndGeneralExpenseList.Sum(x => x.Third);
                    decimal totalOperatingexpenseFourth = sellingAndDistributionExpenseList.Sum(x => x.Fourth) + administrativeAndGeneralExpenseList.Sum(x => x.Fourth);
                    decimal totalOperatingexpenseActualTotal = sellingAndDistributionExpenseList.Sum(x => x.ActualTotal) + administrativeAndGeneralExpenseList.Sum(x => x.ActualTotal);
                    decimal totalOperatingexpensePlanTotal = sellingAndDistributionExpenseList.Sum(x => x.PlanTotal) + administrativeAndGeneralExpenseList.Sum(x => x.PlanTotal);
                    decimal totalOperatingexpensePercentageTotal = sellingAndDistributionExpenseList.Sum(x => x.PercentageTotal) + administrativeAndGeneralExpenseList.Sum(x => x.PercentageTotal);

                    #region total operating expense
                    var totalOperatingexpense = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL OPERATING EXPENSE",
                        PlanFirst = totalOperatingexpensePlanFirst,
                        PlanSecond = totalOperatingexpensePlanSecond,
                        PlanThird = totalOperatingexpensePlanThird,
                        PlanFourth = totalOperatingexpensePlanFourth,
                        DiffrenceFirst = totalOperatingexpenseDiffrenceFirst,
                        DiffrenceSecond = totalOperatingexpenseDiffrenceSecond,
                        DiffrenceThird = totalOperatingexpenseDiffrenceThird,
                        DiffrenceFourth = totalOperatingexpenseDiffrenceFourth,
                        First = totalOperatingexpenseFirst,
                        Second = totalOperatingexpenseSecond,
                        Third = totalOperatingexpenseThird,
                        Fourth = totalOperatingexpenseFourth,
                        ActualTotal = totalOperatingexpenseActualTotal,
                        PlanTotal = totalOperatingexpensePlanTotal,
                        PercentageTotal = totalOperatingexpensePercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(totalOperatingexpense);
                    #endregion

                    #region total operating income/loss

                    decimal totalOperatingInComeorLossPlanFirst = gProfitPlanFirst - totalOperatingexpensePlanFirst;
                    decimal totalOperatingInComeorLossPlanSecond = gProfitPlanSecond - totalOperatingexpensePlanSecond;
                    decimal totalOperatingInComeorLossPlanThird = gProfitPlanThird - totalOperatingexpensePlanThird;
                    decimal totalOperatingInComeorLossPlanFourth = gProfitPlanFourth - totalOperatingexpensePlanFourth;
                    decimal totalOperatingInComeorLossDiffrenceFirst = gProfitDiffrenceFirst - totalOperatingexpenseDiffrenceFirst;
                    decimal totalOperatingInComeorLossDiffrenceSecond = gProfitDiffrenceSecond - totalOperatingexpenseDiffrenceSecond;
                    decimal totalOperatingInComeorLossDiffrenceThird = gProfitDiffrenceThird - totalOperatingexpenseDiffrenceThird;
                    decimal totalOperatingInComeorLossDiffrenceFourth = gProfitDiffrenceFourth - totalOperatingexpenseDiffrenceFourth;
                    decimal totalOperatingInComeorLossFirst = gProfitFirst - totalOperatingexpenseFirst;
                    decimal totalOperatingInComeorLossSecond = gProfitSecond - totalOperatingexpenseSecond;
                    decimal totalOperatingInComeorLossThird = gProfitThird - totalOperatingexpenseThird;
                    decimal totalOperatingInComeorLossFourth = gProfitFourth - totalOperatingexpenseFourth;
                    decimal totalOperatingInComeorLossActualTotal = gProfitActualTotal - totalOperatingexpenseActualTotal;
                    decimal totalOperatingInComeorLossPlanTotal = gProfitPlanTotal - totalOperatingexpensePlanTotal;
                    decimal totalOperatingInComeorLossPercentageTotal = gProfitPercentageTotal - totalOperatingexpensePercentageTotal;

                    #region operating income/loss
                    var totalOperatingincomeorloss = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL OPERATING INCOME/LOSS",
                        PlanFirst = totalOperatingInComeorLossPlanFirst,
                        PlanSecond = totalOperatingInComeorLossPlanSecond,
                        PlanThird = totalOperatingInComeorLossPlanThird,
                        PlanFourth = totalOperatingInComeorLossPlanFourth,
                        DiffrenceFirst = totalOperatingInComeorLossDiffrenceFirst,
                        DiffrenceSecond = totalOperatingInComeorLossDiffrenceSecond,
                        DiffrenceThird = totalOperatingInComeorLossDiffrenceThird,
                        DiffrenceFourth = totalOperatingInComeorLossDiffrenceFourth,
                        First = totalOperatingInComeorLossFirst,
                        Second = totalOperatingInComeorLossSecond,
                        Third = totalOperatingInComeorLossThird,
                        Fourth = totalOperatingInComeorLossFourth,
                        ActualTotal = totalOperatingInComeorLossActualTotal,
                        PlanTotal = totalOperatingInComeorLossPlanTotal,
                        PercentageTotal = totalOperatingInComeorLossPercentageTotal,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(totalOperatingincomeorloss);
                    #endregion

                    #region other income

                    #region other income header


                    var otherIncomeHeader = new FinancialStatementQuarterlyVm
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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var otherIncomeRecord = new FinancialStatementQuarterlyVm
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
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        otherIncomeList.Add(otherIncomeRecord);
                    }
                    balance.AddRange(otherIncomeList);
                    #region total other income
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL OTHER INCOME",
                        PlanFirst = otherIncomeList.Sum(x => x.PlanFirst),
                        PlanSecond = otherIncomeList.Sum(x => x.PlanSecond),
                        PlanThird = otherIncomeList.Sum(x => x.PlanThird),
                        PlanFourth = otherIncomeList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = otherIncomeList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = otherIncomeList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = otherIncomeList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = otherIncomeList.Sum(x => x.DiffrenceFourth),
                        First = otherIncomeList.Sum(x => x.First),
                        Second = otherIncomeList.Sum(x => x.Second),
                        Third = otherIncomeList.Sum(x => x.Third),
                        Fourth = otherIncomeList.Sum(x => x.Fourth),
                        ActualTotal = otherIncomeList.Sum(x => x.ActualTotal),
                        PlanTotal = otherIncomeList.Sum(x => x.PlanTotal),
                        PercentageTotal = otherIncomeList.Sum(x => x.PercentageTotal),
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                        IsHeader = true
                    };
                    #endregion

                    balance.Add(total);

                    #endregion

                    #region total operating income before tax
                    decimal taxableIncomePlanFirst = totalOperatingInComeorLossPlanFirst + otherIncomeList.Sum(x => x.PlanFirst);
                    decimal taxableIncomePlanSecond = totalOperatingInComeorLossPlanSecond + otherIncomeList.Sum(x => x.PlanSecond);
                    decimal taxableIncomePlanThird = totalOperatingInComeorLossPlanThird + otherIncomeList.Sum(x => x.PlanThird);
                    decimal taxableIncomePlanFourth = totalOperatingInComeorLossPlanFourth + otherIncomeList.Sum(x => x.PlanFourth);
                    decimal taxableIncomeDiffrenceFirst = totalOperatingInComeorLossDiffrenceFirst + otherIncomeList.Sum(x => x.DiffrenceFirst);
                    decimal taxableIncomeDiffrenceSecond = totalOperatingInComeorLossDiffrenceSecond + otherIncomeList.Sum(x => x.DiffrenceSecond);
                    decimal taxableIncomeDiffrenceThird = totalOperatingInComeorLossDiffrenceThird + otherIncomeList.Sum(x => x.DiffrenceThird);
                    decimal taxableIncomeDiffrenceFourth = totalOperatingInComeorLossDiffrenceFourth + otherIncomeList.Sum(x => x.DiffrenceFourth);
                    decimal taxableIncomeFirst = totalOperatingInComeorLossFirst + otherIncomeList.Sum(x => x.First);
                    decimal taxableIncomeSecond = totalOperatingInComeorLossSecond + otherIncomeList.Sum(x => x.Second);
                    decimal taxableIncomeThird = totalOperatingInComeorLossThird + otherIncomeList.Sum(x => x.Third);
                    decimal taxableIncomeFourth = totalOperatingInComeorLossFourth + otherIncomeList.Sum(x => x.Fourth);
                    decimal taxableIncomeActualTotal = totalOperatingInComeorLossActualTotal + otherIncomeList.Sum(x => x.ActualTotal);
                    decimal taxableIncomePlanTotal = totalOperatingInComeorLossPlanTotal + otherIncomeList.Sum(x => x.PlanTotal);
                    decimal taxableIncomePercentageTotal = totalOperatingInComeorLossPercentageTotal + otherIncomeList.Sum(x => x.PercentageTotal);

                    #region taxable income
                    var taxableIncome = new FinancialStatementQuarterlyVm
                    {
                        Description = "TOTAL OPERATING INCOME/LOSS BEFORE INCOME TAX",
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
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
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


                    if(profittaxRate!= null)
                    {
                           rate = profittaxRate.ProfitTaxCalculatedRate;

                    }
                   
                    #region profit tax
                    decimal profitTaxPlanFirst = taxableIncomePlanFirst * rate;
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
                        Description = "PROFIT TAX",
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
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
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
                    decimal profitAfterTaxPlanFirst = taxableIncomePlanFirst - profitTaxPlanFirst;
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
                        Description = "NET INCOME AFTER TAX",
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
                        PlanDiffFirst = PlanDiffFirst,
                        PlanDiffSecond = PlanDiffSecond,
                        PlanDiffThird = PlanDiffThird,
                        PlanDiffFourth = PlanDiffFourth,
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