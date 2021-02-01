using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Finance
{
    public partial class ReportHelper
    {
        

        
       
        public  decimal GetPostedAccountLatestBalance(string accountCombination, List<GLLedgerPostingLines> postingLines)
        {
            decimal balance = 0;
            if (postingLines != null)
            {
                var accountTransaction = postingLines.Where(x => x.GLLedgerPosting.AccountCode == accountCombination)
                    .ToList();
                if (accountTransaction.Any())
                {
                    var maxId = accountTransaction.Max(x => x.GLLedgerPostingLinesID);
                    var latestRecord = accountTransaction.SingleOrDefault(x => x.GLLedgerPostingLinesID == maxId);
                    if (latestRecord != null)
                        balance = latestRecord.Balance;
                }
            }

            return balance;
        }

       
       
        public  decimal GetAccountStandardBalances(List<GLLedgerPostingLines> yearBalance, string account)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.GLLedgerPosting.AccountSegment == account).Where(x => x.Type == LedgerPostTypes.Standard).ToList();
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public  decimal GetAccountBegBalances(List<GLLedgerPostingLines> yearBalance, string account)
        {
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.GLLedgerPosting.AccountSegment == account).Where(x => x.Type == LedgerPostTypes.Begining).ToList();
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        
        //by branch
        public  decimal GetAccountBegBalancesWithBranch(List<GLLedgerPostingLines> yearBalance, string account, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

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
        public  decimal GetAccountDebitBalancesWithBranch(List<GLLedgerPostingLines> yearBalance, string account, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.Type == LedgerPostTypes.Standard).ToList();

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


                accountyearEndBalances = balanceObj.Sum(x => x.Debit);
            }

            return accountyearEndBalances;
        }
        public  decimal GetAccountCreditBalancesWithBranch(List<GLLedgerPostingLines> yearBalance, string account, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Where(x => x.Type == LedgerPostTypes.Standard).ToList();

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


                accountyearEndBalances = balanceObj.Sum(x => x.Credit);
            }

            return accountyearEndBalances;
        }

      
        //by segment and branch
        public decimal GetAccountBegBalancesWithSegmentAndBranch(List<GLLedgerPostingLines> yearBalance, string s1, string s2, string s3, string s4, string s5, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
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

                 balanceObj = balanceObj
                    .Where(x => x.GLLedgerPosting.Segment1 == s1)
                    .Where(x => x.GLLedgerPosting.Segment2 == s2)
                    .Where(x => x.GLLedgerPosting.Segment3 == s3)
                    .Where(x => x.GLLedgerPosting.Segment4 == s4)
                    .Where(x => x.GLLedgerPosting.Segment5 == s5)
                    .Where(x => x.Type == LedgerPostTypes.Begining).ToList();


                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountEndBalancesWithSegmentAndBranch(List<GLLedgerPostingLines> yearBalance, string s1, string s2, string s3, string s4, string s5, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
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

                balanceObj = balanceObj
                    .Where(x => x.GLLedgerPosting.Segment1 == s1)
                    .Where(x => x.GLLedgerPosting.Segment2 == s2)
                    .Where(x => x.GLLedgerPosting.Segment3 == s3)
                    .Where(x => x.GLLedgerPosting.Segment4 == s4)
                    .Where(x => x.GLLedgerPosting.Segment5 == s5)
                   .Where(x => x.Type == LedgerPostTypes.Ending).ToList();


                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountDebitBalancesWithSegmentAndBranch(List<GLLedgerPostingLines> yearBalance, string s1, string s2, string s3, string s4, string s5, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
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

                balanceObj = balanceObj
                    .Where(x => x.GLLedgerPosting.Segment1 == s1)
                    .Where(x => x.GLLedgerPosting.Segment2 == s2)
                    .Where(x => x.GLLedgerPosting.Segment3 == s3)
                    .Where(x => x.GLLedgerPosting.Segment4 == s4)
                    .Where(x => x.GLLedgerPosting.Segment5 == s5)
                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                accountyearEndBalances = balanceObj.Sum(x => x.Debit);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountCreditBalancesWithSegmentAndBranch(List<GLLedgerPostingLines> yearBalance, string s1, string s2, string s3, string s4, string s5, string branchAccount)
        {
            _accountHelper = new AccountHelper();
            decimal accountyearEndBalances = 0;
            if (yearBalance.Any())
            {

                //int index = _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.ToList();
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

                balanceObj = balanceObj
                    .Where(x => x.GLLedgerPosting.Segment1 == s1)
                    .Where(x => x.GLLedgerPosting.Segment2 == s2)
                    .Where(x => x.GLLedgerPosting.Segment3 == s3)
                    .Where(x => x.GLLedgerPosting.Segment4 == s4)
                    .Where(x => x.GLLedgerPosting.Segment5 == s5)
                   .Where(x => x.Type == LedgerPostTypes.Standard).ToList();


                accountyearEndBalances = balanceObj.Sum(x => x.Credit);
            }

            return accountyearEndBalances;
        }

     
    }
}
