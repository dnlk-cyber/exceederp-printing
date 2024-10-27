using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.Common;
using System.Runtime.Caching;

namespace ExceedERP.Helpers.Finance
{
    public class AccountHelper
    {
        private readonly ExceedDbContext _db ;
        private readonly DynamicAccountHelper _dynamicAccount;
        private readonly CacheData _cache;

        public AccountHelper()
        {
            _db = new ExceedDbContext();
            _dynamicAccount = new DynamicAccountHelper();
            _cache = new CacheData();
        }
        public GlAccountTypes GetAccountType(string accountAcode)
        {
            GlAccountTypes type = GlAccountTypes.Undefined;
            try
            {
                string[] segments = accountAcode.Split('-');
                int index = _dynamicAccount.GetNaturalAccountSequenceIndex();
                string acccountSeG = segments[index];
                var accountObj = _db.GLChartOfAccountAccounts.FirstOrDefault(x => x.Values.ToUpper() == acccountSeG.ToUpper());
                if (accountObj != null) type = accountObj.AccountType;
            }
            catch (Exception x)
            {
                //
            }
            return type;
        }

        public int GetAccountCombLength()
        {
            return _db.GlSegmentSetups.Count(x => x.Enabled);
        }

        public string GetAccountDesc(string accountAcode)
        {
            var sequence = _cache.GetSegmentSetupCaches();// _db.GlSegmentSetups.Where(x => x.Enabled).AsNoTracking().ToList();

            if (sequence.Any() && !string.IsNullOrEmpty(accountAcode))
            {
                var otherSegments =_cache.GetChartOfAccountSegmentCaches();// _db.GlChartOfAccountSegments.Select(x => new { x.GLSegmentSetupId, x.Values, x.Description }).AsNoTracking().ToList();
                var accountSegments =_cache.GetOfAccountAccountCaches();// _db.GLChartOfAccountAccounts.Select(x => new { x.Values, x.Description }).AsNoTracking().ToList();

                string desc = "";
                string[] segments = accountAcode.Split('-');

                for (int i = 0; i < segments.Length; i++)
                {
                    var specificSegment = sequence.FirstOrDefault(x => x.Sequence == i + 1);
                    if (specificSegment != null && specificSegment.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        var value = accountSegments.FirstOrDefault(x => x.Values == segments[i]);
                        if (value != null) desc = desc + "-" + value.Description;
                    }
                    else if (specificSegment != null && specificSegment.SegmentType != DynamicSegmentTypes.NaturalAccount)
                    {
                        var value = otherSegments.Where(x => x.GLSegmentSetupId == specificSegment.GLSegmentSetupId).FirstOrDefault(x => x.Values == segments[i]);
                        if (value != null) desc = desc + "-" + value.Description;

                    }
                }

                return desc.Remove(0, 1);
            }
            return "";
        }

        public bool IsBankAccount(string accountcode)
        {
            string account = GetNaturalAccountSegment(accountcode);
            string subAccount = GetSubAccountSegment(accountcode);

            using (var context = new ExceedDbContext())
            {
                var banckaccounts = context.BankAccounts.ToList();
                foreach (var bank in banckaccounts)
                {
                    string baccount = GetNaturalAccountSegment(bank.AccountCode);
                    string bsubAccount = GetSubAccountSegment(bank.AccountCode);

                    if (account == baccount && subAccount == bsubAccount)
                        return true;
                }

            }

            return false;
        }

        //public bool IsSameBankAccount_1(string accountcode, string bankAccountCode)
        //{
        //    string account = GetNaturalAccountSegment(accountcode);
        //    string subAccount = GetSubAccountSegment(accountcode);

        //    string baccount = GetNaturalAccountSegment(bankAccountCode);
        //    string bsubAccount = GetSubAccountSegment(bankAccountCode);

        //    if (account == baccount && subAccount == bsubAccount)
        //        return true;

        //    return false;
        //}
        //public bool IsBankAccount2(string accountcode)
        //{

        //    using (var context = new ExceedDbContext())
        //    {
        //        var banckaccounts = context.BankAccounts.Where(x => x.AccountCode == accountcode).ToList();
        //        if (banckaccounts.Any())
        //            return true;
        //    }

