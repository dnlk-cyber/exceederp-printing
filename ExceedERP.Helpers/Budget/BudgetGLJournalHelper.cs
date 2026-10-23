using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Budget
{
    public class BudgetGLJournalHelper
    {
        public List<GLLedgerPostingLines> GetGLPostingByQuarterEnding(int FiscalYearId, BudgetEnums.BudgetQuarter Quarter)
        {
            ReportHelper _ReportHelper = new ReportHelper();
           // FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();


            if (Quarter == BudgetEnums.BudgetQuarter.First)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 3));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Second)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 6));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Third)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 9));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Fourth)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 12));
            }

            return QuarterBalance;

        }
        public List<GLLedgerPostingLines> GetGLPostingByQuarterBeginning(int FiscalYearId, BudgetEnums.BudgetQuarter Quarter)
        {
            ReportHelper _ReportHelper = new ReportHelper();
           // FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();

            if (Quarter == BudgetEnums.BudgetQuarter.First)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 1));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Second)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 4));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Third)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 7));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Fourth)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 9));
            }

            return QuarterBalance;

        }
        public List<GLLedgerPostingLines> GetSinglePeriodGLPosting(int FiscalYearId, int Index)
        {
            ReportHelper _ReportHelper = new ReportHelper();

            return _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, Index));

           

        }

        public List<GLLedgerPostingLines> GetGLPostingByQuarter(int FiscalYearId, BudgetEnums.BudgetQuarter Quarter)
        {
            ReportHelper _ReportHelper = new ReportHelper();
           // FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();

            if (Quarter == BudgetEnums.BudgetQuarter.First)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 3));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Second)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 6));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Third)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 9));
            }

            if (Quarter == BudgetEnums.BudgetQuarter.Fourth)
            {
                QuarterBalance = _ReportHelper.GetAccountBalance(FiscalYearId, _ReportHelper.GetPeriodQuarterIndex(FiscalYearId, 12));
            }

            return QuarterBalance;

        }

        public decimal GetNetJournalsBalance(IEnumerable<GLRecordJournalDetail> JournalDetail)
        {
            decimal NetBalance = decimal.Zero;

            foreach (var budget in JournalDetail)
            {

                if (budget.AccountType == GlAccountTypes.Asset || budget.AccountType == GlAccountTypes.Expense)
                {
                    NetBalance = NetBalance + (budget.Debit - budget.Credit);
                }
                if (budget.AccountType == GlAccountTypes.Liability || budget.AccountType == GlAccountTypes.Capital || budget.AccountType == GlAccountTypes.Revenue)
                {
                    NetBalance = NetBalance + (budget.Credit - budget.Debit);
                }

            }
            return NetBalance;
        }

        public decimal GetJournalNetBalance(ICollection<GLRecordJournalDetail> gLRecordJournalDetails)
        {
 
         return   gLRecordJournalDetails.ToList().Sum(x => GetNetJournalBalance(x));

        }
        public decimal GetGLJournalUtilization(string accountCode, int fiscalYearId)
        {

            ExceedDbContext dbContext = new ExceedDbContext();
            var fiscalYear = dbContext.GLFiscalYears.Where(x => x.GlFiscalYearId == fiscalYearId).FirstOrDefault();

            if (fiscalYear != null)
            {

                var GlJournal = dbContext.GLRecordJournals.Where(x => x.IsOnlineApproved && !x.isVoid && x.GLRecordJournalDetail.Where(y => y.AccountCode == accountCode).Any() && x.FiscalYear == fiscalYear.Name).ToList();


                return GlJournal.Sum(x => GetJournalNetBalance(x.GLRecordJournalDetail));
            }

            return decimal.Zero;


        }

        public List<GLLedgerPostingLines> GetGLPostingByPeriodRange(int FiscalYearId, int periodfrom, int toperiod)
        {
            ReportHelper _ReportHelper = new ReportHelper();
            return _ReportHelper.GetAccountBalanceByPeriodRange(FiscalYearId,periodfrom, toperiod);

        }
        public void GetQuarterIndex(GlFiscalYear FiscalYear, BudgetEnums.BudgetQuarter BudgetQuarter, out int LastPeriodIndex, out int OffSet)
        {

            int OffSetIndex = 0;
            int LastIndex = 0;

            int PeriodRange = FiscalYear.GLPeriod.Count / 4;

            if (BudgetQuarter == BudgetEnums.BudgetQuarter.First)
            {
                LastIndex = PeriodRange;
                OffSetIndex = 0;
            }
            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Second)
            {
                LastIndex = PeriodRange * 2;
                OffSetIndex = LastIndex - PeriodRange;
            }
            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Third)
            {
                LastIndex = PeriodRange * 3;
                OffSetIndex = LastIndex - PeriodRange;
            }
            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Fourth)
            {
                LastIndex = PeriodRange * 4;
                OffSetIndex = LastIndex - PeriodRange;
            }

            OffSet = OffSetIndex;
            LastPeriodIndex = LastIndex;

        }
        public decimal GetQuarterBalance(IEnumerable<GLRecordJournalDetail> JournalDetails, GlFiscalYear FiscalYear, BudgetEnums.BudgetQuarter BudgetQuarter)
        {
            int OffSet = 0;
            int LastPeriodIndex = 0;
            decimal GrowthBalance = decimal.Zero;
            decimal CurrentBalance = decimal.Zero;

            GetQuarterIndex(FiscalYear, BudgetQuarter, out LastPeriodIndex, out OffSet);

            for (int i = OffSet; i < LastPeriodIndex; i++)
            {
                CurrentBalance = decimal.Zero;

                GLPeriod Period = FiscalYear.GLPeriod.ElementAtOrDefault(i);

                if (!Period.Equals(null))
                {
                    CurrentBalance = GetNetJournalsBalance(JournalDetails.Where(x => x.GLRecordJournal.PeriodName == Period.Name).ToList());
                }
                GrowthBalance = GrowthBalance + CurrentBalance;
            }
            return GrowthBalance;
        }
        public decimal GetNetJournalBalance(GLRecordJournalDetail JournalDetail)
        {
            decimal NetBalance = decimal.Zero;


            if (JournalDetail != null)
            {
                if (JournalDetail.AccountType == GlAccountTypes.Asset || JournalDetail.AccountType == GlAccountTypes.Expense)
                {
                    NetBalance = NetBalance + (JournalDetail.Debit - JournalDetail.Credit);
                }
                if (JournalDetail.AccountType == GlAccountTypes.Liability || JournalDetail.AccountType == GlAccountTypes.Capital || JournalDetail.AccountType == GlAccountTypes.Revenue)
                {
                    NetBalance = NetBalance + (JournalDetail.Credit - JournalDetail.Debit);
                }
            }

            return NetBalance;
        }



        public AccountRangeSummaryModel GetAccountRangeSummaryBalance(FinanceDashboard dashBoardRange, int fiscalYearId)
        {
            ExceedDbContext db = new ExceedDbContext();
            ReportHelper _reportHelper = new ReportHelper();

            BudgetGLJournalHelper _budgetGLJournalHelper = new BudgetGLJournalHelper();
            AccountRangeSummaryModel accountRangeSummaryModel = new AccountRangeSummaryModel();





            decimal firstQuarterTotalBalance = decimal.Zero;
            decimal secondQuarterTotalBalance = decimal.Zero;
            decimal thirdQuarterTotalBalance = decimal.Zero;
            decimal fourthQuarterTotalBalance = decimal.Zero;

        

            var fiscalyears = db.GLFiscalYears.Where(x => x.GlFiscalYearId <= fiscalYearId).OrderByDescending(x=>x.GlFiscalYearId).ToList();
            if (fiscalyears.Count >= 3)
            {
                fiscalyears = fiscalyears.Take(3).ToList();
            }
            if (dashBoardRange != null)
            {
                List<FinanceDashboardRange> dashRanges = new List<FinanceDashboardRange>();
                if (dashBoardRange.FinanceDashboardRanges != null)
                {
                    dashRanges = dashBoardRange.FinanceDashboardRanges.ToList();
                }

                foreach (var fiscalyear in fiscalyears)
                {
                    List<GLLedgerPostingLines> firstQuarterBalances = _budgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalyear.GlFiscalYearId, 1, 3);
                    List<GLLedgerPostingLines> secondQuarterBalances = _budgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalyear.GlFiscalYearId, 4, 6);
                    List<GLLedgerPostingLines> thirdQuarterBalances = _budgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalyear.GlFiscalYearId, 7, 9);
                    List<GLLedgerPostingLines> fourthQuarterBalances = _budgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalyear.GlFiscalYearId, 10, 12);


                    foreach (var Range in dashRanges)
                    {
                        firstQuarterTotalBalance +=  _reportHelper.GetAccountEndBalancesWithAllranges(firstQuarterBalances, Range.AccountFrom, Range.AccountTo);
                        secondQuarterTotalBalance +=    _reportHelper.GetAccountEndBalancesWithAllranges(secondQuarterBalances, Range.AccountFrom, Range.AccountTo);
                        thirdQuarterTotalBalance +=  _reportHelper.GetAccountEndBalancesWithAllranges(thirdQuarterBalances, Range.AccountFrom, Range.AccountTo);
                        fourthQuarterTotalBalance +=  _reportHelper.GetAccountEndBalancesWithAllranges(fourthQuarterBalances, Range.AccountFrom, Range.AccountTo);

                    }

                    Random r = new Random();

                    accountRangeSummaryModel.fiscalYear.Add(fiscalyear.Name);
                    accountRangeSummaryModel.firstvalue.Add(new RatioModel { Actual =r.Next(435)*fiscalyear.GlFiscalYearId , FiscalYear = fiscalyear.Name });
                    accountRangeSummaryModel.secondvalue.Add(new RatioModel { Actual = r.Next(234) * fiscalyear.GlFiscalYearId, FiscalYear = fiscalyear.Name });
                    accountRangeSummaryModel.thirdvalue.Add(new RatioModel { Actual = r.Next(124) * fiscalyear.GlFiscalYearId, FiscalYear = fiscalyear.Name });
                    accountRangeSummaryModel.fourthvalue.Add(new RatioModel { Actual = 2 * fiscalyear.GlFiscalYearId, FiscalYear = fiscalyear.Name });
                }

            }

            else
            {
                if (fiscalyears.Any())
                {
                    foreach (var fiscalyear in fiscalyears)
                    {
                        accountRangeSummaryModel.fiscalYear.Add(string.Empty);
                        accountRangeSummaryModel.firstvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                        accountRangeSummaryModel.secondvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                        accountRangeSummaryModel.thirdvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                        accountRangeSummaryModel.fourthvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                    }
                }
                else
                {
                    accountRangeSummaryModel.fiscalYear.Add(string.Empty);
                    accountRangeSummaryModel.firstvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                    accountRangeSummaryModel.secondvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                    accountRangeSummaryModel.thirdvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                    accountRangeSummaryModel.fourthvalue.Add(new RatioModel { Actual = thirdQuarterTotalBalance, FiscalYear = string.Empty });
                }
            }


            return accountRangeSummaryModel;
        }


    }
}
