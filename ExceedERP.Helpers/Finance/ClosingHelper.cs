using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Helpers.Finance
{
    public  class ClosingHelper
    {
        private  readonly ExceedDbContext _db = new ExceedDbContext();
        AccountHelper accountHelper = new AccountHelper();

        public void UpdateEarningForAYear(int fiscalYear)
        {
            var months = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear).OrderBy(x=>x.DateTo).ToList();
            foreach (var glPeriod in months)
            {
                UpdateEarning(glPeriod.GLPeriodId);
            }
        }
        private void UpdateEarning(int period)
        {
            var earningAccount = _db.GlRetainedEarningAccounts.FirstOrDefault();
            if (earningAccount != null)
            {
                var earningLedger = _db.GlEarningLedgers.FirstOrDefault(x=>x.AccountCode==earningAccount.AccountCode);
                if (earningLedger != null)
                {
                    #region lines 

                    var expense = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == period)
                        .Where(x=>x.Type==LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

                    var revenue = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == period)
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

                    var glperiod = _db.GLPeriods.Find(period);
                    if (glperiod != null)
                    {
                        int year = glperiod.GlFiscalYearId;
                        int month = period;
                        decimal monthlyAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);
                        //decimal yearlyAmount = GetUptodateYearlyEarning(period);
                        decimal yearlyAmount = monthlyAmount;

                        var lineRecord = _db.GlEarningLedgerLines
                            .Where(x => x.GlEarningLedger.AccountCode == earningAccount.AccountCode)
                            .Where(x => x.GlFiscalYearId == year).FirstOrDefault(x => x.GLPeriodId == month);
                        if (lineRecord != null)
                        {
                            #region update

                            lineRecord.MonthlyAmount = monthlyAmount;
                            lineRecord.YearlyAmount = yearlyAmount;
                            _db.GlEarningLedgerLines.Attach(lineRecord);
                            _db.Entry(lineRecord).State = EntityState.Modified;
                            _db.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region new 
                            var line = new GlEarningLedgerLine
                            {
                                GlEarningLedger = earningLedger,
                                GlFiscalYearId = year,
                                GLPeriodId = month,
                                MonthlyAmount = monthlyAmount,
                                YearlyAmount = yearlyAmount
                            };
                            _db.GlEarningLedgerLines.Add(line);
                            _db.SaveChanges();

                            #endregion
                        }
                    }

                    #endregion
                }
                else
                {
                    #region header

                    earningLedger = new GlEarningLedger
                    {
                        AccountCode = earningAccount.AccountCode,
                        AccountDesc = earningAccount.AccountDesc,
                        Type = EarningTypes.RetainedEarning,
                        AccountSegment = "",
                        SubAccountSegment = "",
                        BranchSegment = "",
                        CostCenterSegment = "",
                        ChannelSegment = ""
                    };

                    _db.GlEarningLedgers.Add(earningLedger);
                    _db.SaveChanges();
                    #endregion
                    #region lines 

                    var expense = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == period)
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

                    var revenue = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == period)
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

                    var glperiod = _db.GLPeriods.Find(period);
                    if (glperiod != null)
                    {
                        int year = glperiod.GlFiscalYearId;
                        int month = period;
                        decimal monthlyAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance); 
                        //decimal yearlyAmount = GetUptodateYearlyEarning(period);
                        decimal yearlyAmount = monthlyAmount;

                        var line = new GlEarningLedgerLine
                        {
                            GlEarningLedger = earningLedger,
                            GlFiscalYearId = year,
                            GLPeriodId = month,
                            MonthlyAmount = monthlyAmount,
                            YearlyAmount = yearlyAmount
                        };
                        _db.GlEarningLedgerLines.Add(line);
                        _db.SaveChanges();
                    }

                    #endregion
                }
            }
            else
            {
                Logger.Debug("Retained Earning Account not configured");
            }
        }

        //not used
        private decimal GetUptodateYearlyEarning(int period)
        {
            decimal yearlyAmount = 0;

            var glperiod = _db.GLPeriods.Find(period);
            if (glperiod != null)
            {
                var periods = _db.GLPeriods.Where(x => x.DateTo <= glperiod.DateTo)
                    .Where(x => x.GlFiscalYearId == glperiod.GlFiscalYearId).Select(x=>x.GLPeriodId).ToList();

                var expense = _db.GLLedgerPostingLines.Where(x =>periods.Contains(x.GLPeriodId))
                    .Where(x => x.Type == LedgerPostTypes.Standard)
                    .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

                var revenue = _db.GLLedgerPostingLines.Where(x => periods.Contains(x.GLPeriodId))
                    .Where(x=>x.Type==LedgerPostTypes.Standard)
                    .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

                yearlyAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);
            }

            return yearlyAmount;
        }


        private decimal GetEarningBeforeTaxByQuarter(int fiscalYear, int index)
        {
            ReportHelper helper = new ReportHelper();
            decimal earning = 0;
            int q1Period = helper.GetPeriodQuarterIndex(fiscalYear, 3);
            int q2Period = helper.GetPeriodQuarterIndex(fiscalYear, 6);
            int q3Period = helper.GetPeriodQuarterIndex(fiscalYear, 9);
            int q4Period = helper.GetPeriodQuarterIndex(fiscalYear, 12);
            switch (index)
            {
                case 1:
                   

                    var balances = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q1Period);

                    if (balances != null) earning = balances.YearlyAmount;
                    break;
                case 2:
                   

                    var balance1 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q1Period);
                    var balance2 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q2Period);

                    if (balance1 != null &&balance2!=null) earning = balance2.YearlyAmount- balance1.YearlyAmount;
                    break;
                case 3:
                   

                    var bal2 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q2Period);
                    var balance3 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q3Period);

                    if (bal2 != null && balance3 != null) earning = balance3.YearlyAmount - bal2.YearlyAmount;
                    break;
                case 4:
                    var bal3 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q3Period);
                    var balance4 = _db.GlEarningLedgerLines
                        .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                        .FirstOrDefault(x => x.GLPeriodId == q4Period);

                    if (bal3 != null && balance4 != null) earning = balance4.YearlyAmount - bal3.YearlyAmount;
                    break;
            }
            return earning;
        }
        private decimal GetEarningBeforeTax(int fiscalYear, int period)
        {
            decimal earning = 0;

            if (period != 0)
            {
                var balances = _db.GlEarningLedgerLines
                    .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                    .FirstOrDefault(x => x.GLPeriodId == period);

                if (balances != null) earning = balances.YearlyAmount;
            }
            else
            {
                FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
                int lastMonth = fiscalYearHelper.GetFiscalYearLastPeriod(fiscalYear);

                var balances = _db.GlEarningLedgerLines
                    .Where(x => x.GlEarningLedger.Type == EarningTypes.RetainedEarning)
                    .FirstOrDefault(x => x.GLPeriodId == lastMonth);

                if (balances != null) earning = balances.YearlyAmount;

            }
            return earning;
        }

        public decimal GetCurrentEarningByQuarter(int fiscalYear, int index)
        {
            ReportHelper helper=new ReportHelper();
            ;
            decimal earning = 0;
            decimal q1Earning = 0;
            decimal q2Earning = 0;
            decimal q3Earning = 0;
            decimal q4Earning = 0;
            switch (index)
            {
                case 1:
                    earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 3));
                    break;
                case 2:
                    q1Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 3));
                    q2Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 6));
                    earning = q2Earning - q1Earning;
                    break;
                case 3:
                    q2Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 6));
                    q3Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 9));
                    earning = q3Earning - q2Earning;
                    break;
                case 4:
                     q3Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 9));
                     q4Earning = GetCurrentEarning(fiscalYear, helper.GetPeriodQuarterIndex(fiscalYear, 12));
                    earning = q4Earning - q3Earning;
                    break;
            }
            return earning;
        }
        public decimal GetCurrentEarning(int fiscalYear, int period)
        {
            decimal earning = 0;
            decimal profittax = 0;
            decimal profitBeforeTax = 0;
            profitBeforeTax = GetEarningBeforeTax(fiscalYear, period);
            profittax = GetCurrentProfitTax(fiscalYear, period);
            earning = profitBeforeTax - profittax;
            return earning;
        }

        public decimal GetCurrentProfitTaxByQuarter(int fiscalYear, int index)
        {
            decimal profittax = 0;
            decimal profitBeforeTax = 0;
            var profittaxRate = _db.FinanceSettings.FirstOrDefault();
            decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;
            profitBeforeTax = GetEarningBeforeTaxByQuarter(fiscalYear, index);
            profittax = profitBeforeTax * rate;
            return profittax;
        }
        public decimal GetCurrentProfitTax(int fiscalYear, int period)
        {
            decimal profittax = 0;
            decimal profitBeforeTax = 0;
            var profittaxRate = _db.FinanceSettings.FirstOrDefault();
            decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;

            profitBeforeTax = GetEarningBeforeTax(fiscalYear, period);
            profittax = profitBeforeTax * rate;
            return profittax;
        }

    }
}