        //    return false;
        //}
        //delegated
        //public bool IsBankAccount1(string accountcode)
        //{
        //    return _dynamicAccount.IsBankAccount1(accountcode);
        //}
        //different implementation
        //public static int GetSegmentLength(GlSegmentTypes segment)
        //{
        //    int length = 0;

        //    using (var context = new ExceedDbContext())
        //    {
        //        var segmentLength = context.GlSegmentLengths.FirstOrDefault();
        //        if (segmentLength != null)
        //        {
        //            if (segment == GlSegmentTypes.Account)
        //                length = segmentLength.AccountLength;
        //            else if (segment == GlSegmentTypes.Location)
        //                length = segmentLength.LocationLength;
        //            else if (segment == GlSegmentTypes.CostCenter)
        //                length = segmentLength.CostCenterLength;
        //            else if (segment == GlSegmentTypes.SubAccount)
        //                length = segmentLength.SubAccountLength;
        //        }
        //    }

        //    return length;
        //}

        //diff --> only distribution helper remains
        public int GetAccountSequenceIndex(GlSegmentTypes segment)
        {
            int index = 12;//out of range

            switch (segment)
            {
                case GlSegmentTypes.Account:
                    index = _dynamicAccount.GetNaturalAccountSequenceIndex();
                    break;
                case GlSegmentTypes.Location:
                    index = _dynamicAccount.GetBranchAccountSequenceIndex();
                    break;
                case GlSegmentTypes.CostCenter:
                    index = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                    break;
                case GlSegmentTypes.SubAccount:
                    index = _dynamicAccount.GetSubAccountSequenceIndex();
                    break;
            }


            return index;
        }

        public static string GetAccountSegment(string accountCombination, int index)
        {
            return accountCombination.Split('-')[index]; ;
        }
        //same
        //public static decimal GeAccountLastBalance(List<GLLedgerPostingLines> records, string accountCode)
        //{
        //    decimal balance = 0;
        //    var rangebalances = records.Where(x => x.GLLedgerPosting.AccountCode.ToUpper().Equals(accountCode)).ToList();
        //    foreach (var rangebalance in rangebalances)
        //    {
        //        int maxid =
        //            rangebalances.Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode)
        //                .Max(x => x.GLLedgerPostingLinesID);
        //        var lastbalance =
        //            rangebalances
        //                .Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode).FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
        //        if (lastbalance != null)
        //            balance = balance + lastbalance.Balance;
        //    }

        //    return balance;
        //}

        public List<string> GeAccountRange(string fromAccountCombination, string toAccountCombination, GlSegmentTypes segment)
        {
            AccountHelper accountHelper = new AccountHelper();
            List<string> accountRange = null;

            int index = accountHelper.GetAccountSequenceIndex(segment);
            var fromSegment = GetAccountSegment(fromAccountCombination, index);
            var toSegment = GetAccountSegment(toAccountCombination, index);

            if (segment == GlSegmentTypes.Account)
            {
                accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                       _dynamicAccount.GetNaturalAccountSequenceIndex());
            }

            else if (segment == GlSegmentTypes.Location)
            {
                accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                     _dynamicAccount.GetBranchAccountSequenceIndex());
            }

