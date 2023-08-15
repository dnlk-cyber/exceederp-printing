using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Bank
{
    [DataObject]
    public class BankReconciliationStatementOds : List<StockBalanceOds>
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public BankReconciliationHeaderVm GetReconciliationStatementHeader(int id)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            BankReconciliationHeaderVm headerDs = new BankReconciliationHeaderVm();

            var reconciliationSchedule = _db.ReconciliationSchedules.Find(id);

            if (reconciliationSchedule != null)
            {
                headerDs.BankAccount = reconciliationSchedule.BankAccount;
                //headerDs.DateFrom = _fiscalYearHelper.GetStartOfMonth(reconciliationSchedule.Date);//old
                //headerDs.DateTo = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date);//old
                headerDs.DateFrom = _fiscalYearHelper.GetStartOfPeriod(reconciliationSchedule.Date);
                headerDs.DateTo = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date);
                int periodid = _fiscalYearHelper.GetPeriodOfDate(reconciliationSchedule.Date.Date);
                if (periodid != 0)
                {
                    headerDs.PeriodName = _db.GLPeriods.Find(periodid)?.Name;
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

               
            }
            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public BankReconciliationStatementVm GetReconciliationStatement(int id)
        {
            var bankReconciliationStatementVms = new BankReconciliationStatementVm();

            var reconciliationSchedule = _db.ReconciliationSchedules.Find(id);

            if (reconciliationSchedule != null)
            {

                //DateTime dt11 = _fiscalYearHelper.GetStartOfMonth(reconciliationSchedule.Date); //old
                //DateTime dt22 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1);//it is next month //old

                DateTime dt11 = _fiscalYearHelper.GetStartOfPeriod(reconciliationSchedule.Date); //new
                DateTime dt22 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).AddDays(1);//it is next month, new



                var bankaccountobj = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
                string account = _accountHelper.GetNaturalAccountSegment(bankaccountobj.AccountCode);
                string subaccount = _accountHelper.GetSubAccountSegment(bankaccountobj.AccountCode);

                var bookData = _db.ReconcilationBookAccounts
                    //.Where(x => x.AccountCode == bankaccountobj.AccountCode) //old
                    .Where(x => x.AccountSegment == account && x.SubAccountSegment == subaccount)
                    .Where(x => x.Reconciled == false || x.Reconciled && x.ReconciledDate >= dt22)//from || new code
                                                                                                 //.Where(x => x.RecordType != RecordType.Exception)
                    .Where(x => x.RecordType != RecordType.Beginning)
                    // .Where(x => x.Date >= dt1)
                    .Where(x => x.Date < dt22).ToList();
                //filter book
               


                var bankData = _db.ReconcilationBankAccounts
                    .Where(x => x.BankAccount == bankaccountobj.AccountNumber)
                    .Where(x => x.Reconciled == false || x.Reconciled && x.ReconciledDate >= dt22)//from || new code
                    // .Where(x => x.Date >= dt1)
                    .Where(x => x.Date < dt22).ToList();//< less than cuz it is +1 above


                var deposits = bookData.Where(x => x.Debit != 0).OrderBy(x => x.Date).ToList();
                var payments = bookData.Where(x => x.Credit != 0).OrderBy(x => x.Date).ToList();

                var depositByBankButNotOnLedger = bankData.Where(x => x.Debit != 0).OrderBy(x => x.Date).ToList();
                var withdrawByBankButNotOnLedger = bankData.Where(x => x.Credit != 0).OrderBy(x => x.Date).ToList();

                #region deposits

                foreach (var deposit in deposits)
                {
                    var journal = new BankReconciliationDeposit
                    {
                        Date = deposit.Date,
                        Reference = deposit.Reference,
                        ReferenceDoc=deposit.ReferenceDoc,
                        Source = deposit.Source,
                        ActivityBalance = deposit.Debit
                    };
                    bankReconciliationStatementVms.Deposits.Add(journal);
                }

                #endregion
                #region withdrawalby bank

                foreach (var withdrwal in withdrawByBankButNotOnLedger)
                {
                    var journal = new BankReconciliationWithdrawByBankNotRecordedOnLedger
                    {
                        Date = withdrwal.Date,
                        Reference = withdrwal.Reference,
                        ReferenceDoc="",
                        Source = "",
                        ActivityBalance = withdrwal.Credit
                    };
                    bankReconciliationStatementVms.WithdrawByBanks.Add(journal);
                }

                #endregion
                #region payments

                foreach (var payment in payments)
                {
                    var journal = new BankReconciliationOutstandingPayment
                    {
                        Date = payment.Date,
                        Reference = payment.Reference,
                        ReferenceDoc=payment.ReferenceDoc,
                        Source = payment.Source,
                        ActivityBalance = payment.Credit
                    };
                    bankReconciliationStatementVms.OutstandingPayments.Add(journal);
                }

                #endregion
                #region deposits by bank

                foreach (var deposit in depositByBankButNotOnLedger)
                {
                    var journal = new BankReconciliationDepositByBankNotRecordedOnLedger
                    {
                        Date = deposit.Date,
                        Reference = deposit.Reference,
                        ReferenceDoc="",
                        Source = "",
                        ActivityBalance = deposit.Debit
                    };
                    bankReconciliationStatementVms.DepositKnownByBanks.Add(journal);
                }

                #endregion

                bankReconciliationStatementVms.EndingBalanceOfBankStatement = reconciliationSchedule.BankEndingBalance;

                bankReconciliationStatementVms.TotalBalance =
                    bankReconciliationStatementVms.EndingBalanceOfBankStatement 
                    +bankReconciliationStatementVms.Deposits.Sum(x => x.ActivityBalance) 
                    +bankReconciliationStatementVms.WithdrawByBanks.Sum(x => x.ActivityBalance);

                bankReconciliationStatementVms.AdjustedBankBalance =
                    bankReconciliationStatementVms.TotalBalance 
                    -bankReconciliationStatementVms.OutstandingPayments.Sum(x => x.ActivityBalance);

                int periodid = _fiscalYearHelper.GetPeriodOfDate(reconciliationSchedule.Date.Date);
                if (periodid != 0)
                {
                    var period = _db.GLPeriods.Find(periodid);
                    if (period != null)
                    {
                        bankReconciliationStatementVms.EndingBalanceOfBankStatementPeriodName = period.Name;
                        int yearid = period.GlFiscalYearId;
                        List<GLLedgerPostingLines> balances = _reportHelper.GetAccountBalance(yearid, periodid);
                        if (bankaccountobj != null)
                        {
                            //old
                            // bankReconciliationStatementVms.BalanceAsPerLedger = _reportHelper.GetAccountEndBalanceWithFullSegment(balances, bankaccountobj.AccountCode);
                            //new
                            string accont = _accountHelper.GetNaturalAccountSegment(bankaccountobj.AccountCode);
                            string subaccont = _accountHelper.GetSubAccountSegment(bankaccountobj.AccountCode);
                            bankReconciliationStatementVms.BalanceAsPerLedger = _reportHelper.GetAccountEndBalancesWithSubAccount(balances,accont,subaccont);
                        }
                    }
                }
                else
                {
                    bankReconciliationStatementVms.BalanceAsPerLedger = 0;
                }

               
                bankReconciliationStatementVms.BankServiceCharge = reconciliationSchedule.ServiceCharges.Sum(x=>x.Amount);//get from statment

                bankReconciliationStatementVms.AdjustedLedgerBalance =
                    bankReconciliationStatementVms.BalanceAsPerLedger 
                    +bankReconciliationStatementVms.DepositKnownByBanks.Sum(x => x.ActivityBalance)
                    -bankReconciliationStatementVms.BankServiceCharge;

                bankReconciliationStatementVms.UnreconciledDifference =
                    bankReconciliationStatementVms.AdjustedBankBalance -
                    bankReconciliationStatementVms.AdjustedLedgerBalance;

                #region ending balance of bank statement
                var bankStatement = new EndingBalanceOfBankStatement
                {
                    PeriodName = bankReconciliationStatementVms.EndingBalanceOfBankStatementPeriodName,
                    EndingBalance = bankReconciliationStatementVms.EndingBalanceOfBankStatement,
                };
                bankReconciliationStatementVms.EndingBalanceOfBankStatements.Add(bankStatement);
                #endregion

            }



            return bankReconciliationStatementVms;
        }
    }
}