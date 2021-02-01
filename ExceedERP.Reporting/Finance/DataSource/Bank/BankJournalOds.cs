using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Bank
{
    [DataObject]
    public class BankJournalOds : List<StockBalanceOds>
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public BankJournalHeaderVm GetBankJournalHeader(string fromperiod, string toperiod, string bankaccount)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            BankJournalHeaderVm headerDs = new BankJournalHeaderVm();

            headerDs.BankAccount = bankaccount;
            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);

                headerDs.DateFrom = _db.GLPeriods.Find(from).DateFrom.ToShortDateString();
                headerDs.DateTo = _db.GLPeriods.Find(to).DateTo.ToShortDateString();
            }


            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }

                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                        a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BankJournalVm> GetBankJournal(string fromperiod, string toperiod, string bankaccount,string status)
        {
            List<BankJournalVm> bankJournalVms = new List<BankJournalVm>();

           

            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                var bookAccounts = _db.ReconcilationBookAccounts
                    .Where(x => x.RecordType != RecordType.Beginning)//new
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    switch (status)
                    {
                        case "All":
                            break;
                        case "Reconciled":
                            bookAccounts = bookAccounts.Where(x => x.Reconciled).AsQueryable();
                            break;
                        case "Unreconciled":
                            bookAccounts = bookAccounts.Where(x => x.Reconciled == false).AsQueryable();
                            break;

                    }
                }

                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);

                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                bookAccounts = bookAccounts
                    .Where(x => x.Date >= dt11)
                    .Where(x => x.Date <= dt22).AsQueryable();

                if (!string.IsNullOrEmpty(bankaccount))
                {
                    var bankaccountobj = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == bankaccount);
                    // bookAccounts = bookAccounts.Where(x => x.AccountCode == bankaccountobj.AccountCode).AsQueryable();
                    //new     
                    string account = _accountHelper.GetNaturalAccountSegment(bankaccountobj.AccountCode);
                    string subaccount = _accountHelper.GetSubAccountSegment(bankaccountobj.AccountCode);
                    bookAccounts = bookAccounts.Where(x =>x.AccountSegment==account && x.SubAccountSegment==subaccount).AsQueryable();
                }

                var records = bookAccounts.ToList();
                foreach (var record in records)
                {
                    var journal = new BankJournalVm
                    {
                        Date = record.Date,
                        Reference = record.Reference,
                        ReferenceDoc=record.ReferenceDoc,
                        Source = record.Source,
                        SourceId = record.SourceID,
                        Type = record.Debit == 0 ? BookTransactionType.Payment : BookTransactionType.Receipt,
                        TransactDesc = record.AdditionalDescription,
                        Debit = record.Debit,
                        Credit = record.Credit,
                        Balance = 0,
                        ReconciliationStatus = record.Reconciled ? "Reconciled" : "Unreconciled"
                    };
                    bankJournalVms.Add(journal);
                }
            }

           

            return bankJournalVms;
        }
    }
}