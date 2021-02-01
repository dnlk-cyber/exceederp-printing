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
    public class BudgetAccountSheduleDataSource : List<AccountSheduleViewModel>
    {
        public ExceedDbContext Db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountSheduleViewModel> GetAccountSheduleList()
        {

            var budgetAccountShedules = Db.BudgetAccountSheduleCategories.Select(s => new AccountSheduleViewModel
            {

                AccountDesc = s.Name.ToString(),
                AccountCode = s.BudgetAccountSheduleCategoryId.ToString()

            }).ToList();

            return budgetAccountShedules;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountSheduleViewModel> GetIndexList()
        {

            var budgetAccountShedules = Db.BudgetAccountShedules.DistinctByFiled(x => x.Index).Select(s => new AccountSheduleViewModel
            {

                AccountDesc = s.Index.ToString(),
                AccountCode = s.Index.ToString()

            }).ToList();

            return budgetAccountShedules;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountSheduleViewModel> GetAccountShedule(string year, string schedule, string branch, string rule)
        {
            var revenues = new List<AccountSheduleViewModel>();
            ReportHelper reportHelper = new ReportHelper();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            var budgetSchedules = Db.BudgetAccountShedules.ToList();
            var budgetScheduleline = Db.BudgetAccountSheduleLines.ToList();
            var budgetSchedulelineAccount = Db.BudgetLineSheduleAccounts.ToList();
            Branch legerBranch = new Branch();
            bool isBranch = false;
            if (!String.IsNullOrWhiteSpace(branch))
            {
                legerBranch = Db.Branch.FirstOrDefault(x => x.BranchId.ToString() == branch);
                if (legerBranch != null)
                {
                    isBranch = true;
                }
            }
            if (!String.IsNullOrWhiteSpace(year))
            {
                int fiscalyearid = int.Parse(year);
         
                if (!String.IsNullOrWhiteSpace(schedule))
                {
                    budgetSchedules = budgetSchedules.Where(x => x.BudgetAccountSheduleCategoryId.ToString() == schedule).ToList();
                }
                else
                {
                    var scheudleCategory = Db.BudgetAccountSheduleCategories.FirstOrDefault();
                    if (scheudleCategory != null)
                    {
                        budgetSchedules = budgetSchedules.Where(x => x.BudgetAccountSheduleCategoryId == scheudleCategory.BudgetAccountSheduleCategoryId).ToList();
                    }
                }

                var firstMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 1));
                var thirdMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 3));

                var forthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 4));
                var sixthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 6));

                var seventhMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 7));
                var ninthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 9));

                var tenthMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 10));
                var twelvethMonthBalance = reportHelper.GetAccountBalance(fiscalyearid, reportHelper.GetPeriodQuarterIndex(fiscalyearid, 12));

                #region Budget Modifed 1/26/19
                
                    var firstQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalyearid, BudgetEnums.BudgetQuarter.First);
                    var secondQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalyearid, BudgetEnums.BudgetQuarter.Second);
                    var thirdQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalyearid, BudgetEnums.BudgetQuarter.Third);
                    var fourthQuarterMonthlys = budgetReportHelper.GetQuarterPlanBalance(fiscalyearid, BudgetEnums.BudgetQuarter.Fourth);
              
                #endregion
                if (!string.IsNullOrEmpty(rule))
                {
                    budgetSchedules = budgetSchedules.Where(x => x.Index.ToString() == rule).ToList();
                }
                if (budgetSchedules.Any())
                {
                    var budgetSchedule = budgetSchedules.FirstOrDefault();

                    var budgetSchedulesLines = budgetScheduleline.Where(x => x.BudgetAccountSheduleId == budgetSchedule.BudgetAccountSheduleId);

                    foreach (var budgetSchedulesLine in budgetSchedulesLines)
                    {
                        var budgetSchedulesLineAccounts = budgetSchedulelineAccount.Where(x => x.BudgetAccountSheduleLineId == budgetSchedulesLine.BudgetAccountSheduleLineId);
                        decimal beginning = 0;
                        decimal firstQuarter = 0;
                        decimal secondQuarter = 0;
                        decimal thirdQuarter = 0;
                        decimal fourthQuarter = 0;

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


                        foreach (var range in budgetSchedulesLineAccounts)
                        {
                            decimal netChange = 0;
                            if (isBranch)
                            {
                                beginning = beginning + reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(
                                                firstMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount);


                                netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(thirdMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount)
                                            - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(firstMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount);

                                firstQuarter = firstQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(sixthMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount)
                                            - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(forthMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount);
                                secondQuarter = secondQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(ninthMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount)
                                            - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(seventhMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount);
                                thirdQuarter = thirdQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(twelvethMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount)
                                            - reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(tenthMonthBalance, range.FromAccountCode, range.ToAccountCode,
                                                legerBranch.LocationAccount);
                                fourthQuarter = fourthQuarter + netChange;

                                #region Budget Modifed 1/26/19
                                planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                #endregion

                            }
                            else
                            {
                                beginning = beginning + reportHelper.GetAccountBegBalancesWithAllranges(
                                                firstMonthBalance, range.FromAccountCode, range.ToAccountCode);

                                netChange = reportHelper.GetAccountEndBalancesWithAllranges(thirdMonthBalance, range.FromAccountCode, range.ToAccountCode)
                                            - reportHelper.GetAccountBegBalancesWithAllranges(firstMonthBalance, range.FromAccountCode, range.ToAccountCode);
                                firstQuarter = firstQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithAllranges(sixthMonthBalance, range.FromAccountCode, range.ToAccountCode)
                                            - reportHelper.GetAccountBegBalancesWithAllranges(forthMonthBalance, range.FromAccountCode, range.ToAccountCode);
                                secondQuarter = secondQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithAllranges(ninthMonthBalance, range.FromAccountCode, range.ToAccountCode)
                                            - reportHelper.GetAccountBegBalancesWithAllranges(seventhMonthBalance, range.FromAccountCode, range.ToAccountCode);
                                thirdQuarter = thirdQuarter + netChange;

                                netChange = reportHelper.GetAccountEndBalancesWithAllranges(twelvethMonthBalance, range.FromAccountCode, range.ToAccountCode)
                                            - reportHelper.GetAccountBegBalancesWithAllranges(tenthMonthBalance, range.FromAccountCode, range.ToAccountCode);
                                fourthQuarter = fourthQuarter + netChange;

                                #region Budget Modifed 1/26/19
                                planFirstQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(firstQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planSecondQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(secondQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planThirdQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(thirdQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                planFourthQuarter += budgetReportHelper.GetQuarterPlanByAccountRange(fourthQuarterMonthlys, range.FromAccountCode, range.ToAccountCode);
                                #endregion
                            }
                        }


                        percentageFirstQuarter = ((firstQuarter - planFirstQuarter) / planFirstQuarter.DivideByZeroHelper()) * 100;
                        percentageSecondQuarter = ((secondQuarter - planSecondQuarter) / planSecondQuarter.DivideByZeroHelper()) * 100;
                        percentageThirdQuarter = ((thirdQuarter - planThirdQuarter) / planThirdQuarter.DivideByZeroHelper()) * 100;
                        percentageFourthQuarter = ((fourthQuarter - planFourthQuarter) / planFourthQuarter.DivideByZeroHelper()) * 100;

                        actualTotal = beginning + firstQuarter + secondQuarter + thirdQuarter + fourthQuarter;
                        planTotal = planFirstQuarter + planFourthQuarter + planSecondQuarter + planThirdQuarter;
                        //percentageTotal = (percentageFirstQuarter + percentageFourthQuarter + percentageSecondQuarter + percentageThirdQuarter) / 4;
                        // percentageTotal = ((actualTotal - planTotal) / planTotal.DivideByZeroHelper()) * 100;

                        revenues.Add(new AccountSheduleViewModel
                        {
                            Begining = beginning,
                            PlanFirst = planFirstQuarter,
                            PlanSecond = planSecondQuarter,
                            PlanThird = planThirdQuarter,
                            PlanFourth = planFourthQuarter,
                            DiffrenceFirst = percentageFirstQuarter,
                            DiffrenceSecond = percentageSecondQuarter,
                            DiffrenceThird = percentageThirdQuarter,
                            DiffrenceFourth = percentageFourthQuarter,
                            AccountDesc = budgetSchedulesLine.Name,
                            Branch = legerBranch?.Name,
                            First = firstQuarter,
                            Second = secondQuarter,
                            Third = thirdQuarter,
                            Fourth = fourthQuarter,
                            ActualTotal = actualTotal,
                            PercentageTotal = percentageTotal,
                            PlanTotal = planTotal
                        });
                    }
                }
            }

            return revenues;
        }
    }
}
