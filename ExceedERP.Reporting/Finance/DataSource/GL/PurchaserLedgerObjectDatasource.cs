using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class PurchaserLedgerObjectDatasource : List<PurchaserLedgerVm>, IDisposable
    {

        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        public PurchaserLedgerObjectDatasource()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaserLedgerVm> GetPurchaserLedger(DateTime from, DateTime to, string purchaserid, string acct)
        {
            List<PurchaserLedgerVm> purchaserLedgerList = new List<PurchaserLedgerVm> { };

            var journalDetails = _db.GLRecordJournalDetails
                .Where(x => x.GLRecordJournal.IsOnlineApproved)
                .Where(x => x.GLRecordJournal.isVoid == false)
                .Where(x => x.GLRecordJournal.EffectiveDate >= from.Date &&
                            x.GLRecordJournal.EffectiveDate <= to.Date)
                .Select(x => new { x.GLRecordJournal, x.GlRecordJournalId, x.AccountCode, x.SubAccountSegment, x.AccountSegment, x.AdditionalDescription, x.Debit, x.Credit }).AsQueryable();

            List<GLPeriod> periods = new List<GLPeriod>();

            if (!String.IsNullOrEmpty(acct))
            {
                journalDetails = journalDetails.Where(x => x.AccountSegment == acct).AsQueryable();

                if (!String.IsNullOrEmpty(purchaserid))
                {
                    //#region get sub account purchaser
                    //var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId.ToString() == purchaserid);
                    //#endregion
                    journalDetails = journalDetails.Where(x => x.SubAccountSegment == purchaserid).AsQueryable();
                }

                

                #region get covered periods for sorting data

                var journalids = journalDetails.Select(x => x.GlRecordJournalId).Distinct();

                var periodList = _db.GLRecordJournals.Where(x => journalids.Contains(x.GlRecordJournalId)).Select(x => x.PeriodName).Distinct();
                foreach (var p in periodList)
                {
                    var period = _db.GLPeriods.FirstOrDefault(x => x.Name == p);
                    if(period!=null)
					    periods.Add(period);
                }

                #endregion

                var sortedPeriods = periods.OrderBy(x => x.DateTo);

                var journalDetailRange = journalDetails.AsEnumerable().DistinctByField(x => x.AccountCode).ToList();

                foreach (var detail in journalDetailRange)
                {
                    GlAccountTypes accountType = _accountHelper.GetAccountType(detail.AccountCode);

                    var seletctedDetails = journalDetails
                          .Where(x => x.AccountCode == detail.AccountCode).ToList();

                    var purchaserLedgerVm = new PurchaserLedgerVm
                    {
                        PurchaserId = detail.AccountCode, //to be updated
                        PurchaserName = _accountHelper.GetAccountDesc(detail.AccountCode), //to be updated
                        PurchaserLedgerVmLines = new List<TransactionVm>()
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
                                    TransReference = !String.IsNullOrEmpty(journalDetail.AdditionalDescription) ? journalDetail.AdditionalDescription : journalDetail.GLRecordJournal.Description,
                                    ReferenceDoc = journalDetail.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalDetail.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalDetail.GLRecordJournal.InvoiceReferenceDoc,
                                    Debit = journalDetail.Debit,
                                    Credit = journalDetail.Credit,
                                    Balance = 0,
                                    Remark = journalDetail.GLRecordJournal.Remark
                                    //TransactionType = journalDetail.SourcesName //to be updated
                                };

                                if (accountType == GlAccountTypes.Asset || accountType == GlAccountTypes.Expense)
                                    lines.Balance = journalDetail.Debit - journalDetail.Credit;
                                else
                                {
                                    lines.Balance = journalDetail.Credit - journalDetail.Debit;
                                }


                                purchaserLedgerVm.PurchaserLedgerVmLines.Add(lines);
                            }

                            #endregion

                        }

                        #endregion
                    }

                    purchaserLedgerVm.Debit = purchaserLedgerVm.PurchaserLedgerVmLines.Sum(x => x.Debit);
                    purchaserLedgerVm.Credit = purchaserLedgerVm.PurchaserLedgerVmLines.Sum(x => x.Credit);
                    purchaserLedgerVm.Balance = purchaserLedgerVm.PurchaserLedgerVmLines.Sum(x => x.Balance);

                    purchaserLedgerList.Add(purchaserLedgerVm);
                }
            }

            return purchaserLedgerList;
        }
        //public List<PurchaserLedgerVm> GetPurchaserLedger1(DateTime from, DateTime to, string purchaserid,string acct)
        //    {
        //        List<PurchaserLedgerVm> purchaserLedgerList = new List<PurchaserLedgerVm> { };
        //    var ledgerposts = _db.GlSecondayLedgers.AsQueryable();
        //    List<GlSecondayLedger> ledgers = new List<GlSecondayLedger>();
        //    List<GLPeriod> periods = new List<GLPeriod>();
        //        if (!String.IsNullOrEmpty(acct))
        //        {
        //            ledgerposts = ledgerposts.Where(x => x.AccountSegment == acct).AsQueryable();
        //        }
        //        if (!String.IsNullOrEmpty(purchaserid))
        //        {
        //            //if (AccountHelper.IsDynamicCoA())
        //            //{
        //            //    AccountHelper accountHelper = new AccountHelper();
        //            //    string[] account = acct.Split('-');
        //            //    string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
        //            //    string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
        //            //    string costCenterSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
        //            //    string subAccountSeG = purchaserid;
        //            //    string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);
        //            //    acct = newacc;
        //            //}
        //            //else
        //            //{
        //            //    int subaccountindex = DynamicAccountHelper.GetSubAccountSequenceIndex();
        //            //    string subAccountSeG = purchaserid;
        //            //    string newacc = DynamicAccountHelper.ReplaceSegment(acct, subaccountindex, subAccountSeG);
        //            //    acct = newacc;
        //            //}
        //            #region get sub account purchaser
        //            var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId.ToString() == purchaserid);
        //            #endregion

        //            ledgerposts = ledgerposts
        //                .Where(x => x.SubAccountSegment == employeeSubAccount.SubAccountCode).AsQueryable();
        //        }


        //    var ledgerPostingLines = _db.GlSecondayLedgerLines.Where(b => b.EffectiveDate >= from && b.EffectiveDate <= to).ToList();
        //    #region get covered periods for sorting data

        //    var periodList = ledgerPostingLines.Select(x => x.GLPeriodId).Distinct();
        //    foreach (var p in periodList)
        //    {
        //        periods.Add(_db.GLPeriods.Find(p));
        //    }

        //    #endregion
        //    var sortedPeriods = periods.OrderBy(x => x.DateTo);

        //    foreach (var ledgerPosting in ledgerposts)
        //    {
        //        var selectedledgerPostingLines = ledgerPostingLines.Where(x => x.GlSecondayLedger.AccountCode == ledgerPosting.AccountCode).ToList();

        //        var purchaserLedgerVm = new PurchaserLedgerVm
        //        {
        //            PurchaserId = ledgerPosting.AccountCode,//to be updated
        //            PurchaserName = _accountHelper.GetAccountDesc(ledgerPosting.AccountCode),//to be updated
        //            PurchaserLedgerVmLines = new List<TransactionVm>()
        //        };

        //        if (selectedledgerPostingLines.Any())
        //        {
        //            #region current transaction
        //            foreach (var period in sortedPeriods)
        //            {
        //                #region beginning

        //                var begginningRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Begining).ToList();
        //                foreach (var balance in begginningRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    purchaserLedgerVm.PurchaserLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //                #region standard

        //                var standardRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Standard)
        //                    .ToList().OrderBy(x => x.EffectiveDate);
        //                foreach (var balance in standardRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    purchaserLedgerVm.PurchaserLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //                #region ending

        //                var endingRecords = selectedledgerPostingLines
        //                    .Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
        //                foreach (var balance in begginningRecords)
        //                {
        //                    var lines = new TransactionVm
        //                    {
        //                        TranssactionId = balance.JournalNumber,
        //                        TransactionDate = balance.EffectiveDate,
        //                        TransReference = balance.JournalReference,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance,
        //                        TransactionType = balance.SourcesName//to be updated
        //                    };

        //                    purchaserLedgerVm.PurchaserLedgerVmLines.Add(lines);
        //                }

        //                #endregion
        //            }

        //            #endregion
        //        }



        //        purchaserLedgerList.Add(purchaserLedgerVm);
        //    }

        //    return purchaserLedgerList;
        //}

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
    
}
