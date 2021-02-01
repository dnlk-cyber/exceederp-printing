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

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BalanceSheetSingleQuarterOds : List<BalanceSheetViewModel>
    {
        public ExceedDbContext Db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinancialStatementSingleQuarterVm> GetBalanceSheets(string year, string quarter)
        {
            var balance = new List<FinancialStatementSingleQuarterVm>();

            List<GLLedgerPostingLines> yearBegPeriodBalance = new List<GLLedgerPostingLines>();
            List<GLLedgerPostingLines> quarterBegPeriodBalance = new List<GLLedgerPostingLines>();
            List<GLLedgerPostingLines> quarterEndPeriodBalance = new List<GLLedgerPostingLines>();

            List<BudgetPlanMonthly> budgetAnnualMonthlies = new List<BudgetPlanMonthly>();
            List<BudgetPlanMonthly> budgetQuarterMonthlies = new List<BudgetPlanMonthly>();

            var currentAssetList = new List<FinancialStatementSingleQuarterVm>();
            var otherAssetList = new List<FinancialStatementSingleQuarterVm>();
            var fixedAssetList = new List<FinancialStatementSingleQuarterVm>();
            var fixedAssetAccumulatedList = new List<FinancialStatementSingleQuarterVm>();


            var capitalList = new List<FinancialStatementSingleQuarterVm>();
            var longTermLiabilityList = new List<FinancialStatementSingleQuarterVm>();
            var currentLiabilityList = new List<FinancialStatementSingleQuarterVm>();

            ReportHelper reportHelper = new ReportHelper();
            ClosingHelper closingHelper = new ClosingHelper();
            FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();


            if (!string.IsNullOrWhiteSpace(year) && !string.IsNullOrWhiteSpace(quarter))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var balanceSheet = Db.GapBalanceSheets.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPTwo);

                if (balanceSheet != null && fiscalYear != null)
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

                  

                    #region update earning  ledger--current year

                    closingHelper.UpdateEarningForAYear(fiscalyearId);

                    #endregion

                    var assetBalances = balanceSheet.Assets.ToList();
                    var capitalsBalances = balanceSheet.Capitals.ToList();
                    var liabilitiesBalances = balanceSheet.Liabilities.ToList();

                    var allBalnces = reportHelper.GetAccountBalanceByPeriodRange(fiscalyearId, yearBegPeriodId, quarterEndPeriodId);
                    yearBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == yearBegPeriodId).ToList();
                    quarterBegPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterBegPeriodId).ToList();
                    quarterEndPeriodBalance = allBalnces.Where(x => x.GLPeriodId == quarterEndPeriodId).ToList();


                    //#region Budget Modifed 1/25/18
                    //// firstQuarterMonthlys will be Treat as budget between two range.
                    //var periodRangeBalnce =  budgetReportHelper.GetPeriodRangePlanBalance(fiscalyearId, firstPeriodId, lastPeriodId);
                    //var firstQuarterMonthlys = budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.First);
                    //var secondQuarterMonthlys =  budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Second);
                    //var thirdQuarterMonthlys =  budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Third);
                    //var fourthQuarterMonthlys =  budgetReportHelper.ParseQuarterBalanceFromPeriod(periodRangeBalnce, fiscalyearId, BudgetEnums.BudgetQuarter.Fourth);

                   
                    //#endregion

                    #region asset
                    #region asset header


                    var assetHedaer = new FinancialStatementSingleQuarterVm
                    {
                        Description = "ASSETS",
                        IsHeader = true

                    };
                    balance.Add(assetHedaer);

                    #endregion

                    #region  current asset header
                    var currentAssetHedaer = new FinancialStatementSingleQuarterVm
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


                        var currentasset = new FinancialStatementSingleQuarterVm
                        {
                            Description = assetBalance.Description,
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
                        currentAssetList.Add(currentasset);
                    }
                    balance.AddRange(currentAssetList);
                    #region total current asset
                    var total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Current Assets ",
                        Begining = currentAssetList.Sum(x => x.Begining),
                        QuarterPlan = currentAssetList.Sum(x => x.QuarterPlan),
                        QuarterActual = currentAssetList.Sum(x => x.QuarterActual),
                        QuarterPercentage = currentAssetList.Sum(x => x.QuarterPercentage),
                        QuarterVariance = currentAssetList.Sum(x => x.QuarterVariance),

                        TotalBudget = currentAssetList.Sum(x => x.TotalBudget),
                        TotalActual = currentAssetList.Sum(x => x.TotalActual),
                        TotalPercentage = currentAssetList.Sum(x => x.TotalPercentage),
                        TotalVariance = currentAssetList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);

                    #endregion

                    #region  other asset header
                    var otherAssetHedaer = new FinancialStatementSingleQuarterVm
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


                        var otherasset = new FinancialStatementSingleQuarterVm
                        {
                            Description = assetBalance.Description,
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
                        otherAssetList.Add(otherasset);
                    }
                    balance.AddRange(otherAssetList);
                    #region total other asset
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Other Assets ",
                        Begining = otherAssetList.Sum(x => x.Begining),
                        QuarterPlan = otherAssetList.Sum(x => x.QuarterPlan),
                        QuarterActual = otherAssetList.Sum(x => x.QuarterActual),
                        QuarterPercentage = otherAssetList.Sum(x => x.QuarterPercentage),
                        QuarterVariance = otherAssetList.Sum(x => x.QuarterVariance),

                        TotalBudget = otherAssetList.Sum(x => x.TotalBudget),
                        TotalActual = otherAssetList.Sum(x => x.TotalActual),
                        TotalPercentage = otherAssetList.Sum(x => x.TotalPercentage),
                        TotalVariance = otherAssetList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);

                    #endregion

                    #region fixed asset header
                    var fixedAssetHedaer = new FinancialStatementSingleQuarterVm
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



                        var fixedasset = new FinancialStatementSingleQuarterVm
                        {
                            Description = assetBalance.Description,
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
                        fixedAssetList.Add(fixedasset);
                    }
                    balance.AddRange(fixedAssetList);
                    #region total fixed asset
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Fixed Assets ",
                        Begining = fixedAssetList.Sum(x => x.Begining),
                        QuarterPlan = fixedAssetList.Sum(x => x.QuarterPlan),
                        QuarterActual = fixedAssetList.Sum(x => x.QuarterActual),
                        QuarterPercentage = fixedAssetList.Sum(x => x.QuarterPercentage),
                        QuarterVariance = fixedAssetList.Sum(x => x.QuarterVariance),

                        TotalBudget = fixedAssetList.Sum(x => x.TotalBudget),
                        TotalActual = fixedAssetList.Sum(x => x.TotalActual),
                        TotalPercentage = fixedAssetList.Sum(x => x.TotalPercentage),
                        TotalVariance = fixedAssetList.Sum(x => x.TotalVariance),
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

                        var fixedassetAccumulated = new FinancialStatementSingleQuarterVm
                        {
                            Description = assetBalance.Description,
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
                        fixedAssetAccumulatedList.Add(fixedassetAccumulated);
                    }
                    balance.AddRange(fixedAssetAccumulatedList);
                    #region total accumulated fixed asset
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Accumulated Dep. Fixed Assets ",
                        Begining = fixedAssetAccumulatedList.Sum(x => x.Begining),
                        QuarterPlan = fixedAssetAccumulatedList.Sum(x => x.QuarterPlan),                        
                        QuarterActual = fixedAssetAccumulatedList.Sum(x => x.QuarterActual),
                        QuarterVariance = fixedAssetAccumulatedList.Sum(x => x.QuarterVariance),
                        QuarterPercentage = fixedAssetAccumulatedList.Sum(x => x.QuarterPercentage),

                        TotalBudget = fixedAssetAccumulatedList.Sum(x => x.TotalBudget),
                        TotalActual = fixedAssetAccumulatedList.Sum(x => x.TotalActual),
                        TotalVariance = fixedAssetAccumulatedList.Sum(x => x.TotalVariance),
                        TotalPercentage = fixedAssetAccumulatedList.Sum(x => x.TotalPercentage),
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);
                    #endregion
                    #region net fixed asset
                    decimal netFixedAssetBeginning = fixedAssetList.Sum(x => x.Begining) + fixedAssetAccumulatedList.Sum(x => x.Begining);

                    decimal netFixedAssetQuarterBudget = fixedAssetList.Sum(x => x.QuarterPlan) + fixedAssetAccumulatedList.Sum(x => x.QuarterPlan);
                    decimal netFixedAssetQuarterVariance = fixedAssetList.Sum(x => x.QuarterVariance) + fixedAssetAccumulatedList.Sum(x => x.QuarterVariance);
                    decimal netFixedAssetQuarterActual = fixedAssetList.Sum(x => x.QuarterActual) + fixedAssetAccumulatedList.Sum(x => x.QuarterActual);

                    decimal netFixedAssetTotalActual = fixedAssetList.Sum(x => x.TotalActual) + fixedAssetAccumulatedList.Sum(x => x.TotalActual);
                    decimal netFixedAssetTotalBudget = fixedAssetList.Sum(x => x.TotalBudget) + fixedAssetAccumulatedList.Sum(x => x.TotalBudget);
                    decimal netFixedAssetTotalVariance = fixedAssetList.Sum(x => x.TotalVariance) + fixedAssetAccumulatedList.Sum(x => x.TotalVariance);

                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Net Fixed Assets ",
                        Begining = netFixedAssetBeginning,
                        QuarterPlan = netFixedAssetQuarterBudget,
                        QuarterActual = netFixedAssetQuarterActual,
                        QuarterVariance = netFixedAssetQuarterVariance,
                       
                        TotalActual = netFixedAssetTotalActual,
                        TotalBudget = netFixedAssetTotalBudget,
                        TotalVariance = netFixedAssetTotalVariance,
                        IsHeader = true
                    };
                    #endregion

                    #region total asset
                    decimal totalassetBeginning = netFixedAssetBeginning + otherAssetList.Sum(x => x.Begining) + currentAssetList.Sum(x => x.Begining);
                    decimal totalassetQuarterBudget = netFixedAssetQuarterBudget + otherAssetList.Sum(x => x.QuarterPlan) + currentAssetList.Sum(x => x.QuarterPlan);
                    decimal totalassetQuarterVariance = netFixedAssetQuarterVariance + otherAssetList.Sum(x => x.QuarterVariance) + currentAssetList.Sum(x => x.QuarterVariance);
                    decimal totalassetQuarterActual = netFixedAssetQuarterActual + otherAssetList.Sum(x => x.QuarterActual) + currentAssetList.Sum(x => x.QuarterActual);

                    decimal totalassetTotalActual = netFixedAssetTotalActual + otherAssetList.Sum(x => x.TotalActual) + currentAssetList.Sum(x => x.TotalActual);
                    decimal totalassetTotalBudget = netFixedAssetTotalBudget + otherAssetList.Sum(x => x.TotalBudget) + currentAssetList.Sum(x => x.TotalBudget);
                    decimal totalassetTotalVariance = netFixedAssetTotalVariance + otherAssetList.Sum(x => x.TotalVariance) + currentAssetList.Sum(x => x.TotalVariance);

                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Assets ",
                        Begining = totalassetBeginning,
                        QuarterActual = totalassetQuarterActual,
                        QuarterPlan = totalassetQuarterBudget,
                        QuarterVariance = totalassetQuarterVariance,

                        TotalActual = totalassetTotalActual,
                        TotalBudget = totalassetTotalBudget,
                        TotalVariance = totalassetTotalVariance,
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);
                    #endregion

                    #region liabilities and capital
                    #region liabilities and capital header
                    var liabilityandCapitalHedaer = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Liabilities and Capital",
                        IsHeader = true
                    };
                    balance.Add(liabilityandCapitalHedaer);


                    #endregion


                    #region  current liability header
                    var currentLiabilityHedaer = new FinancialStatementSingleQuarterVm
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

                        var currentLiability = new FinancialStatementSingleQuarterVm
                        {
                            Description = liabilitiesBalance.Description,
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

                        currentLiabilityList.Add(currentLiability);
                    }

                    //profit tax
                    if (FinanceHelper.CalculateProfitTax())
                    {
                        #region profit tax
                        decimal profitTaxQuarter = 0;

                        decimal profitTaxFirst = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearId, 1);
                        decimal profitTaxSecond = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearId, 2);
                        decimal profitTaxThird = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearId, 3);
                        decimal profitTaxFourth = closingHelper.GetCurrentProfitTaxByQuarter(fiscalyearId, 4);

                        decimal profitTaxTotal = profitTaxFirst + profitTaxSecond + profitTaxThird + profitTaxFourth;

                        switch (quarter)
                        {
                            case "1":
                                profitTaxQuarter = profitTaxFirst;
                                break;
                            case "2":
                                profitTaxQuarter = profitTaxSecond;
                                break;
                            case "3":
                                profitTaxQuarter = profitTaxThird;
                                break;
                            case "4":
                                profitTaxQuarter = profitTaxFourth;
                                break;
                        }

                       

                        var profitTax = new FinancialStatementSingleQuarterVm
                        {
                            Description = "Profit Tax Payable",
                            Begining = 0,
                            QuarterPlan = 0,
                            QuarterActual = profitTaxQuarter,
                            QuarterPercentage=0,
                            QuarterVariance = profitTaxQuarter,

                            TotalBudget = 0,
                            TotalActual = profitTaxTotal,
                            TotalPercentage = 0,
                            TotalVariance = 0
                        };
                        #endregion
                        currentLiabilityList.Add(profitTax);
                    }

                    balance.AddRange(currentLiabilityList);

                    #region total current liability
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Current Liability ",
                        Begining = currentLiabilityList.Sum(x => x.Begining),
                        QuarterPlan = currentLiabilityList.Sum(x => x.QuarterPlan),
                        QuarterActual = currentLiabilityList.Sum(x => x.QuarterActual),
                        QuarterVariance = currentLiabilityList.Sum(x => x.QuarterVariance),

                        TotalBudget = currentLiabilityList.Sum(x => x.TotalBudget),
                        TotalActual = currentLiabilityList.Sum(x => x.TotalActual),
                        TotalVariance = currentLiabilityList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);
                    #endregion

                    #region long term liability header
                    var longtermLiabilityHedaer = new FinancialStatementSingleQuarterVm
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

                        var longTermLiability = new FinancialStatementSingleQuarterVm
                        {
                            Description = liabilitiesBalance.Description,
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

                        longTermLiabilityList.Add(longTermLiability);
                    }
                    balance.AddRange(longTermLiabilityList);
                    #region total non current liability
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Long Term Liability ",
                        Begining = longTermLiabilityList.Sum(x => x.Begining),
                        QuarterActual = longTermLiabilityList.Sum(x => x.QuarterActual),
                        QuarterPlan = longTermLiabilityList.Sum(x => x.QuarterPlan),
                        QuarterVariance = longTermLiabilityList.Sum(x => x.QuarterVariance),

                        TotalActual = longTermLiabilityList.Sum(x => x.TotalActual),
                        TotalBudget = longTermLiabilityList.Sum(x => x.TotalBudget),
                        TotalVariance = longTermLiabilityList.Sum(x => x.TotalVariance),
                        IsHeader = true
                    };
                    #endregion
                    balance.Add(total);
                    #endregion

                    #region total  liability
                    decimal totalLiabilityBeginning = longTermLiabilityList.Sum(x => x.Begining) + currentLiabilityList.Sum(x => x.Begining);
                    decimal totalLiabilityQuarterPlan = longTermLiabilityList.Sum(x => x.QuarterPlan) + currentLiabilityList.Sum(x => x.QuarterPlan);
                    decimal totalLiabilityQuarterVariance = longTermLiabilityList.Sum(x => x.QuarterVariance) + currentLiabilityList.Sum(x => x.QuarterVariance);
                    decimal totalLiabilityQuarterActual = longTermLiabilityList.Sum(x => x.QuarterActual) + currentLiabilityList.Sum(x => x.QuarterActual);
                   
                    decimal totalLiabilityTotalActual = longTermLiabilityList.Sum(x => x.TotalActual) + currentLiabilityList.Sum(x => x.TotalActual);
                    decimal totalLiabilityTotalBudget = longTermLiabilityList.Sum(x => x.TotalBudget) + currentLiabilityList.Sum(x => x.TotalBudget);
                    decimal totalLiabilityTotalVariance = longTermLiabilityList.Sum(x => x.TotalVariance) + currentLiabilityList.Sum(x => x.TotalVariance);

                    var totalLiability = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Liability ",
                        Begining = totalLiabilityBeginning,
                        QuarterPlan = totalLiabilityQuarterPlan,                      
                        QuarterVariance = totalLiabilityQuarterVariance,                       
                        QuarterActual = totalLiabilityQuarterActual,
                        TotalActual = totalLiabilityTotalActual,
                        TotalBudget = totalLiabilityTotalBudget,
                        TotalVariance = totalLiabilityTotalVariance,
                        IsHeader = true
                    };
                    balance.Add(totalLiability);
                    #endregion

                    #region capital
                    #region capital header

                    var reportitem = new FinancialStatementSingleQuarterVm
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

                        var capital = new FinancialStatementSingleQuarterVm
                        {
                            Description = capitalsBalance.Description,
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
                        capitalList.Add(capital);
                    }

                    #region earning
                    decimal quarterEarning = 0;
                    decimal earningFirst = closingHelper.GetCurrentEarningByQuarter(fiscalyearId, 1);
                    decimal earningSecond = closingHelper.GetCurrentEarningByQuarter(fiscalyearId, 2);
                    decimal earningThird = closingHelper.GetCurrentEarningByQuarter(fiscalyearId, 3);
                    decimal earningFourth = closingHelper.GetCurrentEarningByQuarter(fiscalyearId, 4);
                    decimal earningTotal = earningFirst + earningSecond + earningThird + earningFourth;

                    switch (quarter)
                    {
                        case "1":
                            quarterEarning = earningFirst;
                            break;
                        case "2":
                            quarterEarning = earningSecond;
                            break;
                        case "3":
                            quarterEarning = earningThird;
                            break;
                        case "4":
                            quarterEarning = earningFourth;
                            break;
                    }
                    var earning = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Retained Earning",
                        Begining = 0,
                        QuarterPlan = 0,
                        QuarterActual = quarterEarning,
                        QuarterPercentage = 0,
                        QuarterVariance = 0,

                        TotalBudget = 0,
                        TotalActual = earningTotal,
                        TotalPercentage = 0,
                        TotalVariance = 0,
                        IsHeader = true
                    };
                    #endregion
                    capitalList.Add(earning);
                    balance.AddRange(capitalList);

                    #endregion

                    #region total capital
                    total = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Capital ",
                        Begining = capitalList.Sum(x => x.Begining),
                        QuarterActual = capitalList.Sum(x => x.QuarterActual),
                        QuarterPlan = capitalList.Sum(x => x.QuarterPlan),
                        QuarterPercentage = capitalList.Sum(x => x.QuarterPercentage),
                        QuarterVariance = capitalList.Sum(x => x.QuarterVariance),

                        TotalActual = capitalList.Sum(x => x.TotalActual),
                        TotalBudget = capitalList.Sum(x => x.TotalBudget),
                        TotalVariance = capitalList.Sum(x => x.TotalVariance),
                        TotalPercentage = capitalList.Sum(x => x.TotalPercentage),
                        IsHeader = true
                    };
                    balance.Add(total);
                    #endregion

                    #region total  liability and capital
                    decimal totalLiabilityandCapitalBeginning = capitalList.Sum(x => x.Begining) + longTermLiabilityList.Sum(x => x.Begining) + currentLiabilityList.Sum(x => x.Begining);
                    decimal totalLiabilityandCapitalQuarterBudget = capitalList.Sum(x => x.QuarterPlan) + longTermLiabilityList.Sum(x => x.QuarterPlan) + currentLiabilityList.Sum(x => x.QuarterPlan);
                    decimal totalLiabilityandCapitalQuarterVariance = capitalList.Sum(x => x.QuarterVariance) + longTermLiabilityList.Sum(x => x.QuarterVariance) + currentLiabilityList.Sum(x => x.QuarterVariance);
                    decimal totalLiabilityandCapitalQuarterActual = capitalList.Sum(x => x.QuarterActual) + longTermLiabilityList.Sum(x => x.QuarterActual) + currentLiabilityList.Sum(x => x.QuarterActual);

                    decimal totalLiabilityandCapitalTotalActual = capitalList.Sum(x => x.TotalActual) + longTermLiabilityList.Sum(x => x.TotalActual) + currentLiabilityList.Sum(x => x.TotalActual);
                    decimal totalLiabilityandCapitalTotalBudget = capitalList.Sum(x => x.TotalBudget) + longTermLiabilityList.Sum(x => x.TotalBudget) + currentLiabilityList.Sum(x => x.TotalBudget);
                    decimal totalLiabilityandCapitalTotalPercentage = capitalList.Sum(x => x.TotalVariance) + longTermLiabilityList.Sum(x => x.TotalVariance) + currentLiabilityList.Sum(x => x.TotalVariance);

                    var totalLiabilityandCapitalandCapital = new FinancialStatementSingleQuarterVm
                    {
                        Description = "Total Liability and Capital",
                        Begining = totalLiabilityandCapitalBeginning,
                        QuarterPlan = totalLiabilityandCapitalQuarterBudget,                      
                        QuarterVariance = totalLiabilityandCapitalQuarterVariance,
                        QuarterActual = totalLiabilityandCapitalQuarterActual,

                        TotalBudget = totalLiabilityandCapitalTotalBudget,
                        TotalActual = totalLiabilityandCapitalTotalActual,
                        TotalPercentage = totalLiabilityandCapitalTotalPercentage,
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
