using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;


namespace ExceedERP.Helpers.Finance
{
    public  class DynamicAccountHelper
    {
        private  readonly ExceedDbContext _db = new ExceedDbContext();
        //done
        public void UpdateLedgerAccountType(string GLValue, GlAccountTypes type)
        {
            var ledgers = _db.GLLedgerPostings.Where(x => x.AccountSegment == GLValue).ToList();
            foreach(var l in ledgers)
            {
                l.AccountType = type;
                _db.GLLedgerPostings.Attach(l);
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
        public  GlAccountTypes GetAccountType(string accountAcode)
        {
            GlAccountTypes type = GlAccountTypes.Undefined;
            try
            {
                string[] segments = accountAcode.Split('-');
                int index = GetNaturalAccountSequenceIndex();
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
        //done
        public  string GetAccountDesc(string accountAcode)
        {
            var sequence = _db.GlSegmentSetups.Where(x=>x.Enabled).ToList();
            var otherSegments = _db.GlChartOfAccountSegments.ToList();
            var accountSegments = _db.GLChartOfAccountAccounts.ToList();

            string desc = "";
            try
            {
                if (sequence.Any())
                {
                    string[] segments = accountAcode.Split('-');

                    for (int i = 0; i < segments.Length; i++)
                    {
                        var specificSegment = sequence.FirstOrDefault(x => x.Sequence == i + 1);
                        if (specificSegment != null && specificSegment.SegmentType==DynamicSegmentTypes.NaturalAccount)
                        {
                            var value = accountSegments.FirstOrDefault(x => x.Values == segments[i]);
                            if (value != null) desc = desc + "-" + value.Description;
                        }
                        else if (specificSegment != null && specificSegment.SegmentType!= DynamicSegmentTypes.NaturalAccount)
                        {
                            var value = otherSegments.Where(x=>x.GLSegmentSetupId==specificSegment.GLSegmentSetupId).FirstOrDefault(x => x.Values == segments[i]);
                            if (value != null) desc = desc + "-" + value.Description;

                        }
                    }

                    desc = desc.Remove(0, 1);//remove first -
                }
            }
            catch (Exception x)
            {
                // ignored
            }
            return desc;
        }
     //same
        public  bool IsBankAccount(string accountcode)
        {
           
            using (var context = new ExceedDbContext())
            {
                var banckaccounts = context.BankAccounts.Where(x=>x.AccountCode==accountcode).ToList();
                if (banckaccounts.Any())
                    return true;
            }

            return false;
        }
        //different implementation
        public  bool IsBankAccount1(string accountcode)
        {
            bool bankaccount = false;
            string[] segments = accountcode.Split('-');
            int index = GetSubAccountSequenceIndex();
            string subAccount = segments[index];
            using (var context = new ExceedDbContext())
            {
                var banckaccounts = context.BankAccounts.Select(x => x.AccountCode).ToList();
                foreach (var acc in banckaccounts)
                {
                    string[] myaccouts = acc.Split('-');
                    if(subAccount.ToUpper() == myaccouts[index].ToUpper())
                    {
                        bankaccount = true;
                        break;
                    }

                }
            }

            return bankaccount;
        }
        //different implementation
        public  int GeSegmentLength(GLSegmentSetup segment)
        {
            int length = 0;

            using (var context = new ExceedDbContext())
            {
                var segmentSetup = context.GlSegmentSetups.Find(segment.GLSegmentSetupId);
                if (segmentSetup != null)
                {
                    length = segmentSetup.SegmentLength;
                }
            }

            return length;
        }
        //different implementation
        public  int GetNaturalAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups.FirstOrDefault(x=>x.SegmentType== DynamicSegmentTypes.NaturalAccount);
                if (sequence != null)
                {
                    index = sequence.Sequence-1;
                }
            }

            return index;
        }
        //different implementation
        public  int GetCostCenterAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType== DynamicSegmentTypes.CostCenter);
                if (sequence != null)
                {
                    index = sequence.Sequence-1;
                }
            }

