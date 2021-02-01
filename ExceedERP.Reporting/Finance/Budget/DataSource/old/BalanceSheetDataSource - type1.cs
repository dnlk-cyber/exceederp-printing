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

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BalanceSheetGaapOne : List<BalanceSheetViewModel>
    {
        public ExceedDbContext Db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementQuarterlyVm> GetBalanceSheets(string year)
        {
            var balance = new List<FinancialStatementQuarterlyVm>();
            var nonCurrentAssetList = new List<FinancialStatementQuarterlyVm>();
            var currentAssetList = new List<FinancialStatementQuarterlyVm>();
            var capitalList = new List<FinancialStatementQuarterlyVm>();
            var nonCurrentLiabilityList = new List<FinancialStatementQuarterlyVm>();
            var currentLiabilityList = new List<FinancialStatementQuarterlyVm>();
            ReportHelper reportHelper = new ReportHelper();
            ClosingHelper closingHelper = new ClosingHelper();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();


            if (!string.IsNullOrWhiteSpace(year))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var balanceSheet = Db.GapBalanceSheets.FirstOrDefault();

                if (balanceSheet != null && fiscalYear != null)
                {
                    int fiscalyearid = fiscalYear.GlFiscalYearId;
                    #region update earning  ledger--current year

                    closingHelper.UpdateEarningForAYear(fiscalyearid);

                    #endregion

                    var assetBalances = balanceSheet.Assets.ToList();
                    var capitalsBalances = balanceSheet.Capitals.ToList();
                    var liabilitiesBalances = balanceSheet.Liabilities.ToList();

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

                    #region asset 
                    #region asset header


                    var assetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "ASSETS",
                        IsHeader=true

                    };
                    balance.Add(assetHedaer);

                    #endregion

                    #region non current asset header
                    var nonCurrentAssetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Non Current Assets ",
                        IsHeader = true
                    };
                    balance.Add(nonCurrentAssetHedaer);
                   

                    #endregion
                    #region non current asset

                    var nonCurrentAssets = assetBalances.Where(x => x.Category == GaapAssetTypes.NonCurrentAsset);

                    foreach (var assetBalance in nonCurrentAssets)
                    {
                        var ranges = assetBalance.GaapBalanceSheetAssetRanges.ToList();
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


                      
                        var noncurrentasset=new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
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

                            PercentageTotal = percentageTotal,
                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        nonCurrentAssetList.Add(noncurrentasset);
                    }
                    balance.AddRange(nonCurrentAssetList);
                    #region total non current asset
                    var total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Non Current Assets ",
                        PlanFirst = nonCurrentAssetList.Sum(x => x.PlanFirst),
                        PlanSecond = nonCurrentAssetList.Sum(x => x.PlanSecond),
                        PlanThird = nonCurrentAssetList.Sum(x => x.PlanThird),
                        PlanFourth = nonCurrentAssetList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = nonCurrentAssetList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = nonCurrentAssetList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = nonCurrentAssetList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = nonCurrentAssetList.Sum(x => x.DiffrenceFourth),
                        First = nonCurrentAssetList.Sum(x => x.First),
                        Second = nonCurrentAssetList.Sum(x => x.Second),
                        Third = nonCurrentAssetList.Sum(x => x.Third),
                        Fourth = nonCurrentAssetList.Sum(x => x.Fourth),
                        ActualTotal = nonCurrentAssetList.Sum(x => x.ActualTotal),
                        PlanTotal = nonCurrentAssetList.Sum(x => x.PlanTotal),
                        PercentageTotal = nonCurrentAssetList.Sum(x => x.PercentageTotal),
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

                    #region  current asset header
                    var currentAssetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Current Assets ",
                        IsHeader = true
                    };
                    balance.Add(currentAssetHedaer);


                    #endregion
                    #region  current asset

                    var currentAssets = assetBalances.Where(x => x.Category == GaapAssetTypes.CurrentAsset);

                    foreach (var assetBalance in currentAssets)
                    {
                        var ranges = assetBalance.GaapBalanceSheetAssetRanges.ToList();
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



                        var currentasset = new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
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

                            PercentageTotal = percentageTotal,
                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        currentAssetList.Add(currentasset);
                    }
                    balance.AddRange(currentAssetList);
                    #region total current asset
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Current Assets ",
                        PlanFirst = currentAssetList.Sum(x => x.PlanFirst),
                        PlanSecond = currentAssetList.Sum(x => x.PlanSecond),
                        PlanThird = currentAssetList.Sum(x => x.PlanThird),
                        PlanFourth = currentAssetList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = currentAssetList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = currentAssetList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = currentAssetList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = currentAssetList.Sum(x => x.DiffrenceFourth),
                        First = currentAssetList.Sum(x => x.First),
                        Second = currentAssetList.Sum(x => x.Second),
                        Third = currentAssetList.Sum(x => x.Third),
                        Fourth = currentAssetList.Sum(x => x.Fourth),
                        ActualTotal = currentAssetList.Sum(x => x.ActualTotal),
                        PlanTotal = currentAssetList.Sum(x => x.PlanTotal),
                        PercentageTotal = currentAssetList.Sum(x => x.PercentageTotal),
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
                    #region total asset
                    decimal totalassetPlanFirst = nonCurrentAssetList.Sum(x => x.PlanFirst) + currentAssetList.Sum(x => x.PlanFirst);
                    decimal totalassetPlanSecond = nonCurrentAssetList.Sum(x => x.PlanSecond) + currentAssetList.Sum(x => x.PlanSecond);
                    decimal totalassetPlanThird = nonCurrentAssetList.Sum(x => x.PlanThird) + currentAssetList.Sum(x => x.PlanThird);
                    decimal totalassetPlanFourth = nonCurrentAssetList.Sum(x => x.PlanFourth) + currentAssetList.Sum(x => x.PlanFourth);
                    decimal totalassetDiffrenceFirst = nonCurrentAssetList.Sum(x => x.DiffrenceFirst) + currentAssetList.Sum(x => x.DiffrenceFirst);
                    decimal totalassetDiffrenceSecond = nonCurrentAssetList.Sum(x => x.DiffrenceSecond) + currentAssetList.Sum(x => x.DiffrenceSecond);
                    decimal totalassetDiffrenceThird = nonCurrentAssetList.Sum(x => x.DiffrenceThird) + currentAssetList.Sum(x => x.DiffrenceThird);
                    decimal totalassetDiffrenceFourth = nonCurrentAssetList.Sum(x => x.DiffrenceFourth) + currentAssetList.Sum(x => x.DiffrenceFourth);
                    decimal totalassetFirst = nonCurrentAssetList.Sum(x => x.First) + currentAssetList.Sum(x => x.First);
                    decimal totalassetSecond = nonCurrentAssetList.Sum(x => x.Second) + currentAssetList.Sum(x => x.Second);
                    decimal totalassetThird = nonCurrentAssetList.Sum(x => x.Third) + currentAssetList.Sum(x => x.Third);
                    decimal totalassetFourth = nonCurrentAssetList.Sum(x => x.Fourth) + currentAssetList.Sum(x => x.Fourth);
                    decimal totalassetActualTotal = nonCurrentAssetList.Sum(x => x.ActualTotal) + currentAssetList.Sum(x => x.ActualTotal);
                    decimal totalassetPlanTotal = nonCurrentAssetList.Sum(x => x.PlanTotal) + currentAssetList.Sum(x => x.PlanTotal);
                    decimal totalassetPercentageTotal = nonCurrentAssetList.Sum(x => x.PercentageTotal) +currentAssetList.Sum(x => x.PercentageTotal);
                
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Assets ",
                        PlanFirst = totalassetPlanFirst,
                        PlanSecond = totalassetPlanSecond,
                        PlanThird = totalassetPlanThird,
                        PlanFourth = totalassetPlanFourth,
                        DiffrenceFirst = totalassetDiffrenceFirst,
                        DiffrenceSecond = totalassetDiffrenceSecond,
                        DiffrenceThird = totalassetDiffrenceThird,
                        DiffrenceFourth = totalassetDiffrenceFourth,
                        First = totalassetFirst,
                        Second = totalassetSecond,
                        Third = totalassetThird,
                        Fourth = totalassetFourth,
                        ActualTotal = totalassetActualTotal,
                        PlanTotal = totalassetPlanTotal,
                        PercentageTotal = totalassetPercentageTotal,
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
                    #region capital
                    #region equity and liability header

                    var reportitem = new FinancialStatementQuarterlyVm
                    {
                        Description = "Equity and Liablities",
                        IsHeader=true
                    };
                    balance.Add(reportitem);

                    #endregion

                    #region Capital and Reserves header

                    reportitem = new FinancialStatementQuarterlyVm
                    {
                        Description = "Capital and Reserves",
                        IsHeader = true
                    };
                    balance.Add(reportitem);

                    #endregion
                    # region capital and Reserves--capital

                    var capitals = capitalsBalances.Where(x => x.Category == GaapEquityTypes.CapitalAndReserves);
                    foreach (var capitalsBalance in capitals)
                    {

                        var ranges = capitalsBalance.GaapBalanceSheetCapitalRanges.ToList();
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

                        var capital=new FinancialStatementQuarterlyVm
                        {
                            Description = capitalsBalance.Description,
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

                            PercentageTotal = percentageTotal,
                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        capitalList.Add(capital);
                    }
                    balance.AddRange(capitalList);
                    #region earning

                    decimal earningFirst = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 1); 
                    decimal earningSecond = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 2);
                    decimal earningThird = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 3);
                    decimal earningFourth = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 4);
                    decimal earningTotal = earningFirst + earningSecond + earningThird + earningFourth;
                    var earning = new FinancialStatementQuarterlyVm
                    {
                        Description = "Earning",
                        PlanFirst =0,
                        PlanSecond = 0,
                        PlanThird = 0,
                        PlanFourth = 0,
                        DiffrenceFirst = 0,
                        DiffrenceSecond = 0,
                        DiffrenceThird = 0,
                        DiffrenceFourth = 0,
                        First = earningFirst,
                        Second =earningSecond,
                        Third = earningThird,
                        Fourth = earningFourth,
                        ActualTotal = earningTotal,
                        PlanTotal = 0,
                        PercentageTotal = 0,
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
                    balance.Add(earning);
                    #region total capital and reserve

                    decimal totalcapitalAndReserveFirst = earningFirst + capitalList.Sum(x => x.First);
                    decimal totalcapitalAndReserveSecond = earningSecond + capitalList.Sum(x => x.Second);
                    decimal totalcapitalAndReserveThird = earningThird + capitalList.Sum(x => x.Third);
                    decimal totalcapitalAndReserveFourth = earningFourth + capitalList.Sum(x => x.Fourth);
                    decimal totalcapitalAndReserveActualTotal = earningTotal + capitalList.Sum(x => x.ActualTotal);
                   
                    var totalCapitalandReserver = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Capital and Reserves ",
                        PlanFirst = 0,
                        PlanSecond = 0,
                        PlanThird = 0,
                        PlanFourth = 0,
                        DiffrenceFirst = 0,
                        DiffrenceSecond = 0,
                        DiffrenceThird = 0,
                        DiffrenceFourth = 0,
                        First = totalcapitalAndReserveFirst,
                        Second = totalcapitalAndReserveSecond ,
                        Third = totalcapitalAndReserveThird ,
                        Fourth = totalcapitalAndReserveFourth ,
                        ActualTotal = totalcapitalAndReserveActualTotal,
                        PlanTotal = 0,
                        PercentageTotal = 0,
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
                    balance.Add(totalCapitalandReserver);
                    #endregion
                    #endregion

                    #region liability

                    #region non current liability header
                    var nonCurrentLiabilityHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Non Current Liability ",
                        IsHeader = true
                    };
                    balance.Add(nonCurrentLiabilityHedaer);


                    #endregion
                    #region non current liability 

                    var nonCurrentLiabilities =liabilitiesBalances.Where(x => x.Category == GaapLiablityTypes.NonCurrentLiability);

                    foreach (var liabilitiesBalance in nonCurrentLiabilities)
                    {
                        var ranges = liabilitiesBalance.GaapBalanceSheetLiabilityRanges.ToList();
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




                            #region Budget Modifed 1/25/18 Modifed 1/25/18
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

                        var nonCurrentLiability=new FinancialStatementQuarterlyVm
                        {
                            Description = liabilitiesBalance.Description,
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
                            PercentageTotal = percentageTotal,
                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        nonCurrentLiabilityList.Add(nonCurrentLiability);
                    }
                    balance.AddRange(nonCurrentLiabilityList);
                    #region total non current liability
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Non Current Liability ",
                        PlanFirst = nonCurrentLiabilityList.Sum(x => x.PlanFirst),
                        PlanSecond = nonCurrentLiabilityList.Sum(x => x.PlanSecond),
                        PlanThird = nonCurrentLiabilityList.Sum(x => x.PlanThird),
                        PlanFourth = nonCurrentLiabilityList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = nonCurrentLiabilityList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = nonCurrentLiabilityList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = nonCurrentLiabilityList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = nonCurrentLiabilityList.Sum(x => x.DiffrenceFourth),
                        First = nonCurrentLiabilityList.Sum(x => x.First),
                        Second = nonCurrentLiabilityList.Sum(x => x.Second),
                        Third = nonCurrentLiabilityList.Sum(x => x.Third),
                        Fourth = nonCurrentLiabilityList.Sum(x => x.Fourth),
                        ActualTotal = nonCurrentLiabilityList.Sum(x => x.ActualTotal),
                        PlanTotal = nonCurrentLiabilityList.Sum(x => x.PlanTotal),
                        PercentageTotal = nonCurrentLiabilityList.Sum(x => x.PercentageTotal),
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

                    #region  current liability header
                    var currentLiabilityHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Current Liability ",
                        IsHeader = true
                    };
                    balance.Add(currentLiabilityHedaer);


                    #endregion
                    #region current liability 

                    var currentLiabilities = liabilitiesBalances.Where(x => x.Category == GaapLiablityTypes.CurrentLiability);

                    foreach (var liabilitiesBalance in currentLiabilities)
                    {
                        var ranges = liabilitiesBalance.GaapBalanceSheetLiabilityRanges.ToList();
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




                            #region Budget Modifed 1/25/18 Modifed 1/25/18
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

                        var currentLiability = new FinancialStatementQuarterlyVm
                        {
                            Description = liabilitiesBalance.Description,
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
                            PercentageTotal = percentageTotal,
                            ActualTotal = actualTotal,
                            PlanTotal = planTotal,
                            //PlanDiffFirst = PlanDiffFirst * 100,
                            //PlanDiffSecond = PlanDiffSecond * 100,
                            //PlanDiffThird = PlanDiffThird * 100,
                            //PlanDiffFourth = PlanDiffFourth * 100,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        currentLiabilityList.Add(currentLiability);
                    }

                    //profit tax
                    #region profit tax

                    decimal profitTaxFirst = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 1);
                    decimal profitTaxSecond = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 2);
                    decimal profitTaxThird = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 3);
                    decimal profitTaxFourth = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 4);
                    decimal profitTaxTotal = profitTaxFirst + profitTaxSecond + profitTaxThird + profitTaxFourth;
                    var profitTax = new FinancialStatementQuarterlyVm
                    {
                        Description = "Profit Tax",
                        PlanFirst = 0,
                        PlanSecond = 0,
                        PlanThird = 0,
                        PlanFourth = 0,
                        DiffrenceFirst = 0,
                        DiffrenceSecond = 0,
                        DiffrenceThird = 0,
                        DiffrenceFourth = 0,
                        First = profitTaxFirst,
                        Second = profitTaxSecond,
                        Third = profitTaxThird,
                        Fourth = profitTaxFourth,
                        ActualTotal = profitTaxTotal,
                        PlanTotal = 0,
                        PercentageTotal = 0,
                        //PlanDiffFirst = PlanDiffFirst * 100,
                        //PlanDiffSecond = PlanDiffSecond * 100,
                        //PlanDiffThird = PlanDiffThird * 100,
                        //PlanDiffFourth = PlanDiffFourth * 100,
                        ActualDiffFirst = 0,
                        ActualDiffFourth = 0,
                        ActualDiffSecond = 0,
                        ActualDiffThird = 0,
                    };
                    #endregion
                    currentLiabilityList.Add(profitTax);

                    balance.AddRange(currentLiabilityList);
                    #region total current liability
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Current Liability ",
                        PlanFirst = currentLiabilityList.Sum(x => x.PlanFirst),
                        PlanSecond = currentLiabilityList.Sum(x => x.PlanSecond),
                        PlanThird = currentLiabilityList.Sum(x => x.PlanThird),
                        PlanFourth = currentLiabilityList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = currentLiabilityList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = currentLiabilityList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = currentLiabilityList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = currentLiabilityList.Sum(x => x.DiffrenceFourth),
                        First = currentLiabilityList.Sum(x => x.First),
                        Second = currentLiabilityList.Sum(x => x.Second),
                        Third = currentLiabilityList.Sum(x => x.Third),
                        Fourth = currentLiabilityList.Sum(x => x.Fourth),
                        ActualTotal = currentLiabilityList.Sum(x => x.ActualTotal),
                        PlanTotal = currentLiabilityList.Sum(x => x.PlanTotal),
                        PercentageTotal = currentLiabilityList.Sum(x => x.PercentageTotal),
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

                    #region total  liability
                    decimal totalLiabilityPlanFirst = nonCurrentLiabilityList.Sum(x => x.PlanFirst)+ currentLiabilityList.Sum(x => x.PlanFirst);
                    decimal totalLiabilityPlanSecond = nonCurrentLiabilityList.Sum(x => x.PlanSecond) + currentLiabilityList.Sum(x => x.PlanSecond);
                    decimal totalLiabilityPlanThird = nonCurrentLiabilityList.Sum(x => x.PlanThird) + currentLiabilityList.Sum(x => x.PlanThird);
                    decimal totalLiabilityPlanFourth = nonCurrentLiabilityList.Sum(x => x.PlanFourth) + currentLiabilityList.Sum(x => x.PlanFourth);
                    decimal totalLiabilityDiffrenceFirst = nonCurrentLiabilityList.Sum(x => x.DiffrenceFirst) + currentLiabilityList.Sum(x => x.DiffrenceFirst);
                    decimal totalLiabilityDiffrenceSecond = nonCurrentLiabilityList.Sum(x => x.DiffrenceSecond) + currentLiabilityList.Sum(x => x.DiffrenceSecond);
                    decimal totalLiabilityDiffrenceThird = nonCurrentLiabilityList.Sum(x => x.DiffrenceThird) + currentLiabilityList.Sum(x => x.DiffrenceThird);
                    decimal totalLiabilityDiffrenceFourth = nonCurrentLiabilityList.Sum(x => x.DiffrenceFourth) + currentLiabilityList.Sum(x => x.DiffrenceFourth);
                    decimal totalLiabilityFirst = nonCurrentLiabilityList.Sum(x => x.First) + currentLiabilityList.Sum(x => x.First);
                    decimal totalLiabilitySecond = nonCurrentLiabilityList.Sum(x => x.Second) + currentLiabilityList.Sum(x => x.Second);
                    decimal totalLiabilityThird = nonCurrentLiabilityList.Sum(x => x.Third) + currentLiabilityList.Sum(x => x.Third);
                    decimal totalLiabilityFourth = nonCurrentLiabilityList.Sum(x => x.Fourth) + currentLiabilityList.Sum(x => x.Fourth);
                    decimal totalLiabilityActualTotal = nonCurrentLiabilityList.Sum(x => x.ActualTotal) + currentLiabilityList.Sum(x => x.ActualTotal);
                    decimal totalLiabilityPlanTotal = nonCurrentLiabilityList.Sum(x => x.PlanTotal) + currentLiabilityList.Sum(x => x.PlanTotal);
                    decimal totalLiabilityPercentageTotal = nonCurrentLiabilityList.Sum(x => x.PercentageTotal) + currentLiabilityList.Sum(x => x.PercentageTotal);

                    var totalLiability = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Liability ",
                        PlanFirst = totalLiabilityPlanFirst,
                        PlanSecond = totalLiabilityPlanSecond,
                        PlanThird = totalLiabilityPlanThird,
                        PlanFourth = totalLiabilityPlanFourth,
                        DiffrenceFirst = totalLiabilityDiffrenceFirst,
                        DiffrenceSecond = totalLiabilityDiffrenceSecond,
                        DiffrenceThird = totalLiabilityDiffrenceThird,
                        DiffrenceFourth = totalLiabilityDiffrenceFourth,
                        First = totalLiabilityFirst,
                        Second = totalLiabilitySecond,
                        Third =totalLiabilityThird,
                        Fourth =totalLiabilityFourth,
                        ActualTotal = totalLiabilityActualTotal,
                        PlanTotal = totalLiabilityPlanTotal,
                        PercentageTotal = totalLiabilityPercentageTotal,
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
                    balance.Add(totalLiability);
                    #endregion

                    #region total  Equity and Liabilities
                    decimal totalLiabilityAndEquityPlanFirst = totalLiabilityPlanFirst;
                    decimal totalLiabilityAndEquityPlanSecond = totalLiabilityPlanSecond;
                    decimal totalLiabilityAndEquityPlanThird = totalLiabilityPlanThird;
                    decimal totalLiabilityAndEquityPlanFourth = totalLiabilityPlanFourth;
                    decimal totalLiabilityAndEquityDiffrenceFirst = totalLiabilityDiffrenceFirst;
                    decimal totalLiabilityAndEquityDiffrenceSecond = totalLiabilityDiffrenceSecond;
                    decimal totalLiabilityAndEquityDiffrenceThird = totalLiabilityDiffrenceThird;
                    decimal totalLiabilityAndEquityDiffrenceFourth = totalLiabilityDiffrenceFourth;
                    decimal totalLiabilityAndEquityFirst = totalLiabilityFirst + totalcapitalAndReserveFirst;
                    decimal totalLiabilityAndEquitySecond = totalLiabilitySecond + totalcapitalAndReserveSecond;
                    decimal totalLiabilityAndEquityThird = totalLiabilityThird + totalcapitalAndReserveThird;
                    decimal totalLiabilityAndEquityFourth = totalLiabilityFourth + totalcapitalAndReserveFourth;
                    decimal totalLiabilityAndEquityActualTotal = totalLiabilityActualTotal + totalcapitalAndReserveActualTotal;
                    decimal totalLiabilityAndEquityPlanTotal = totalLiabilityPlanTotal;
                    decimal totalLiabilityAndEquityPercentageTotal = 0;

                    var totalEquityandLiability = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Equity and Liabilities ",
                        PlanFirst = totalLiabilityAndEquityPlanFirst,
                        PlanSecond = totalLiabilityAndEquityPlanSecond,
                        PlanThird = totalLiabilityAndEquityPlanThird,
                        PlanFourth = totalLiabilityAndEquityPlanFourth,
                        DiffrenceFirst = totalLiabilityAndEquityDiffrenceFirst,
                        DiffrenceSecond = totalLiabilityAndEquityDiffrenceSecond,
                        DiffrenceThird = totalLiabilityAndEquityDiffrenceThird,
                        DiffrenceFourth = totalLiabilityAndEquityDiffrenceFourth,
                        First = totalLiabilityAndEquityFirst,
                        Second = totalLiabilityAndEquitySecond,
                        Third = totalLiabilityAndEquityThird,
                        Fourth = totalLiabilityAndEquityFourth,
                        ActualTotal = totalLiabilityAndEquityActualTotal,
                        PlanTotal = totalLiabilityAndEquityPlanTotal,
                        PercentageTotal = totalLiabilityAndEquityPercentageTotal,
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
                    balance.Add(totalEquityandLiability);
                    #endregion

                    #endregion

                }


            }
            return balance;
        }
    }
}
