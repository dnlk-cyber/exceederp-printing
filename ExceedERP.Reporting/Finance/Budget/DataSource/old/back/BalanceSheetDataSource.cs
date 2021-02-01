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

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BalanceSheetDataSource : List<BalanceSheetViewModel>
    {
        public ExceedDbContext Db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementQuarterlyVm> GetBalanceSheets(string year, string periodFrom, string periodTo)
        {
            var balance = new List<FinancialStatementQuarterlyVm>();

            var currentAssetList = new List<FinancialStatementQuarterlyVm>();
            var otherAssetList = new List<FinancialStatementQuarterlyVm>();
            var fixedAssetList = new List<FinancialStatementQuarterlyVm>();
            var fixedAssetAccumulatedList = new List<FinancialStatementQuarterlyVm>();


            var capitalList = new List<FinancialStatementQuarterlyVm>();
            var longTermLiabilityList = new List<FinancialStatementQuarterlyVm>();
            var currentLiabilityList = new List<FinancialStatementQuarterlyVm>();
            ReportHelper reportHelper = new ReportHelper();
            ClosingHelper closingHelper = new ClosingHelper();
            FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();


            if (!string.IsNullOrWhiteSpace(year))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var balanceSheet = Db.GapBalanceSheets.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPTwo);

                if (balanceSheet != null && fiscalYear != null)
                {
                    int fiscalyearid = fiscalYear.GlFiscalYearId;
                    int firtPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(fiscalyearid);
                    int lastPeriodId = _fiscalYearHelper.GetFiscalYearLastPeriod(fiscalyearid);

                    if (!string.IsNullOrWhiteSpace(periodFrom) && !string.IsNullOrWhiteSpace(periodTo))
                    {
                        firtPeriodId = int.Parse(periodFrom);
                        lastPeriodId = int.Parse(periodTo);
                    }

                    int firstPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 1);
                    int thirdPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 3);
                    int fourthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 4);
                    int sixthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 6);
                    int seventhPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 7);
                    int ninthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 9);
                    int tenthPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 10);
                    int twelvethPeriod = reportHelper.GetPeriodQuarterIndex(fiscalyearid, 12);

                    #region update earning  ledger--current year

                    closingHelper.UpdateEarningForAYear(fiscalyearid);

                    #endregion

                    var assetBalances = balanceSheet.Assets.ToList();
                    var capitalsBalances = balanceSheet.Capitals.ToList();
                    var liabilitiesBalances = balanceSheet.Liabilities.ToList();

                    var allBalnces = reportHelper.GetAccountBalanceByPeriodRange(fiscalyearid, firstPeriod, lastPeriodId);

                    var firstMonthBalance = allBalnces.Where(x => x.GLPeriodId == firstPeriod).ToList();
                    var thirdMonthBalance = allBalnces.Where(x => x.GLPeriodId == thirdPeriod).ToList();

                    var forthMonthBalance = allBalnces.Where(x => x.GLPeriodId == fourthPeriod).ToList();
                    var sixthMonthBalance = allBalnces.Where(x => x.GLPeriodId == sixthPeriod).ToList();

                    var seventhMonthBalance = allBalnces.Where(x => x.GLPeriodId == seventhPeriod).ToList();
                    var ninthMonthBalance = allBalnces.Where(x => x.GLPeriodId == ninthPeriod).ToList();

                    var tenthMonthBalance = allBalnces.Where(x => x.GLPeriodId == tenthPeriod).ToList();
                    var twelvethMonthBalance = allBalnces.Where(x => x.GLPeriodId == twelvethPeriod).ToList();


                    #region Budget Modifed 1/25/18
                    var firstQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.First);
                    var secondQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Second);
                    var thirdQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Third);
                    var fourthQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalYear, BudgetEnums.BudgetQuarter.Fourth);
                   
                    decimal PlanDiffFirst = 0;
                    decimal PlanDiffSecond = 0;
                    decimal PlanDiffThird = 0;
                    decimal PlanDiffFourth = 0;
                    #endregion

                    #region asset
                    #region asset header


                    var assetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "ASSETS",
                        IsHeader = true

                    };
                    balance.Add(assetHedaer);

                    #endregion

                    #region  current asset header
                    var currentAssetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Current Assets",
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

                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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



                        var currentasset = new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        currentAssetList.Add(currentasset);
                    }
                    balance.AddRange(currentAssetList);
                    #region total current asset
                    var total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Current Assets ",
                        BeginningActual = currentAssetList.Sum(x => x.BeginningActual),
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
                    balance.Add(total);

                    #endregion

                    #region  other asset header
                    var otherAssetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Other Assets ",
                        IsHeader = true
                    };
                    balance.Add(otherAssetHedaer);


                    #endregion
                    #region  other asset

                    var otherAssets = assetBalances.Where(x => x.Category == GaapAssetTypes.OtherAsset);

                    foreach (var assetBalance in otherAssets)
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

                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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



                        var otherasset = new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        otherAssetList.Add(otherasset);
                    }
                    balance.AddRange(otherAssetList);
                    #region total other asset
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Other Assets ",
                        BeginningActual = otherAssetList.Sum(x => x.BeginningActual),
                        PlanFirst = otherAssetList.Sum(x => x.PlanFirst),
                        PlanSecond = otherAssetList.Sum(x => x.PlanSecond),
                        PlanThird = otherAssetList.Sum(x => x.PlanThird),
                        PlanFourth = otherAssetList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = otherAssetList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = otherAssetList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = otherAssetList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = otherAssetList.Sum(x => x.DiffrenceFourth),
                        First = otherAssetList.Sum(x => x.First),
                        Second = otherAssetList.Sum(x => x.Second),
                        Third = otherAssetList.Sum(x => x.Third),
                        Fourth = otherAssetList.Sum(x => x.Fourth),
                        ActualTotal = otherAssetList.Sum(x => x.ActualTotal),
                        PlanTotal = otherAssetList.Sum(x => x.PlanTotal),
                        PercentageTotal = otherAssetList.Sum(x => x.PercentageTotal),
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
                    balance.Add(total);

                    #endregion

                    #region fixed asset header
                    var fixedAssetHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Fixed Assets ",
                        IsHeader = true
                    };
                    balance.Add(fixedAssetHedaer);


                    #endregion
                    #region fixed asset

                    var fixedAssets = assetBalances.Where(x => x.Category == GaapAssetTypes.FixedAsset);

                    foreach (var assetBalance in fixedAssets)
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

                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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



                        var fixedasset = new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        fixedAssetList.Add(fixedasset);
                    }
                    balance.AddRange(fixedAssetList);
                    #region total fixed asset
                    total = new FinancialStatementQuarterlyVm
                   {
                       Description = "Total Fixed Assets ",
                       BeginningActual = fixedAssetList.Sum(x => x.BeginningActual),
                       PlanFirst = fixedAssetList.Sum(x => x.PlanFirst),
                       PlanSecond = fixedAssetList.Sum(x => x.PlanSecond),
                       PlanThird = fixedAssetList.Sum(x => x.PlanThird),
                       PlanFourth = fixedAssetList.Sum(x => x.PlanFourth),
                       DiffrenceFirst = fixedAssetList.Sum(x => x.DiffrenceFirst),
                       DiffrenceSecond = fixedAssetList.Sum(x => x.DiffrenceSecond),
                       DiffrenceThird = fixedAssetList.Sum(x => x.DiffrenceThird),
                       DiffrenceFourth = fixedAssetList.Sum(x => x.DiffrenceFourth),
                       First = fixedAssetList.Sum(x => x.First),
                       Second = fixedAssetList.Sum(x => x.Second),
                       Third = fixedAssetList.Sum(x => x.Third),
                       Fourth = fixedAssetList.Sum(x => x.Fourth),
                       ActualTotal = fixedAssetList.Sum(x => x.ActualTotal),
                       PlanTotal = fixedAssetList.Sum(x => x.PlanTotal),
                       PercentageTotal = fixedAssetList.Sum(x => x.PercentageTotal),
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
                    balance.Add(total);
                    #endregion

                    #region fixed asset accumulated

                    var fixedAssetsAccumulated = assetBalances.Where(x => x.Category == GaapAssetTypes.FixedAssetAccumulated);

                    foreach (var assetBalance in fixedAssetsAccumulated)
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


                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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



                        var fixedassetAccumulated = new FinancialStatementQuarterlyVm
                        {
                            Description = assetBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        fixedAssetAccumulatedList.Add(fixedassetAccumulated);
                    }
                    balance.AddRange(fixedAssetAccumulatedList);
                    #region total accumulated fixed asset
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Accumulated Dep. Fixed Assets ",
                        BeginningActual = fixedAssetAccumulatedList.Sum(x => x.BeginningActual),
                        PlanFirst = fixedAssetAccumulatedList.Sum(x => x.PlanFirst),
                        PlanSecond = fixedAssetAccumulatedList.Sum(x => x.PlanSecond),
                        PlanThird = fixedAssetAccumulatedList.Sum(x => x.PlanThird),
                        PlanFourth = fixedAssetAccumulatedList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = fixedAssetAccumulatedList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = fixedAssetAccumulatedList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = fixedAssetAccumulatedList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = fixedAssetAccumulatedList.Sum(x => x.DiffrenceFourth),
                        First = fixedAssetAccumulatedList.Sum(x => x.First),
                        Second = fixedAssetAccumulatedList.Sum(x => x.Second),
                        Third = fixedAssetAccumulatedList.Sum(x => x.Third),
                        Fourth = fixedAssetAccumulatedList.Sum(x => x.Fourth),
                        ActualTotal = fixedAssetAccumulatedList.Sum(x => x.ActualTotal),
                        PlanTotal = fixedAssetAccumulatedList.Sum(x => x.PlanTotal),
                        PercentageTotal = fixedAssetAccumulatedList.Sum(x => x.PercentageTotal),
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
                    balance.Add(total);
                    #endregion
                    #region net fixed asset
                    decimal netFixedAssetBeginning = fixedAssetList.Sum(x => x.BeginningActual) + fixedAssetAccumulatedList.Sum(x => x.BeginningActual);
                    decimal netFixedAssetPlanFirst = fixedAssetList.Sum(x => x.PlanFirst) + fixedAssetAccumulatedList.Sum(x => x.PlanFirst);
                    decimal netFixedAssetPlanSecond = fixedAssetList.Sum(x => x.PlanSecond) + fixedAssetAccumulatedList.Sum(x => x.PlanSecond);
                    decimal netFixedAssetPlanThird = fixedAssetList.Sum(x => x.PlanThird) + fixedAssetAccumulatedList.Sum(x => x.PlanThird);
                    decimal netFixedAssetPlanFourth = fixedAssetList.Sum(x => x.PlanFourth) + fixedAssetAccumulatedList.Sum(x => x.PlanFourth);
                    decimal netFixedAssetDiffrenceFirst = fixedAssetList.Sum(x => x.DiffrenceFirst) + fixedAssetAccumulatedList.Sum(x => x.DiffrenceFirst);
                    decimal netFixedAssetDiffrenceSecond = fixedAssetList.Sum(x => x.DiffrenceSecond) + fixedAssetAccumulatedList.Sum(x => x.DiffrenceSecond);
                    decimal netFixedAssetDiffrenceThird = fixedAssetList.Sum(x => x.DiffrenceThird) + fixedAssetAccumulatedList.Sum(x => x.DiffrenceThird);
                    decimal netFixedAssetDiffrenceFourth = fixedAssetList.Sum(x => x.DiffrenceFourth) + fixedAssetAccumulatedList.Sum(x => x.DiffrenceFourth);
                    decimal netFixedAssetFirst = fixedAssetList.Sum(x => x.First) + fixedAssetAccumulatedList.Sum(x => x.First);
                    decimal netFixedAssetSecond = fixedAssetList.Sum(x => x.Second) + fixedAssetAccumulatedList.Sum(x => x.Second);
                    decimal netFixedAssetThird = fixedAssetList.Sum(x => x.Third) + fixedAssetAccumulatedList.Sum(x => x.Third);
                    decimal netFixedAssetFourth = fixedAssetList.Sum(x => x.Fourth) + fixedAssetAccumulatedList.Sum(x => x.Fourth);
                    decimal netFixedAssetActualTotal = fixedAssetList.Sum(x => x.ActualTotal) + fixedAssetAccumulatedList.Sum(x => x.ActualTotal);
                    decimal netFixedAssetPlanTotal = fixedAssetList.Sum(x => x.PlanTotal) + fixedAssetAccumulatedList.Sum(x => x.PlanTotal);
                    decimal netFixedAssetPercentageTotal = fixedAssetList.Sum(x => x.PercentageTotal) + fixedAssetAccumulatedList.Sum(x => x.PercentageTotal);

                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Net Fixed Assets ",
                        BeginningActual = netFixedAssetBeginning,
                        PlanFirst = netFixedAssetPlanFirst,
                        PlanSecond = netFixedAssetPlanSecond,
                        PlanThird = netFixedAssetPlanThird,
                        PlanFourth = netFixedAssetPlanFourth,
                        DiffrenceFirst = netFixedAssetDiffrenceFirst,
                        DiffrenceSecond = netFixedAssetDiffrenceSecond,
                        DiffrenceThird = netFixedAssetDiffrenceThird,
                        DiffrenceFourth = netFixedAssetDiffrenceFourth,
                        First = netFixedAssetFirst,
                        Second = netFixedAssetSecond,
                        Third = netFixedAssetThird,
                        Fourth = netFixedAssetFourth,
                        ActualTotal = netFixedAssetActualTotal,
                        PlanTotal = netFixedAssetPlanTotal,
                        PercentageTotal = netFixedAssetPercentageTotal,
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

                    #region total asset
                    decimal totalassetBeginning = netFixedAssetBeginning + otherAssetList.Sum(x => x.BeginningActual) + currentAssetList.Sum(x => x.BeginningActual);
                    decimal totalassetPlanFirst = netFixedAssetPlanFirst + otherAssetList.Sum(x => x.PlanFirst) + currentAssetList.Sum(x => x.PlanFirst);
                    decimal totalassetPlanSecond = netFixedAssetPlanSecond + otherAssetList.Sum(x => x.PlanSecond) + currentAssetList.Sum(x => x.PlanSecond);
                    decimal totalassetPlanThird = netFixedAssetPlanThird + otherAssetList.Sum(x => x.PlanThird) + currentAssetList.Sum(x => x.PlanThird);
                    decimal totalassetPlanFourth = netFixedAssetPlanFourth + otherAssetList.Sum(x => x.PlanFourth) + currentAssetList.Sum(x => x.PlanFourth);
                    decimal totalassetDiffrenceFirst = netFixedAssetDiffrenceFirst + otherAssetList.Sum(x => x.DiffrenceFirst) + currentAssetList.Sum(x => x.DiffrenceFirst);
                    decimal totalassetDiffrenceSecond = netFixedAssetDiffrenceSecond + otherAssetList.Sum(x => x.DiffrenceSecond) + currentAssetList.Sum(x => x.DiffrenceSecond);
                    decimal totalassetDiffrenceThird = netFixedAssetDiffrenceThird + otherAssetList.Sum(x => x.DiffrenceThird) + currentAssetList.Sum(x => x.DiffrenceThird);
                    decimal totalassetDiffrenceFourth = netFixedAssetDiffrenceFourth + otherAssetList.Sum(x => x.DiffrenceFourth) + currentAssetList.Sum(x => x.DiffrenceFourth);
                    decimal totalassetFirst = netFixedAssetFirst + otherAssetList.Sum(x => x.First) + currentAssetList.Sum(x => x.First);
                    decimal totalassetSecond = netFixedAssetSecond + otherAssetList.Sum(x => x.Second) + currentAssetList.Sum(x => x.Second);
                    decimal totalassetThird = netFixedAssetThird + otherAssetList.Sum(x => x.Third) + currentAssetList.Sum(x => x.Third);
                    decimal totalassetFourth = netFixedAssetFourth + otherAssetList.Sum(x => x.Fourth) + currentAssetList.Sum(x => x.Fourth);
                    decimal totalassetActualTotal = netFixedAssetActualTotal + otherAssetList.Sum(x => x.ActualTotal) + currentAssetList.Sum(x => x.ActualTotal);
                    decimal totalassetPlanTotal = netFixedAssetPlanTotal + otherAssetList.Sum(x => x.PlanTotal) + currentAssetList.Sum(x => x.PlanTotal);
                    decimal totalassetPercentageTotal = netFixedAssetPercentageTotal + otherAssetList.Sum(x => x.PercentageTotal) + currentAssetList.Sum(x => x.PercentageTotal);

                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Assets ",
                        BeginningActual = totalassetBeginning,
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
                    balance.Add(total);
                    #endregion

                    #region liabilities and capital
                    #region liabilities and capital header
                    var liabilityandCapitalHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Liabilities and Capital",
                        IsHeader = true
                    };
                    balance.Add(liabilityandCapitalHedaer);


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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var currentLiability = new FinancialStatementQuarterlyVm
                        {
                            Description = liabilitiesBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        currentLiabilityList.Add(currentLiability);
                    }

                    //profit tax
                    if (FinanceHelper.CalculateProfitTax())
                    {
                        #region profit tax

                        decimal profitTaxFirst = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 1);
                        decimal profitTaxSecond = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 2);
                        decimal profitTaxThird = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 3);
                        decimal profitTaxFourth = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearid, 4);
                        decimal profitTaxTotal = profitTaxFirst + profitTaxSecond + profitTaxThird + profitTaxFourth;
                        var profitTax = new FinancialStatementQuarterlyVm
                        {
                            Description = "Profit Tax Payable",
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0,
                        };
                        #endregion
                        currentLiabilityList.Add(profitTax);
                    }

                    balance.AddRange(currentLiabilityList);

                    #region total current liability
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Current Liability ",
                        BeginningActual = currentLiabilityList.Sum(x => x.BeginningActual),
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
                    balance.Add(total);
                    #endregion

                    #region long term liability header
                    var longtermLiabilityHedaer = new FinancialStatementQuarterlyVm
                    {
                        Description = "Long Term Liability ",
                        IsHeader = true
                    };
                    balance.Add(longtermLiabilityHedaer);


                    #endregion
                    #region  long term liability

                    var longtermLiabilities = liabilitiesBalances.Where(x => x.Category == GaapLiablityTypes.LongTermLiability);

                    foreach (var liabilitiesBalance in longtermLiabilities)
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

                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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


                        PlanDiffFirst = actualFirstQuarter - planFirstQuarter;
                        PlanDiffSecond = actualSecondQuarter - planSecondQuarter;
                        PlanDiffThird = actualThirdQuarter - planThirdQuarter;
                        PlanDiffFourth = actualFourthQuarter - planFourthQuarter;

                        actualTotal = beginning + actualFirstQuarter + actualSecondQuarter + actualThirdQuarter + actualFourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;

                        var longTermLiability = new FinancialStatementQuarterlyVm
                        {
                            Description = liabilitiesBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };

                        longTermLiabilityList.Add(longTermLiability);
                    }
                    balance.AddRange(longTermLiabilityList);
                    #region total non current liability
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Long Term Liability ",
                        BeginningActual = longTermLiabilityList.Sum(x => x.BeginningActual),
                        PlanFirst = longTermLiabilityList.Sum(x => x.PlanFirst),
                        PlanSecond = longTermLiabilityList.Sum(x => x.PlanSecond),
                        PlanThird = longTermLiabilityList.Sum(x => x.PlanThird),
                        PlanFourth = longTermLiabilityList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = longTermLiabilityList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = longTermLiabilityList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = longTermLiabilityList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = longTermLiabilityList.Sum(x => x.DiffrenceFourth),
                        First = longTermLiabilityList.Sum(x => x.First),
                        Second = longTermLiabilityList.Sum(x => x.Second),
                        Third = longTermLiabilityList.Sum(x => x.Third),
                        Fourth = longTermLiabilityList.Sum(x => x.Fourth),
                        ActualTotal = longTermLiabilityList.Sum(x => x.ActualTotal),
                        PlanTotal = longTermLiabilityList.Sum(x => x.PlanTotal),
                        PercentageTotal = longTermLiabilityList.Sum(x => x.PercentageTotal),
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
                    balance.Add(total);
                    #endregion

                    #region total  liability
                    decimal totalLiabilityBeginning = longTermLiabilityList.Sum(x => x.BeginningActual) + currentLiabilityList.Sum(x => x.BeginningActual);
                    decimal totalLiabilityPlanFirst = longTermLiabilityList.Sum(x => x.PlanFirst) + currentLiabilityList.Sum(x => x.PlanFirst);
                    decimal totalLiabilityPlanSecond = longTermLiabilityList.Sum(x => x.PlanSecond) + currentLiabilityList.Sum(x => x.PlanSecond);
                    decimal totalLiabilityPlanThird = longTermLiabilityList.Sum(x => x.PlanThird) + currentLiabilityList.Sum(x => x.PlanThird);
                    decimal totalLiabilityPlanFourth = longTermLiabilityList.Sum(x => x.PlanFourth) + currentLiabilityList.Sum(x => x.PlanFourth);
                    decimal totalLiabilityDiffrenceFirst = longTermLiabilityList.Sum(x => x.DiffrenceFirst) + currentLiabilityList.Sum(x => x.DiffrenceFirst);
                    decimal totalLiabilityDiffrenceSecond = longTermLiabilityList.Sum(x => x.DiffrenceSecond) + currentLiabilityList.Sum(x => x.DiffrenceSecond);
                    decimal totalLiabilityDiffrenceThird = longTermLiabilityList.Sum(x => x.DiffrenceThird) + currentLiabilityList.Sum(x => x.DiffrenceThird);
                    decimal totalLiabilityDiffrenceFourth = longTermLiabilityList.Sum(x => x.DiffrenceFourth) + currentLiabilityList.Sum(x => x.DiffrenceFourth);
                    decimal totalLiabilityFirst = longTermLiabilityList.Sum(x => x.First) + currentLiabilityList.Sum(x => x.First);
                    decimal totalLiabilitySecond = longTermLiabilityList.Sum(x => x.Second) + currentLiabilityList.Sum(x => x.Second);
                    decimal totalLiabilityThird = longTermLiabilityList.Sum(x => x.Third) + currentLiabilityList.Sum(x => x.Third);
                    decimal totalLiabilityFourth = longTermLiabilityList.Sum(x => x.Fourth) + currentLiabilityList.Sum(x => x.Fourth);
                    decimal totalLiabilityActualTotal = longTermLiabilityList.Sum(x => x.ActualTotal) + currentLiabilityList.Sum(x => x.ActualTotal);
                    decimal totalLiabilityPlanTotal = longTermLiabilityList.Sum(x => x.PlanTotal) + currentLiabilityList.Sum(x => x.PlanTotal);
                    decimal totalLiabilityPercentageTotal = longTermLiabilityList.Sum(x => x.PercentageTotal) + currentLiabilityList.Sum(x => x.PercentageTotal);

                    var totalLiability = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Liability ",
                        BeginningActual = totalLiabilityBeginning,
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
                        Third = totalLiabilityThird,
                        Fourth = totalLiabilityFourth,
                        ActualTotal = totalLiabilityActualTotal,
                        PlanTotal = totalLiabilityPlanTotal,
                        PercentageTotal = totalLiabilityPercentageTotal,
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
                    balance.Add(totalLiability);
                    #endregion

                    #region capital
                    #region capital header

                    var reportitem = new FinancialStatementQuarterlyVm
                    {
                        Description = "Capital",
                        IsHeader = true
                    };
                    balance.Add(reportitem);

                    #endregion


                    # region capital

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


                          PlanDiffFirst = 0;
                          PlanDiffSecond = 0;
                          PlanDiffThird = 0;
                          PlanDiffFourth = 0;

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

                        var capital = new FinancialStatementQuarterlyVm
                        {
                            Description = capitalsBalance.Description,
                            BeginningActual = beginning,
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
                            PlanDiffFirst = PlanDiffFirst,
                            PlanDiffSecond = PlanDiffSecond,
                            PlanDiffThird = PlanDiffThird,
                            PlanDiffFourth = PlanDiffFourth,
                            ActualDiffFirst = 0,
                            ActualDiffFourth = 0,
                            ActualDiffSecond = 0,
                            ActualDiffThird = 0

                        };
                        capitalList.Add(capital);
                    }

                    #region earning

                    decimal earningFirst = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 1);
                    decimal earningSecond = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 2);
                    decimal earningThird = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 3);
                    decimal earningFourth = closingHelper.GetCurrentEarningByQuarter(fiscalyearid, 4);
                    decimal earningTotal = earningFirst + earningSecond + earningThird + earningFourth;
                    var earning = new FinancialStatementQuarterlyVm
                    {
                        Description = "Retained Earning",
                        PlanFirst = 0,
                        PlanSecond = 0,
                        PlanThird = 0,
                        PlanFourth = 0,
                        DiffrenceFirst = 0,
                        DiffrenceSecond = 0,
                        DiffrenceThird = 0,
                        DiffrenceFourth = 0,
                        First = earningFirst,
                        Second = earningSecond,
                        Third = earningThird,
                        Fourth = earningFourth,
                        ActualTotal = earningTotal,
                        PlanTotal = 0,
                        PercentageTotal = 0,
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
                    capitalList.Add(earning);
                    balance.AddRange(capitalList);

                    #endregion

                    #region total capital
                    total = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Capital ",
                        BeginningActual = capitalList.Sum(x => x.BeginningActual),
                        PlanFirst = capitalList.Sum(x => x.PlanFirst),
                        PlanSecond = capitalList.Sum(x => x.PlanSecond),
                        PlanThird = capitalList.Sum(x => x.PlanThird),
                        PlanFourth = capitalList.Sum(x => x.PlanFourth),
                        DiffrenceFirst = capitalList.Sum(x => x.DiffrenceFirst),
                        DiffrenceSecond = capitalList.Sum(x => x.DiffrenceSecond),
                        DiffrenceThird = capitalList.Sum(x => x.DiffrenceThird),
                        DiffrenceFourth = capitalList.Sum(x => x.DiffrenceFourth),
                        First = capitalList.Sum(x => x.First),
                        Second = capitalList.Sum(x => x.Second),
                        Third = capitalList.Sum(x => x.Third),
                        Fourth = capitalList.Sum(x => x.Fourth),
                        ActualTotal = capitalList.Sum(x => x.ActualTotal),
                        PlanTotal = capitalList.Sum(x => x.PlanTotal),
                        PercentageTotal = capitalList.Sum(x => x.PercentageTotal),
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
                    balance.Add(total);
                    #endregion

                    #region total  liability and capital
                    decimal totalLiabilityandCapitalBeginning = capitalList.Sum(x => x.BeginningActual) + longTermLiabilityList.Sum(x => x.BeginningActual) + currentLiabilityList.Sum(x => x.BeginningActual);
                    decimal totalLiabilityandCapitalPlanFirst = capitalList.Sum(x => x.PlanFirst) + longTermLiabilityList.Sum(x => x.PlanFirst) + currentLiabilityList.Sum(x => x.PlanFirst);
                    decimal totalLiabilityandCapitalPlanSecond = capitalList.Sum(x => x.PlanSecond) + longTermLiabilityList.Sum(x => x.PlanSecond) + currentLiabilityList.Sum(x => x.PlanSecond);
                    decimal totalLiabilityandCapitalPlanThird = capitalList.Sum(x => x.PlanThird) + longTermLiabilityList.Sum(x => x.PlanThird) + currentLiabilityList.Sum(x => x.PlanThird);
                    decimal totalLiabilityandCapitalPlanFourth = capitalList.Sum(x => x.PlanFourth) + longTermLiabilityList.Sum(x => x.PlanFourth) + currentLiabilityList.Sum(x => x.PlanFourth);
                    decimal totalLiabilityandCapitalDiffrenceFirst = capitalList.Sum(x => x.DiffrenceFirst) + longTermLiabilityList.Sum(x => x.DiffrenceFirst) + currentLiabilityList.Sum(x => x.DiffrenceFirst);
                    decimal totalLiabilityandCapitalDiffrenceSecond = capitalList.Sum(x => x.DiffrenceSecond) + longTermLiabilityList.Sum(x => x.DiffrenceSecond) + currentLiabilityList.Sum(x => x.DiffrenceSecond);
                    decimal totalLiabilityandCapitalDiffrenceThird = capitalList.Sum(x => x.DiffrenceThird) + longTermLiabilityList.Sum(x => x.DiffrenceThird) + currentLiabilityList.Sum(x => x.DiffrenceThird);
                    decimal totalLiabilityandCapitalDiffrenceFourth = capitalList.Sum(x => x.DiffrenceFourth) + longTermLiabilityList.Sum(x => x.DiffrenceFourth) + currentLiabilityList.Sum(x => x.DiffrenceFourth);
                    decimal totalLiabilityandCapitalFirst = capitalList.Sum(x => x.First) + longTermLiabilityList.Sum(x => x.First) + currentLiabilityList.Sum(x => x.First);
                    decimal totalLiabilityandCapitalSecond = capitalList.Sum(x => x.Second) + longTermLiabilityList.Sum(x => x.Second) + currentLiabilityList.Sum(x => x.Second);
                    decimal totalLiabilityandCapitalThird = capitalList.Sum(x => x.Third) + longTermLiabilityList.Sum(x => x.Third) + currentLiabilityList.Sum(x => x.Third);
                    decimal totalLiabilityandCapitalFourth = capitalList.Sum(x => x.Fourth) + longTermLiabilityList.Sum(x => x.Fourth) + currentLiabilityList.Sum(x => x.Fourth);
                    decimal totalLiabilityandCapitalActualTotal = capitalList.Sum(x => x.ActualTotal) + longTermLiabilityList.Sum(x => x.ActualTotal) + currentLiabilityList.Sum(x => x.ActualTotal);
                    decimal totalLiabilityandCapitalPlanTotal = capitalList.Sum(x => x.PlanTotal) + longTermLiabilityList.Sum(x => x.PlanTotal) + currentLiabilityList.Sum(x => x.PlanTotal);
                    decimal totalLiabilityandCapitalPercentageTotal = capitalList.Sum(x => x.PercentageTotal) + longTermLiabilityList.Sum(x => x.PercentageTotal) + currentLiabilityList.Sum(x => x.PercentageTotal);

                    var totalLiabilityandCapitalandCapital = new FinancialStatementQuarterlyVm
                    {
                        Description = "Total Liability and Capital",
                        BeginningActual = totalLiabilityandCapitalBeginning,
                        PlanFirst = totalLiabilityandCapitalPlanFirst,
                        PlanSecond = totalLiabilityandCapitalPlanSecond,
                        PlanThird = totalLiabilityandCapitalPlanThird,
                        PlanFourth = totalLiabilityandCapitalPlanFourth,
                        DiffrenceFirst = totalLiabilityandCapitalDiffrenceFirst,
                        DiffrenceSecond = totalLiabilityandCapitalDiffrenceSecond,
                        DiffrenceThird = totalLiabilityandCapitalDiffrenceThird,
                        DiffrenceFourth = totalLiabilityandCapitalDiffrenceFourth,
                        First = totalLiabilityandCapitalFirst,
                        Second = totalLiabilityandCapitalSecond,
                        Third = totalLiabilityandCapitalThird,
                        Fourth = totalLiabilityandCapitalFourth,
                        ActualTotal = totalLiabilityandCapitalActualTotal,
                        PlanTotal = totalLiabilityandCapitalPlanTotal,
                        PercentageTotal = totalLiabilityandCapitalPercentageTotal,
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
                    balance.Add(totalLiabilityandCapitalandCapital);
                    #endregion

                    #endregion

                    #endregion
                }


            }
            return balance;
        }
    }
}
