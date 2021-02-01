using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{
   
    [DataObject]
    public class PartnerAndAgentSettlementDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<PartnerAndAgentSettlementViewModel> GetPartnerAndAgent(string from, string to, string partnerid, string transactionType, string acct)
        {
            List<PartnerAndAgentSettlementViewModel> agentSettlementViewModels = new List<PartnerAndAgentSettlementViewModel> { };

            if (!String.IsNullOrEmpty(from) && !String.IsNullOrEmpty(to) && !String.IsNullOrEmpty(acct))
            {
                DateTime dt11 = _db.GLPeriods.Find(int.Parse(from)).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(int.Parse(to)).DateTo.Date;

                var journalDetails = _db.GLRecordJournalDetails

                    .Where(x => x.GLRecordJournal.IsOnlineApproved)
                    .Where(x => x.GLRecordJournal.isVoid == false)
                    .Where(b => b.GLRecordJournal.EffectiveDate >= dt11 &&
                                b.GLRecordJournal.EffectiveDate <= dt22).AsQueryable();
           

                     List<GLPeriod> periods = new List<GLPeriod>();

           
                journalDetails = journalDetails.Where(x => x.AccountSegment == acct)
                    .AsQueryable();

                if (!String.IsNullOrEmpty(partnerid))
                {
                    journalDetails = journalDetails.Where(x => x.SubAccountSegment == partnerid).AsQueryable();
                }

                #region get covered periods for sorting data

                var journalids = journalDetails.Select(x => x.GlRecordJournalId).Distinct();

                var periodList = _db.GLRecordJournals.Where(x => journalids.Contains(x.GlRecordJournalId)).Select(x => x.PeriodName).Distinct();
                foreach (var p in periodList)
                {
                    periods.Add(_db.GLPeriods.FirstOrDefault(x => x.Name == p));
                }

                #endregion

                var sortedPeriods = periods.OrderBy(x => x.DateTo);

                var journalDetailRange = journalDetails.AsEnumerable().DistinctByField(x => x.AccountCode).ToList();

                foreach (var detail in journalDetailRange)
                {
                    GlAccountTypes accountType = _accountHelper.GetAccountType(detail.AccountCode);

                    var seletctedDetails = journalDetails
                          .Where(x => x.AccountCode == detail.AccountCode).ToList();

                    var agentSettlementViewModel = new PartnerAndAgentSettlementViewModel
                    {
                        PartnerId = detail.AccountCode, //to be updated
                        PartnerName = _accountHelper.GetAccountDesc(detail.AccountCode), //to be updated
                        PartnerTransactionVms = new List<TransactionVm>()
                    };

                    if (seletctedDetails.Any())
                    {
                        #region current transaction

                        foreach (var period in sortedPeriods)
                        {
                            #region records

                            var recordJournalDetails = seletctedDetails
                                .Where(x => x.GLRecordJournal.PeriodName == period.Name).ToList();
                            foreach (var journalDetail in recordJournalDetails)
                            {
                                var lines = new TransactionVm
                                {
                                    TranssactionId = journalDetail.GLRecordJournal.JournalNumber,
                                    TransactionDate = journalDetail.GLRecordJournal.EffectiveDate,
                                    TransReference = journalDetail.GLRecordJournal.Description,
                                    Debit = journalDetail.Debit,
                                    Credit = journalDetail.Credit,
                                    Balance = 0,
                                    ReferenceDoc=journalDetail.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalDetail.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalDetail.GLRecordJournal.InvoiceReferenceDoc
                                };

                                if (accountType == GlAccountTypes.Asset || accountType == GlAccountTypes.Expense)
                                    lines.Balance = journalDetail.Debit - journalDetail.Credit;
                                else
                                {
                                    lines.Balance = journalDetail.Credit - journalDetail.Debit;
                                }


                                agentSettlementViewModel.PartnerTransactionVms.Add(lines);
                            }

                            #endregion

                        }

                        #endregion
                    }

                    agentSettlementViewModel.Debit = agentSettlementViewModel.PartnerTransactionVms.Sum(x => x.Debit);
                    agentSettlementViewModel.Credit = agentSettlementViewModel.PartnerTransactionVms.Sum(x => x.Credit);
                    agentSettlementViewModel.Balance = agentSettlementViewModel.PartnerTransactionVms.Sum(x => x.Balance);

                    agentSettlementViewModels.Add(agentSettlementViewModel);
                }
            }

            return agentSettlementViewModels;
        }
        public List<PartnerAndAgentSettlementViewModel> GetPartnerAndAgent1(DateTime from, DateTime to, string partnerid, string transactionType, string acct)
        {
            List<PartnerAndAgentSettlementViewModel> agentSettlementViewModels = new List<PartnerAndAgentSettlementViewModel> { };

            var ledgerposts = _db.GlSecondayLedgers.AsQueryable();
           
            List<GLPeriod> periods = new List<GLPeriod>();
            if (!String.IsNullOrEmpty(acct))
            {
                ledgerposts = ledgerposts.Where(x => x.AccountSegment == acct).AsQueryable();

                if (!String.IsNullOrEmpty(partnerid))
                {
                    ledgerposts = ledgerposts.Where(x => x.SubAccountSegment == partnerid).AsQueryable();
                }

                var ledgerPostingLines = _db.GlSecondayLedgerLines
                    .Where(b => b.EffectiveDate >= from && b.EffectiveDate <= to).ToList();

                #region get covered periods for sorting data

                var periodList = ledgerPostingLines.Select(x => x.GLPeriodId).Distinct().ToList();
                foreach (var p in periodList)
                {
                    var period = _db.GLPeriods.Find(p);
                    if (period != null)
                       periods.Add(period);
                }

                #endregion

                var sortedPeriods = periods.OrderBy(x => x.DateTo);

                foreach (var ledgerPosting in ledgerposts)
                {
                    var selectedledgerPostingLines = ledgerPostingLines
                        .Where(x => x.GlSecondayLedger.AccountCode == ledgerPosting.AccountCode).ToList();

                    var partnerAndAgentSettlementViewModel = new PartnerAndAgentSettlementViewModel
                    {
                        PartnerId = ledgerPosting.AccountCode, //to be updated
                        PartnerName = _accountHelper.GetAccountDesc(ledgerPosting.AccountCode), //to be updated
                        PartnerTransactionVms = new List<TransactionVm>()
                    };

                    if (selectedledgerPostingLines.Any())
                    {
                        #region current transaction

                        foreach (var period in sortedPeriods)
                        {
                            #region beginning

                            var begginningRecords = selectedledgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Begining).ToList();
                            foreach (var balance in begginningRecords)
                            {
                                var lines = new TransactionVm
                                {
                                    TranssactionId = balance.JournalNumber,
                                    TransactionDate = balance.EffectiveDate,
                                    TransReference = balance.JournalReference,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    TransactionType = balance.SourcesName //to be updated
                                };

                                partnerAndAgentSettlementViewModel.PartnerTransactionVms.Add(lines);
                            }

                            #endregion

                            #region standard

                            var standardRecords = selectedledgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard)
                                .ToList().OrderBy(x => x.EffectiveDate);
                            foreach (var balance in standardRecords)
                            {
                                var lines = new TransactionVm
                                {
                                    TranssactionId = balance.JournalNumber,
                                    TransactionDate = balance.EffectiveDate,
                                    TransReference = balance.JournalReference,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    TransactionType = balance.SourcesName //to be updated
                                };

                                partnerAndAgentSettlementViewModel.PartnerTransactionVms.Add(lines);
                            }

                            #endregion

                            #region ending

                            var endingRecords = selectedledgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in endingRecords)
                            {
                                var lines = new TransactionVm
                                {
                                    TranssactionId = balance.JournalNumber,
                                    TransactionDate = balance.EffectiveDate,
                                    TransReference = balance.JournalReference,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    TransactionType = balance.SourcesName //to be updated
                                };

                                partnerAndAgentSettlementViewModel.PartnerTransactionVms.Add(lines);
                            }

                            #endregion
                        }

                        #endregion
                    }

                    agentSettlementViewModels.Add(partnerAndAgentSettlementViewModel);
                }
            }

            return agentSettlementViewModels;
        }
    }
}
