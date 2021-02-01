using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.Budget;
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
using System.Web.Mvc;


namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class BudgetPayableReceiviableDataSource : List<AccountPayableReceviableViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        private ReportHelper reportHelper = new ReportHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetSheduleCategoryType()
        {

            var budgetAccountShedules = EnumExtension.EnumToSelectList(typeof(SheduleCategoryType));

            return budgetAccountShedules;
        }




        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetQuarterType()
        {

            var budgetAccountShedules = EnumExtension.EnumToSelectList(typeof(BudgetEnums.BudgetQuarter));

            return budgetAccountShedules;
        }




        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountPayableReceviableViewModel> GetPayableRecivevables(string type, string FiscalYear, string periodfrom, string periodto, string Branch)
        {
            List<AccountPayableReceviableViewModel> list = new List<AccountPayableReceviableViewModel>();
            ReportHelper reportHelper = new ReportHelper();
            BudgetGLJournalHelper budgetGlJournalHelper = new BudgetGLJournalHelper();
            var budgetSchedules = new List<BudgetAccountShedule>();
            var budgetSchedulesLines = db.BudgetAccountSheduleLines.ToList();
            var budgetSchedulesLinesAccount = db.BudgetLineSheduleAccounts.ToList();
            var accountShedules = db.BudgetAccountShedules.ToList();
            Branch legerBranch = new Branch();
            
            bool isBranch = false;
            int periodFrom = 1;
            int periodTo = 12;
            

            if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(FiscalYear))
            {
                int categoryType = int.Parse(type);
                int year = int.Parse(FiscalYear);
                SheduleCategoryType scheduleType = (SheduleCategoryType)Enum.ToObject(typeof(SheduleCategoryType), categoryType);


                if (!string.IsNullOrWhiteSpace(Branch))
                {
                    legerBranch = db.Branch.FirstOrDefault(x => x.BranchId.ToString() == Branch);
                    if (legerBranch != null)
                    {
                        isBranch = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(periodfrom))
                {
                    periodFrom = int.Parse(periodfrom);
                }


                if (!string.IsNullOrWhiteSpace(periodto))
                {
                    periodTo = int.Parse(periodto);
                }


                var scheudleCategorys = db.BudgetAccountSheduleCategories.Where(x => x.SheduleCategoryType == scheduleType).ToList();
                foreach (var scheudleCategory in scheudleCategorys)
                {
                    var schedule = accountShedules.Where(x => x.BudgetAccountSheduleCategoryId == scheudleCategory.BudgetAccountSheduleCategoryId).ToList();
                    if (schedule.Any())
                    {
                        budgetSchedules.AddRange(schedule);
                    }
                }

                List<GLLedgerPostingLines> quarterBalance = reportHelper
                    .GetAccountBalanceByPeriodRange(year, periodFrom, periodTo)
                    .Where(x => x.Type == LedgerPostTypes.Standard).ToList();

                List<GLLedgerPostingLines> beginningQuarterBalance = reportHelper.GetAccountBegBalance(year, periodFrom).ToList(); 

                List<GLLedgerPostingLines> endingQuarterBalance = reportHelper.GetAccountEndBalance(year, periodTo).ToList();



                foreach (var budgetSchedule in budgetSchedules)
                {
                    var budgetScheduleSubCategories = db.BudgetAccountSheduleSubCategories
                                                        .Where(x => x.BudgetAccountSheduleId == budgetSchedule.BudgetAccountSheduleId).ToList();

                    foreach (var subCat in budgetScheduleSubCategories)
                    {
                        var selectedbudgetSchedulesLines = budgetSchedulesLines
                        .Where(x => x.BudgetAccountSheduleSubCategoryId == subCat.BudgetAccountSheduleSubCategoryId).ToList();

                        foreach (var budgetSchedulesLine in selectedbudgetSchedulesLines)
                        {
                            var budgetSchedulesLineAccounts = budgetSchedulesLinesAccount
                                .Where(x => x.BudgetAccountSheduleLineId == budgetSchedulesLine.BudgetAccountSheduleLineId).ToList();


                            decimal total = decimal.Zero;
                            decimal beg = decimal.Zero;
                            decimal addition = decimal.Zero;
                            decimal collected = decimal.Zero;
                            decimal end = decimal.Zero;

                            if (budgetSchedulesLineAccounts.Any())
                            {

                                foreach (var range in budgetSchedulesLineAccounts)
                                {
                                    if (isBranch)
                                    {
                                        beg += reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(beginningQuarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);
                                        if (scheduleType == SheduleCategoryType.PAYABLES)
                                        {
                                            addition += reportHelper.GetAccountCreditBalancesWithSegmentRangeAndBranch(quarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);
                                            collected += reportHelper.GetAccountDebitBalancesWithSegmentRangeAndBranch(quarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);

                                        }
                                        else if (scheduleType == SheduleCategoryType.RECEIVABLES)
                                        {
                                            addition += reportHelper.GetAccountDebitBalancesWithSegmentRangeAndBranch(quarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);
                                            collected += reportHelper.GetAccountCreditBalancesWithSegmentRangeAndBranch(quarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);

                                        }

                                        end += reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(endingQuarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);
                                        // total += reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(quarterBalance, range.FromAccountCode, range.ToAccountCode, legerBranch.LocationAccount);

                                    }
                                    else
                                    {
                                        beg += reportHelper.GetAccountBegBalancesWithAllranges(beginningQuarterBalance, range.FromAccountCode, range.ToAccountCode);
                                        if (scheduleType == SheduleCategoryType.PAYABLES)
                                        {
                                            addition += reportHelper.GetAccountCreditBalancesWithSegmentRange(quarterBalance, range.FromAccountCode, range.ToAccountCode);
                                            collected += reportHelper.GetAccountDebitBalancesWithSegmentRange(quarterBalance, range.FromAccountCode, range.ToAccountCode);

                                        }
                                        else if (scheduleType == SheduleCategoryType.RECEIVABLES)
                                        {
                                            addition += reportHelper.GetAccountDebitBalancesWithSegmentRange(quarterBalance, range.FromAccountCode, range.ToAccountCode);
                                            collected += reportHelper.GetAccountCreditBalancesWithSegmentRange(quarterBalance, range.FromAccountCode, range.ToAccountCode);

                                        }
                                        end += reportHelper.GetAccountEndBalancesWithAllranges(endingQuarterBalance, range.FromAccountCode, range.ToAccountCode);
                                        // total += reportHelper.GetAccountEndBalancesWithAllranges(quarterBalance, range.FromAccountCode, range.ToAccountCode);
                                    }



                                }
                            }
                            total = beg + addition;
                            list.Add(new AccountPayableReceviableViewModel
                            {
                                ADDITION = addition,
                                BIG = beg,
                                COLLECTED = collected,
                                END = end,
                                Name = budgetSchedulesLine.Name,
                                TOTAL = total
                            });
                        }
                    }

                }
            }
            return list;
        }


    }
}
