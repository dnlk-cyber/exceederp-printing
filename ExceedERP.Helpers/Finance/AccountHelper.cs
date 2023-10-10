using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;


namespace ExceedERP.Helpers.Finance
{
    public class AccountHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
        //AccountHelper accountHelper = new AccountHelper();

        public AccountHelper()
        {
            _firstSegment = "";
            _secondSegment = "";
            _thirdSegment = "";
            _fourthSegment = "";

            _companyGlobal = "";
            _accountGlobal = "";
            _costcenterGlobal = "";
            _subaccountGlobal = "";
        }

        //delegated
        public GlAccountTypes GetAccountType(string accountAcode)
        {
            AccountHelper accountHelper = new AccountHelper();
            GlAccountTypes type = GlAccountTypes.Undefined;
            if (IsDynamicCoA())
            {
                type = _dynamicAccount.GetAccountType(accountAcode);
            }
            else
            {

                try
                {
                    string[] account = accountAcode.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    var accountObj = _db.GLChartOfAccountAccounts.FirstOrDefault(x => x.Values == acccountSeG);
                    if (accountObj != null)
                        type = accountObj.AccountType;
                }
                catch (Exception x)
                {
                    //
                }
            }

            return type;
        }

        public int GetAccountCombLength()
        {
            return IsDynamicCoA() ? _db.GlSegmentSetups.Count(x => x.Enabled) : 4;
        }

