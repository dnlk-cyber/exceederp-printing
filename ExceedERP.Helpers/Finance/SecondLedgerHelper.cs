using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Helpers.Finance
{
    public class SecondLedgerHelper
    {
        private  readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        public  void PostToSecondLedger(PostJournalVm gLRecordJournal, string user)
        {
            var journallines = gLRecordJournal.PostJournalLineVms;

            foreach (var journalline in journallines)
            {
                #region search if ledger is already created
                var ledger = _db.GlSecondayLedgers.FirstOrDefault(x => x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());
                #endregion
                if (ledger != null)
                {
                    var period = _db.GLPeriods.Find(gLRecordJournal.GlPeriodId);

                    GlSecondayLedger ledgerpost = ledger;
                    #region post ledger line

                    var ledgerpostline = new GlSecondayLedgerLine
                    {
                        GlSecondayLedger = ledgerpost,
                        JournalNumber = gLRecordJournal.PostJournalVmId + "-" + journalline.PostJournalLineVmId,
                        JournalReference = gLRecordJournal.JournalNumber,
                        EffectiveDate = gLRecordJournal.EffectiveDate,
                        JournalName = gLRecordJournal.JournalName,
                        CategoryName = gLRecordJournal.CategoryName,
                        SourcesName = gLRecordJournal.SourcesName,
                        PostingSource=gLRecordJournal.PostingSource
                    };
                    if (period != null)
                    {
                        ledgerpostline.GLPeriodId = period.GLPeriodId;
                        ledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                    }
                    if (journalline.Debit > 0)
                    {
                        #region debit balance
                        ledgerpostline.Debit = journalline.Debit;
                        ledgerpostline.Credit = 0;
                        #endregion
                    }
                    else if (journalline.Credit > 0)
                    {
                        #region credit balance
                        ledgerpostline.Credit = journalline.Credit;
                        ledgerpostline.Debit = 0;
                        #endregion
                    }

                    _db.GlSecondayLedgerLines.Add(ledgerpostline);
                    _db.SaveChanges();
                    #endregion
                }
                else
                {
                    #region create ledger
                    GlSecondayLedger ledgerpost = new GlSecondayLedger();
                    AccountHelper accountHelper = new AccountHelper();
                    ledgerpost.AccountCode = journalline.AccountCode;
                    ledgerpost.AccountDesc = accountHelper.GetAccountDesc(ledgerpost.AccountCode);
                    ledgerpost.AccountType = accountHelper.GetAccountType(ledgerpost.AccountCode);
                    _db.GlSecondayLedgers.Add(ledgerpost);
                    _db.SaveChanges();
                    #endregion
                    #region post ledger line

                    var period = _db.GLPeriods.Find(gLRecordJournal.GlPeriodId);

                    var ledgerpostline = new GlSecondayLedgerLine
                    {
                        GlSecondayLedger = ledgerpost,
                        JournalNumber = gLRecordJournal.PostJournalVmId + "-" + journalline.PostJournalLineVmId,
                        JournalReference = gLRecordJournal.JournalNumber,
                        EffectiveDate = gLRecordJournal.EffectiveDate,
                        JournalName = gLRecordJournal.JournalName,
                        CategoryName = gLRecordJournal.CategoryName,
                        SourcesName = gLRecordJournal.SourcesName,
                        PostingSource = gLRecordJournal.PostingSource
                    };
                    if (period != null)
                    {
                        ledgerpostline.GLPeriodId = period.GLPeriodId;
                        ledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                    }
                    if (journalline.Debit > 0)
                    {
                        #region debit balance
                        ledgerpostline.Debit = journalline.Debit;
                        ledgerpostline.Credit = 0;

                        #endregion
                    }
                    else if (journalline.Credit > 0)
                    {
                        #region credit balance
                        ledgerpostline.Credit = journalline.Credit;
                        ledgerpostline.Debit = 0;

                        #endregion
                    }

                    _db.GlSecondayLedgerLines.Add(ledgerpostline);
                    _db.SaveChanges();
                    #endregion

                    #region split ledger account

                    //if (AccountHelper.IsDynamicCoA())
                    //{
                    //    DynamicAccountHelper.SplitSecondayLedgerAccount(ledgerpost);
                    //}
                    //else
                    //{
                    //    AccountHelper.SplitSecondaryLedgerAccount(ledgerpost);
                    //}

                    #endregion
                }

            }
           

            #region update ledger balances

            //update ledger
            foreach (var journalline in gLRecordJournal.PostJournalLineVms)
            {
                UpdateBegEndBalances(journalline.AccountCode);
            }

            #endregion
        }
        public  void UpdateBegEndBalances(string ledger)
        {
            var ledgerPostingLines = _db.GlSecondayLedgerLines.AsQueryable();
            var ledgerPostings = _db.GlSecondayLedgers.ToList();
            if (!String.IsNullOrEmpty(ledger))
                ledgerPostings = ledgerPostings.Where(x => x.AccountCode == ledger).ToList();

            var periods = _db.GLPeriods.OrderBy(x => x.GLPeriodId).ToList();

            var beginningPeriod = _db.GlBeginningPeriods.FirstOrDefault();
            if (beginningPeriod != null)
            {
                var beginningPeriodObj = _db.GLPeriods.Find(beginningPeriod.GLPeriodId);
                if (beginningPeriodObj != null)
                {
                    // periods after beginning and and already started
                    periods = periods.Where(x => x.DateFrom > beginningPeriodObj.DateFrom)
                                     .Where(x => x.DateFrom <= DateTime.Today).ToList();
                    decimal begninning = 0;
                    decimal ending = 0;
                    decimal netIncrement = 0;
                    decimal debit = 0;
                    decimal credit = 0;
                    decimal nextBegining = 0;

                    foreach (var ledgerPosting in ledgerPostings)
                    {
                        var posting = ledgerPosting;
                        var selectedLines = ledgerPostingLines.Where(x => x.GlSecondayLedger.AccountCode == posting.AccountCode);

                        #region beginning period

                        var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
                        var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Standard);
                        var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

                        if (begRecord != null)
                        {

                            if (posting.AccountType == GlAccountTypes.Asset ||
                                posting.AccountType == GlAccountTypes.Expense)
                            {
                                begninning = begRecord.Debit - begRecord.Credit;
                                begRecord.Balance = begninning;

                            }
                            else if (posting.AccountType == GlAccountTypes.Liability ||
                                     posting.AccountType == GlAccountTypes.Capital ||
                                     posting.AccountType == GlAccountTypes.Revenue)
                            {
                                begninning = begRecord.Credit - begRecord.Debit;
                                begRecord.Balance = begninning;
                            }

                            _db.Entry(begRecord).State = EntityState.Modified;

                        }
                        else
                        {
                            begninning = 0;
                            #region add beginning ledger line

                            var periodName = _fiscalYearHelper.GetPeriodName(beginningPeriod.GLPeriodId);
                            var newBeginning = new GlSecondayLedgerLine
                            {
                                GlSecondayLedger = posting,
                                JournalNumber = "Beginning-" + periodName,
                                JournalReference = "Beginning-" + periodName,
                                EffectiveDate = beginningPeriodObj.DateFrom,
                                JournalName = "Beginning-" + periodName,
                                CategoryName = "Beginning-" + periodName,
                                SourcesName = "Beginning-" + periodName,
                                GLPeriodId = beginningPeriod.GLPeriodId,
                                GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                Type = LedgerPostTypes.Begining,
                                Balance = begninning
                            };
                            _db.GlSecondayLedgerLines.Add(newBeginning);
                            // _db.SaveChanges();

                            #endregion

                        }

                        if (standardRecord.Any())
                        {
                            debit = standardRecord.Sum(x => x.Debit);
                            credit = standardRecord.Sum(x => x.Credit);
                        }


                        if (posting.AccountType == GlAccountTypes.Asset ||
                            posting.AccountType == GlAccountTypes.Expense)
                        {
                            netIncrement = debit - credit;
                            ending = begninning + netIncrement;
                            nextBegining = begninning + netIncrement;

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital ||
                                 posting.AccountType == GlAccountTypes.Revenue)
                        {
                            netIncrement = credit - debit;
                            ending = begninning + netIncrement;
                            nextBegining = begninning + netIncrement;
                        }
                        //update or create
                        if (endRecord != null)
                        {
                            #region update ending post ledger line

                            endRecord.Debit = debit;
                            endRecord.Credit = credit;
                            endRecord.Balance = ending;
                            _db.Entry(endRecord).State = EntityState.Modified;
                            //_db.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region add ending post ledger line

                            var periodName = _fiscalYearHelper.GetPeriodName(beginningPeriod.GLPeriodId);
                            var newLedgerEnding = new GlSecondayLedgerLine
                            {
                                GlSecondayLedger = posting,
                                JournalNumber = "Ending-" + periodName,
                                JournalReference = "Ending-" + periodName,
                                EffectiveDate = beginningPeriodObj.DateTo,
                                JournalName = "Ending-" + periodName,
                                CategoryName = "Ending-" + periodName,
                                SourcesName = "Ending-" + periodName,
                                GLPeriodId = beginningPeriodObj.GLPeriodId,
                                GlFiscalYearId = beginningPeriodObj.GlFiscalYearId,
                                Type = LedgerPostTypes.Ending,
                                Debit = debit,
                                Credit = credit,
                                Balance = ending
                            };
                            _db.GlSecondayLedgerLines.Add(newLedgerEnding);
                            // _db.SaveChanges();

                            #endregion
                        }

                        #endregion

                        foreach (var p in periods)
                        {
                            netIncrement = 0;
                            debit = 0;
                            credit = 0;
                            var period = p;
                            #region next periods

                            var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);

                            var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard);

                            var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);


                            begninning = nextBegining;

                            //update or create beginning
                            if (begnningRecod != null)
                            {
                                #region update beginning ledger line
                                begnningRecod.Balance = begninning;
                                _db.Entry(begnningRecod).State = EntityState.Modified;
                                // _db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add beginning ledger line

                                var periodName = _fiscalYearHelper.GetPeriodName(period.GLPeriodId);
                                var newBeginning = new GlSecondayLedgerLine
                                {
                                    GlSecondayLedger = posting,
                                    JournalNumber = "Beginning-" + periodName,
                                    JournalReference = "Beginning-" + periodName,
                                    EffectiveDate = period.DateFrom,
                                    JournalName = "Beginning-" + periodName,
                                    CategoryName = "Beginning-" + periodName,
                                    SourcesName = "Beginning-" + periodName,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Begining,
                                    Balance = begninning
                                };
                                _db.GlSecondayLedgerLines.Add(newBeginning);
                                // _db.SaveChanges();

                                #endregion
                            }
                            if (standardRecords.Any())
                            {
                                debit = standardRecords.Sum(x => x.Debit);
                                credit = standardRecords.Sum(x => x.Credit);
                            }
                            if (posting.AccountType == GlAccountTypes.Asset ||
                                posting.AccountType == GlAccountTypes.Expense)
                            {
                                netIncrement = debit - credit;
                                ending = begninning + netIncrement;
                                nextBegining = begninning + netIncrement;

                            }
                            else if (posting.AccountType == GlAccountTypes.Liability ||
                                     posting.AccountType == GlAccountTypes.Capital ||
                                     posting.AccountType == GlAccountTypes.Revenue)
                            {
                                netIncrement = credit - debit;
                                ending = begninning + netIncrement;
                                nextBegining = begninning + netIncrement;
                            }
                            //update or create
                            if (endingRecord != null)
                            {
                                #region update ending post ledger line

                                endingRecord.Debit = debit;
                                endingRecord.Credit = credit;
                                endingRecord.Balance = ending;
                                _db.Entry(endingRecord).State = EntityState.Modified;
                                //_db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add ending post ledger line

                                var periodName = _fiscalYearHelper.GetPeriodName(period.GLPeriodId);
                                var newEnding = new GlSecondayLedgerLine
                                {
                                    GlSecondayLedger = posting,
                                    JournalNumber = "Ending-" + periodName,
                                    JournalReference = "Ending-" + periodName,
                                    EffectiveDate = period.DateTo,
                                    JournalName = "Ending-" + periodName,
                                    CategoryName = "Ending-" + periodName,
                                    SourcesName = "Ending-" + periodName,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Ending,
                                    Debit = debit,
                                    Credit = credit,
                                    Balance = ending
                                };
                                _db.GlSecondayLedgerLines.Add(newEnding);
                                //_db.SaveChanges();

                                #endregion
                            }

                            #endregion 

                        }

                        //_db.SaveChanges();

                    }
                    _db.SaveChanges();
                }
            }
        }
        public  void UpdateAllBegEndBalances()
        {
            var ledgerPostingLines = _db.GlSecondayLedgerLines.AsQueryable();
            var ledgerPostings = _db.GlSecondayLedgers.OrderBy(x => x.GlSecondayLedgerId).ToList();

            var status = _db.GlobalStatus.FirstOrDefault(x => x.OperationType == "SecondayLedgerUpdate");

            if (status == null)
            {
                var newStatus = new GlobalStatus
                {
                    OperationType = "SecondayLedgerUpdate",
                    Status = "Running",
                    Step = ""
                };
                _db.GlobalStatus.Add(newStatus);
                _db.SaveChanges();

                status = newStatus;

            }
            else
            {
                if (status.Status == "Finished")
                {
                    status.Step = "0";
                    status.Status = "Running";
                    _db.Entry(status).State = EntityState.Modified;
                }
                ledgerPostings = ledgerPostings.Where(x => x.GlSecondayLedgerId > int.Parse(status.Step)).ToList();

            }




            var periods = _db.GLPeriods.OrderBy(x => x.GLPeriodId).ToList();

            var beginningPeriod = _db.GlBeginningPeriods.FirstOrDefault();
            if (beginningPeriod != null)
            {
                var beginningPeriodObj = _db.GLPeriods.Find(beginningPeriod.GLPeriodId);
                if (beginningPeriodObj != null)
                {
                    // periods after beginning and and already started
                    periods = periods.Where(x => x.DateFrom > beginningPeriodObj.DateFrom)
                                    .Where(x => x.DateFrom <= DateTime.Today).ToList();
                    decimal begninning = 0;
                    decimal ending = 0;
                    decimal netIncrement = 0;
                    decimal debit = 0;
                    decimal credit = 0;
                    decimal nextBegining = 0;

                    foreach (var ledgerPosting in ledgerPostings)
                    {
                        var posting = ledgerPosting;
                        var selectedLines = ledgerPostingLines.Where(x => x.GlSecondayLedger.AccountCode == posting.AccountCode);

                        #region beginning period

                        var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
                        var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Standard);
                        var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                            .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

                        if (begRecord != null)
                        {

                            if (posting.AccountType == GlAccountTypes.Asset ||
                                posting.AccountType == GlAccountTypes.Expense)
                            {
                                begninning = begRecord.Debit - begRecord.Credit;
                                begRecord.Balance = begninning;

                            }
                            else if (posting.AccountType == GlAccountTypes.Liability ||
                                     posting.AccountType == GlAccountTypes.Capital ||
                                     posting.AccountType == GlAccountTypes.Revenue)
                            {
                                begninning = begRecord.Credit - begRecord.Debit;
                                begRecord.Balance = begninning;
                            }

                            _db.Entry(begRecord).State = EntityState.Modified;

                        }
                        else
                        {
                            begninning = 0;
                            #region add beginning ledger line

                            var newBeginning = new GlSecondayLedgerLine
                            {
                                GlSecondayLedger = posting,
                                JournalNumber = "Beginning-" + beginningPeriod.GLPeriodId,
                                JournalReference = "Beginning-" + beginningPeriod.GLPeriodId,
                                EffectiveDate = beginningPeriodObj.DateFrom,
                                JournalName = "Beginning-" + beginningPeriod.GLPeriodId,
                                CategoryName = "Beginning-" + beginningPeriod.GLPeriodId,
                                SourcesName = "Beginning-" + beginningPeriod.GLPeriodId,
                                GLPeriodId = beginningPeriod.GLPeriodId,
                                GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                Type = LedgerPostTypes.Begining,
                                Balance = begninning
                            };
                            _db.GlSecondayLedgerLines.Add(newBeginning);
                            // _db.SaveChanges();

                            #endregion 
                        }

                        if (standardRecord.Any())
                        {
                            debit = standardRecord.Sum(x => x.Debit);
                            credit = standardRecord.Sum(x => x.Credit);
                        }


                        if (posting.AccountType == GlAccountTypes.Asset ||
                            posting.AccountType == GlAccountTypes.Expense)
                        {
                            netIncrement = debit - credit;
                            ending = begninning + netIncrement;
                            nextBegining = begninning + netIncrement;

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital ||
                                 posting.AccountType == GlAccountTypes.Revenue)
                        {
                            netIncrement = credit - debit;
                            ending = begninning + netIncrement;
                            nextBegining = begninning + netIncrement;
                        }
                        //update or create
                        if (endRecord != null)
                        {
                            #region update ending post ledger line

                            endRecord.Debit = debit;
                            endRecord.Credit = credit;
                            endRecord.Balance = ending;
                            _db.Entry(endRecord).State = EntityState.Modified;
                            //_db.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region add ending post ledger line

                            var newLedgerEnding = new GlSecondayLedgerLine
                            {
                                GlSecondayLedger = posting,
                                JournalNumber = "Ending-" + beginningPeriod.GLPeriodId,
                                JournalReference = "Ending-" + beginningPeriod.GLPeriodId,
                                EffectiveDate = beginningPeriodObj.DateTo,
                                JournalName = "Ending-" + beginningPeriod.GLPeriodId,
                                CategoryName = "Ending-" + beginningPeriod.GLPeriodId,
                                SourcesName = "Ending-" + beginningPeriod.GLPeriodId,
                                GLPeriodId = beginningPeriodObj.GLPeriodId,
                                GlFiscalYearId = beginningPeriodObj.GlFiscalYearId,
                                Type = LedgerPostTypes.Ending,
                                Debit = debit,
                                Credit = credit,
                                Balance = ending
                            };
                            _db.GlSecondayLedgerLines.Add(newLedgerEnding);
                            // _db.SaveChanges();

                            #endregion
                        }

                        #endregion

                        foreach (var p in periods)
                        {
                            netIncrement = 0;
                            debit = 0;
                            credit = 0;
                            var period = p;
                            #region next periods

                            var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);

                            var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard);

                            var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);


                            begninning = nextBegining;

                            //update or create beginning
                            if (begnningRecod != null)
                            {
                                #region update beginning ledger line
                                begnningRecod.Balance = begninning;
                                _db.Entry(begnningRecod).State = EntityState.Modified;
                                // _db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add beginning ledger line

                                var newBeginning = new GlSecondayLedgerLine
                                {
                                    GlSecondayLedger = posting,
                                    JournalNumber = "Beginning-" + period.GLPeriodId,
                                    JournalReference = "Beginning-" + period.GLPeriodId,
                                    EffectiveDate = period.DateFrom,
                                    JournalName = "Beginning-" + period.GLPeriodId,
                                    CategoryName = "Beginning-" + period.GLPeriodId,
                                    SourcesName = "Beginning-" + period.GLPeriodId,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Begining,
                                    Balance = begninning
                                };
                                _db.GlSecondayLedgerLines.Add(newBeginning);
                                // _db.SaveChanges();

                                #endregion
                            }
                            if (standardRecords.Any())
                            {
                                debit = standardRecords.Sum(x => x.Debit);
                                credit = standardRecords.Sum(x => x.Credit);
                            }
                            if (posting.AccountType == GlAccountTypes.Asset ||
                                posting.AccountType == GlAccountTypes.Expense)
                            {
                                netIncrement = debit - credit;
                                ending = begninning + netIncrement;
                                nextBegining = begninning + netIncrement;

                            }
                            else if (posting.AccountType == GlAccountTypes.Liability ||
                                     posting.AccountType == GlAccountTypes.Capital ||
                                     posting.AccountType == GlAccountTypes.Revenue)
                            {
                                netIncrement = credit - debit;
                                ending = begninning + netIncrement;
                                nextBegining = begninning + netIncrement;
                            }
                            //update or create
                            if (endingRecord != null)
                            {
                                #region update ending post ledger line

                                endingRecord.Debit = debit;
                                endingRecord.Credit = credit;
                                endingRecord.Balance = ending;
                                _db.Entry(endingRecord).State = EntityState.Modified;
                                //_db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add ending post ledger line

                                var newEnding = new GlSecondayLedgerLine
                                {
                                    GlSecondayLedger = posting,
                                    JournalNumber = "Ending-" + period.GLPeriodId,
                                    JournalReference = "Ending-" + period.GLPeriodId,
                                    EffectiveDate = period.DateTo,
                                    JournalName = "Ending-" + period.GLPeriodId,
                                    CategoryName = "Ending-" + period.GLPeriodId,
                                    SourcesName = "Ending-" + period.GLPeriodId,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Ending,
                                    Debit = debit,
                                    Credit = credit,
                                    Balance = ending
                                };
                                _db.GlSecondayLedgerLines.Add(newEnding);
                                //_db.SaveChanges();

                                #endregion
                            }

                            #endregion

                        }

                        status.Step = ledgerPosting.GlSecondayLedgerId.ToString();
                        _db.SaveChanges();

                    }
                    status.Status = "Finished";
                    _db.Entry(status).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
        }

        public  void PostBegToLedger(int id,string user)
        {
           
            var record = _db.GLRecordJournals.Find(id);
            if (record != null)
            {
                #region header

                var ledger = new PostJournalVm
                {
                    PostJournalVmId = id,
                    JournalName = record.JournalName,
                    JournalNumber = record.JournalNumber,
                    Description = record.Description,
                    CategoryName = record.CategoryName,
                    SourcesName = record.SourcesName,
                    EffectiveDate = record.EffectiveDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.EffectiveDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.EffectiveDate),
                    PostingSource = JournalSources.GeneralLedger,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id).AsNoTracking().ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.GlRecordJournalDetailId,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit
                    };
                    ledger.PostJournalLineVms.Add(ledgerline);
                }

                #endregion

                PostToSecondLedger(ledger, user);
            }

        }

        //reporting

        public  decimal GetAccountEndBalancesWithSubAccount(string account, string subaccount)
        {
            List<GLPeriod> periods = new List<GLPeriod>();

            decimal accountyearEndBalances = 0;

            var ledgerPostingLines = _db.GlSecondayLedgerLines
                .Where(x => x.GlSecondayLedger.AccountSegment == account)
                .Where(x => x.GlSecondayLedger.SubAccountSegment == subaccount)
                .Where(x => x.Type == LedgerPostTypes.Ending).AsQueryable();

            var periodList = ledgerPostingLines.Select(x => x.GLPeriodId).Distinct();
            foreach (var p in periodList)
            {
                periods.Add(_db.GLPeriods.Find(p));
            }


            if (periods.Any())
            {
                var maxPeriodDateTo = periods.Max(x => x.DateTo);
                var maxPeriod = periods.FirstOrDefault(x => x.DateTo == maxPeriodDateTo);
                accountyearEndBalances =ledgerPostingLines.Where(x => x.GLPeriodId == maxPeriod.GLPeriodId).Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public  decimal GetSupplierAdvanceBalance(string supplierid)
        {
            decimal balance = 0;
            var advanceaccount = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Supplier);
            if (advanceaccount != null)
            {
                string account = _accountHelper.GetNaturalAccountSegment(advanceaccount.PrepaymentAccountCode);
                balance = GetAccountEndBalancesWithSubAccount(account, supplierid);
            }

            return balance;
        }

        public  decimal GetPurchaserAdvanceBalance(int empid)
        {

            decimal balance = 0;
            var subAccount = _db.EmployeeSubAccounts.Where(x => x.EmployeeId == empid).FirstOrDefault(x => x.Enable);

            var advanceaccount = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Purchaser);
            if (advanceaccount != null && subAccount!=null)
            {
                string account = _accountHelper.GetNaturalAccountSegment(advanceaccount.PrepaymentAccountCode);
                balance = GetAccountEndBalancesWithSubAccount(account, subAccount.SubAccountCode);
            }

            return balance;
        }

        public  decimal GetEmployeeAdvanceBalance(int empid)
        {
            decimal balance = 0;
            var subAccount = _db.EmployeeSubAccounts.Where(x => x.EmployeeId == empid).FirstOrDefault(x => x.Enable);

            var advanceaccount = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Employee);
            if (advanceaccount != null && subAccount != null)
            {
                string account = _accountHelper.GetNaturalAccountSegment(advanceaccount.PrepaymentAccountCode);
                balance = GetAccountEndBalancesWithSubAccount(account, subAccount.SubAccountCode);
            }

            return balance;

        }
    }

    public class PostJournalVm 
    {
        public int PostJournalVmId { get; set; }
        public string JournalName { get; set; }
        public string Description { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GlPeriodId { get; set; }
        public string CategoryName { get; set; }
        public string SourcesName { get; set; }
        public JournalSources PostingSource { get; set; }
        public string JournalNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public virtual ICollection<PostJournalLineVm> PostJournalLineVms { get; set; }
        public class PostJournalLineVm 
        {
            public int PostJournalLineVmId { get; set; }
            public string AccountCode { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
        }

    }
}