            return index;
        }
        //different implementation
        public  int GetBranchAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }

        //different implementation
        public int GetBizUnitAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.BusinessUnit);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }
        //different implementation
        public  int GetSubAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType== DynamicSegmentTypes.SubAccount);
                if (sequence != null)
                {
                    index = sequence.Sequence-1;
                }
            }

            return index;
        }
       
        public  string GeAccountSegment(string accountCombination, int index)
        {
            string account = "";
            try
            {
                account = accountCombination.Split('-')[index];
            }
            catch (Exception)
            {

                //log if index is out of range;
            }

            return account;
        }
        //same
        public  decimal GeAccountLastBalance(List<GLLedgerPostingLines> records, string accountCode)
        {
            decimal balance = 0;
            var rangebalances = records.Where(x => x.GLLedgerPosting.AccountCode.ToUpper().Equals(accountCode)).ToList();
            foreach (var rangebalance in rangebalances)
            {
                int maxid =
                    rangebalances.Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode)
                        .Max(x => x.GLLedgerPostingLinesID);
                var lastbalance =
                    rangebalances
                        .Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode).FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
                if (lastbalance != null)
                    balance = balance + lastbalance.Balance;
            }

            return balance;
        }
        //different implementation
        public  List<string> GetAccountRange(string fromAccountCombination, string toAccountCombination, int index)
        {
            List<string> accountRange = null;
            var fromSegment = GeAccountSegment(fromAccountCombination, index);
            var toSegment = GeAccountSegment(toAccountCombination, index);

            using (var context = new ExceedDbContext())
            {
                var segmentSetup = context.GlSegmentSetups.FirstOrDefault(x => x.Sequence == index + 1);
                if (segmentSetup != null && segmentSetup.SegmentType== DynamicSegmentTypes.NaturalAccount)
                {
                    accountRange = context.GLChartOfAccountAccounts
                        .Where(x => String.Compare(x.Values.ToUpper(), fromSegment, StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.Values.ToUpper(), toSegment, StringComparison.Ordinal) <= 0)
                        .Select(x => x.Values)
                        .ToList();
                }
                else
                {
                    accountRange = context.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == segmentSetup.GLSegmentSetupId)
                        .Where(x => String.Compare(x.Values.ToUpper(), fromSegment, StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.Values.ToUpper(), toSegment, StringComparison.Ordinal) <= 0)
                        .Select(x => x.Values)
                        .ToList();
                }
            }

            return accountRange;
        }
        
        //done
        public  void SplitLedgerAccountCombination()
        {
            var ledgers = _db.GLLedgerPostings
                .Where(x => x.AccountSegment == null || x.SubAccountSegment==null)
                .ToList();
            foreach (var ledger in ledgers)
            {
                SplitLedgerAccount(ledger);

            }
        }

        

        public  void SplitLedgerAccount(GLLedgerPosting ledger)
        {
            string[] account = ledger.AccountCode.Split('-');

            if (GetNaturalAccountSequenceIndex() != -1)
                ledger.AccountSegment = account[GetNaturalAccountSequenceIndex()];
            if (GetCostCenterAccountSequenceIndex() != -1)
                ledger.CostCenterSegment = account[GetCostCenterAccountSequenceIndex()];
            if (GetSubAccountSequenceIndex() != -1)
                ledger.SubAccountSegment = account[GetSubAccountSequenceIndex()];
            if (GetBizUnitAccountSequenceIndex() != -1)
                ledger.BusinessUnit = account[GetBizUnitAccountSequenceIndex()];
            if (GetBranchAccountSequenceIndex() != -1)
                ledger.BranchSegment = account[GetBranchAccountSequenceIndex()];

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

        //done
        public  void SplitJournalAccountCombination()
        {
            var journalDetails = _db.GLRecordJournalDetails.Where(x => x.AccountSegment == null ||x.BranchSegment==null).ToList();
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
                    journalDetail.AccountSegment = account[GetNaturalAccountSequenceIndex()];
                    if (GetCostCenterAccountSequenceIndex() != -1)
                        journalDetail.CostCenterSegment = account[GetCostCenterAccountSequenceIndex()];
                    if (GetSubAccountSequenceIndex() != -1)
                        journalDetail.SubAccountSegment = account[GetSubAccountSequenceIndex()];
                    if (GetBizUnitAccountSequenceIndex() != -1)
                        journalDetail.BusinessUnit = account[GetBizUnitAccountSequenceIndex()];
                    if (GetBranchAccountSequenceIndex() != -1)
                        journalDetail.BranchSegment = account[GetBranchAccountSequenceIndex()];

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
                    journalDetail.AccountSegment = account[GetNaturalAccountSequenceIndex()];
                    if (GetCostCenterAccountSequenceIndex() != -1)
                        journalDetail.CostCenterSegment = account[GetCostCenterAccountSequenceIndex()];
                    if (GetSubAccountSequenceIndex() != -1)
                        journalDetail.SubAccountSegment = account[GetSubAccountSequenceIndex()];
                    if (GetBizUnitAccountSequenceIndex() != -1)
                        journalDetail.BusinessUnit = account[GetBizUnitAccountSequenceIndex()];
                    if (GetBranchAccountSequenceIndex() != -1)
                        journalDetail.BranchSegment = account[GetBranchAccountSequenceIndex()];

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
                    bookRecord.AccountSegment = account[GetNaturalAccountSequenceIndex()];
                    if (GetCostCenterAccountSequenceIndex() != -1)
                        bookRecord.CostCenterSegment = account[GetCostCenterAccountSequenceIndex()];
                    if (GetSubAccountSequenceIndex() != -1)
                        bookRecord.SubAccountSegment = account[GetSubAccountSequenceIndex()];
                    if (GetBizUnitAccountSequenceIndex() != -1)
                        bookRecord.BusinessUnitSegment = account[GetBizUnitAccountSequenceIndex()];
                    if (GetBranchAccountSequenceIndex() != -1)
                        bookRecord.BranchSegment = account[GetBranchAccountSequenceIndex()];

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

        //new functions
        //done
        public  string ReplaceSegment(string accountAcode,int index,string value)
        {
            if (index >= 0 && !string.IsNullOrEmpty(value))
            {
                string newAccComb = "";
                string[] account = accountAcode.Split('-');
                account[index] = value;

                foreach (var t in account)
                {
                    newAccComb = newAccComb + "-" + t;
                }

                if (newAccComb.Length > 0)
                {
                    newAccComb = newAccComb.Remove(0, 1);
                }
                return newAccComb;
            }
            else
            {
                return accountAcode;
            }
        }

        //new
        public  string ValidateSegmentCombinationLength(string accountComb)
        {
            string errror = "";
            int combLength = accountComb.Length;
            int totalLength = 0;
            var length = _db.GlSegmentSetups.Where(x => x.Enabled).ToList();
            foreach (var l in length)
            {
                totalLength = totalLength + l.SegmentLength;
            }

            totalLength = totalLength + length.Count - 1;//number of - underscores added
            if (combLength != totalLength)
                errror = $"Account combination segment length for {accountComb} should be {totalLength} characters.";
            if (length.Count==0)
            {
                errror = "Segment length setting is not defined.";
            }

            return errror;
        }

    }

}