        //delegated
        public string GetAccountDesc(string accountAcode)
        {
            string desc = "";
            if (IsDynamicCoA())
            {
                desc = _dynamicAccount.GetAccountDesc(accountAcode);
            }


            return desc;
        }
        //only here
        private string GetSequenceDesc(GlSegmentTypes segment, string segmentvalue)
        {
            string segmentdesc = "";
            switch (segment)
            {
                case GlSegmentTypes.Account:
                    segmentdesc = _db.GLChartOfAccountAccounts.Where(x => x.Values.ToUpper() == segmentvalue.ToUpper()).Select(x => x.Description).FirstOrDefault();
                    break;
                case GlSegmentTypes.Location:
                    var locationSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
                    segmentdesc = _db.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId)
                        .Where(x => x.Values.ToUpper() == segmentvalue.ToUpper()).Select(x => x.Description).FirstOrDefault();
                    break;
                case GlSegmentTypes.CostCenter:
                    var costCenterSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                    segmentdesc = _db.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == costCenterSeg.GLSegmentSetupId)
                        .Where(x => x.Values.ToUpper() == segmentvalue.ToUpper()).Select(x => x.Description).FirstOrDefault();
                    break;
                case GlSegmentTypes.SubAccount:
                    var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                    segmentdesc = _db.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId)
                        .Where(x => x.Values.ToUpper() == segmentvalue.ToUpper()).Select(x => x.Description).FirstOrDefault();
                    break;
                default:
                    segmentdesc = "";
                    break;
            }
            return segmentdesc;
        }
        //same 

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

        public bool IsSameBankAccount_1(string accountcode, string bankAccountCode)
        {
            string account = GetNaturalAccountSegment(accountcode);
            string subAccount = GetSubAccountSegment(accountcode);

            string baccount = GetNaturalAccountSegment(bankAccountCode);
            string bsubAccount = GetSubAccountSegment(bankAccountCode);

            if (account == baccount && subAccount == bsubAccount)
                return true;

            return false;
        }
        public bool IsBankAccount2(string accountcode)
        {

            using (var context = new ExceedDbContext())
            {
                var banckaccounts = context.BankAccounts.Where(x => x.AccountCode == accountcode).ToList();
                if (banckaccounts.Any())
                    return true;
            }

            return false;
        }
        //delegated
        public bool IsBankAccount1(string accountcode)
        {
            if (IsDynamicCoA())
            {
                return _dynamicAccount.IsBankAccount1(accountcode);
            }
            else
            {

                bool bankaccount = false;
                string[] account = accountcode.Split('-');
                string subAccount = account[GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                using (var context = new ExceedDbContext())
                {
                    var banckaccounts = context.BankAccounts.Select(x => x.AccountCode).ToList();
                    foreach (var acc in banckaccounts)
                    {

                        string[] myaccouts = acc.Split('-');
                        if (subAccount.ToUpper() ==
                            myaccouts[GetAccountSequenceIndex(GlSegmentTypes.SubAccount)].ToUpper())
                        {
                            bankaccount = true;
                            break;
                        }

                    }
                }

                return bankaccount;
            }
        }
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

            if (IsDynamicCoA())
            {
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
            }


            return index;
        }
        //same
        public static string GetAccountSegment(string accountCombination, int index)
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
        //different impl
        public List<string> GeAccountRange(string fromAccountCombination, string toAccountCombination, GlSegmentTypes segment)
        {
            AccountHelper accountHelper = new AccountHelper();
            List<string> accountRange = null;

            int index = accountHelper.GetAccountSequenceIndex(segment);
            var fromSegment = GetAccountSegment(fromAccountCombination, index);
            var toSegment = GetAccountSegment(toAccountCombination, index);

            if (segment == GlSegmentTypes.Account)
            {
                if (accountHelper.IsDynamicCoA())
                {
                    accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                        _dynamicAccount.GetNaturalAccountSequenceIndex());
                }
                else
                {
                    using (var context = new ExceedDbContext())
                    {
                        accountRange = context.GLChartOfAccountAccounts
                            .Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                            .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                }
            }

            else if (segment == GlSegmentTypes.Location)
            {
                using (var context = new ExceedDbContext())
                {
                    var locationSeg = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == Core.Domain.Finance.GL.Dynamic.DynamicSegmentTypes.Location);
                    accountRange = context.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId)
                          .Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                          .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                          .Select(x => x.Values)
                          .ToList();
                }
            }

            else if (segment == GlSegmentTypes.CostCenter)
            {
                if (accountHelper.IsDynamicCoA())
                {
                    accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                        _dynamicAccount.GetCostCenterAccountSequenceIndex());
                }
                else
                {
                    using (var context = new ExceedDbContext())
                    {
                        var costCenterSeg = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                        accountRange = context.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costCenterSeg.GLSegmentSetupId)
                          .Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                        .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                        .Select(x => x.Values)
                        .ToList();
                    }
                }
            }

            else if (segment == GlSegmentTypes.SubAccount)
            {
                if (accountHelper.IsDynamicCoA())
                {
                    accountRange = _dynamicAccount.GetAccountRange(fromAccountCombination, toAccountCombination,
                        _dynamicAccount.GetSubAccountSequenceIndex());
                }
                else
                {
                    using (var context = new ExceedDbContext())
                    {

                        var subAccountSeg = context.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                        accountRange = context.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId)
                    .Where(x => x.Values.ToUpper().CompareTo(fromSegment) >= 0)
                            .Where(x => x.Values.ToUpper().CompareTo(toSegment) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                }
            }


            return accountRange;
        }

        //for the below function
        //new sequence
        static string _firstSegment = "";
        static string _secondSegment = "";
        static string _thirdSegment = "";
        static string _fourthSegment = "";

        static string _companyGlobal = "";
        static string _accountGlobal = "";
        static string _costcenterGlobal = "";
        static string _subaccountGlobal = "";

        //needed here only
        public string RearrangeAccountSequence(string account, string location, string costcenter, string subaccount)
        {


            _companyGlobal = location;
            _accountGlobal = account;
            _costcenterGlobal = costcenter;
            _subaccountGlobal = subaccount;

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
        //needed here only
        private static string GetSequenceValue(GlSegmentTypes segment)
        {
            string segmentValue = "";
            switch (segment)
            {
                case GlSegmentTypes.Account:
                    segmentValue = _accountGlobal;
                    break;
                case GlSegmentTypes.Location:
                    segmentValue = _companyGlobal;
                    break;
                case GlSegmentTypes.CostCenter:
                    segmentValue = _costcenterGlobal;
                    break;
                case GlSegmentTypes.SubAccount:
                    segmentValue = _subaccountGlobal;
                    break;
                default:
                    segmentValue = "";
                    break;
            }
            return segmentValue;
        }
        //delegated
        public void SplitLedgerAccountCombination()
        {
            if (IsDynamicCoA())
            {
                _dynamicAccount.SplitLedgerAccountCombination();
            }

            else
            {
                var ledgers = _db.GLLedgerPostings
                    //.Where(x => x.AccountSegment == null)
                    .ToList();
                foreach (var ledger in ledgers)
                {
                    SplitLedgerAccount(ledger);
                }
            }


        }



        //local
        public void SplitLedgerAccount(GLLedgerPosting ledger)
        {
            AccountHelper accountHelper = new AccountHelper();
            string[] account = ledger.AccountCode.Split('-');

            ledger.AccountSegment = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
            ledger.BranchSegment = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
            ledger.CostCenterSegment = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
            ledger.SubAccountSegment = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];

            ledger.Segment1 = account[0];
            ledger.Segment2 = account[1];
            ledger.Segment3 = account[2];
            ledger.Segment4 = account[3];

            _db.Entry(ledger).State = EntityState.Modified;
            _db.SaveChanges();
        }


        //delegated
        public void SplitJournalAccountCombination()
        {
            if (IsDynamicCoA())
            {
                _dynamicAccount.SplitJournalAccountCombination();
            }
            else
            {
                var journalDetails = _db.GLRecordJournalDetails.Where(x => x.AccountSegment == null || x.BranchSegment == null).ToList();
                foreach (var journalDetail in journalDetails)
                {
                    SplitJournal(journalDetail.GlRecordJournalDetailId);
                }
            }


        }

        public void SplitJournal(int id)
        {
            using (var context = new ExceedDbContext())
            {
                var journalDetail = context.GLRecordJournalDetails.Find(id);
                if (IsDynamicCoA())
                {
                    _dynamicAccount.SplitJournal(id);
                }
                else
                {
                    if (journalDetail != null)
                    {
                        string[] account = journalDetail.AccountCode.Split('-');

                        journalDetail.AccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        journalDetail.BranchSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        journalDetail.CostCenterSegment = account[GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        journalDetail.SubAccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                    }

                    context.Entry(journalDetail).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void SplitBegJournalAccountCombination()
        {
            if (IsDynamicCoA())
            {
                _dynamicAccount.SplitBegJournalAccountCombination();
            }
            else
            {
                var lines = _db.GLRecordJournalBegDistributions.Where(x => x.AccountSegment == null || x.BranchSegment == null).ToList();
                foreach (var line in lines)
                {
                    SplitBegJournal(line.GLRecordJournalBegDistributionId);
                }
            }


        }

        public void SplitBegJournal(int id)
        {
            using (var context = new ExceedDbContext())
            {
                var journalDetail = context.GLRecordJournalBegDistributions.Find(id);
                if (IsDynamicCoA())
                {
                    _dynamicAccount.SplitJournal(id);
                }
                else
                {
                    if (journalDetail != null)
                    {
                        string[] account = journalDetail.AccountCode.Split('-');

                        journalDetail.AccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        journalDetail.BranchSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        journalDetail.CostCenterSegment = account[GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        journalDetail.SubAccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                    }

                    context.Entry(journalDetail).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void SplitBookAccountCombination()
        {
            if (IsDynamicCoA())
            {
                _dynamicAccount.SplitBookAccountCombination();
            }
            else
            {
                var bookRecords = _db.ReconcilationBookAccounts.Where(x => x.AccountSegment == null).ToList();
                foreach (var record in bookRecords)
                {
                    SplitBookRecord(record.ReconciliationBookAccountId);
                }
            }


        }

        public void SplitBookRecord(int id)
        {
            using (var context = new ExceedDbContext())
            {
                var bookRecord = context.ReconcilationBookAccounts.Find(id);
                if (IsDynamicCoA())
                {
                    _dynamicAccount.SplitBookRecord(id);
                }
                else
                {
                    if (bookRecord != null)
                    {
                        string[] account = bookRecord.AccountCode.Split('-');

                        bookRecord.AccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        bookRecord.BranchSegment = account[GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        bookRecord.CostCenterSegment = account[GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        bookRecord.SubAccountSegment = account[GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                    }

                    context.Entry(bookRecord).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public bool IsDynamicCoA()
        {
            var ruleValues = _db.GlRuleValues.Where(x => x.DynamicCoA).ToList();
            if (ruleValues.Any())
                return true;
            else
            {
                return false;
            }
        }

        //new 
        //delegated
        public void UpdateLedgerAccountType(string GLValue, GlAccountTypes type)
        {
            _dynamicAccount.UpdateLedgerAccountType(GLValue, type);
        }


        public string ValidateAccountType(GlAccountTypes type,string value)
        {
            string errror = "";
            if (type == GlAccountTypes.Undefined)
                errror = "You can't define undefined account type";
            if (type==GlAccountTypes.Asset && !value.StartsWith("1"))
                errror= "Asset account should start with 1.";
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
            if (String.IsNullOrEmpty(accountComb))
                return "Account code is null.";
            int combLength = accountComb.Length;
            if (IsDynamicCoA())
            {
                errror=_dynamicAccount.ValidateSegmentCombinationLength(accountComb);

            }
            

            return errror;
        }

        //public string IsValidForPost(IEnumerable<GLRecordJournalDetail> list)
        //{
        //    string errorMsg = "";

        //    var glRecordJournalDetails = list as IList<GLRecordJournalDetail> ?? list.ToList();
        //    foreach (var detail in glRecordJournalDetails)
        //    {
        //        if (detail.Debit != 0 && detail.Credit != 0)
        //        {
        //            errorMsg = $"{detail.AccountCode} Account has both debit and credit value.";
        //            return errorMsg;
        //        }
        //    }

        //    if (glRecordJournalDetails.Sum(x => x.Debit) != glRecordJournalDetails.Sum(x => x.Credit))
        //    {
        //        errorMsg = $"Journal id {glRecordJournalDetails.FirstOrDefault()?.GlRecordJournalId} does not have balanced debit and credit.";
        //        return errorMsg;
        //    }
        //    return errorMsg;
        //}

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

            decimal sumDebit =Math.Round(glRecordJournalDetails.Sum(x => x.Debit),4);
            decimal sumCredit = Math.Round(glRecordJournalDetails.Sum(x => x.Credit), 4);
            if (sumDebit != sumCredit)
            {
                errorMsg = $"Journal {glRecordJournalDetails.FirstOrDefault()?.GlRecordJournalId} does not have balanced debit and credit.";
                return errorMsg;
            }


            if (sumDebit== 0)
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

        //public string IsValidJournal(IQueryable<TDSJournalDetail> lines)
        //{
        //    throw new NotImplementedException();
        //}

        //both
        public  string GetNaturalAccountSegment(string acct)
        {
            AccountHelper accountHelper = new AccountHelper();
            int accountindex = 0;
            accountindex = accountHelper.IsDynamicCoA() ? _dynamicAccount.GetNaturalAccountSequenceIndex() : accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account);
            string naturalAccountSegment = acct.Split('-')[accountindex];

            return naturalAccountSegment;
        }       

        public  string GetSubAccountSegment(string acct)
        {
            AccountHelper accountHelper = new AccountHelper();
            int accountindex = 0;
            accountindex = accountHelper.IsDynamicCoA() ? _dynamicAccount.GetSubAccountSequenceIndex() : accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount);
            string subAccountSegment = acct.Split('-')[accountindex];

            return subAccountSegment;
        }
        //
        public string GetCostCenterSegment(string acct)
        {
            AccountHelper accountHelper = new AccountHelper();
            int accountindex = 0;
            accountindex = IsDynamicCoA() ? _dynamicAccount.GetCostCenterAccountSequenceIndex(): accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter);
            string centerSegment = acct.Split('-')[accountindex];

            return centerSegment;
        }
        public string GetBranchSegment(string acct)
        {
            AccountHelper accountHelper = new AccountHelper();
            int accountindex = 0;
            accountindex = IsDynamicCoA() ? _dynamicAccount.GetBranchAccountSequenceIndex() : accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location);
            string centerSegment = acct.Split('-')[accountindex];

            return centerSegment;
        }

        public string GetBusinessUnitSegment(string acct)
        {
            int accountindex = 0;
            string bizunitSegment = "";//for non dynamic no biz unit
            if (IsDynamicCoA())
            {
                accountindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                if (accountindex != -1)
                    bizunitSegment = acct.Split('-')[accountindex];
            }

            return bizunitSegment;
        }

        public  List<string> GetBankSubAccounts()
        {
            var banks = _db.BankAccounts.ToList();
            List<string> banckaccounts = banks.Select(x => x.AccountCode).ToList();
            List<string> subaccounts = new List<string>();

            foreach (var banckaccount in banckaccounts)
            {
                if (IsDynamicCoA())
                {
                    string[] account = banckaccount.Split('-');
                    string subacc = account[_dynamicAccount.GetSubAccountSequenceIndex()];
                    subaccounts.Add(subacc);
                }
                else
                {
                    AccountHelper accountHelper = new AccountHelper();
                    string[] account = banckaccount.Split('-');
                    string subacc = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];
                    subaccounts.Add(subacc);
                }
            }

            return subaccounts;
        }

        
    }

}
