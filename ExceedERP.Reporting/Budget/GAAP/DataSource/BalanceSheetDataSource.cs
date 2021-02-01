using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Budget.GAAP.ViewModel;
using ExceedERP.Reporting.Finance.Budget.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BalanceSheetDataSource : List<BalanceSheetViewModel>
    {
        public ExceedDbContext Db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetBalanceSheetViewModel> GetBalanceSheets(string year)
        {
            var balance = new List<BudgetBalanceSheetViewModel>();
            ReportHelper reportHelper = new ReportHelper();

            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();

            if (!string.IsNullOrWhiteSpace(year))
            {
                int did = int.Parse(year);
                var fiscalYear = Db.GLFiscalYears.Find(did);
                var balanceSheet = Db.GapBalanceSheets.FirstOrDefault();
               
                if (balanceSheet != null && fiscalYear != null)
                {
                    int fiscalyearid = fiscalYear.GlFiscalYearId;

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

                    foreach (var assetBalance in assetBalances)
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


                        balance.Add(new BudgetBalanceSheetViewModel
                        {
                            AccountType = "Assets",
                            AccountDesc = assetBalance.Category.ToString(),
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      AccountCode = AssetBalance.,
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

                        });
                    }
                    foreach (var capitalsBalance in capitalsBalances)
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

                        balance.Add(new BudgetBalanceSheetViewModel
                        {
                            AccountType = "Capital",
                            AccountDesc = capitalsBalance.Category.ToString(),
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      AccountCode = AssetBalance.,
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

                        });
                    }
                    foreach (var liabilitiesBalance in liabilitiesBalances)
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

                        balance.Add(new BudgetBalanceSheetViewModel
                        {
                            AccountType = "Liabilities",
                            AccountDesc = liabilitiesBalance.Category.ToString(),
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            //      AccountCode = AssetBalance.,
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

                        });
                    }



                }
                

            }
            return balance;
        }
    }
}