            else if (segment == GlSegmentTypes.CostCenter)
            {
                accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                       _dynamicAccount.GetCostCenterAccountSequenceIndex());
            }

            else if (segment == GlSegmentTypes.SubAccount)
            {
                accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                       _dynamicAccount.GetSubAccountSequenceIndex());
            }




            return accountRange;
        }



        //needed here only
        public string RearrangeAccountSequence(string account, string location, string costcenter, string subaccount)
        {


            //_companyGlobal = location;
            //_accountGlobal = account;
            //_costcenterGlobal = costcenter;
            //_subaccountGlobal = subaccount;

            string accountCombination = "";
            //using (var context = new ExceedDbContext())
            //{
            //    var segmentSequence = context.GlSegmentSequences.FirstOrDefault();
            //    if (segmentSequence != null)
            //    {
            //        var first = segmentSequence.FirstSegment;
            //        var second = segmentSequence.SecondSegment;
            //        var third = segmentSequence.ThirdSegment;
            //        var fourth = segmentSequence.FourthSegment;

            //        //get the sequence
            //        _firstSegment = GetSequenceValue(first);
            //        _secondSegment = GetSequenceValue(second);
            //        _thirdSegment = GetSequenceValue(third);
            //        _fourthSegment = GetSequenceValue(fourth);

            //        accountCombination = _firstSegment + "-" + _secondSegment + "-" + _thirdSegment + "-" + _fourthSegment;
            //    }
            //}



            return accountCombination;
        }


        public void SplitLedgerAccountCombination()
        {
            var ledgers = _db.GLLedgerPostings
                .Where(x => x.AccountSegment == null || x.SubAccountSegment == null)
                .ToList();
            foreach (var ledger in ledgers)
            {
                SplitLedgerAccount(ledger);

            }

        }

        public void SplitLedgerAccount(GLLedgerPosting ledger)
        {
            string[] account = ledger.AccountCode.Split('-');

            if (_dynamicAccount.GetNaturalAccountSequenceIndex() != -1)
                ledger.AccountSegment = account[_dynamicAccount.GetNaturalAccountSequenceIndex()];
            if (_dynamicAccount.GetCostCenterAccountSequenceIndex() != -1)
                ledger.CostCenterSegment = account[_dynamicAccount.GetCostCenterAccountSequenceIndex()];
            if (_dynamicAccount.GetSubAccountSequenceIndex() != -1)
                ledger.SubAccountSegment = account[_dynamicAccount.GetSubAccountSequenceIndex()];
            if (_dynamicAccount.GetBizUnitAccountSequenceIndex() != -1)
                ledger.BusinessUnit = account[_dynamicAccount.GetBizUnitAccountSequenceIndex()];
            if (_dynamicAccount.GetBranchAccountSequenceIndex() != -1)
                ledger.BranchSegment = account[_dynamicAccount.GetBranchAccountSequenceIndex()];

            #region dynamic segments

            if (account.Length == 8)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
                ledger.Segment4 = account[3];
                ledger.Segment5 = account[4];
                ledger.Segment6 = account[5];
                ledger.Segment7 = account[6];
                ledger.Segment8 = account[7];
            }
            else if (account.Length == 7)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
                ledger.Segment4 = account[3];
                ledger.Segment5 = account[4];
                ledger.Segment6 = account[5];
                ledger.Segment7 = account[6];
            }
            else if (account.Length == 6)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
                ledger.Segment4 = account[3];
                ledger.Segment5 = account[4];
                ledger.Segment6 = account[5];
            }
            else if (account.Length == 5)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
                ledger.Segment4 = account[3];
                ledger.Segment5 = account[4];
            }
            else if (account.Length == 4)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
                ledger.Segment4 = account[3];
            }
            else if (account.Length == 3)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
                ledger.Segment3 = account[2];
            }
            else if (account.Length == 2)
            {
                ledger.Segment1 = account[0];
                ledger.Segment2 = account[1];
            }
            else if (account.Length == 1)
            {
                ledger.Segment1 = account[0];
            }

            #endregion
            _db.Entry(ledger).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void SplitJournalAccountCombination()
        {
            var journalDetails = _db.GLRecordJournalDetails.Where(x => x.AccountSegment == null || x.BranchSegment == null).ToList();
            foreach (var journalDetail in journalDetails)
            {
                SplitJournal(journalDetail.GlRecordJournalDetailId);
            }


        }

        public void SplitJournal(int id)
        {
            using (var context = new ExceedDbContext())
            {

                var journalDetail = context.GLRecordJournalDetails.Find(id);

                if (journalDetail != null)
                {
                    string[] account = journalDetail.AccountCode.Split('-');
                    journalDetail.AccountSegment = account[_dynamicAccount.GetNaturalAccountSequenceIndex()];
                    if (_dynamicAccount.GetCostCenterAccountSequenceIndex() != -1)
                        journalDetail.CostCenterSegment = account[_dynamicAccount.GetCostCenterAccountSequenceIndex()];
                    if (_dynamicAccount.GetSubAccountSequenceIndex() != -1)
                        journalDetail.SubAccountSegment = account[_dynamicAccount.GetSubAccountSequenceIndex()];
                    if (_dynamicAccount.GetBizUnitAccountSequenceIndex() != -1)
                        journalDetail.BusinessUnit = account[_dynamicAccount.GetBizUnitAccountSequenceIndex()];
                    if (_dynamicAccount.GetBranchAccountSequenceIndex() != -1)
                        journalDetail.BranchSegment = account[_dynamicAccount.GetBranchAccountSequenceIndex()];

                    #region dynamic segments

                    if (account.Length == 8)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                        journalDetail.Segment7 = account[6];
                        journalDetail.Segment8 = account[7];
                    }
                    else if (account.Length == 7)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                        journalDetail.Segment7 = account[6];
                    }
                    else if (account.Length == 6)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                    }
                    else if (account.Length == 5)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                    }
                    else if (account.Length == 4)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                    }
                    else if (account.Length == 3)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                    }
                    else if (account.Length == 2)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                    }
                    else if (account.Length == 1)
                    {
                        journalDetail.Segment1 = account[0];
                    }

                    #endregion

                    context.Entry(journalDetail).State = EntityState.Modified;
                    context.SaveChanges();
                }


            }
        }

        public void SplitBegJournalAccountCombination()
        {
            var journalDetails = _db.GLRecordJournalBegDistributions.Where(x => x.AccountSegment == null || x.BranchSegment == null).ToList();
            foreach (var journalDetail in journalDetails)
            {
                SplitBegJournal(journalDetail.GLRecordJournalBegDistributionId);
            }
        }

        public void SplitBegJournal(int id)
        {
            using (var context = new ExceedDbContext())
            {

                var journalDetail = context.GLRecordJournalBegDistributions.Find(id);

                if (journalDetail != null)
                {
                    string[] account = journalDetail.AccountCode.Split('-');
                    journalDetail.AccountSegment = account[_dynamicAccount.GetNaturalAccountSequenceIndex()];
                    if (_dynamicAccount.GetCostCenterAccountSequenceIndex() != -1)
                        journalDetail.CostCenterSegment = account[_dynamicAccount.GetCostCenterAccountSequenceIndex()];
                    if (_dynamicAccount.GetSubAccountSequenceIndex() != -1)
                        journalDetail.SubAccountSegment = account[_dynamicAccount.GetSubAccountSequenceIndex()];
                    if (_dynamicAccount.GetBizUnitAccountSequenceIndex() != -1)
                        journalDetail.BusinessUnit = account[_dynamicAccount.GetBizUnitAccountSequenceIndex()];
                    if (_dynamicAccount.GetBranchAccountSequenceIndex() != -1)
                        journalDetail.BranchSegment = account[_dynamicAccount.GetBranchAccountSequenceIndex()];

                    #region dynamic segments

                    if (account.Length == 8)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                        journalDetail.Segment7 = account[6];
                        journalDetail.Segment8 = account[7];
                    }
                    else if (account.Length == 7)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                        journalDetail.Segment7 = account[6];
                    }
                    else if (account.Length == 6)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                        journalDetail.Segment6 = account[5];
                    }
                    else if (account.Length == 5)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                        journalDetail.Segment5 = account[4];
                    }
                    else if (account.Length == 4)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                        journalDetail.Segment4 = account[3];
                    }
                    else if (account.Length == 3)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                        journalDetail.Segment3 = account[2];
                    }
                    else if (account.Length == 2)
                    {
                        journalDetail.Segment1 = account[0];
                        journalDetail.Segment2 = account[1];
                    }
                    else if (account.Length == 1)
                    {
                        journalDetail.Segment1 = account[0];
                    }

                    #endregion
                }

                context.Entry(journalDetail).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void SplitBookAccountCombination()
        {
            var bookRecords = _db.ReconcilationBookAccounts.Where(x => x.AccountSegment == null).ToList();
            foreach (var record in bookRecords)
            {
                SplitBookRecord(record.ReconciliationBookAccountId);
            }
        }

        public void SplitBookRecord(int id)
        {
            using (var context = new ExceedDbContext())
            {

                var bookRecord = context.ReconcilationBookAccounts.Find(id);

                if (bookRecord != null)
                {
                    string[] account = bookRecord.AccountCode.Split('-');
                    bookRecord.AccountSegment = account[_dynamicAccount.GetNaturalAccountSequenceIndex()];
                    if (_dynamicAccount.GetCostCenterAccountSequenceIndex() != -1)
                        bookRecord.CostCenterSegment = account[_dynamicAccount.GetCostCenterAccountSequenceIndex()];
                    if (_dynamicAccount.GetSubAccountSequenceIndex() != -1)
                        bookRecord.SubAccountSegment = account[_dynamicAccount.GetSubAccountSequenceIndex()];
                    if (_dynamicAccount.GetBizUnitAccountSequenceIndex() != -1)
                        bookRecord.BusinessUnitSegment = account[_dynamicAccount.GetBizUnitAccountSequenceIndex()];
                    if (_dynamicAccount.GetBranchAccountSequenceIndex() != -1)
                        bookRecord.BranchSegment = account[_dynamicAccount.GetBranchAccountSequenceIndex()];

                    #region dynamic segments

                    if (account.Length == 8)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                        bookRecord.Segment4 = account[3];
                        bookRecord.Segment5 = account[4];
                        bookRecord.Segment6 = account[5];
                        bookRecord.Segment7 = account[6];
                        bookRecord.Segment8 = account[7];
                    }
                    else if (account.Length == 7)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                        bookRecord.Segment4 = account[3];
                        bookRecord.Segment5 = account[4];
                        bookRecord.Segment6 = account[5];
                        bookRecord.Segment7 = account[6];
                    }
                    else if (account.Length == 6)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                        bookRecord.Segment4 = account[3];
                        bookRecord.Segment5 = account[4];
                        bookRecord.Segment6 = account[5];
                    }
                    else if (account.Length == 5)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                        bookRecord.Segment4 = account[3];
                        bookRecord.Segment5 = account[4];
                    }
                    else if (account.Length == 4)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                        bookRecord.Segment4 = account[3];
                    }
                    else if (account.Length == 3)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                        bookRecord.Segment3 = account[2];
                    }
                    else if (account.Length == 2)
                    {
                        bookRecord.Segment1 = account[0];
                        bookRecord.Segment2 = account[1];
                    }
                    else if (account.Length == 1)
                    {
                        bookRecord.Segment1 = account[0];
                    }

                    #endregion
                }

                context.Entry(bookRecord).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void UpdateLedgerAccountType(string GLValue, GlAccountTypes type)
        {
            var ledgers = _db.GLLedgerPostings.Where(x => x.AccountSegment == GLValue).ToList();
            foreach (var l in ledgers)
            {
                l.AccountType = type;
                _db.GLLedgerPostings.Attach(l);
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }


        public string ValidateAccountType(GlAccountTypes type, string value)
        {
            string errror = "";
            if (type == GlAccountTypes.Undefined)
                errror = "You can't define undefined account type";
            if (type == GlAccountTypes.Asset && !value.StartsWith("1"))
                errror = "Asset account should start with 1.";
            else if (type == GlAccountTypes.Liability && !value.StartsWith("2"))
                errror = "Liability account should start with 2.";
            else if (type == GlAccountTypes.Capital && !value.StartsWith("3"))
                errror = "Capital/Equity account should start with 3.";
            else if (type == GlAccountTypes.Revenue && !value.StartsWith("4"))
                errror = "Capital/Equity account should start with 4.";
            else if (type == GlAccountTypes.Expense && (value.StartsWith("0") || value.StartsWith("1") || value.StartsWith("2") || value.StartsWith("3") || value.StartsWith("4")))
                errror = "Expense/Cost account should not start start with 0 to 4.";

            return errror;
        }
        public string ValidateSegmentCombinationLength(string accountComb)
        {
            string errror = "";
            if (string.IsNullOrEmpty(accountComb))
                return "Account code is null.";
            errror = _dynamicAccount.ValidateSegmentCombinationLength(accountComb);

            return errror;
        }
      

        public string IsValidJournal(IEnumerable<GLRecordJournalDetail> list)
        {
            string errorMsg = "";

            var glRecordJournalDetails = list as IList<GLRecordJournalDetail> ?? list.ToList();
            foreach (var detail in glRecordJournalDetails)
            {
                errorMsg = ValidateSegmentCombinationLength(detail.AccountCode);
                if (!string.IsNullOrEmpty(errorMsg))
                    return errorMsg;

                if (detail.Debit != 0 && detail.Credit != 0)
                {
                    errorMsg = $"{detail.AccountCode} Account has both debit and credit value.";
                    return errorMsg;
                }

                if (detail.Debit == 0 && detail.Credit == 0)
                {
                    errorMsg = $"{detail.AccountCode} Both debit and credit can't have zero value at the same time.";
                    return errorMsg;
                }

                if (detail.Debit > 0 && detail.Credit > 0)
                {
                    errorMsg = $"{detail.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    return errorMsg;
                }

                if (detail.Debit < 0 || detail.Credit < 0)
                {
                    errorMsg = $"{detail.AccountCode} Negative values are not allowed.";
                    return errorMsg;
                }
            }

            decimal sumDebit = Math.Round(glRecordJournalDetails.Sum(x => x.Debit), 4);
            decimal sumCredit = Math.Round(glRecordJournalDetails.Sum(x => x.Credit), 4);
            if (sumDebit != sumCredit)
            {
                errorMsg = $"Journal {glRecordJournalDetails.FirstOrDefault()?.GlRecordJournalId} does not have balanced debit and credit.";
                return errorMsg;
            }


            if (sumDebit == 0)
            {
                errorMsg = $"Journal {glRecordJournalDetails.FirstOrDefault()?.GlRecordJournalId} should not have total debit and credit as zero.";
                return errorMsg;
            }


            return errorMsg;
        }
        public string IsValidBegJournal(IEnumerable<GLRecordJournalBegDistribution> list)
        {
            string errorMsg = "";

            var glRecordJournalDetails = list as IList<GLRecordJournalBegDistribution> ?? list.ToList();
            foreach (var detail in glRecordJournalDetails)
            {
                errorMsg = ValidateSegmentCombinationLength(detail.AccountCode);
                if (!string.IsNullOrEmpty(errorMsg))
                    return errorMsg;

                if (detail.Debit != 0 && detail.Credit != 0)
                {
                    errorMsg = $"{detail.AccountCode} Account has both debit and credit value.";
                    return errorMsg;
                }

                if (detail.Debit == 0 && detail.Credit == 0)
                {
                    errorMsg = $"{detail.AccountCode} Both debit and credit can't have zero value at the same time.";
                    return errorMsg;
                }

                if (detail.Debit > 0 && detail.Credit > 0)
                {
                    errorMsg = $"{detail.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    return errorMsg;
                }

                if (detail.Debit < 0 || detail.Credit < 0)
                {
                    errorMsg = $"{detail.AccountCode} Negative values are not allowed.";
                    return errorMsg;
                }
            }

            decimal sumDebit = Math.Round(glRecordJournalDetails.Sum(x => x.Debit), 4);
            decimal sumCredit = Math.Round(glRecordJournalDetails.Sum(x => x.Debit), 4);
            if (sumDebit != sumCredit)
            {
                errorMsg = $"Journal  does not have balanced debit and credit.";
                return errorMsg;
            }


            if (sumDebit == 0)
            {
                errorMsg = $"Journal should not have total debit and credit as zero.";
                return errorMsg;
            }


            return errorMsg;
        }

        

       
        public string GetNaturalAccountSegment(string acct)
        {
            int accountindex = _dynamicAccount.GetNaturalAccountSequenceIndex();
            if (accountindex != -1)
                return acct.Split('-')[accountindex];
            else
                return "";
        }

        public string GetSubAccountSegment(string acct)
        {
            int accountindex = _dynamicAccount.GetSubAccountSequenceIndex();
            if (accountindex != -1)
                return acct.Split('-')[accountindex];
            else
                return "";
        }
        //
        public string GetCostCenterSegment(string acct)
        {
            int accountindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
            if (accountindex != -1)
                return acct.Split('-')[accountindex];
            else
                return "";
        }
        public string GetBranchSegment(string acct)
        {
            int accountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
            if(accountindex!=-1)
                return acct.Split('-')[accountindex];
            else 
                return "";
        }

        public string GetBusinessUnitSegment(string acct)
        {
            int accountindex = 0;
            string bizunitSegment = "";//for non dynamic no biz unit
            accountindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
            if (accountindex != -1)
                bizunitSegment = acct.Split('-')[accountindex];

            return bizunitSegment;
        }

        public List<string> GetBankSubAccounts()
        {
            var banks = _db.BankAccounts.ToList();
            List<string> banckaccounts = banks.Select(x => x.AccountCode).ToList();
            List<string> subaccounts = new List<string>();

            foreach (var banckaccount in banckaccounts)
            {
                string[] account = banckaccount.Split('-');
                string subacc = account[_dynamicAccount.GetSubAccountSequenceIndex()];
                subaccounts.Add(subacc);
            }

            return subaccounts;
        }


    }

}
