using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Finance
{
    public partial class ReportHelper
    {
        private  readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        private readonly DynamicAccountHelper _dynamicAccountHelper;
        private readonly FiscalYearHelper _fiscalYearHelper;
        public ReportHelper()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
            _dynamicAccountHelper = new DynamicAccountHelper();
            _fiscalYearHelper = new FiscalYearHelper();
        }

        #region Balance Sheet Report Helper

        public  decimal GetAccountRangeAssetBalanceSheet(List<GaapBalanceSheetAssetRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {
        
            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                var accounts = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                foreach (var account in accounts)
                {

                    totalBalance = totalBalance + GetAccountEndBalances(currentBalance, account);


                }
            }
            return totalBalance;
        }

        public  decimal GetAccountRangeCapitalBalanceSheet(List<GaapBalanceSheetCapitalRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {
            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                var accounts = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                foreach (var account in accounts)
                {

                    totalBalance = totalBalance +GetAccountEndBalances(currentBalance, account);


                }
            }
            return totalBalance;
        }

        public  decimal GetAccountRangeLiabilityBalanceSheet(List<GaapBalanceSheetLiabilityRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {
            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                var accounts = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                foreach (var account in accounts)
                {

                    totalBalance = totalBalance + GetAccountEndBalances(currentBalance, account);


                }
            }
            return totalBalance;
        }


        #endregion

        #region Income Statemrent Report Helper

        public  decimal GetAccountRangeIncomeRevenues(List<GaapIncomeStatementOperationRevenueRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {
            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                var accountCodes = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                foreach (var accountCode in accountCodes)
                {

                    totalBalance = totalBalance + GetAccountEndBalances(currentBalance, accountCode);


                }
            }
            return totalBalance;
        }

        public  decimal GetAccountRangeIncomeExpenses(List<GaapIncomeStatementOperationExpenseRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {
            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                var accountCodes = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                foreach (var accountCode in accountCodes)
                {

                    totalBalance = totalBalance + GetAccountEndBalances(currentBalance, accountCode);


                }
            }
            return totalBalance;
        }



        #endregion
        public  decimal GetAccountRangeBalance(List<FinanceDashboardRange> ranges, List<GLLedgerPostingLines> currentBalance)
        {

            decimal totalBalance = decimal.Zero;

            foreach (var range in ranges)
            {
                //var accounts = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo, GlSegmentTypes.Account);

                //foreach (var account in accounts)
                //{

                //    totalBalance = totalBalance + GetAccountEndBalances(currentBalance, account);

                //}
                totalBalance = totalBalance + GetAccountEndBalancesWithAllranges(currentBalance, range.AccountFrom,range.AccountTo);
            }
            return totalBalance;
        }

       
        public  List<GLLedgerPostingLines> GetAccountBalance(int fiscalyear,int month=0)
        {
            //if zero end of year
            if (month == 0)
            {
                month = _fiscalYearHelper.GetFiscalYearLastPeriod(fiscalyear);
            }
            return _db.GLLedgerPostingLines
                .Where(x => x.GLPeriodId == month).AsNoTracking().ToList();
        }

    

        public List<GLLedgerPostingLines> GetAccountEndBalance(int fiscalyear, int month = 0)
        {
            //if zero end of year
            if (month == 0)
            {
                month = _fiscalYearHelper.GetFiscalYearLastPeriod(fiscalyear);
            }
            return _db.GLLedgerPostingLines.Where(x => x.Type == LedgerPostTypes.Ending).Where(x => x.GLPeriodId == month).AsNoTracking().ToList();
        }

        public List<GLLedgerPostingLines> GetAccountBegBalance(int fiscalyear, int month = 0)
        {
            //if zero beg of year
            if (month == 0)
            {
                month = _fiscalYearHelper.GetFiscalYearFirstPeriod(fiscalyear);
            }
            return _db.GLLedgerPostingLines.Where(x=>x.Type==LedgerPostTypes.Begining).Where(x => x.GLPeriodId == month).AsNoTracking().ToList();
        }

        public List<GLLedgerPostingLines> GetAccountBalanceByPeriodRange(int fiscalyear, int fromMonth,int toMonth)
        {
            var fromDate = _db.GLPeriods.Find(fromMonth)?.DateFrom.Date;
            var toDate = _db.GLPeriods.Find(toMonth)?.DateTo.Date;
            var periodList = _db.GLPeriods.Where(x => x.DateFrom >= fromDate)
                .Where(x => x.DateTo <= toDate).Select(x => x.GLPeriodId).ToList();
            
            var balances = _db.GLLedgerPostingLines.Where(x =>periodList.Contains(x.GLPeriodId)).AsNoTracking().ToList();
            var yearEndBalances = balances;

            return yearEndBalances;
        }

        public  decimal GetAccountEndBalances(List<GLLedgerPostingLines> yearBalance,string account)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.GLLedgerPosting.AccountSegment == account).Where(x=>x.Type==LedgerPostTypes.Ending).ToList();
                    accountyearEndBalances = balanceObj.Sum(x=>x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountEndBalancesbyAccountAndBizUnit(List<GLLedgerPostingLines> yearBalance, string account,string bizAccountCode)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.GLLedgerPosting.BusinessUnit == bizAccountCode)
                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public decimal GetAccountEndBalanceWithFullSegment(List<GLLedgerPostingLines> yearBalance, string acctComb)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.GLLedgerPosting.AccountCode == acctComb).Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }



        public  decimal GetAccountEndBalancesWithBranch(List<GLLedgerPostingLines> yearBalance, string account, string branchAccount)
        {
     
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;
                   
                }

               
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
       
        //sub account
        public  decimal GetAccountEndBalancesWithSubAccount(List<GLLedgerPostingLines> yearBalance, string account,string subaccount)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount)
                    .Where(x => x.Type == LedgerPostTypes.Ending);
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public  decimal GetAccountBegBalancesWithSubAccount(List<GLLedgerPostingLines> yearBalance, string account, string subaccount)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount)
                    .Where(x => x.Type == LedgerPostTypes.Begining);
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        
        //by segment and branch
        public decimal GetAccountBegBalancesWithSegmentRangeAndBranch(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

            
            decimal alances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        seg1From = branchAccount;
                        seg1To = branchAccount;
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        seg2From = branchAccount;
                        seg2To = branchAccount;
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        seg3From = branchAccount;
                        seg3To = branchAccount;
                        break;
                    case 3:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment4 == branchAccount).ToList();
                        seg4From = branchAccount;
                        seg4To = branchAccount;
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        seg5From = branchAccount;
                        seg5To = branchAccount;
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                   .Where(x => x.Type == LedgerPostTypes.Begining).ToList();


                alances = balanceObj.Sum(x => x.Balance);
            }

            return alances;
        }
        public decimal GetAccountEndBalancesWithSegmentRangeAndBranch(string fromAccountCode, List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

           
            decimal balances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        seg1From = branchAccount;
                        seg1To = branchAccount;
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        seg2From = branchAccount;
                        seg2To = branchAccount;
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        seg3From = branchAccount;
                        seg3To = branchAccount;
                        break;
                    case 3:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment4 == branchAccount).ToList();
                        seg4From = branchAccount;
                        seg4To = branchAccount;
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        seg5From = branchAccount;
                        seg5To = branchAccount;
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                balanceObj = balanceObj
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                        .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                        .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                        .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                        .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                        .Where(x => x.Type == LedgerPostTypes.Ending).ToList();


                balances = balanceObj.Sum(x => x.Balance);
            }

            return balances;
        }
        public decimal GetAccountStandardBalancesWithSegmentRangeAndBranch(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

           
            decimal balances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        seg1From = branchAccount;
                        seg1To = branchAccount;
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        seg2From = branchAccount;
                        seg2To = branchAccount;
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        seg3From = branchAccount;
                        seg3To = branchAccount;
                        break;
                    case 3:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment4 == branchAccount).ToList();
                        seg4From = branchAccount;
                        seg4To = branchAccount;
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        seg5From = branchAccount;
                        seg5To = branchAccount;
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                balances = balanceObj.Sum(x => x.Balance);
            }

            return balances;
        }

        public decimal GetAccountDebitBalancesWithSegmentRange(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

           
            decimal alances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
       
                var balanceObj = yearBalance.ToList();
                

                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)

                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                alances = balanceObj.Sum(x => x.Debit);
            }

            return alances;
        }
        public decimal GetAccountCreditBalancesWithSegmentRange(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

            
            decimal alances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);

                var balanceObj = yearBalance.ToList();


                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)

                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                alances = balanceObj.Sum(x => x.Credit);
            }

            return alances;
        }
        public decimal GetAccountDebitBalancesWithSegmentRangeAndBranch(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

            
            decimal alances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        seg1From = branchAccount;
                        seg1To = branchAccount;
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        seg2From = branchAccount;
                        seg2To = branchAccount;
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        seg3From = branchAccount;
                        seg3To = branchAccount;
                        break;
                    case 3:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment4 == branchAccount).ToList();
                        seg4From = branchAccount;
                        seg4To = branchAccount;
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        seg5From = branchAccount;
                        seg5To = branchAccount;
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)

                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                alances = balanceObj.Sum(x => x.Debit);
            }

            return alances;
        }
        public decimal GetAccountCreditBalancesWithSegmentRangeAndBranch(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            string seg1From = fromAccCom.Split('-')[0];
            string seg1To = toAccCom.Split('-')[0];

            string seg2From = fromAccCom.Split('-')[1];
            string seg2To = toAccCom.Split('-')[1];

            string seg3From = fromAccCom.Split('-')[2];
            string seg3To = toAccCom.Split('-')[2];

            string seg4From = fromAccCom.Split('-')[3];
            string seg4To = toAccCom.Split('-')[3];

            string seg5From = fromAccCom.Split('-')[4];
            string seg5To = toAccCom.Split('-')[4];

            
            decimal alances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        seg1From = branchAccount;
                        seg1To = branchAccount;
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        seg2From = branchAccount;
                        seg2To = branchAccount;
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        seg3From = branchAccount;
                        seg3To = branchAccount;
                        break;
                    case 3:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment4 == branchAccount).ToList();
                        seg4From = branchAccount;
                        seg4To = branchAccount;
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        seg5From = branchAccount;
                        seg5To = branchAccount;
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                balanceObj = balanceObj
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                alances = balanceObj.Sum(x => x.Credit);
            }

            return alances;
        }


        public int GetPeriodQuarterIndex(int fiscalyear, int index)
        {
            int GLPeriodId = 0;
            var period = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalyear).FirstOrDefault(x => x.Index == index);
            if (period != null)
            {
                return period.GLPeriodId;
            }
            return GLPeriodId;
        }

        public decimal GetEarningByBranch(string branchCode,int periodid)
        {           

            decimal earningAmount = 0;
            
            var expense = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == periodid)
                         .Where(x => x.GLLedgerPosting.BranchSegment == branchCode)
                         .Where(x => x.Type == LedgerPostTypes.Ending)
                         .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

            var revenue = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == periodid)
                        .Where(x => x.GLLedgerPosting.BranchSegment == branchCode)
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

            earningAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);

            return earningAmount;
            
        }

        public decimal GetEarningByBusinessUnit(string bizunitCode, int periodid)
        {

            decimal earningAmount = 0;

            var expense = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == periodid)
                         .Where(x => x.GLLedgerPosting.BusinessUnit == bizunitCode)
                         .Where(x => x.Type == LedgerPostTypes.Ending)
                         .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

            var revenue = _db.GLLedgerPostingLines.Where(x => x.GLPeriodId == periodid)
                        .Where(x => x.GLLedgerPosting.BusinessUnit == bizunitCode)
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

            earningAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);

            return earningAmount;

        }

        public decimal GetProfitTax(decimal amount)
        {
            decimal earningAmount = 0;
            decimal profittax = 0;
            var profittaxRate = _db.FinanceSettings.FirstOrDefault();

            decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;
            profittax = earningAmount * rate;

            return profittax;           
        }
    }
}
