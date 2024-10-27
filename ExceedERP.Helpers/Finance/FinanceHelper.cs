using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Finance.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Helpers.Finance
{
    public partial class FinanceHelper
    {

        private readonly AccountHelper _accountHelper;
        private readonly DynamicAccountHelper _dynamicAccountHelper;
        private readonly FiscalYearHelper _fiscalYearHelper ;

        public FinanceHelper()
        {
            _accountHelper = new AccountHelper();
            _dynamicAccountHelper=new DynamicAccountHelper();
            _fiscalYearHelper=new FiscalYearHelper();   
        }

        public static bool CalculateProfitTax()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.GlRuleValues.Where(x => x.CalculateProfitTax).Any();
            }
        }


        //public  List<InventoryTransactionViewModel> getInventoryTransactionData()
        //{
        //    List<InventoryTransactionViewModel> list = new List<InventoryTransactionViewModel> { };
        //    InventoryTransactionViewModel record;

        //    var goodreceive = db.GoodReceives.Where(x => x.isOnlineApproved == true)
        //                                     .Where(x => x.isOnlineTransferred == false).ToList();

        //    var issue = db.Issues.Where(x => x.isOnlineApproved == true)
        //                                    .Where(x => x.isOnlineTransferred == false).ToList();

        //    var issuefuel = db.IssueFuels.Where(x => x.isOnlineApproved == true)
        //                                    .Where(x => x.isOnlineTransferred == false).ToList();

        //    var transfer = db.ItemTransfers.Where(x => x.isOnlineApproved == true)
        //                                    .Where(x => x.isOnlineTransferred == false).ToList();

        //    var purchasereturn = db.PurchaseReturns.Where(x => x.isOnlineApproved == true)
        //                                   .Where(x => x.isOnlineTransferred == false).ToList();

        //    var storereturn = db.StoreReturns.Where(x => x.isOnlineApproved == true)
        //                                   .Where(x => x.isOnlineTransferred == false).ToList();

        //    var stockadjustment = db.StockAdjustments.Where(x => x.isOnlineApproved == true)
        //                                  .Where(x => x.isOnlineTransferred == false).ToList();
        //    #region receive
        //    foreach (GoodReceive item in goodreceive)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Goods Recive";
        //        record.ID = item.GoodReceiveId;
        //        record.Controller = "GoodReceives";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region issue
        //    foreach (Issue item in issue)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Issue";
        //        record.ID = item.IssueID;
        //        record.Controller = "Issues";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region issue fuel
        //    foreach (IssueFuel item in issuefuel)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Issue Fuel";
        //        record.ID = item.IssueFuelID;
        //        record.Controller = "IssueFuels";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region transfer
        //    foreach (ItemTransfer item in transfer)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Item Transfer";
        //        record.ID = item.ItemTransferID;
        //        record.Controller = "ItemTransfers";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region purchase return
        //    foreach (PurchaseReturn item in purchasereturn)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Purchase Return";
        //        record.ID = item.PurchaseReturnID;
        //        record.Controller = "PurchaseReturns";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region store return
        //    foreach (StoreReturn item in storereturn)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Store Return";
        //        record.ID = item.StoreReturnID;
        //        record.Controller = "StoreReturns";
        //        list.Add(record);
        //    }
        //    #endregion
        //    #region adjustment
        //    foreach (StockAdjustment item in stockadjustment)
        //    {
        //        record = new InventoryTransactionViewModel();
        //        record.User = item.CreatedBy;
        //        record.Date = item.DateCreated;
        //        record.Type = "Stock Adjustment";
        //        record.ID = item.StockAdjustmentID;
        //        record.Controller = "StockAdjustments";
        //        list.Add(record);
        //    }
        //    #endregion

        //    return list;
        //}


        public void TransferIncomeStatement()
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal totalRevenue = 0;
            decimal totalExpense = 0;
            decimal netAmount = 0;
            decimal netAmount30 = 0;
            decimal netAmount70 = 0;
            decimal netAmount7040 = 0;
            decimal netAmount7060 = 0;

            DateTime today = DateTime.Today;

            var allFyears = db.GLFiscalYears.ToList();
            var previousFyears = allFyears.Where(x => x.DateTo < today).ToList();

            DateTime dt11 = new DateTime();
            DateTime dt22 = new DateTime();
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == Core.Domain.Finance.GL.Dynamic.DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            //var locations = db.GLChartOfAccountLocations.ToList();
            var revenueAccounts = db.GLChartOfAccountAccounts.Where(x => x.AccountType.Equals(GlAccountTypes.Revenue)).ToList();
            var expenseAccounts = db.GLChartOfAccountAccounts.Where(x => x.AccountType.Equals(GlAccountTypes.Expense)).ToList();

            foreach (GlFiscalYear fyear in previousFyears)
            {
                totalRevenue = 0;
                totalExpense = 0;
                netAmount = 0;
                netAmount30 = 0;
                netAmount70 = 0;
                netAmount7040 = 0;
                netAmount7060 = 0;

                int nextYear = 0;
                //check record
                var record = db.FinInStatementRecords.Where(x => x.FYear == fyear.Name.ToString()).Where(x => x.Status == "OK").ToList();
                if (!record.Any())
                {
                    //get next year
                    nextYear = _fiscalYearHelper.GetNextFYearName(fyear.Name);
                    if (nextYear != 0)
                    {
                        string period = _fiscalYearHelper.GetFirstPeriodName(nextYear);
                        var periodObj = db.GLPeriods.FirstOrDefault(x => x.Name == period);
                        if (periodObj != null)
                        {
                            dt11 = fyear.DateFrom;
                            dt22 = fyear.DateTo;

                            var journals = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.IsOnlineApproved == true)
                                                                    .Where(x => x.GLRecordJournal.EffectiveDate >= dt11)
                                                                    .Where(x => x.GLRecordJournal.EffectiveDate <= dt22).ToList();
                            foreach (var location in locations)
                            {
                                foreach (GLChartOfAccountAccount account in revenueAccounts)
                                {
                                    #region total revenue
                                    var revenueObj = journals.Where(x => x.AccountCode == location.Values + "-" + account.Values).ToList();
                                    if (revenueObj != null)
                                    {
                                        totalRevenue = totalRevenue + revenueObj.Sum(x => x.Credit) - revenueObj.Sum(x => x.Debit);
                                    }
                                    else
                                    {
                                        totalRevenue = totalRevenue + 0;
                                    }
                                    #endregion
                                }

                                foreach (GLChartOfAccountAccount account in expenseAccounts)
                                {
                                    #region total expense
                                    var expenseObj = journals.Where(x => x.AccountCode == location.Values + "-" + account.Values).ToList();
                                    if (expenseObj != null)
                                    {
                                        totalExpense = totalExpense + expenseObj.Sum(x => x.Debit) - expenseObj.Sum(x => x.Credit);
                                    }
                                    else
                                    {
                                        totalExpense = totalExpense + 0;
                                    }
                                    #endregion
                                }
                            }

                            netAmount = totalRevenue - totalExpense;
                            netAmount30 = netAmount * (decimal)0.30;
                            netAmount70 = netAmount * (decimal)0.70;
                            netAmount7040 = netAmount70 * (decimal)0.40;
                            netAmount7060 = netAmount70 * (decimal)0.60;
                            #region record gl
                            GlRecordJournal journal = new GlRecordJournal();
                            journal.DateCreated = DateTime.Now;
                            journal.CreatedBy = "System";
                            journal.JournalName = "Transfer";
                            journal.Description = "Income Statement Transfer from " + fyear.Name.ToString();
                            journal.FiscalYear = nextYear.ToString();
                            journal.PeriodName = periodObj.Name;
                            journal.CategoryName = "Income Statement Transfer";
                            journal.SourcesName = "GL";
                            journal.JournalNumber = "Income Statement Transfer-" + fyear.Name.ToString();
                            journal.EffectiveDate = periodObj.DateFrom;
                            journal.Status = "Approved";
                            journal.IsOnlineApproved = true;
                            journal.OnlineApprovedBy = "System";
                            journal.OnlineApprovedTime = DateTime.Now;
                            db.GLRecordJournals.Add(journal);
                            db.SaveChanges();
                            #endregion
                            #region credit 30 %
                            GLRecordJournalDetail journalDetail = new GLRecordJournalDetail();
                            journalDetail.DateCreated = DateTime.Now;
                            journalDetail.CreatedBy = "System";
                            journalDetail.GLRecordJournal = journal;
                            journalDetail.AccountCode = "1000-2301";
                            journalDetail.Debit = 0;
                            journalDetail.Credit = netAmount30;
                            db.GLRecordJournalDetails.Add(journalDetail);
                            db.SaveChanges();
                            #endregion
                            #region credit 70-40 %
                            journalDetail = new GLRecordJournalDetail();
                            journalDetail.DateCreated = DateTime.Now;
                            journalDetail.CreatedBy = "System";
                            journalDetail.GLRecordJournal = journal;
                            journalDetail.AccountCode = "1000-2303";
                            journalDetail.Debit = 0;
                            journalDetail.Credit = netAmount7040;
                            db.GLRecordJournalDetails.Add(journalDetail);
                            db.SaveChanges();
                            #endregion
                            #region credit 70-60 %
                            journalDetail = new GLRecordJournalDetail();
                            journalDetail.DateCreated = DateTime.Now;
                            journalDetail.CreatedBy = "System";
                            journalDetail.GLRecordJournal = journal;
                            journalDetail.AccountCode = "1000-2302";
                            journalDetail.Debit = 0;
                            journalDetail.Credit = netAmount7060;
                            db.GLRecordJournalDetails.Add(journalDetail);
                            db.SaveChanges();
                            #endregion
                            #region log
                            FinInStatementRecord log = new FinInStatementRecord();
                            log.DateCreated = DateTime.Now;
                            log.CreatedBy = "System";
                            log.FYear = fyear.Name.ToString();
                            log.CreatedBy = "OK";
                            db.FinInStatementRecords.Add(log);
                            db.SaveChanges();
                            #endregion

                        }
                    }
                }
            }




        }

        //public  void PostSingleToLedger1(int journalId, string user)
        //{
        //    GlRecordJournal gLRecordJournal = _db.GLRecordJournals.Find(journalId);
        //    if (gLRecordJournal != null)
        //    {
        //        foreach (GLRecordJournalDetail journalline in gLRecordJournal.GLRecordJournalDetail)
        //        {
        //            #region search if ledger is already created
        //            var ledger = _db.GLLedgerPostings.FirstOrDefault(x => x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());
        //            #endregion
        //            if (ledger != null)
        //            {
        //                var period = _db.GLPeriods
        //                    .FirstOrDefault(x => x.Name.ToUpper() == journalline.GLRecordJournal.PeriodName.ToUpper());

        //                GLLedgerPosting ledgerpost = ledger;
        //                #region post ledger line

        //                var ledgerpostline = new GLLedgerPostingLines
        //                {
        //                    GLLedgerPosting = ledgerpost,
        //                    JournalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId,
        //                    JournalReference = journalline.GLRecordJournal.JournalNumber,
        //                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
        //                    JournalName = journalline.GLRecordJournal.JournalName,
        //                    CategoryName = journalline.GLRecordJournal.CategoryName,
        //                    SourcesName = journalline.GLRecordJournal.SourcesName
        //                };
        //                if (period != null)
        //                {
        //                    ledgerpostline.GLPeriodId = period.GLPeriodId;
        //                    ledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
        //                }
        //                if (journalline.Debit > 0)
        //                {
        //                    #region debit balance
        //                    ledgerpostline.Debit = journalline.Debit;
        //                    ledgerpostline.Credit = 0;
        //                    if ((ledgerpost.AccountType == GlAccountTypes.Asset) || (ledgerpost.AccountType == GlAccountTypes.Expense))
        //                    {
        //                        #region get last balance
        //                        int maxid = ledgerpost.GLLedgerPostingLines.Max(x => x.GLLedgerPostingLinesID);
        //                        var balance = ledgerpost.GLLedgerPostingLines.FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
        //                        #endregion

        //                        if (balance != null) ledgerpostline.Balance = balance.Balance + ledgerpostline.Debit;
        //                    }
        //                    else if ((ledgerpost.AccountType == GlAccountTypes.Liability) || (ledgerpost.AccountType == GlAccountTypes.Capital) || (ledgerpost.AccountType == GlAccountTypes.Revenue))
        //                    {
        //                        #region get last balance
        //                        int maxid = ledgerpost.GLLedgerPostingLines.Max(x => x.GLLedgerPostingLinesID);
        //                        var balance = ledgerpost.GLLedgerPostingLines.FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
        //                        #endregion

        //                        if (balance != null) ledgerpostline.Balance = balance.Balance - ledgerpostline.Debit;
        //                    }


        //                    #endregion
        //                }
        //                else if (journalline.Credit > 0)
        //                {
        //                    #region credit balance
        //                    ledgerpostline.Credit = journalline.Credit;
        //                    ledgerpostline.Debit = 0;
        //                    if ((ledgerpost.AccountType == GlAccountTypes.Asset) || (ledgerpost.AccountType == GlAccountTypes.Expense))
        //                    {
        //                        #region get last balance
        //                        int maxid = ledgerpost.GLLedgerPostingLines.Max(x => x.GLLedgerPostingLinesID);
        //                        var balance = ledgerpost.GLLedgerPostingLines.FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
        //                        #endregion

        //                        if (balance != null) ledgerpostline.Balance = balance.Balance - ledgerpostline.Credit;
        //                    }
        //                    else if ((ledgerpost.AccountType == GlAccountTypes.Liability) || (ledgerpost.AccountType == GlAccountTypes.Capital) || (ledgerpost.AccountType == GlAccountTypes.Revenue))
        //                    {
        //                        #region get last balance
        //                        int maxid = ledgerpost.GLLedgerPostingLines.Max(x => x.GLLedgerPostingLinesID);
        //                        var balance = ledgerpost.GLLedgerPostingLines.FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
        //                        #endregion

        //                        if (balance != null) ledgerpostline.Balance = balance.Balance + ledgerpostline.Credit;
        //                    }

        //                    else
        //                    {

        //                    }
        //                    #endregion
        //                }
        //                else
        //                {

        //                }
        //                _db.GLLedgerPostingLines.Add(ledgerpostline);
        //                _db.SaveChanges();
        //                #endregion
        //            }
        //            else
        //            {
        //                #region create ledger
        //                GLLedgerPosting ledgerpost = new GLLedgerPosting();
        //                ledgerpost.AccountCode = journalline.AccountCode.ToUpper();
        //                ledgerpost.AccountDesc = FinanceHelper.GetAccountDesc(ledgerpost.AccountCode);
        //                ledgerpost.AccountType = FinanceHelper.GetAccountType(ledgerpost.AccountCode);
        //                _db.GLLedgerPostings.Add(ledgerpost);
        //                _db.SaveChanges();
        //                #endregion
        //                #region post ledger line

        //                var period = _db.GLPeriods
        //                    .FirstOrDefault(x => x.Name.ToUpper() == journalline.GLRecordJournal.PeriodName.ToUpper());

        //                var ledgerpostline = new GLLedgerPostingLines
        //                {
        //                    GLLedgerPosting = ledgerpost,
        //                    JournalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId,
        //                    JournalReference = journalline.GLRecordJournal.JournalNumber,
        //                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
        //                    JournalName = journalline.GLRecordJournal.JournalName,
        //                    CategoryName = journalline.GLRecordJournal.CategoryName,
        //                    SourcesName = journalline.GLRecordJournal.SourcesName
        //                };
        //                if (period != null)
        //                {
        //                    ledgerpostline.GLPeriodId = period.GLPeriodId;
        //                    ledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
        //                }
        //                if (journalline.Debit > 0)
        //                {
        //                    #region debit balance
        //                    ledgerpostline.Debit = journalline.Debit;
        //                    ledgerpostline.Credit = 0;
        //                    if ((ledgerpost.AccountType == GlAccountTypes.Asset) || (ledgerpost.AccountType == GlAccountTypes.Expense))
        //                    {
        //                        ledgerpostline.Balance = ledgerpostline.Debit;
        //                    }
        //                    else if ((ledgerpost.AccountType == GlAccountTypes.Liability) || (ledgerpost.AccountType == GlAccountTypes.Capital) || (ledgerpost.AccountType == GlAccountTypes.Revenue))
        //                    {
        //                        ledgerpostline.Balance = -ledgerpostline.Debit;
        //                    }

        //                    else
        //                    {

        //                    }
        //                    #endregion
        //                }
        //                else if (journalline.Credit > 0)
        //                {
        //                    #region credit balance
        //                    ledgerpostline.Credit = journalline.Credit;
        //                    ledgerpostline.Debit = 0;
        //                    if ((ledgerpost.AccountType == GlAccountTypes.Asset) || (ledgerpost.AccountType == GlAccountTypes.Expense))
        //                    {
        //                        ledgerpostline.Balance = -ledgerpostline.Credit;
        //                    }
        //                    else if ((ledgerpost.AccountType == GlAccountTypes.Liability) || (ledgerpost.AccountType == GlAccountTypes.Capital) || (ledgerpost.AccountType == GlAccountTypes.Revenue))
        //                    {
        //                        ledgerpostline.Balance = ledgerpostline.Credit;
        //                    }

        //                    else
        //                    {

        //                    }
        //                    #endregion
        //                }
        //                else
        //                {

        //                }
        //                _db.GLLedgerPostingLines.Add(ledgerpostline);
        //                _db.SaveChanges();
        //                #endregion
        //            }

        //        }
        //        #region update journal posting status
        //        gLRecordJournal.Posting = "POSTED";
        //        gLRecordJournal.Status = "POSTED";
        //        gLRecordJournal.isPosted = true;
        //        gLRecordJournal.PostedBy = user;
        //        gLRecordJournal.PostedTime = DateTime.Now;
        //        _db.Entry(gLRecordJournal).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        #endregion
        //    }


        //}
        public  bool PostSingleToLedger(int journalId, string user)
        {
           ExceedDbContext db = new ExceedDbContext();
           var gLRecordJournal = db.GLRecordJournals.Find(journalId);

            if (gLRecordJournal != null && !gLRecordJournal.isPosted)
            {
                #region check and create ledegr--- new code

                //var lines = db.GLRecordJournalDetails
                //    .Where(x => x.GlRecordJournalId == gLRecordJournal.GlRecordJournalId).ToList();
                //foreach (var line in lines)
                //{
                //    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                //        x.AccountCode.ToUpper() == line.AccountCode.ToUpper());
                //    if (ledger == null)
                //    {
                //        #region create ledger

                //        var ledgerPosting = new GLLedgerPosting
                //        {
                //            AccountCode = line.AccountCode.ToUpper()
                //        };
                //        AccountHelper accountHelper = new AccountHelper();
                //        ledgerPosting.AccountDesc = accountHelper.GetAccountDesc(ledgerPosting.AccountCode);
                //        ledgerPosting.AccountType = accountHelper.GetAccountType(ledgerPosting.AccountCode);
                //        ledgerPosting.CreatedBy = "System-" + user;
                //        ledgerPosting.DateCreated=DateTime.Now;
                //        db.GLLedgerPostings.Add(ledgerPosting);
                //        db.SaveChanges();

                //        #endregion
                //        #region split ledger account

                //        if (accountHelper.IsDynamicCoA())
                //        {
                //            _dynamicAccountHelper.SplitLedgerAccount(ledgerPosting);
                //        }
                //        else
                //        {
                //            accountHelper.SplitLedgerAccount(ledgerPosting);
                //        }

                //        #endregion
                //    }
                //}

                #endregion
                var status = db.GlobalStatus.FirstOrDefault(x =>
                    x.OperationType == "LedgerUpdateSingle-" + gLRecordJournal.GlRecordJournalId);
                if (status == null)
                {
                    var newStatus = new GlobalStatus
                    {
                        OperationType = "LedgerUpdateSingle-" + gLRecordJournal.GlRecordJournalId,
                        Status = "Running",
                        Step = gLRecordJournal.GlRecordJournalId.ToString()
                    };
                    db.GlobalStatus.Add(newStatus);
                    db.SaveChanges();

                    status = newStatus;

                    Post(journalId, user);
                    //PostRawSql(journalId, user);

                    status.Status = "Finished";
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (status.Status == "Finished" && !gLRecordJournal.IsUnPosted)
                    {
                        db.GlobalStatus.Remove(status);
                        db.SaveChanges();
                    }
                    else if (status.Status == "Finished" && gLRecordJournal.IsUnPosted)
                    {
                        db.GlobalStatus.Remove(status);
                        db.SaveChanges();
                        //because it is unposted ,need to post again
                        Post(journalId, user);
                    }
                    else
                    {
                        //run again..it was not finished
                        status.Step = gLRecordJournal.GlRecordJournalId.ToString();
                        status.Status = "Running";
                        db.Entry(status).State = EntityState.Modified;
                        db.SaveChanges();
                        string start = gLRecordJournal.GlRecordJournalId + "-";
                        var incompletePost = db.GLLedgerPostingLines.Where(x => x.JournalNumber.StartsWith(start))
                            .ToList();
                        db.GLLedgerPostingLines.RemoveRange(incompletePost);
                        db.SaveChanges();

                        Post(journalId, user);
                        //PostRawSql(journalId, user);
                        status.Status = "Finished";
                        db.Entry(status).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

                return true;
            }

            return false;
        }

        //public void PostSingleToLedger1(int journalId, string user)
        //{
        //    var gLRecordJournal = Db.GLRecordJournals.Find(journalId);

        //    if (gLRecordJournal != null && !gLRecordJournal.isPosted)
        //    {
        //        var status = Db.GlobalStatus.FirstOrDefault(x => x.OperationType == "LedgerUpdateSingle-" + gLRecordJournal.GlRecordJournalId);
        //        if (status == null)
        //        {
        //            var newStatus = new GlobalStatus
        //            {
        //                OperationType = "LedgerUpdateSingle-" + gLRecordJournal.GlRecordJournalId,
        //                Status = "Running",
        //                Step = gLRecordJournal.GlRecordJournalId.ToString()
        //            };
        //            Db.GlobalStatus.Add(newStatus);
        //            Db.SaveChanges();

        //            status = newStatus;

        //            Post(journalId, user);

        //            status.Status = "Finished";
        //            Db.Entry(status).State = EntityState.Modified;
        //            Db.SaveChanges();
        //        }
        //        else
        //        {
        //            if (status.Status == "Finished")
        //            {
        //                Db.GlobalStatus.Remove(status);
        //                Db.SaveChanges();
        //            }
        //            else
        //            {
        //                //run again..it was not finished
        //                status.Step = gLRecordJournal.GlRecordJournalId.ToString();
        //                status.Status = "Running";
        //                Db.Entry(status).State = EntityState.Modified;
        //                Db.SaveChanges();
        //                string start = gLRecordJournal.GlRecordJournalId + "-";
        //                var incompletePost = Db.GLLedgerPostingLines.Where(x => x.JournalNumber.StartsWith(start))
        //                    .ToList();
        //                Db.GLLedgerPostingLines.RemoveRange(incompletePost);
        //                Db.SaveChanges();

        //                Post(journalId, user);
        //                status.Status = "Finished";
        //                Db.Entry(status).State = EntityState.Modified;
        //                Db.SaveChanges();
        //            }

        //        }

        //    }


        //}

        private  void Post1_old(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = db.GLRecordJournals.Find(journalId);
            if (gLRecordJournal != null)
            {
                var journallines = db.GLRecordJournalDetails
                    .Where(x => x.GlRecordJournalId == journalId).AsNoTracking().ToList();

                foreach (var journalline in journallines)
                {
                    string journalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId;
                    #region jump if journal line is already posted

                    var lines = db.GLLedgerPostingLines.Where(x => x.JournalNumber == journalNumber).ToList();

                    if (lines.Any())
                        continue;

                    #endregion

                    #region search if ledger is already created

                    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                        x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());

                    var period = db.GLPeriods
                        .FirstOrDefault(x =>
                            x.Name.ToUpper() == journalline.GLRecordJournal.PeriodName.ToUpper());

                    #endregion

                    if (ledger != null)
                    {
                        #region post ledger line

                        var ledgerpostline = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledger,
                            JournalNumber = journalNumber,
                            JournalReference = gLRecordJournal.JournalNumber,
                            EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                            JournalName = journalline.GLRecordJournal.JournalName,
                            CategoryName = journalline.GLRecordJournal.CategoryName,
                            SourcesName = journalline.GLRecordJournal.SourcesName,
                            Remark = gLRecordJournal.Remark,
                            AdditionalDescription = journalline.AdditionalDescription,
                            CreatedBy = user,
                            DateCreated = DateTime.Now,
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

                        db.GLLedgerPostingLines.Add(ledgerpostline);
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region create ledger

                        ledger = new GLLedgerPosting
                        {
                            AccountCode = journalline.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        ledger.AccountDesc = accountHelper.GetAccountDesc(ledger.AccountCode);
                        ledger.AccountType = accountHelper.GetAccountType(ledger.AccountCode);
                        //System.InvalidOperationException: 
                        //'The property 'GLLedgerPostingLinesID' is part of the object's key information and cannot be modified. '
                        db.GLLedgerPostings.Add(ledger);
                        db.SaveChanges();

                        #endregion

                        #region post ledger line



                        var newledgerpostline = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledger,
                            JournalNumber = journalNumber,
                            JournalReference = gLRecordJournal.JournalNumber,
                            EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                            JournalName = journalline.GLRecordJournal.JournalName,
                            CategoryName = journalline.GLRecordJournal.CategoryName,
                            SourcesName = journalline.GLRecordJournal.SourcesName,
                            Remark = gLRecordJournal.Remark,
                            AdditionalDescription = journalline.AdditionalDescription,
                            CreatedBy = user,
                            DateCreated = DateTime.Now
                        };
                        if (period != null)
                        {
                            newledgerpostline.GLPeriodId = period.GLPeriodId;
                            newledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                        }

                        if (journalline.Debit > 0)
                        {
                            #region debit balance

                            newledgerpostline.Debit = journalline.Debit;
                            newledgerpostline.Credit = 0;

                            #endregion
                        }
                        else if (journalline.Credit > 0)
                        {
                            #region credit balance

                            newledgerpostline.Credit = journalline.Credit;
                            newledgerpostline.Debit = 0;

                            #endregion
                        }

                        db.GLLedgerPostingLines.Add(newledgerpostline);
                        db.SaveChanges();

                        #endregion

                        #region split ledger account

                        _accountHelper.SplitLedgerAccount(ledger);

                        #endregion
                    }

                }

                #region update journal posting status

                UpdateJournalPostingStatus(journalId,user);

                #endregion

                #region update ledger balances

                //update ledger
                foreach (var journalline in journallines)
                {
                    UpdateBegEndBalances(journalline.AccountCode, gLRecordJournal.PeriodName,user);
                }

                #endregion
            }

        }

        private void  UpdateJournalPostingStatus(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = db.GLRecordJournals.Find(journalId);
            if (gLRecordJournal != null)
            {
                gLRecordJournal.Posting = "POSTED";
                gLRecordJournal.Status = "POSTED";
                gLRecordJournal.isPosted = true;
                gLRecordJournal.PostedBy = user;
                gLRecordJournal.PostedTime = DateTime.Now;
                db.Entry(gLRecordJournal).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void Post_notgood(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = db.GLRecordJournals.Find(journalId);
            if (gLRecordJournal != null)
            {
                var journallines = db.GLRecordJournalDetails
                    .Where(x => x.GlRecordJournalId == journalId).AsNoTracking().ToList();
               
                    using (var context = new ExceedDbContext())
                    {
                        foreach (var journalline in journallines)
                        {
                            string journalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId;
                            #region jump if journal line is already posted

                            var lines = context.GLLedgerPostingLines.Where(x => x.JournalNumber == journalNumber).ToList();

                            if (lines.Any())
                                continue;

                            #endregion

                            #region search if ledger is already created

                            var ledger = context.GLLedgerPostings.FirstOrDefault(x =>
                                x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());

                            var period = context.GLPeriods
                                .FirstOrDefault(x =>
                                    x.Name.ToUpper() == journalline.GLRecordJournal.PeriodName.ToUpper());

                            #endregion

                            if (ledger != null)
                            {
                                #region post ledger line

                                var ledgerpostline = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = ledger,
                                    JournalNumber = journalNumber,
                                    JournalReference = gLRecordJournal.JournalNumber,
                                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                                    JournalName = journalline.GLRecordJournal.JournalName,
                                    CategoryName = journalline.GLRecordJournal.CategoryName,
                                    SourcesName = journalline.GLRecordJournal.SourcesName,
                                    Remark = gLRecordJournal.Remark,
                                    AdditionalDescription=journalline.AdditionalDescription,
                                    ReferenceDoc = journalline.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalline.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalline.GLRecordJournal.InvoiceReferenceDoc,
                                    CreatedBy = user,
                                    DateCreated = DateTime.Now,
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

                                context.GLLedgerPostingLines.Add(ledgerpostline);
                                context.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region create ledger

                                var newledgerpost = new GLLedgerPosting
                                {
                                    AccountCode = journalline.AccountCode.ToUpper()
                                };
                                AccountHelper accountHelper = new AccountHelper();
                                newledgerpost.AccountDesc = accountHelper.GetAccountDesc(newledgerpost.AccountCode);
                                newledgerpost.AccountType = accountHelper.GetAccountType(newledgerpost.AccountCode);
                                context.GLLedgerPostings.Add(newledgerpost);
                                context.SaveChanges();

                                #endregion

                                #region post ledger line



                                var newledgerpostline = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = newledgerpost,
                                    JournalNumber = journalNumber,
                                    JournalReference = gLRecordJournal.JournalNumber,
                                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                                    JournalName = journalline.GLRecordJournal.JournalName,
                                    CategoryName = journalline.GLRecordJournal.CategoryName,
                                    SourcesName = journalline.GLRecordJournal.SourcesName,
                                    Remark = gLRecordJournal.Remark,
                                    AdditionalDescription = journalline.AdditionalDescription,
                                    ReferenceDoc = journalline.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalline.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalline.GLRecordJournal.InvoiceReferenceDoc,
                                    CreatedBy = user,
                                    DateCreated = DateTime.Now
                                };
                                if (period != null)
                                {
                                    newledgerpostline.GLPeriodId = period.GLPeriodId;
                                    newledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                                }

                                if (journalline.Debit > 0)
                                {
                                    #region debit balance

                                    newledgerpostline.Debit = journalline.Debit;
                                    newledgerpostline.Credit = 0;

                                    #endregion
                                }
                                else if (journalline.Credit > 0)
                                {
                                    #region credit balance

                                    newledgerpostline.Credit = journalline.Credit;
                                    newledgerpostline.Debit = 0;

                                    #endregion
                                }

                                context.GLLedgerPostingLines.Add(newledgerpostline);
                                context.SaveChanges();

                            #endregion

                            #region split ledger account

                            _accountHelper.SplitLedgerAccount(newledgerpost);

                            #endregion
                        }
                        }
                    }               

                #region update journal posting status

                gLRecordJournal.Posting = "POSTED";
                gLRecordJournal.Status = "POSTED";
                gLRecordJournal.isPosted = true;
                gLRecordJournal.PostedBy = user;
                gLRecordJournal.PostedTime = DateTime.Now;
                db.Entry(gLRecordJournal).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region update ledger balances

                //update ledger
                foreach (var journalline in journallines)
                {
                    UpdateBegEndBalances(journalline.AccountCode, gLRecordJournal.PeriodName,user);
                }

                #endregion
            }

        }
        //running
        private void Post(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = db.GLRecordJournals.Find(journalId);
            if (gLRecordJournal != null)
            {
                var journallines = db.GLRecordJournalDetails
                    .Where(x => x.GlRecordJournalId == journalId).AsNoTracking().ToList();

                foreach (var journalline in journallines)
                {
                    using (var context = new ExceedDbContext())
                    {
                        string journalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId;
                        #region jump if journal line is already posted

                        var lines = context.GLLedgerPostingLines.FirstOrDefault(x => x.JournalNumber == journalNumber);

                        if (lines==null)
                        {
                            #region search if ledger is already created

                            var ledger = context.GLLedgerPostings.FirstOrDefault(x =>
                                x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());

                            var period = context.GLPeriods
                                .FirstOrDefault(x =>
                                    x.Name.ToUpper() == journalline.GLRecordJournal.PeriodName.ToUpper());

                            #endregion

                            if (ledger != null)
                            {
                                #region post ledger line

                                var ledgerpostline = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = ledger,
                                    JournalNumber = journalNumber,
                                    JournalId= journalline.GlRecordJournalId,
                                    Description = gLRecordJournal.Description,
                                    JournalReference = gLRecordJournal.JournalNumber,
                                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                                    JournalName = journalline.GLRecordJournal.JournalName,
                                    CategoryName = journalline.GLRecordJournal.CategoryName,
                                    SourcesName = journalline.GLRecordJournal.SourcesName,
                                    Remark = gLRecordJournal.Remark,
                                    AdditionalDescription = journalline.AdditionalDescription,                                    
                                    ReferenceDoc = journalline.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalline.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalline.GLRecordJournal.InvoiceReferenceDoc,
                                    CreatedBy = user,
                                    DateCreated = DateTime.Now,
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

                                context.GLLedgerPostingLines.Add(ledgerpostline);
                                context.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region create ledger

                                var newledgerpost = new GLLedgerPosting
                                {
                                    AccountCode = journalline.AccountCode.ToUpper()
                                };
                                AccountHelper accountHelper = new AccountHelper();
                                newledgerpost.AccountDesc = accountHelper.GetAccountDesc(newledgerpost.AccountCode);
                                newledgerpost.AccountType = accountHelper.GetAccountType(newledgerpost.AccountCode);
                                context.GLLedgerPostings.Add(newledgerpost);
                                context.SaveChanges();

                                #endregion

                                #region post ledger line



                                var newledgerpostline = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = newledgerpost,
                                    JournalNumber = journalNumber,
                                    JournalId = journalline.GlRecordJournalId,
                                    JournalReference = gLRecordJournal.JournalNumber,
                                    EffectiveDate = journalline.GLRecordJournal.EffectiveDate,
                                    JournalName = journalline.GLRecordJournal.JournalName,
                                    CategoryName = journalline.GLRecordJournal.CategoryName,
                                    SourcesName = journalline.GLRecordJournal.SourcesName,
                                    Remark = gLRecordJournal.Remark,
                                    AdditionalDescription = journalline.AdditionalDescription,
                                    ReferenceDoc = journalline.GLRecordJournal.ReferenceDoc,
                                    PaymentReference = journalline.GLRecordJournal.PaymentReference,
                                    InvoiceReferenceDoc = journalline.GLRecordJournal.InvoiceReferenceDoc,
                                    CreatedBy = user,
                                    DateCreated = DateTime.Now
                                };
                                if (period != null)
                                {
                                    newledgerpostline.GLPeriodId = period.GLPeriodId;
                                    newledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                                }

                                if (journalline.Debit > 0)
                                {
                                    #region debit balance

                                    newledgerpostline.Debit = journalline.Debit;
                                    newledgerpostline.Credit = 0;

                                    #endregion
                                }
                                else if (journalline.Credit > 0)
                                {
                                    #region credit balance

                                    newledgerpostline.Credit = journalline.Credit;
                                    newledgerpostline.Debit = 0;

                                    #endregion
                                }

                                context.GLLedgerPostingLines.Add(newledgerpostline);
                                context.SaveChanges();

                                #endregion

                                #region split ledger account

                                _accountHelper.SplitLedgerAccount(newledgerpost);

                                #endregion
                            }
                        }
                        #endregion
                        //new
                        #region transfer to bank recon
                        //check if this is bank account
                           //if (_accountHelper.IsBankAccount(journalline.AccountCode))
                           // {
                           //     var bookExist = context.ReconcilationBookAccounts.FirstOrDefault(x => x.SourceID == journalNumber);
                           //     if (bookExist == null)
                           //     {
                           //         var book = new ReconciliationBookAccount
                           //         {
                           //             CreatedBy = user,
                           //             DateCreated = DateTime.Now,
                           //             AccountCode = journalline.AccountCode,
                           //             Debit = journalline.Debit,
                           //             Credit = journalline.Credit,
                           //             Date = journalline.GLRecordJournal.EffectiveDate,
                           //             Type = "Posting",
                           //             ReferenceDoc = !String.IsNullOrWhiteSpace(gLRecordJournal.JournalNumber) ? gLRecordJournal.JournalNumber : "NA",
                           //             Reference = !String.IsNullOrWhiteSpace(journalline.AdditionalDescription) ? journalline.AdditionalDescription : "NA",
                           //             Source = gLRecordJournal.SourcesName,
                           //             SourceID = journalNumber,
                           //             AccountSegment = _accountHelper.GetNaturalAccountSegment(journalline.AccountCode),
                           //             SubAccountSegment = _accountHelper.GetSubAccountSegment(journalline.AccountCode)
                           //         };
                           //         context.ReconcilationBookAccounts.Add(book);
                           //         context.SaveChanges();
                           //     }
                           // }
                        #endregion
                       
                    }
                }

                #region update journal posting status

                gLRecordJournal.Posting = "POSTED";
                gLRecordJournal.Status = "POSTED";
                gLRecordJournal.isPosted = true;
                gLRecordJournal.IsUnPosted = false;
                gLRecordJournal.PostedBy = user;
                gLRecordJournal.PostedTime = DateTime.Now;
                db.Entry(gLRecordJournal).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region update ledger balances

                //update ledger
                foreach (var journalline in journallines)
                {
                    UpdateBegEndBalances(journalline.AccountCode, gLRecordJournal.PeriodName, user);
                }

                #endregion
            }

        }
        public string Unpost(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var journal = db.GLRecordJournals.Find(journalId);         
            if (journal != null)
            {
                #region validate if period is closed
                var glperiod = db.GLPeriods.Where(x => x.Name == journal.PeriodName).FirstOrDefault();
                if (glperiod == null)
                    return "Period not found";
                else 
                {
                    var year = db.GLFiscalYears.Find(glperiod.GlFiscalYearId);
                    if(year==null)
						return "Fiscal year not found";
					else
                    {
                        if(year.Status != FiscalYearStatus.Open || glperiod.Status != FiscalYearStatus.Open)
                            return "Either fiscal year or period is closed";
                    }
                }
                #endregion

                try
                {
                    var journallines = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journalId).AsNoTracking().ToList();

                    foreach (var journalline in journallines)
                    {
                        string sourceid = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId;
                        #region check and remove if exisit in ledger

                        var ledgerLine = db.GLLedgerPostingLines.FirstOrDefault(x => x.JournalNumber == sourceid);

                        if (ledgerLine != null)
                        {
                            db.GLLedgerPostingLines.Attach(ledgerLine);
                            db.GLLedgerPostingLines.Remove(ledgerLine);
                        }
                        #endregion

                        #region check and remove if book
                        //if (journal.SourcesName == "GL" || journal.Source == "Payroll")
                        //{
                            var bookline = db.ReconcilationBookAccounts.FirstOrDefault(x => x.SourceID == sourceid);

                            if (bookline != null)
                            {
                                db.ReconcilationBookAccounts.Attach(bookline);
                                db.ReconcilationBookAccounts.Remove(bookline);
                            }
                        //}
                        //else
                        //{
                        //    var booklines = db.ReconcilationBookAccounts
                        //        .Where(x=>x.Source==journal.Source)
                        //        .Where(x => x.SourceID.StartsWith(journal.SourceId+"-")).ToList();

                        //    if (booklines.Any())
                        //    {
                        //        db.ReconcilationBookAccounts.RemoveRange(booklines);
                        //    }
                        //}
                        #endregion
                    }

                    #region update journal posting status

                    journal.Posting = "UN_POSTED";
                    journal.Status = "UN_POSTED";
                    journal.isPosted = false;
                    journal.IsUnPosted = true;
                    journal.IsOnlineApproved = false;
                    journal.IsSendForApproval = false;
                    db.Entry(journal).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region update ledger balances

                    //update ledger
                    foreach (var journalline in journallines)
                    {
                        UpdateBegEndBalances(journalline.AccountCode, journal.PeriodName, user);
                    }

                    #endregion

                    return "";
                }
                catch (Exception ex)
                {
                    return "Error happend";
                }


            }

            return "Journal not found";

        }
        public  void CreateBegEndInitial()
        {

        }

        public  void UpdateBegEndBalances(string accountCode,string pName,string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            DateTime timeStamp = DateTime.Now;
            //var ledgerPostingLines = db.GLLedgerPostingLines.AsQueryable();
            var ledgerPostingIds = db.GLLedgerPostings.Where(x => x.AccountCode == accountCode).OrderBy(x => x.GLLedgerPostingID).Select(x => x.GLLedgerPostingID).ToList();
            //if (!String.IsNullOrEmpty(accountCode))
            //    ledgerPostings = ledgerPostings.Where(x => x.AccountCode == accountCode).ToList();

            var postPeriod = db.GLPeriods.FirstOrDefault(x => x.Name == pName);
            var yearPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == postPeriod.GlFiscalYearId).OrderBy(x => x.DateFrom).ToList();

            var beginningPeriodObj = yearPeriods.FirstOrDefault();

            if (beginningPeriodObj != null)
            {
                var periods = yearPeriods.Where(x => x.GLPeriodId != beginningPeriodObj.GLPeriodId).ToList();

                decimal begninning = 0;
                decimal ending = 0;
                decimal debit = 0;
                decimal credit = 0;
                decimal netIncrement = 0;
                decimal nextBegining = 0;

                foreach (var ledgerId in ledgerPostingIds)
                {
                    //db = new ExceedDbContext();
                    var posting = db.GLLedgerPostings.Find(ledgerId);
                    if (posting != null)
                    {
                        var selectedLines = db.GLLedgerPostingLines.Where(x =>
                            x.GLLedgerPosting.GLLedgerPostingID == posting.GLLedgerPostingID);

                        #region beginning period

                        var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriodObj.GLPeriodId)
                            .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
                        var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriodObj.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Standard);
                        var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriodObj.GLPeriodId)
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

                            begRecord.ModifiedBy = "UpdateBegEndBalances by " + user;
                            begRecord.LastModified = timeStamp;
                            db.Entry(begRecord).State = EntityState.Modified;

                        }
                        else
                        {
                            begninning = 0;

                            #region add beginning ledger line

                            var periodName = _fiscalYearHelper.GetPeriodName(beginningPeriodObj.GLPeriodId);
                            var newBeginning = new GLLedgerPostingLines
                            {
                                GLLedgerPosting = posting,
                                JournalNumber = "Beginning-" + periodName,
                                JournalReference = "Beginning-" + periodName,
                                EffectiveDate = beginningPeriodObj.DateFrom,
                                JournalName = "Beginning-" + periodName,
                                CategoryName = "Beginning-" + periodName,
                                SourcesName = "Beginning-" + periodName,
                                GLPeriodId = beginningPeriodObj.GLPeriodId,
                                GlFiscalYearId = beginningPeriodObj.GlFiscalYearId,
                                Type = LedgerPostTypes.Begining,
                                Balance = begninning,
                                CreatedBy = "UpdateBegEndBalances by " + user,
                                DateCreated = timeStamp
                            };
                            db.GLLedgerPostingLines.Add(newBeginning);
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
                            endRecord.ModifiedBy = "UpdateBegEndBalances by " + user;
                            endRecord.LastModified = timeStamp;
                            db.Entry(endRecord).State = EntityState.Modified;
                            db.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region add ending post ledger line

                            var periodName = _fiscalYearHelper.GetPeriodName(beginningPeriodObj.GLPeriodId);
                            var newLedgerEnding = new GLLedgerPostingLines
                            {
                                GLLedgerPosting = posting,
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
                                Balance = ending,
                                CreatedBy = "UpdateBegEndBalances by " + user,
                                DateCreated = timeStamp
                            };
                            db.GLLedgerPostingLines.Add(newLedgerEnding);
                            db.SaveChanges();

                            #endregion
                        }

                        #endregion

                        foreach (var p in periods)
                        {
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
                                begnningRecod.ModifiedBy = "UpdateBegEndBalances by " + user;
                                begnningRecod.LastModified = timeStamp;
                                db.Entry(begnningRecod).State = EntityState.Modified;
                                db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add beginning ledger line

                                var periodName = _fiscalYearHelper.GetPeriodName(period.GLPeriodId);
                                var newBeginning = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
                                    JournalNumber = "Beginning-" + periodName,
                                    JournalReference = "Beginning-" + periodName,
                                    EffectiveDate = period.DateFrom,
                                    JournalName = "Beginning-" + periodName,
                                    CategoryName = "Beginning-" + periodName,
                                    SourcesName = "Beginning-" + periodName,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Begining,
                                    Balance = begninning,
                                    CreatedBy = "UpdateBegEndBalances by " + user,
                                    DateCreated = timeStamp
                                };
                                db.GLLedgerPostingLines.Add(newBeginning);
                                db.SaveChanges();

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
                                endingRecord.ModifiedBy = "UpdateBegEndBalances by " + user;
                                endingRecord.LastModified = timeStamp;
                                db.Entry(endingRecord).State = EntityState.Modified;
                                db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add ending post ledger line

                                var periodName = _fiscalYearHelper.GetPeriodName(period.GLPeriodId);
                                var newEnding = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
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
                                    Balance = ending,
                                    CreatedBy = "UpdateBegEndBalances by " + user,
                                    DateCreated = timeStamp
                                };
                                db.GLLedgerPostingLines.Add(newEnding);
                                db.SaveChanges();

                                #endregion
                            }

                            #endregion

                        }

                        db.SaveChanges();
                    }

                }
                db.SaveChanges();
            }
        }
        public void UpdateStandardBalances(string accountCode, string pName)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal balance = 0;
            var ledger = db.GLLedgerPostings.FirstOrDefault(x => x.AccountCode == accountCode);
            var postPeriod = db.GLPeriods.FirstOrDefault(x => x.Name == pName);
            var yearPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == postPeriod.GlFiscalYearId).OrderBy(x => x.DateFrom).ToList();

            if (ledger != null)
            {
                foreach (var period in yearPeriods)
                {
                    var beginningRecord = db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Begining)
                        .Where(x => x.GLPeriodId == period.GLPeriodId)
                        .FirstOrDefault(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID);

                    var standardPostlines = db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Standard)
                        .Where(x => x.GLPeriodId == period.GLPeriodId)
                        .Where(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID)
                        .OrderBy(x => x.EffectiveDate).ToArray();

                    for (int i = 0; i < standardPostlines.Length; i++)
                    {
                        decimal begninningBalance=0;
                        decimal netIncrement = 0;
                        if (i == 0)
                        {
                            if(beginningRecord!=null)
                            begninningBalance=beginningRecord.Balance;
                        }
                        else
                        {
                            begninningBalance=standardPostlines[i - 1].Balance;
                        }

                        if (ledger.AccountType == GlAccountTypes.Asset ||
                            ledger.AccountType == GlAccountTypes.Expense)
                        {

                            netIncrement = standardPostlines[i].Debit - standardPostlines[i].Credit;
                            balance = begninningBalance + netIncrement;

                        }
                        else if (ledger.AccountType == GlAccountTypes.Liability ||
                                 ledger.AccountType == GlAccountTypes.Capital ||
                                 ledger.AccountType == GlAccountTypes.Revenue)
                        {

                            netIncrement = standardPostlines[i].Credit - standardPostlines[i].Debit;
                            balance = begninningBalance + netIncrement;
                        }

                        standardPostlines[i].Balance = balance;
                        db.Entry(standardPostlines[i]).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }

        }

        //public  void UpdateAllBegEndBalances()
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    var ledgerLines = db.GLLedgerPostingLines.AsQueryable();
        //    var ledger = db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).ToList();

        //    var status = db.GlobalStatus.FirstOrDefault(x => x.OperationType == "LedgerUpdate");

        //    if (status == null)
        //    {
        //        var newStatus = new GlobalStatus
        //        {
        //            OperationType = "LedgerUpdate",
        //            Status = "Running",
        //            Step = ""
        //        };
        //        db.GlobalStatus.Add(newStatus);
        //        db.SaveChanges();

        //        status = newStatus;

        //    }
        //    else
        //    {
        //        if (status.Status == "Finished")
        //        {
        //            status.Step = "0";
        //            status.Status = "Running";
        //            db.Entry(status).State = EntityState.Modified;
        //        }
        //        ledger = ledger.Where(x => x.GLLedgerPostingID > int.Parse(status.Step)).ToList();

        //    }




        //    var periods = db.GLPeriods.OrderBy(x => x.DateFrom).ToList();

        //    var beginningPeriod = db.GlBeginningPeriods.FirstOrDefault();
        //    if (beginningPeriod != null)
        //    {
        //        var beginningPeriodObj = db.GLPeriods.Find(beginningPeriod.GLPeriodId);
        //        if (beginningPeriodObj != null)
        //        {
        //            // periods after beginning and and already started
        //            periods = periods.Where(x => x.DateFrom > beginningPeriodObj.DateFrom)
        //                            .Where(x => x.DateFrom <= DateTime.Today).ToList();
        //            decimal begninning = 0;
        //            decimal ending = 0;
        //            decimal netIncrement = 0;
        //            decimal debit = 0;
        //            decimal credit = 0;
        //            decimal nextBegining = 0;

        //            foreach (var ledgerPosting in ledger)
        //            {
        //                var posting = ledgerPosting;
        //                var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.AccountCode == posting.AccountCode);

        //                #region beginning period

        //                var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
        //                var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Standard);
        //                var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

        //                if (begRecord != null)
        //                {

        //                    if (posting.AccountType == GlAccountTypes.Asset ||
        //                        posting.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        begninning = begRecord.Debit - begRecord.Credit;
        //                        begRecord.Balance = begninning;

        //                    }
        //                    else if (posting.AccountType == GlAccountTypes.Liability ||
        //                             posting.AccountType == GlAccountTypes.Capital ||
        //                             posting.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        begninning = begRecord.Credit - begRecord.Debit;
        //                        begRecord.Balance = begninning;
        //                    }

        //                    db.Entry(begRecord).State = EntityState.Modified;

        //                }
        //                else
        //                {
        //                    begninning = 0;
        //                    #region add beginning ledger line

        //                    var newBeginning = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        JournalReference = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        EffectiveDate = beginningPeriodObj.DateFrom,
        //                        JournalName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        CategoryName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        SourcesName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        GLPeriodId = beginningPeriod.GLPeriodId,
        //                        GlFiscalYearId = beginningPeriod.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Begining,
        //                        Balance = begninning
        //                    };
        //                    db.GLLedgerPostingLines.Add(newBeginning);
        //                    // _db.SaveChanges();

        //                    #endregion 
        //                }

        //                if (standardRecord.Any())
        //                {
        //                    debit = standardRecord.Sum(x => x.Debit);
        //                    credit = standardRecord.Sum(x => x.Credit);
        //                }


        //                if (posting.AccountType == GlAccountTypes.Asset ||
        //                    posting.AccountType == GlAccountTypes.Expense)
        //                {
        //                    netIncrement = debit - credit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;

        //                }
        //                else if (posting.AccountType == GlAccountTypes.Liability ||
        //                         posting.AccountType == GlAccountTypes.Capital ||
        //                         posting.AccountType == GlAccountTypes.Revenue)
        //                {
        //                    netIncrement = credit - debit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;
        //                }
        //                //update or create
        //                if (endRecord != null)
        //                {
        //                    #region update ending post ledger line

        //                    endRecord.Debit = debit;
        //                    endRecord.Credit = credit;
        //                    endRecord.Balance = ending;
        //                    db.Entry(endRecord).State = EntityState.Modified;
        //                    //_db.SaveChanges();

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region add ending post ledger line

        //                    var newLedgerEnding = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Ending-" + beginningPeriod.GLPeriodId,
        //                        JournalReference = "Ending-" + beginningPeriod.GLPeriodId,
        //                        EffectiveDate = beginningPeriodObj.DateTo,
        //                        JournalName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        CategoryName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        SourcesName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        GLPeriodId = beginningPeriodObj.GLPeriodId,
        //                        GlFiscalYearId = beginningPeriodObj.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Ending,
        //                        Debit = debit,
        //                        Credit = credit,
        //                        Balance = ending
        //                    };
        //                    db.GLLedgerPostingLines.Add(newLedgerEnding);
        //                    // _db.SaveChanges();

        //                    #endregion
        //                }

        //                #endregion

        //                foreach (var p in periods)
        //                {
        //                    netIncrement = 0;
        //                    debit = 0;
        //                    credit = 0;
        //                    var period = p;
        //                    #region next periods

        //                    var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);

        //                    var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .Where(x => x.Type == LedgerPostTypes.Standard);

        //                    var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);


        //                    begninning = nextBegining;

        //                    //update or create beginning
        //                    if (begnningRecod != null)
        //                    {
        //                        #region update beginning ledger line
        //                        begnningRecod.Balance = begninning;
        //                        db.Entry(begnningRecod).State = EntityState.Modified;
        //                        // _db.SaveChanges();

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region add beginning ledger line

        //                        var newBeginning = new GLLedgerPostingLines
        //                        {
        //                            GLLedgerPosting = posting,
        //                            JournalNumber = "Beginning-" + period.GLPeriodId,
        //                            JournalReference = "Beginning-" + period.GLPeriodId,
        //                            EffectiveDate = period.DateFrom,
        //                            JournalName = "Beginning-" + period.GLPeriodId,
        //                            CategoryName = "Beginning-" + period.GLPeriodId,
        //                            SourcesName = "Beginning-" + period.GLPeriodId,
        //                            GLPeriodId = period.GLPeriodId,
        //                            GlFiscalYearId = period.GlFiscalYearId,
        //                            Type = LedgerPostTypes.Begining,
        //                            Balance = begninning
        //                        };
        //                        db.GLLedgerPostingLines.Add(newBeginning);
        //                        // _db.SaveChanges();

        //                        #endregion
        //                    }
        //                    if (standardRecords.Any())
        //                    {
        //                        debit = standardRecords.Sum(x => x.Debit);
        //                        credit = standardRecords.Sum(x => x.Credit);
        //                    }
        //                    if (posting.AccountType == GlAccountTypes.Asset ||
        //                        posting.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        netIncrement = debit - credit;
        //                        ending = begninning + netIncrement;
        //                        nextBegining = begninning + netIncrement;

        //                    }
        //                    else if (posting.AccountType == GlAccountTypes.Liability ||
        //                             posting.AccountType == GlAccountTypes.Capital ||
        //                             posting.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        netIncrement = credit - debit;
        //                        ending = begninning + netIncrement;
        //                        nextBegining = begninning + netIncrement;
        //                    }
        //                    //update or create
        //                    if (endingRecord != null)
        //                    {
        //                        #region update ending post ledger line

        //                        endingRecord.Debit = debit;
        //                        endingRecord.Credit = credit;
        //                        endingRecord.Balance = ending;
        //                        db.Entry(endingRecord).State = EntityState.Modified;
        //                        //_db.SaveChanges();

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region add ending post ledger line

        //                        var newEnding = new GLLedgerPostingLines
        //                        {
        //                            GLLedgerPosting = posting,
        //                            JournalNumber = "Ending-" + period.GLPeriodId,
        //                            JournalReference = "Ending-" + period.GLPeriodId,
        //                            EffectiveDate = period.DateTo,
        //                            JournalName = "Ending-" + period.GLPeriodId,
        //                            CategoryName = "Ending-" + period.GLPeriodId,
        //                            SourcesName = "Ending-" + period.GLPeriodId,
        //                            GLPeriodId = period.GLPeriodId,
        //                            GlFiscalYearId = period.GlFiscalYearId,
        //                            Type = LedgerPostTypes.Ending,
        //                            Debit = debit,
        //                            Credit = credit,
        //                            Balance = ending
        //                        };
        //                        db.GLLedgerPostingLines.Add(newEnding);
        //                        //_db.SaveChanges();

        //                        #endregion
        //                    }

        //                    #endregion

        //                }

        //                status.Step = ledgerPosting.GLLedgerPostingID.ToString();
        //                db.SaveChanges();

        //            }
        //            status.Status = "Finished";
        //            db.Entry(status).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }
        //}
        //public void UpdateAllBegEndBalances1()
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    var ledgerLines = db.GLLedgerPostingLines.AsQueryable();
        //    var ledger = db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).ToList();

        //    var status = db.GlobalStatus.FirstOrDefault(x => x.OperationType == "LedgerUpdate");

        //    if (status == null)
        //    {
        //        var newStatus = new GlobalStatus
        //        {
        //            OperationType = "LedgerUpdate",
        //            Status = "Running",
        //            Step = ""
        //        };
        //        db.GlobalStatus.Add(newStatus);
        //        db.SaveChanges();

        //        status = newStatus;

        //    }
        //    else
        //    {
        //        if (status.Status == "Finished")
        //        {
        //            status.Step = "0";
        //            status.Status = "Running";
        //            db.Entry(status).State = EntityState.Modified;
        //        }
        //        ledger = ledger.Where(x => x.GLLedgerPostingID > int.Parse(status.Step)).ToList();

        //    }




        //    var periods = db.GLPeriods.OrderBy(x => x.DateFrom).ToList();

        //    var beginningPeriod = db.GlBeginningPeriods.FirstOrDefault();
        //    if (beginningPeriod != null)
        //    {
        //        var beginningPeriodObj = db.GLPeriods.Find(beginningPeriod.GLPeriodId);
        //        if (beginningPeriodObj != null)
        //        {
        //            // periods after beginning and and already started
        //            periods = periods.Where(x => x.DateFrom > beginningPeriodObj.DateFrom)
        //                            .Where(x => x.DateFrom <= DateTime.Today).ToList();
        //            decimal begninning = 0;
        //            decimal ending = 0;
        //            decimal netIncrement = 0;
        //            decimal debit = 0;
        //            decimal credit = 0;
        //            decimal nextBegining = 0;

        //            foreach (var ledgerPosting in ledger)
        //            {
        //                var posting = ledgerPosting;
        //                var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.AccountCode == posting.AccountCode);

        //                #region beginning period

        //                var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
        //                var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Standard);
        //                var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

        //                if (begRecord != null)
        //                {

        //                    if (posting.AccountType == GlAccountTypes.Asset ||
        //                        posting.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        begninning = begRecord.Debit - begRecord.Credit;
        //                        begRecord.Balance = begninning;

        //                    }
        //                    else if (posting.AccountType == GlAccountTypes.Liability ||
        //                             posting.AccountType == GlAccountTypes.Capital ||
        //                             posting.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        begninning = begRecord.Credit - begRecord.Debit;
        //                        begRecord.Balance = begninning;
        //                    }

        //                    db.Entry(begRecord).State = EntityState.Modified;

        //                }
        //                else
        //                {
        //                    begninning = 0;
        //                    #region add beginning ledger line

        //                    var newBeginning = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        JournalReference = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        EffectiveDate = beginningPeriodObj.DateFrom,
        //                        JournalName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        CategoryName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        SourcesName = "Beginning-" + beginningPeriod.GLPeriodId,
        //                        GLPeriodId = beginningPeriod.GLPeriodId,
        //                        GlFiscalYearId = beginningPeriod.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Begining,
        //                        Balance = begninning
        //                    };
        //                    db.GLLedgerPostingLines.Add(newBeginning);
        //                    // _db.SaveChanges();

        //                    #endregion 
        //                }

        //                if (standardRecord.Any())
        //                {
        //                    debit = standardRecord.Sum(x => x.Debit);
        //                    credit = standardRecord.Sum(x => x.Credit);
        //                }


        //                if (posting.AccountType == GlAccountTypes.Asset ||
        //                    posting.AccountType == GlAccountTypes.Expense)
        //                {
        //                    netIncrement = debit - credit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;

        //                }
        //                else if (posting.AccountType == GlAccountTypes.Liability ||
        //                         posting.AccountType == GlAccountTypes.Capital ||
        //                         posting.AccountType == GlAccountTypes.Revenue)
        //                {
        //                    netIncrement = credit - debit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;
        //                }
        //                //update or create
        //                if (endRecord != null)
        //                {
        //                    #region update ending post ledger line

        //                    endRecord.Debit = debit;
        //                    endRecord.Credit = credit;
        //                    endRecord.Balance = ending;
        //                    db.Entry(endRecord).State = EntityState.Modified;
        //                    //_db.SaveChanges();

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region add ending post ledger line

        //                    var newLedgerEnding = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Ending-" + beginningPeriod.GLPeriodId,
        //                        JournalReference = "Ending-" + beginningPeriod.GLPeriodId,
        //                        EffectiveDate = beginningPeriodObj.DateTo,
        //                        JournalName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        CategoryName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        SourcesName = "Ending-" + beginningPeriod.GLPeriodId,
        //                        GLPeriodId = beginningPeriodObj.GLPeriodId,
        //                        GlFiscalYearId = beginningPeriodObj.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Ending,
        //                        Debit = debit,
        //                        Credit = credit,
        //                        Balance = ending
        //                    };
        //                    db.GLLedgerPostingLines.Add(newLedgerEnding);
        //                    // _db.SaveChanges();

        //                    #endregion
        //                }

        //                #endregion

        //                foreach (var p in periods)
        //                {
        //                    netIncrement = 0;
        //                    debit = 0;
        //                    credit = 0;
        //                    var period = p;
        //                    #region next periods

        //                    var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);

        //                    var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .Where(x => x.Type == LedgerPostTypes.Standard);

        //                    var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                        .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);


        //                    begninning = nextBegining;

        //                    //update or create beginning
        //                    if (begnningRecod != null)
        //                    {
        //                        #region update beginning ledger line
        //                        begnningRecod.Balance = begninning;
        //                        db.Entry(begnningRecod).State = EntityState.Modified;
        //                        // _db.SaveChanges();

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region add beginning ledger line

        //                        var newBeginning = new GLLedgerPostingLines
        //                        {
        //                            GLLedgerPosting = posting,
        //                            JournalNumber = "Beginning-" + period.GLPeriodId,
        //                            JournalReference = "Beginning-" + period.GLPeriodId,
        //                            EffectiveDate = period.DateFrom,
        //                            JournalName = "Beginning-" + period.GLPeriodId,
        //                            CategoryName = "Beginning-" + period.GLPeriodId,
        //                            SourcesName = "Beginning-" + period.GLPeriodId,
        //                            GLPeriodId = period.GLPeriodId,
        //                            GlFiscalYearId = period.GlFiscalYearId,
        //                            Type = LedgerPostTypes.Begining,
        //                            Balance = begninning
        //                        };
        //                        db.GLLedgerPostingLines.Add(newBeginning);
        //                        // _db.SaveChanges();

        //                        #endregion
        //                    }
        //                    if (standardRecords.Any())
        //                    {
        //                        debit = standardRecords.Sum(x => x.Debit);
        //                        credit = standardRecords.Sum(x => x.Credit);
        //                    }
        //                    if (posting.AccountType == GlAccountTypes.Asset ||
        //                        posting.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        netIncrement = debit - credit;
        //                        ending = begninning + netIncrement;
        //                        nextBegining = begninning + netIncrement;

        //                    }
        //                    else if (posting.AccountType == GlAccountTypes.Liability ||
        //                             posting.AccountType == GlAccountTypes.Capital ||
        //                             posting.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        netIncrement = credit - debit;
        //                        ending = begninning + netIncrement;
        //                        nextBegining = begninning + netIncrement;
        //                    }
        //                    //update or create
        //                    if (endingRecord != null)
        //                    {
        //                        #region update ending post ledger line

        //                        endingRecord.Debit = debit;
        //                        endingRecord.Credit = credit;
        //                        endingRecord.Balance = ending;
        //                        db.Entry(endingRecord).State = EntityState.Modified;
        //                        //_db.SaveChanges();

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region add ending post ledger line

        //                        var newEnding = new GLLedgerPostingLines
        //                        {
        //                            GLLedgerPosting = posting,
        //                            JournalNumber = "Ending-" + period.GLPeriodId,
        //                            JournalReference = "Ending-" + period.GLPeriodId,
        //                            EffectiveDate = period.DateTo,
        //                            JournalName = "Ending-" + period.GLPeriodId,
        //                            CategoryName = "Ending-" + period.GLPeriodId,
        //                            SourcesName = "Ending-" + period.GLPeriodId,
        //                            GLPeriodId = period.GLPeriodId,
        //                            GlFiscalYearId = period.GlFiscalYearId,
        //                            Type = LedgerPostTypes.Ending,
        //                            Debit = debit,
        //                            Credit = credit,
        //                            Balance = ending
        //                        };
        //                        db.GLLedgerPostingLines.Add(newEnding);
        //                        //_db.SaveChanges();

        //                        #endregion
        //                    }

        //                    #endregion

        //                }

        //                status.Step = ledgerPosting.GLLedgerPostingID.ToString();
        //                db.SaveChanges();

        //            }
        //            status.Status = "Finished";
        //            db.Entry(status).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }
        //}

        public void UpdateAllBegEndBalancesFastP1(int id, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            string strcon = ConfigurationManager.ConnectionStrings["ExceedDbContext"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            DateTime timeStamp = DateTime.Now;
            var ledgerLines = db.GLLedgerPostingLines.AsNoTracking().AsQueryable();
            var ledgerPostingIds = db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).Select(x => x.GLLedgerPostingID).ToList();
            // var ledger = Db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).ToList();
            var beginningPeriod = db.GLPeriods.Find(id);
            if (beginningPeriod != null)
            {
                var nextPeriods = db.GLPeriods
                    .Where(x => x.DateFrom > beginningPeriod.DateFrom)
                    .Where(x => x.GlFiscalYearId == beginningPeriod.GlFiscalYearId)
                    .OrderBy(x => x.DateFrom)
                    .ToList();

                foreach (var ledgerId in ledgerPostingIds)
                {
                    //db = new ExceedDbContext();
                    decimal begninning = 0;
                    decimal ending = 0;
                    decimal netIncrement = 0;
                    decimal debit = 0;
                    decimal credit = 0;
                    decimal nextBegining = 0;

                    var posting = db.GLLedgerPostings.Find(ledgerId);
                    var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.GLLedgerPostingID == posting.GLLedgerPostingID);

                    if (posting != null)
                    {
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

                            // db.Entry(begRecord).State = EntityState.Modified;
                            #region UPDATE DATA
                            try
                            {
                                con.Open();
                                SqlCommand cmd = new SqlCommand("UPDATE [EXCEED].GLLedgerPostingLines SET Balance=Balance,ModifiedBy=@ModifiedBy,LastModified=@LastModified  " +
                                    "WHERE GLLedgerPostingLinesID= @Id", con);
                                cmd.Parameters.AddWithValue("@Id", begRecord.GLLedgerPostingLinesID);
                                cmd.Parameters.AddWithValue("@Balance", begRecord.Balance);
                                cmd.Parameters.AddWithValue("@ModifiedBy", "Fast (Raw SQL)" + user);
                                cmd.Parameters.AddWithValue("@LastModified", timeStamp);

                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                //log
                                throw ex;
                            }
                            finally
                            {
                                if (myReader != null)
                                {
                                    myReader.Close();
                                }
                                if (con != null)
                                {
                                    con.Close();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            begninning = 0;

                            #region add beginning ledger line

                            var newBeginning = new GLLedgerPostingLines
                            {
                                GLLedgerPosting = posting,
                                JournalNumber = "Beginning-" + beginningPeriod.Name,
                                JournalReference = "Beginning-" + beginningPeriod.Name,
                                EffectiveDate = beginningPeriod.DateFrom,
                                JournalName = "Beginning-" + beginningPeriod.Name,
                                CategoryName = "Beginning-" + beginningPeriod.Name,
                                SourcesName = "Beginning-" + beginningPeriod.Name,
                                GLPeriodId = beginningPeriod.GLPeriodId,
                                GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                Type = LedgerPostTypes.Begining,
                                Balance = begninning,
                                CreatedBy = "Fast" + user,
                                DateCreated = timeStamp
                            };
                            //db.GLLedgerPostingLines.Add(newBeginning);
                            //db.SaveChanges();
                            #region INSERT DATA
                            try
                            {
                                con.Open();
                                SqlCommand cmd = new SqlCommand("INSERT INTO [EXCEED].GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,JournalReference,EffectiveDate,CategoryName,JournalName,SourcesName,GlFiscalYearId" +
                                    ",GLPeriodId,Type,Debit,Credit,Balance,CreatedBy,DateCreated) VALUES (@ParentId,@JournalNumber,@JournalReference,@EffectiveDate,@CategoryName,@JournalName,@SourcesName,@GlFiscalYearId" +
                                    ",@GLPeriodId,@Type,@Debit,@Credit,@Balance,@CreatedBy,@DateCreated)", con);
                                cmd.Parameters.AddWithValue("@ParentId", posting.GLLedgerPostingID);
                                cmd.Parameters.AddWithValue("@JournalNumber", newBeginning.JournalNumber);
                                cmd.Parameters.AddWithValue("@JournalReference", newBeginning.JournalReference);
                                cmd.Parameters.AddWithValue("@EffectiveDate", newBeginning.EffectiveDate);
                                cmd.Parameters.AddWithValue("@CategoryName", newBeginning.CategoryName);
                                cmd.Parameters.AddWithValue("@JournalName", newBeginning.JournalName);
                                cmd.Parameters.AddWithValue("@SourcesName", newBeginning.SourcesName);
                                cmd.Parameters.AddWithValue("@GlFiscalYearId", newBeginning.GlFiscalYearId);
                                cmd.Parameters.AddWithValue("@GLPeriodId", newBeginning.GLPeriodId);
                                cmd.Parameters.AddWithValue("@Type", newBeginning.Type);
                                cmd.Parameters.AddWithValue("@Debit", newBeginning.Debit);
                                cmd.Parameters.AddWithValue("@Credit", newBeginning.Credit);
                                cmd.Parameters.AddWithValue("@Balance", newBeginning.Balance);
                                cmd.Parameters.AddWithValue("@CreatedBy", "Fast (Raw SQL)" + user);
                                cmd.Parameters.AddWithValue("@DateCreated", timeStamp);

                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                //log
                                throw ex;
                            }
                            finally
                            {
                                if (myReader != null)
                                {
                                    myReader.Close();
                                }
                                if (con != null)
                                {
                                    con.Close();
                                }
                            }
                            #endregion

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
                            endRecord.ModifiedBy = "Fast" + user;
                            endRecord.LastModified = timeStamp;
                            //db.Entry(endRecord).State = EntityState.Modified;
                            // db.SaveChanges();

                            #region UPDATE DATA
                            try
                            {
                                con.Open();
                                SqlCommand cmd = new SqlCommand("UPDATE [EXCEED].GLLedgerPostingLines SET Debit=@Debit ,Credit=@Credit,Balance=Balance,ModifiedBy=@ModifiedBy,LastModified=@LastModified  " +
                                    "WHERE GLLedgerPostingLinesID= @Id", con);
                                cmd.Parameters.AddWithValue("@Id", endRecord.GLLedgerPostingLinesID);
                                cmd.Parameters.AddWithValue("@Debit", endRecord.Debit);
                                cmd.Parameters.AddWithValue("@Credit", endRecord.Credit);
                                cmd.Parameters.AddWithValue("@Balance", endRecord.Balance);
                                cmd.Parameters.AddWithValue("@ModifiedBy", "Fast (Raw SQL)" + user);
                                cmd.Parameters.AddWithValue("@LastModified", timeStamp);

                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                //log
                                throw ex;
                            }
                            finally
                            {
                                if (myReader != null)
                                {
                                    myReader.Close();
                                }
                                if (con != null)
                                {
                                    con.Close();
                                }
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region add ending post ledger line

                            var newLedgerEnding = new GLLedgerPostingLines
                            {
                                GLLedgerPosting = posting,
                                JournalNumber = "Ending-" + beginningPeriod.Name,
                                JournalReference = "Ending-" + beginningPeriod.Name,
                                EffectiveDate = beginningPeriod.DateTo,
                                JournalName = "Ending-" + beginningPeriod.Name,
                                CategoryName = "Ending-" + beginningPeriod.Name,
                                SourcesName = "Ending-" + beginningPeriod.Name,
                                GLPeriodId = beginningPeriod.GLPeriodId,
                                GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                Type = LedgerPostTypes.Ending,
                                Debit = debit,
                                Credit = credit,
                                Balance = ending,
                                CreatedBy = "Fast" + user,
                                DateCreated = timeStamp
                            };
                            //db.GLLedgerPostingLines.Add(newLedgerEnding);
                            //db.SaveChanges();
                            #region INSERT DATA
                            try
                            {
                                con.Open();
                                SqlCommand cmd = new SqlCommand("INSERT INTO [EXCEED].GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,JournalReference,EffectiveDate,CategoryName,JournalName,SourcesName,GlFiscalYearId" +
                                    ",GLPeriodId,Type,Debit,Credit,Balance,CreatedBy,DateCreated) VALUES (@ParentId,@JournalNumber,@JournalReference,@EffectiveDate,@CategoryName,@JournalName,@SourcesName,@GlFiscalYearId" +
                                    ",@GLPeriodId,@Type,@Debit,@Credit,@Balance,@CreatedBy,@DateCreated)", con);
                                cmd.Parameters.AddWithValue("@ParentId", posting.GLLedgerPostingID);
                                cmd.Parameters.AddWithValue("@JournalNumber", newLedgerEnding.JournalNumber);
                                cmd.Parameters.AddWithValue("@JournalReference", newLedgerEnding.JournalReference);
                                cmd.Parameters.AddWithValue("@EffectiveDate", newLedgerEnding.EffectiveDate);
                                cmd.Parameters.AddWithValue("@CategoryName", newLedgerEnding.CategoryName);
                                cmd.Parameters.AddWithValue("@JournalName", newLedgerEnding.JournalName);
                                cmd.Parameters.AddWithValue("@SourcesName", newLedgerEnding.SourcesName);
                                cmd.Parameters.AddWithValue("@GlFiscalYearId", newLedgerEnding.GlFiscalYearId);
                                cmd.Parameters.AddWithValue("@GLPeriodId", newLedgerEnding.GLPeriodId);
                                cmd.Parameters.AddWithValue("@Type", newLedgerEnding.Type);
                                cmd.Parameters.AddWithValue("@Debit", newLedgerEnding.Debit);
                                cmd.Parameters.AddWithValue("@Credit", newLedgerEnding.Credit);
                                cmd.Parameters.AddWithValue("@Balance", newLedgerEnding.Balance);
                                cmd.Parameters.AddWithValue("@CreatedBy", "Fast (Raw SQL)" + user);
                                cmd.Parameters.AddWithValue("@DateCreated", timeStamp);

                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                //log
                                throw ex;
                            }
                            finally
                            {
                                if (myReader != null)
                                {
                                    myReader.Close();
                                }
                                if (con != null)
                                {
                                    con.Close();
                                }
                            }
                            #endregion

                            #endregion
                        }

                        #endregion

                        foreach (var p in nextPeriods)
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
                                begnningRecod.ModifiedBy = "Fast" + user;
                                begnningRecod.LastModified = timeStamp;
                                //db.Entry(begnningRecod).State = EntityState.Modified;
                                //db.SaveChanges();

                                #region UPDATE DATA
                                try
                                {
                                    con.Open();
                                    SqlCommand cmd = new SqlCommand("UPDATE [EXCEED].GLLedgerPostingLines SET Balance=Balance,ModifiedBy=@ModifiedBy,LastModified=@LastModified  " +
                                        "WHERE GLLedgerPostingLinesID= @Id", con);
                                    cmd.Parameters.AddWithValue("@Id", begnningRecod.GLLedgerPostingLinesID);
                                    cmd.Parameters.AddWithValue("@Balance", begnningRecod.Balance);
                                    cmd.Parameters.AddWithValue("@ModifiedBy", "Fast (Raw SQL)" + user);
                                    cmd.Parameters.AddWithValue("@LastModified", timeStamp);

                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //log
                                    throw ex;
                                }
                                finally
                                {
                                    if (myReader != null)
                                    {
                                        myReader.Close();
                                    }
                                    if (con != null)
                                    {
                                        con.Close();
                                    }
                                }
                                #endregion
                                #endregion
                            }
                            else
                            {
                                #region add beginning ledger line

                                var newBeginning = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
                                    JournalNumber = "Beginning-" + period.Name,
                                    JournalReference = "Beginning-" + period.Name,
                                    EffectiveDate = period.DateFrom,
                                    JournalName = "Beginning-" + period.Name,
                                    CategoryName = "Beginning-" + period.Name,
                                    SourcesName = "Beginning-" + period.Name,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Begining,
                                    Balance = begninning,
                                    CreatedBy = "Fast" + user,
                                    DateCreated = timeStamp,
                                };
                                //db.GLLedgerPostingLines.Add(newBeginning);
                                //db.SaveChanges();
                                #region INSERT DATA
                                try
                                {
                                    con.Open();
                                    SqlCommand cmd = new SqlCommand("INSERT INTO [EXCEED].GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,JournalReference,EffectiveDate,CategoryName,JournalName,SourcesName,GlFiscalYearId" +
                                        ",GLPeriodId,Type,Debit,Credit,Balance,CreatedBy,DateCreated) VALUES (@ParentId,@JournalNumber,@JournalReference,@EffectiveDate,@CategoryName,@JournalName,@SourcesName,@GlFiscalYearId" +
                                        ",@GLPeriodId,@Type,@Debit,@Credit,@Balance,@CreatedBy,@DateCreated)", con);
                                    cmd.Parameters.AddWithValue("@ParentId", posting.GLLedgerPostingID);
                                    cmd.Parameters.AddWithValue("@JournalNumber", newBeginning.JournalNumber);
                                    cmd.Parameters.AddWithValue("@JournalReference", newBeginning.JournalReference);
                                    cmd.Parameters.AddWithValue("@EffectiveDate", newBeginning.EffectiveDate);
                                    cmd.Parameters.AddWithValue("@CategoryName", newBeginning.CategoryName);
                                    cmd.Parameters.AddWithValue("@JournalName", newBeginning.JournalName);
                                    cmd.Parameters.AddWithValue("@SourcesName", newBeginning.SourcesName);
                                    cmd.Parameters.AddWithValue("@GlFiscalYearId", newBeginning.GlFiscalYearId);
                                    cmd.Parameters.AddWithValue("@GLPeriodId", newBeginning.GLPeriodId);
                                    cmd.Parameters.AddWithValue("@Type", newBeginning.Type);
                                    cmd.Parameters.AddWithValue("@Debit", newBeginning.Debit);
                                    cmd.Parameters.AddWithValue("@Credit", newBeginning.Credit);
                                    cmd.Parameters.AddWithValue("@Balance", newBeginning.Balance);
                                    cmd.Parameters.AddWithValue("@CreatedBy", "Fast (Raw SQL)" + user);
                                    cmd.Parameters.AddWithValue("@DateCreated", timeStamp);

                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //log
                                    throw ex;
                                }
                                finally
                                {
                                    if (myReader != null)
                                    {
                                        myReader.Close();
                                    }
                                    if (con != null)
                                    {
                                        con.Close();
                                    }
                                }
                                #endregion

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
                                endingRecord.ModifiedBy = "Fast" + user;
                                endingRecord.LastModified = timeStamp;
                                //db.Entry(endingRecord).State = EntityState.Modified;
                                //db.SaveChanges();
                                #region UPDATE DATA
                                try
                                {
                                    con.Open();
                                    SqlCommand cmd = new SqlCommand("UPDATE [EXCEED].GLLedgerPostingLines SET Debit=@Debit ,Credit=@Credit,Balance=Balance,ModifiedBy=@ModifiedBy,LastModified=@LastModified  " +
                                        "WHERE GLLedgerPostingLinesID= @Id", con);
                                    cmd.Parameters.AddWithValue("@Id", endingRecord.GLLedgerPostingLinesID);
                                    cmd.Parameters.AddWithValue("@Debit", endingRecord.Debit);
                                    cmd.Parameters.AddWithValue("@Credit", endingRecord.Credit);
                                    cmd.Parameters.AddWithValue("@Balance", endingRecord.Balance);
                                    cmd.Parameters.AddWithValue("@ModifiedBy", "Fast (Raw SQL)" + user);
                                    cmd.Parameters.AddWithValue("@LastModified", timeStamp);

                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //log
                                    throw ex;
                                }
                                finally
                                {
                                    if (myReader != null)
                                    {
                                        myReader.Close();
                                    }
                                    if (con != null)
                                    {
                                        con.Close();
                                    }
                                }
                                #endregion
                                #endregion
                            }
                            else
                            {
                                #region add ending post ledger line

                                var newEnding = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
                                    JournalNumber = "Ending-" + period.Name,
                                    JournalReference = "Ending-" + period.Name,
                                    EffectiveDate = period.DateTo,
                                    JournalName = "Ending-" + period.Name,
                                    CategoryName = "Ending-" + period.Name,
                                    SourcesName = "Ending-" + period.Name,
                                    GLPeriodId = period.GLPeriodId,
                                    GlFiscalYearId = period.GlFiscalYearId,
                                    Type = LedgerPostTypes.Ending,
                                    Debit = debit,
                                    Credit = credit,
                                    Balance = ending,
                                    CreatedBy = "Fast" + user,
                                    DateCreated = timeStamp
                                };
                                //db.GLLedgerPostingLines.Add(newEnding);
                                //db.SaveChanges();
                                #region INSERT DATA
                                try
                                {
                                    con.Open();
                                    SqlCommand cmd = new SqlCommand("INSERT INTO [EXCEED].GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,JournalReference,EffectiveDate,CategoryName,JournalName,SourcesName,GlFiscalYearId" +
                                        ",GLPeriodId,Type,Debit,Credit,Balance,CreatedBy,DateCreated) VALUES (@ParentId,@JournalNumber,@JournalReference,@EffectiveDate,@CategoryName,@JournalName,@SourcesName,@GlFiscalYearId" +
                                        ",@GLPeriodId,@Type,@Debit,@Credit,@Balance,@CreatedBy,@DateCreated)", con);
                                    cmd.Parameters.AddWithValue("@ParentId", posting.GLLedgerPostingID);
                                    cmd.Parameters.AddWithValue("@JournalNumber", newEnding.JournalNumber);
                                    cmd.Parameters.AddWithValue("@JournalReference", newEnding.JournalReference);
                                    cmd.Parameters.AddWithValue("@EffectiveDate", newEnding.EffectiveDate);
                                    cmd.Parameters.AddWithValue("@CategoryName", newEnding.CategoryName);
                                    cmd.Parameters.AddWithValue("@JournalName", newEnding.JournalName);
                                    cmd.Parameters.AddWithValue("@SourcesName", newEnding.SourcesName);
                                    cmd.Parameters.AddWithValue("@GlFiscalYearId", newEnding.GlFiscalYearId);
                                    cmd.Parameters.AddWithValue("@GLPeriodId", newEnding.GLPeriodId);
                                    cmd.Parameters.AddWithValue("@Type", newEnding.Type);
                                    cmd.Parameters.AddWithValue("@Debit", newEnding.Debit);
                                    cmd.Parameters.AddWithValue("@Credit", newEnding.Credit);
                                    cmd.Parameters.AddWithValue("@Balance", newEnding.Balance);
                                    cmd.Parameters.AddWithValue("@CreatedBy", "Fast (Raw SQL)" + user);
                                    cmd.Parameters.AddWithValue("@DateCreated", timeStamp);

                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //log
                                    throw ex;
                                }
                                finally
                                {
                                    if (myReader != null)
                                    {
                                        myReader.Close();
                                    }
                                    if (con != null)
                                    {
                                        con.Close();
                                    }
                                }
                                #endregion
                                #endregion
                            }

                            #endregion

                        }

                    }

                }


            }
        }
        public void UpdateAllBegEndBalancesFastP(int id,string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            DateTime timeStamp = DateTime.Now;
            var ledgerLines = db.GLLedgerPostingLines.AsNoTracking().AsQueryable();
            var ledgerPostingIds = db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).Select(x=>x.GLLedgerPostingID).ToList();
           // var ledger = Db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).ToList();
            var beginningPeriod = db.GLPeriods.Find(id);
            if (beginningPeriod != null)
            {
                   var nextPeriods = db.GLPeriods
                       .Where(x => x.DateFrom > beginningPeriod.DateFrom)
                       .Where(x=>x.GlFiscalYearId==beginningPeriod.GlFiscalYearId)
                       .OrderBy(x=>x.DateFrom)
                       .ToList();

                    foreach (var ledgerId in ledgerPostingIds)
                    {
                        db = new ExceedDbContext();
                        decimal begninning = 0;
                        decimal ending = 0;
                        decimal netIncrement = 0;
                        decimal debit = 0;
                        decimal credit = 0;
                        decimal nextBegining = 0;

                        var posting = db.GLLedgerPostings.Find(ledgerId);
                        var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.GLLedgerPostingID == posting.GLLedgerPostingID);

                        if (posting != null)
                        {
                            #region beginning period

                            var begRecord = selectedLines
                                .Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Begining).FirstOrDefault();
                            var standardRecord = selectedLines
                                .Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard);
                            var endRecord = selectedLines.
                                Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
                               .Where(x => x.Type == LedgerPostTypes.Ending).FirstOrDefault();

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

                                db.Entry(begRecord).State = EntityState.Modified;

                            }
                            else
                            {
                                begninning = 0;

                                #region add beginning ledger line

                                var newBeginning = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
                                    JournalNumber = "Beginning-" + beginningPeriod.Name,
                                    JournalReference = "Beginning-" + beginningPeriod.Name,
                                    EffectiveDate = beginningPeriod.DateFrom,
                                    JournalName = "Beginning-" + beginningPeriod.Name,
                                    CategoryName = "Beginning-" + beginningPeriod.Name,
                                    SourcesName = "Beginning-" + beginningPeriod.Name,
                                    GLPeriodId = beginningPeriod.GLPeriodId,
                                    GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                    Type = LedgerPostTypes.Begining,
                                    Balance = begninning,
                                    CreatedBy = "Fast"+ user,
                                    DateCreated=timeStamp
                                };
                                db.GLLedgerPostingLines.Add(newBeginning);
                                db.SaveChanges();

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
                                endRecord.ModifiedBy = "Fast"+ user;
                                endRecord.LastModified = timeStamp;
                                db.Entry(endRecord).State = EntityState.Modified;
                                db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                #region add ending post ledger line

                                var newLedgerEnding = new GLLedgerPostingLines
                                {
                                    GLLedgerPosting = posting,
                                    JournalNumber = "Ending-" + beginningPeriod.Name,
                                    JournalReference = "Ending-" + beginningPeriod.Name,
                                    EffectiveDate = beginningPeriod.DateTo,
                                    JournalName = "Ending-" + beginningPeriod.Name,
                                    CategoryName = "Ending-" + beginningPeriod.Name,
                                    SourcesName = "Ending-" + beginningPeriod.Name,
                                    GLPeriodId = beginningPeriod.GLPeriodId,
                                    GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                                    Type = LedgerPostTypes.Ending,
                                    Debit = debit,
                                    Credit = credit,
                                    Balance = ending,
                                    CreatedBy = "Fast"+ user,
                                    DateCreated=timeStamp
                                };
                                db.GLLedgerPostingLines.Add(newLedgerEnding);
                                db.SaveChanges();

                                #endregion
                            }

                            #endregion

                            foreach (var p in nextPeriods)
                            {
                                netIncrement = 0;
                                debit = 0;
                                credit = 0;
                                var period = p;

                                #region next periods

                                var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                    .Where(x => x.Type == LedgerPostTypes.Begining).FirstOrDefault();

                                var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                    .Where(x => x.Type == LedgerPostTypes.Standard);

                                var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
                                    .Where(x => x.Type == LedgerPostTypes.Ending).FirstOrDefault();


                                begninning = nextBegining;

                                //update or create beginning
                                if (begnningRecod != null)
                                {
                                    #region update beginning ledger line

                                    begnningRecod.Balance = begninning;
                                    begnningRecod.ModifiedBy = "Fast"+ user;
                                    begnningRecod.LastModified = timeStamp;
                                    db.Entry(begnningRecod).State = EntityState.Modified;
                                    db.SaveChanges();

                                    #endregion
                                }
                                else
                                {
                                    #region add beginning ledger line

                                    var newBeginning = new GLLedgerPostingLines
                                    {
                                        GLLedgerPosting = posting,
                                        JournalNumber = "Beginning-" + period.Name,
                                        JournalReference = "Beginning-" + period.Name,
                                        EffectiveDate = period.DateFrom,
                                        JournalName = "Beginning-" + period.Name,
                                        CategoryName = "Beginning-" + period.Name,
                                        SourcesName = "Beginning-" + period.Name,
                                        GLPeriodId = period.GLPeriodId,
                                        GlFiscalYearId = period.GlFiscalYearId,
                                        Type = LedgerPostTypes.Begining,
                                        Balance = begninning,
                                        CreatedBy = "Fast"+ user,
                                        DateCreated=timeStamp,
                                    };
                                    db.GLLedgerPostingLines.Add(newBeginning);
                                    db.SaveChanges();

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
                                    endingRecord.ModifiedBy = "Fast" + user;
                                    endingRecord.LastModified = timeStamp;
                                    db.Entry(endingRecord).State = EntityState.Modified;
                                    db.SaveChanges();

                                    #endregion
                                }
                                else
                                {
                                    #region add ending post ledger line

                                    var newEnding = new GLLedgerPostingLines
                                    {
                                        GLLedgerPosting = posting,
                                        JournalNumber = "Ending-" + period.Name,
                                        JournalReference = "Ending-" + period.Name,
                                        EffectiveDate = period.DateTo,
                                        JournalName = "Ending-" + period.Name,
                                        CategoryName = "Ending-" + period.Name,
                                        SourcesName = "Ending-" + period.Name,
                                        GLPeriodId = period.GLPeriodId,
                                        GlFiscalYearId = period.GlFiscalYearId,
                                        Type = LedgerPostTypes.Ending,
                                        Debit = debit,
                                        Credit = credit,
                                        Balance = ending,
                                        CreatedBy = "Fast"+ user,
                                        DateCreated=timeStamp
                                    };
                                    db.GLLedgerPostingLines.Add(newEnding);
                                    db.SaveChanges();

                                    #endregion
                                }

                                #endregion

                            }

                        }

                    }
                    
                
            }
        }
        public void UpdateAllStandardBalancesFastP(int id,string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            DateTime timestamp = DateTime.Now;
            decimal balance = 0;
            int startMonth = 0;
            int startLedger = 0;

            #region update status

            var status = db.GlobalStatus.FirstOrDefault(x => x.OperationType == "StandardUpdateDaily");

            if (status == null)
            {
                var newStatus = new GlobalStatus
                {
                    OperationType = "StandardUpdateDaily",
                    Status = "Running",
                    Step = "0-0"
                };
                db.GlobalStatus.Add(newStatus);
                db.SaveChanges();

                status = newStatus;

            }
            else
            {
                if (status.Status == "Finished")
                {
                    status.Step = "0-0";
                    status.Status = "Running";
                    db.Entry(status).State = EntityState.Modified;
                }
                else
                {
                    startMonth = int.Parse(status.Step.Split('-')[0]);
                    startLedger = int.Parse(status.Step.Split('-')[1]);
                }

            }

            #endregion

            var givenPeriod = db.GLPeriods
                .Select(x => new {x.DateFrom, x.DateTo, x.GLPeriodId,x.GlFiscalYearId})//new 
                .FirstOrDefault(x => x.GLPeriodId == id);

            var beginningPeriod = givenPeriod;
            if (startMonth != 0)
            {
                var startingPeriod= db.GLPeriods
                    .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId, x.GlFiscalYearId })//new
                        .FirstOrDefault(x => x.GLPeriodId == startMonth);
                if (givenPeriod?.DateTo.Date > startingPeriod?.DateTo.Date)
                {
                    beginningPeriod = startingPeriod;
                }
            }

            var yearPeriods = db.GLPeriods
                .Where(x => x.DateFrom >= beginningPeriod.DateFrom)
                .Where(x => x.GlFiscalYearId == givenPeriod.GlFiscalYearId)//to filter current year only
                .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId, x.GlFiscalYearId })//new
                .OrderBy(x => x.DateFrom)
                .ToList();

            foreach (var period in yearPeriods)
            {
                var ledgerIds = db.GLLedgerPostingLines
                    .Where(x => x.GLPeriodId == period.GLPeriodId)
                    .Where(x=>x.GLLedgerPostingID>=startLedger)//to continue where it stop
                    .Select(x => x.GLLedgerPostingID).Distinct().OrderBy(n=>n).ToList();

                foreach (var ledgerId in ledgerIds)
                {
                    var ledger = db.GLLedgerPostings.Find(ledgerId);

                    //for test
                    //var ledger = db.GLLedgerPostings.Where(x=>x.AccountCode=="11104-00-000-00-CBE01").FirstOrDefault();
                    if (ledger != null)
                    {
                        var beginningRecord = db.GLLedgerPostingLines
                            .Where(x => x.Type == LedgerPostTypes.Begining)
                            .Where(x => x.GLPeriodId == period.GLPeriodId)
                            .Select(x => new { x.Debit, x.Credit,x.Balance, x.GLPeriodId, x.GLLedgerPosting })//new
                            .FirstOrDefault(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID);

                        var standardPostlines = db.GLLedgerPostingLines
                            .Where(x => x.Type == LedgerPostTypes.Standard)
                            .Where(x => x.GLPeriodId == period.GLPeriodId)
                            .Where(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID)
                            //.Select(x => new { x.Debit, x.Credit, x.Balance, x.GLPeriodId, x.GLLedgerPosting,x.EffectiveDate,x.ModifiedBy,x.LastModified })//new
                            .OrderBy(x=>x.EffectiveDate).ThenBy(x=>x.GLLedgerPostingLinesID).ToArray();

                        for (int i = 0; i < standardPostlines.Length; i++)
                        {
                            decimal begninningBalance = 0;
                            decimal netIncrement = 0;
                            if (i == 0)
                            {
                                if (beginningRecord != null)
                                    begninningBalance = beginningRecord.Balance;
                            }
                            else
                            {
                                begninningBalance = standardPostlines[i - 1].Balance;
                            }

                            if (ledger.AccountType == GlAccountTypes.Asset ||
                                ledger.AccountType == GlAccountTypes.Expense)
                            {

                                netIncrement = standardPostlines[i].Debit - standardPostlines[i].Credit;
                                balance = begninningBalance + netIncrement;

                            }
                            else if (ledger.AccountType == GlAccountTypes.Liability ||
                                     ledger.AccountType == GlAccountTypes.Capital ||
                                     ledger.AccountType == GlAccountTypes.Revenue)
                            {

                                netIncrement = standardPostlines[i].Credit - standardPostlines[i].Debit;
                                balance = begninningBalance + netIncrement;
                            }

                            standardPostlines[i].Balance = balance;
                            standardPostlines[i].ModifiedBy = "Standard update-"+user;
                            standardPostlines[i].LastModified = timestamp;
                            db.Entry(standardPostlines[i]).State = EntityState.Modified;
                            //db.SaveChanges();

                        }

                        db.SaveChanges();
                    }

                    #region update status progress

                    status.Step = period.GLPeriodId+"-"+ledgerId;
                    db.SaveChanges();

                    #endregion
                }
               
            }

            #region finish status update

            status.Status = "Finished";
            db.Entry(status).State = EntityState.Modified;
            db.SaveChanges();

            #endregion

        }
        public void UpdateAllStandardBalancesMonthlyFastP(int id, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            DateTime timestamp = DateTime.Now;
            decimal balance = 0;
            int startMonth = 0;
            int startLedger = 0;

            #region update status

            var status = db.GlobalStatus.FirstOrDefault(x => x.OperationType == "StandardUpdatePerMonthDaily");

            if (status == null)
            {
                var newStatus = new GlobalStatus
                {
                    OperationType = "StandardUpdatePerMonthDaily",
                    Status = "Running",
                    Step = "0-0"
                };
                db.GlobalStatus.Add(newStatus);
                db.SaveChanges();

                status = newStatus;

            }
            else
            {
                if (status.Status == "Finished")
                {
                    status.Step = $"{id}-0";
                    status.Status = "Running";
                    db.Entry(status).State = EntityState.Modified;
                }
                else
                {
                    startMonth = int.Parse(status.Step.Split('-')[0]);
                    startLedger = int.Parse(status.Step.Split('-')[1]);
                }

            }

            #endregion

       
            var ledgerIds = db.GLLedgerPostingLines
                .Where(x => x.GLPeriodId == id)
                .Where(x => x.GLLedgerPostingID >= startLedger)//to continue where it stop
                .Select(x => x.GLLedgerPostingID).Distinct().OrderBy(n => n).ToList();

            foreach (var ledgerId in ledgerIds)
            {
                var ledger = db.GLLedgerPostings.Find(ledgerId);

                //for test
                //var ledger = db.GLLedgerPostings.Where(x=>x.AccountCode=="11104-00-000-00-CBE01").FirstOrDefault();
                if (ledger != null)
                {
                    var beginningRecord = db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Begining)
                        .Where(x => x.GLPeriodId == id)
                        .Select(x => new { x.Debit, x.Credit, x.Balance, x.GLPeriodId, x.GLLedgerPosting })//new
                        .FirstOrDefault(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID);

                    var standardPostlines = db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Standard)
                        .Where(x => x.GLPeriodId ==id)
                        .Where(x => x.GLLedgerPosting.GLLedgerPostingID == ledger.GLLedgerPostingID)
                        //.Select(x => new { x.Debit, x.Credit, x.Balance, x.GLPeriodId, x.GLLedgerPosting,x.EffectiveDate,x.ModifiedBy,x.LastModified })//new
                        .OrderBy(x => x.EffectiveDate).ThenBy(x => x.GLLedgerPostingLinesID).ToArray();

                    for (int i = 0; i < standardPostlines.Length; i++)
                    {
                        decimal begninningBalance = 0;
                        decimal netIncrement = 0;
                        if (i == 0)
                        {
                            if (beginningRecord != null)
                                begninningBalance = beginningRecord.Balance;
                        }
                        else
                        {
                            begninningBalance = standardPostlines[i - 1].Balance;
                        }

                        if (ledger.AccountType == GlAccountTypes.Asset ||
                            ledger.AccountType == GlAccountTypes.Expense)
                        {

                            netIncrement = standardPostlines[i].Debit - standardPostlines[i].Credit;
                            balance = begninningBalance + netIncrement;

                        }
                        else if (ledger.AccountType == GlAccountTypes.Liability ||
                                    ledger.AccountType == GlAccountTypes.Capital ||
                                    ledger.AccountType == GlAccountTypes.Revenue)
                        {

                            netIncrement = standardPostlines[i].Credit - standardPostlines[i].Debit;
                            balance = begninningBalance + netIncrement;
                        }

                        standardPostlines[i].Balance = balance;
                        standardPostlines[i].ModifiedBy = "Standard update per month-" + user;
                        standardPostlines[i].LastModified = timestamp;
                        db.Entry(standardPostlines[i]).State = EntityState.Modified;
                        //db.SaveChanges();

                    }

                    db.SaveChanges();
                }

                #region update status progress

                status.Step = id + "-" + ledgerId;
                db.SaveChanges();

                #endregion
            }

            

            #region finish status update

            status.Status = "Finished";
            db.Entry(status).State = EntityState.Modified;
            db.SaveChanges();

            #endregion

        }
        //public void UpdateAllBegEndBalancesFast()
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    var ledgerLines = db.GLLedgerPostingLines.AsQueryable();
        //    var ledger = db.GLLedgerPostings.OrderBy(x => x.GLLedgerPostingID).ToList();

        //    var status = db.GlobalStatus.FirstOrDefault(x => x.OperationType == "LedgerUpdateFast");

        //    if (status == null)
        //    {
        //        var newStatus = new GlobalStatus
        //        {
        //            OperationType = "LedgerUpdateFast",
        //            Status = "Running",
        //            Step = ""
        //        };
        //        db.GlobalStatus.Add(newStatus);
        //        db.SaveChanges();

        //        status = newStatus;

        //    }
        //    else
        //    {
        //        if (status.Status == "Finished")
        //        {
        //            status.Step = "0";
        //            status.Status = "Running";
        //            db.Entry(status).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        ledger = ledger.Where(x => x.GLLedgerPostingID > int.Parse(status.Step)).ToList();

        //    }



        //    var currentYear = _fiscalYearHelper.GetFiscalYearOfDate(DateTime.Today);
        //    var yearPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == currentYear).OrderBy(x => x.DateFrom).ToList();
        //    GLPeriod beginningPeriod = null;
        //    foreach (var period in yearPeriods)
        //    {
        //        var noBegning = ledgerLines
        //            .Where(x => x.GLPeriodId == period.GLPeriodId)
        //            .Where(x => x.Type != LedgerPostTypes.Begining)
        //            .FirstOrDefault(x => x.Type != LedgerPostTypes.Standard);

        //        if (noBegning != null)
        //        {
        //            var previousP = yearPeriods.Where(x => x.DateFrom < period.DateFrom)
        //                .OrderByDescending(x => x.DateFrom).FirstOrDefault();
        //            beginningPeriod = previousP ?? period;
        //            break;
        //        }
        //    }


        //    if (beginningPeriod != null)
        //    {
        //        var periods = yearPeriods.Where(x => x.DateFrom >= beginningPeriod.DateFrom).ToList();
        //        decimal begninning = 0;
        //        decimal ending = 0;
        //        decimal netIncrement = 0;
        //        decimal debit = 0;
        //        decimal credit = 0;
        //        decimal nextBegining = 0;

        //        foreach (var ledgerPosting in ledger)
        //        {
        //            var posting = ledgerPosting;
        //            var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.AccountCode == posting.AccountCode);

        //            #region beginning period

        //            var begRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);
        //            var standardRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                .Where(x => x.Type == LedgerPostTypes.Standard);
        //            var endRecord = selectedLines.Where(x => x.GLPeriodId == beginningPeriod.GLPeriodId)
        //                .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

        //            if (begRecord != null)
        //            {

        //                if (posting.AccountType == GlAccountTypes.Asset ||
        //                    posting.AccountType == GlAccountTypes.Expense)
        //                {
        //                    begninning = begRecord.Debit - begRecord.Credit;
        //                    begRecord.Balance = begninning;

        //                }
        //                else if (posting.AccountType == GlAccountTypes.Liability ||
        //                         posting.AccountType == GlAccountTypes.Capital ||
        //                         posting.AccountType == GlAccountTypes.Revenue)
        //                {
        //                    begninning = begRecord.Credit - begRecord.Debit;
        //                    begRecord.Balance = begninning;
        //                }

        //                db.Entry(begRecord).State = EntityState.Modified;

        //            }
        //            else
        //            {
        //                begninning = 0;
        //                #region add beginning ledger line

        //                var newBeginning = new GLLedgerPostingLines
        //                {
        //                    GLLedgerPosting = posting,
        //                    JournalNumber = "Beginning-" + beginningPeriod.Name,
        //                    JournalReference = "Beginning-" + beginningPeriod.Name,
        //                    EffectiveDate = beginningPeriod.DateFrom,
        //                    JournalName = "Beginning-" + beginningPeriod.Name,
        //                    CategoryName = "Beginning-" + beginningPeriod.Name,
        //                    SourcesName = "Beginning-" + beginningPeriod.Name,
        //                    GLPeriodId = beginningPeriod.GLPeriodId,
        //                    GlFiscalYearId = beginningPeriod.GlFiscalYearId,
        //                    Type = LedgerPostTypes.Begining,
        //                    Balance = begninning,
        //                    CreatedBy = "Fast"
        //                };
        //                db.GLLedgerPostingLines.Add(newBeginning);
        //                db.SaveChanges();

        //                #endregion
        //            }

        //            if (standardRecord.Any())
        //            {
        //                debit = standardRecord.Sum(x => x.Debit);
        //                credit = standardRecord.Sum(x => x.Credit);
        //            }


        //            if (posting.AccountType == GlAccountTypes.Asset ||
        //                posting.AccountType == GlAccountTypes.Expense)
        //            {
        //                netIncrement = debit - credit;
        //                ending = begninning + netIncrement;
        //                nextBegining = begninning + netIncrement;

        //            }
        //            else if (posting.AccountType == GlAccountTypes.Liability ||
        //                     posting.AccountType == GlAccountTypes.Capital ||
        //                     posting.AccountType == GlAccountTypes.Revenue)
        //            {
        //                netIncrement = credit - debit;
        //                ending = begninning + netIncrement;
        //                nextBegining = begninning + netIncrement;
        //            }
        //            //update or create
        //            if (endRecord != null)
        //            {
        //                #region update ending post ledger line

        //                endRecord.Debit = debit;
        //                endRecord.Credit = credit;
        //                endRecord.Balance = ending;
        //                endRecord.ModifiedBy = "Fast";
        //                db.Entry(endRecord).State = EntityState.Modified;
        //                db.SaveChanges();

        //                #endregion
        //            }
        //            else
        //            {
        //                #region add ending post ledger line

        //                var newLedgerEnding = new GLLedgerPostingLines
        //                {
        //                    GLLedgerPosting = posting,
        //                    JournalNumber = "Ending-" + beginningPeriod.Name,
        //                    JournalReference = "Ending-" + beginningPeriod.Name,
        //                    EffectiveDate = beginningPeriod.DateTo,
        //                    JournalName = "Ending-" + beginningPeriod.Name,
        //                    CategoryName = "Ending-" + beginningPeriod.Name,
        //                    SourcesName = "Ending-" + beginningPeriod.Name,
        //                    GLPeriodId = beginningPeriod.GLPeriodId,
        //                    GlFiscalYearId = beginningPeriod.GlFiscalYearId,
        //                    Type = LedgerPostTypes.Ending,
        //                    Debit = debit,
        //                    Credit = credit,
        //                    Balance = ending,
        //                    CreatedBy = "Fast"
        //                };
        //                db.GLLedgerPostingLines.Add(newLedgerEnding);
        //                db.SaveChanges();

        //                #endregion
        //            }

        //            #endregion

        //            foreach (var p in periods)
        //            {
        //                netIncrement = 0;
        //                debit = 0;
        //                credit = 0;
        //                var period = p;
        //                #region next periods

        //                var begnningRecod = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);

        //                var standardRecords = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .Where(x => x.Type == LedgerPostTypes.Standard);

        //                var endingRecord = selectedLines.Where(x => x.GLPeriodId == period.GLPeriodId)
        //                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);


        //                begninning = nextBegining;

        //                //update or create beginning
        //                if (begnningRecod != null)
        //                {
        //                    #region update beginning ledger line
        //                    begnningRecod.Balance = begninning;
        //                    begnningRecod.ModifiedBy = "Fast";
        //                    db.Entry(begnningRecod).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region add beginning ledger line

        //                    var newBeginning = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Beginning-" + period.Name,
        //                        JournalReference = "Beginning-" + period.Name,
        //                        EffectiveDate = period.DateFrom,
        //                        JournalName = "Beginning-" + period.Name,
        //                        CategoryName = "Beginning-" + period.Name,
        //                        SourcesName = "Beginning-" + period.Name,
        //                        GLPeriodId = period.GLPeriodId,
        //                        GlFiscalYearId = period.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Begining,
        //                        Balance = begninning,
        //                        CreatedBy = "Fast"
        //                    };
        //                    db.GLLedgerPostingLines.Add(newBeginning);
        //                    db.SaveChanges();

        //                    #endregion
        //                }
        //                if (standardRecords.Any())
        //                {
        //                    debit = standardRecords.Sum(x => x.Debit);
        //                    credit = standardRecords.Sum(x => x.Credit);
        //                }
        //                if (posting.AccountType == GlAccountTypes.Asset ||
        //                    posting.AccountType == GlAccountTypes.Expense)
        //                {
        //                    netIncrement = debit - credit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;

        //                }
        //                else if (posting.AccountType == GlAccountTypes.Liability ||
        //                         posting.AccountType == GlAccountTypes.Capital ||
        //                         posting.AccountType == GlAccountTypes.Revenue)
        //                {
        //                    netIncrement = credit - debit;
        //                    ending = begninning + netIncrement;
        //                    nextBegining = begninning + netIncrement;
        //                }
        //                //update or create
        //                if (endingRecord != null)
        //                {
        //                    #region update ending post ledger line

        //                    endingRecord.Debit = debit;
        //                    endingRecord.Credit = credit;
        //                    endingRecord.Balance = ending;
        //                    db.Entry(endingRecord).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region add ending post ledger line

        //                    var newEnding = new GLLedgerPostingLines
        //                    {
        //                        GLLedgerPosting = posting,
        //                        JournalNumber = "Ending-" + period.Name,
        //                        JournalReference = "Ending-" + period.Name,
        //                        EffectiveDate = period.DateTo,
        //                        JournalName = "Ending-" + period.Name,
        //                        CategoryName = "Ending-" + period.Name,
        //                        SourcesName = "Ending-" + period.Name,
        //                        GLPeriodId = period.GLPeriodId,
        //                        GlFiscalYearId = period.GlFiscalYearId,
        //                        Type = LedgerPostTypes.Ending,
        //                        Debit = debit,
        //                        Credit = credit,
        //                        Balance = ending,
        //                        CreatedBy = "Fast"
        //                    };
        //                    db.GLLedgerPostingLines.Add(newEnding);
        //                    db.SaveChanges();

        //                    #endregion
        //                }

        //                #endregion

        //            }

        //            status.Step = ledgerPosting.GLLedgerPostingID.ToString();
        //            //Db.SaveChanges();

        //        }
        //        status.Status = "Finished";
        //        db.Entry(status).State = EntityState.Modified;
        //        db.SaveChanges();

        //    }
        //}

        public  void TransferGlRecordsToBank1()
        {
            ExceedDbContext db = new ExceedDbContext();
            #region journal


            List<string> bankSubAccounts = _accountHelper.GetBankSubAccounts();

            var journalDetails = db.GLRecordJournalDetails
                    .Where(x=>x.GLRecordJournal.IsOnlineApproved)
                    .Where(x => x.GLRecordJournal.isVoid==false)
                   // .Where(x => bankSubAccounts.Contains(x.SubAccountSegment))
                    .ToList();

            foreach (var detail in journalDetails)
            {
                if (_accountHelper.IsBankAccount(detail.AccountCode))
                {
                    var journal = db.GLRecordJournals.Find(detail.GlRecordJournalId);

                    if (journal != null && journal.SourcesName == "GL" && journal.IsOnlineApproved && journal.isVoid == false)
                    {
                        string sourceid = journal.GlRecordJournalId + "-" + detail.GlRecordJournalDetailId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == detail.AccountCode)
                            .Where(x => x.Source == JournalSources.GeneralLedger.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = detail.AccountCode,
                                Debit = detail.Debit,
                                Credit = detail.Credit,
                                Date = journal.EffectiveDate,
                                Type = "GL Record",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(journal.JournalNumber) ? journal.JournalNumber : "NA",
                                Reference = !String.IsNullOrWhiteSpace(detail.AdditionalDescription) ? detail.AdditionalDescription : "NA",
                                Source = JournalSources.GeneralLedger.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
            #region petty cash
            var pettyCashDistributions = db.ApPettyCashDistributions
                    .Where(x => x.PettyCash.IsOnlineApproved)
                    .Where(x => x.PettyCash.IsVoid == false).ToList();

            foreach (var distribution in pettyCashDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var pettyCash = db.ApPettyCash.Find(distribution.ApPettyCashId);

                    if (pettyCash != null && pettyCash.IsOnlineApproved && pettyCash.IsVoid == false)
                    {
                        string sourceid = pettyCash.ApPettyCashId + "-" + distribution.ApPettyCashDistributionId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AP_PettyCash.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = pettyCash.Date,
                                Type = "Petty Cash",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(pettyCash.Reference) ? pettyCash.Reference : "NA",
                                Reference = !String.IsNullOrWhiteSpace(distribution.AdditionalDescription) ? distribution.AdditionalDescription : "NA",
                                Source = JournalSources.AP_PettyCash.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
            #region misc payment
            var cashDistributions = db.APMiscellaneousPaymentDistributions
                    .Where(x => x.ApMiscellaneousPayment.IsOnlineApproved)
                    .Where(x => x.ApMiscellaneousPayment.IsVoid == false).ToList();

            foreach (var distribution in cashDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var apMiscellaneousPayment = db.APMiscellaneousPayments.Find(distribution.ApMiscellaneousPaymentId);

                    if (apMiscellaneousPayment != null && apMiscellaneousPayment.IsOnlineApproved && apMiscellaneousPayment.IsVoid == false)
                    {
                        string sourceid = apMiscellaneousPayment.ApMiscellaneousPaymentId + "-" + distribution.ApMiscellaneousPaymentDistributionId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AP_MiscPayment.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = apMiscellaneousPayment.PaymentDate,
                                Type = "Misc Payment",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(apMiscellaneousPayment.Remark) ? apMiscellaneousPayment.Remark : "NA",
                                Reference = !String.IsNullOrWhiteSpace(apMiscellaneousPayment.PaymentReference) ? apMiscellaneousPayment.PaymentReference : "NA",
                                Source = JournalSources.AP_MiscPayment.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
            #region payment
            var apPaymentDistributions = db.APPaymentDistributions
                    .Where(x => x.ApPayment.IsOnlineApproved)
                    .Where(x => x.ApPayment.IsVoid == false).ToList();

            foreach (var distribution in apPaymentDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var payment = db.APPayments.Find(distribution.ApPaymentId);

                    if (payment != null && payment.IsOnlineApproved && payment.IsVoid == false)
                    {
                        string sourceid = payment.ApPaymentId + "-" + distribution.ApPaymentDistributionId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AP_Payment.ToString() || x.Source == JournalSources.AP_PrePayment.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = payment.PaymentDate,
                                Type = "Payment",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(payment.Remark) ? payment.Remark : "NA",
                                Reference = !String.IsNullOrWhiteSpace(payment.PaymentReference) ? payment.PaymentReference : "NA",
                                Source = payment.IsPrepayment ? JournalSources.AP_PrePayment.ToString() : JournalSources.AP_Payment.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
            #region direct payment
            var directDistributions = db.ApDirectPaymentDistributions
                    .Where(x => x.ApDirectPayment.IsOnlineApproved)
                    .Where(x => x.ApDirectPayment.IsVoid == false).ToList();

            foreach (var distribution in directDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var payment = db.APPayments.Find(distribution.ApDirectPaymentId);

                    if (payment != null && payment.IsOnlineApproved && payment.IsVoid == false)
                    {
                        string sourceid = payment.ApPaymentId + "-" + distribution.ApDirectPaymentDistributionId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AP_Direct_Payment.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = payment.PaymentDate,
                                Type = "Direct Payment",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(payment.Remark) ? payment.Remark : "NA",
                                Reference = !String.IsNullOrWhiteSpace(payment.PaymentReference) ? payment.PaymentReference : "NA",
                                Source = JournalSources.AP_Direct_Payment.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion     
       
            #region misc collection
            var arMiscelaneousCollectionDistributions = db.ARMiscelaneousCollectionDistributions
                    .Where(x => x.ARMiscelaneousCollection.IsOnlineApproved)
                    .Where(x => x.ARMiscelaneousCollection.IsVoid == false).ToList();

            foreach (var distribution in arMiscelaneousCollectionDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var miscelaneousCollection = db.ARMiscelaneousCollections.Find(distribution.ARMiscelaneousCollectionID);

                    if (miscelaneousCollection != null && miscelaneousCollection.IsOnlineApproved && miscelaneousCollection.IsVoid == false)
                    {
                        string sourceid = miscelaneousCollection.ARMiscelaneousCollectionID + "-" + distribution.ARMiscelaneousCollectionDistributionID;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AR_MiscCollection.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = miscelaneousCollection.ReceiptDate,
                                Type = "Misc Collection",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(miscelaneousCollection.ReceiptNumber) ? miscelaneousCollection.ReceiptNumber : "NA",
                                Reference = !String.IsNullOrWhiteSpace(miscelaneousCollection.Remark) ? miscelaneousCollection.Remark : "NA",
                                Source = JournalSources.AR_MiscCollection.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
            #region collection
            var arStandardCollectionDistributions = db.ARStandardCollectionDistributions
                    .Where(x => x.ArStandardCollection.IsOnlineApproved)
                    .Where(x => x.ArStandardCollection.IsVoid == false).ToList();

            foreach (var distribution in arStandardCollectionDistributions)
            {
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var standardCollection = db.ARStandardCollections.Find(distribution.ArStandardCollectionId);

                    if (standardCollection != null && standardCollection.IsOnlineApproved && standardCollection.IsVoid == false)
                    {
                        string sourceid = standardCollection.ArStandardCollectionId + "-" + distribution.ArStandardCollectionDistributionId;
                        #region pass data for bank reconciliation

                        //check if it exists
                        var exist = db.ReconcilationBookAccounts
                            .Where(x => x.SourceID == sourceid)
                            .Where(x => x.AccountCode == distribution.AccountCode)
                            .Where(x => x.Source == JournalSources.AR_Collection.ToString()).ToList();

                        if (!exist.Any())
                        {
                            var book = new ReconciliationBookAccount
                            {
                                CreatedBy = "System",
                                DateCreated = DateTime.Now,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Date = standardCollection.ReceiptDate,
                                Type = "Collection",
                                ReferenceDoc = !String.IsNullOrWhiteSpace(standardCollection.ReceiptNumber) ? standardCollection.ReceiptNumber : "NA",
                                Reference = !String.IsNullOrWhiteSpace(standardCollection.Reference) ? standardCollection.Reference : "NA",
                                Source = JournalSources.AR_Collection.ToString(),
                                SourceID = sourceid
                            };
                            db.ReconcilationBookAccounts.Add(book);
                            db.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            #endregion
        }
        //new method, Dapper
        private class GLLedgerPostingLine
        {
            public int GLLedgerPostingLinesID { get; set; }
            public int GLLedgerPostingID { get; set; }
            public string AccountCode { get; set; }
            public string AccountDesc { get; set; }
            public GlAccountTypes AccountType { get; set; }
            public LedgerPostTypes Type { get; set; }
            public int GLPeriodId { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal Balance { get; set; }
        }
        public void TransferFiscalYearEndingFast(int sourcePeriod, int destPeriod, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            db.Database.CommandTimeout = 7200;//added
            DateTime timeStamp = DateTime.Now;

            var beginningPeriod = db.GLPeriods.Find(destPeriod);

            List<GLLedgerPostingLine> postings = new List<GLLedgerPostingLine>();
            #region transfer permanent accounts

            string strcon = ConfigurationManager.ConnectionStrings["ExceedDbContext"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select GLLedgerPostingLinesID,[EXCEED].GLLedgerPosting.GLLedgerPostingID,AccountType,AccountCode,Type,GLPeriodId,Debit,Credit,Balance from [EXCEED].GLLedgerPosting INNER JOIN [EXCEED].GLLedgerPostingLines ON [EXCEED].GLLedgerPosting.GLLedgerPostingID=[EXCEED].GLLedgerPostingLines.GLLedgerPostingID " +
                    "where (GLPeriodId=@sourcePeriod OR GLPeriodId=@destPeriod) AND (Type=2 OR Type=1)", con);
                cmd.Parameters.AddWithValue("@destPeriod", destPeriod);
                cmd.Parameters.AddWithValue("@sourcePeriod", sourcePeriod);
                cmd.CommandTimeout = 7200;//added
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    var record = new GLLedgerPostingLine
                    {
                        GLLedgerPostingLinesID = int.Parse(myReader[0].ToString()),
                        GLLedgerPostingID = int.Parse(myReader[1].ToString()),
                        AccountType = (GlAccountTypes)int.Parse(myReader[2].ToString()),
                        AccountCode = myReader[3].ToString(),
                        Type = (LedgerPostTypes)int.Parse(myReader[4].ToString()),
                        GLPeriodId = int.Parse(myReader[5].ToString()),
                        Debit = decimal.Parse(myReader[6].ToString()),
                        Credit = decimal.Parse(myReader[7].ToString()),
                        Balance = decimal.Parse(myReader[8].ToString()),

                    };
                    postings.Add(record);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }


            var distinctLedgerIds = postings.DistinctByField(p => p.GLLedgerPostingID).ToList();

            foreach (var posting in distinctLedgerIds)
            {

                #region beginning period

                var endRecord = postings
                    .Where(x => x.GLLedgerPostingID == posting.GLLedgerPostingID)
                    .Where(x => x.GLPeriodId == sourcePeriod)
                   .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

                var begRecord = postings
                     .Where(x => x.GLLedgerPostingID == posting.GLLedgerPostingID)
                    .Where(x => x.GLPeriodId == destPeriod)
                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);


                if (endRecord != null)
                {
                    if (begRecord != null)
                    {
                        #region balance type
                        if (posting.AccountType == GlAccountTypes.Revenue ||
                               posting.AccountType == GlAccountTypes.Expense)
                        {
                            begRecord.Debit = 0;
                            begRecord.Credit = 0;
                            begRecord.Balance = 0;
                        }
                        if (posting.AccountType == GlAccountTypes.Asset)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                begRecord.Debit = endRecord.Balance;
                                begRecord.Credit = 0;
                                begRecord.Balance = endRecord.Balance;
                            }
                            else
                            {
                                begRecord.Debit = 0;
                                begRecord.Credit = Math.Abs(endRecord.Balance);
                                begRecord.Balance = endRecord.Balance;
                            }

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                begRecord.Debit = 0;
                                begRecord.Credit = endRecord.Balance;
                                begRecord.Balance = endRecord.Balance;

                            }
                            else
                            {
                                begRecord.Debit = Math.Abs(endRecord.Balance);
                                begRecord.Credit = 0;
                                begRecord.Balance = endRecord.Balance;
                            }
                        }
                        #endregion
                        #region UPDATE DATA
                        try
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand("UPDATE [EXCEED].GLLedgerPostingLines SET Debit=@Debit ,Credit=@Credit,Balance=@Balance,ModifiedBy=@ModifiedBy,LastModified=@LastModified  " +
                                "WHERE GLLedgerPostingLinesID= @Id", con);
                            cmd.Parameters.AddWithValue("@Id", begRecord.GLLedgerPostingLinesID);
                            cmd.Parameters.AddWithValue("@Debit", begRecord.Debit);
                            cmd.Parameters.AddWithValue("@Credit", begRecord.Credit);
                            cmd.Parameters.AddWithValue("@Balance", begRecord.Balance);
                            cmd.Parameters.AddWithValue("@ModifiedBy", "Transfer Ending (Raw SQL)" + user);
                            cmd.Parameters.AddWithValue("@LastModified", timeStamp);

                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //log
                            throw ex;
                        }
                        finally
                        {
                            if (myReader != null)
                            {
                                myReader.Close();
                            }
                            if (con != null)
                            {
                                con.Close();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region add beginning ledger line


                        var newBeginning = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = posting.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };


                        #region balance type

                        if (posting.AccountType == GlAccountTypes.Revenue ||
                              posting.AccountType == GlAccountTypes.Expense)
                        {
                            newBeginning.Debit = 0;
                            newBeginning.Credit = 0;
                            newBeginning.Balance = 0;
                        }

                        if (posting.AccountType == GlAccountTypes.Asset)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                newBeginning.Debit = endRecord.Balance;
                                newBeginning.Credit = 0;
                                newBeginning.Balance = endRecord.Balance;
                            }
                            else
                            {
                                newBeginning.Debit = 0;
                                newBeginning.Credit = Math.Abs(endRecord.Balance);
                                newBeginning.Balance = endRecord.Balance;
                            }

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                newBeginning.Debit = 0;
                                newBeginning.Credit = endRecord.Balance;
                                newBeginning.Balance = endRecord.Balance;


                            }
                            else
                            {
                                newBeginning.Debit = Math.Abs(endRecord.Balance);
                                newBeginning.Credit = 0;
                                newBeginning.Balance = endRecord.Balance;
                            }
                        }
                        #endregion
                        #region INSERT DATA
                        try
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand("INSERT INTO [EXCEED].GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,JournalReference,EffectiveDate,CategoryName,JournalName,SourcesName,GlFiscalYearId" +
                                ",GLPeriodId,Type,Debit,Credit,Balance,CreatedBy,DateCreated) VALUES (@ParentId,@JournalNumber,@JournalReference,@EffectiveDate,@CategoryName,@JournalName,@SourcesName,@GlFiscalYearId" +
                                ",@GLPeriodId,@Type,@Debit,@Credit,@Balance,@CreatedBy,@DateCreated)", con);
                            cmd.Parameters.AddWithValue("@ParentId", newBeginning.GLLedgerPostingID);
                            cmd.Parameters.AddWithValue("@JournalNumber", newBeginning.JournalNumber);
                            cmd.Parameters.AddWithValue("@JournalReference", newBeginning.JournalReference);
                            cmd.Parameters.AddWithValue("@EffectiveDate", newBeginning.EffectiveDate);
                            cmd.Parameters.AddWithValue("@CategoryName", newBeginning.CategoryName);
                            cmd.Parameters.AddWithValue("@JournalName", newBeginning.JournalName);
                            cmd.Parameters.AddWithValue("@SourcesName", newBeginning.SourcesName);
                            cmd.Parameters.AddWithValue("@GlFiscalYearId", newBeginning.GlFiscalYearId);
                            cmd.Parameters.AddWithValue("@GLPeriodId", newBeginning.GLPeriodId);
                            cmd.Parameters.AddWithValue("@Type", newBeginning.Type);
                            cmd.Parameters.AddWithValue("@Debit", newBeginning.Debit);
                            cmd.Parameters.AddWithValue("@Credit", newBeginning.Credit);
                            cmd.Parameters.AddWithValue("@Balance", newBeginning.Balance);
                            cmd.Parameters.AddWithValue("@CreatedBy", "Transfer Ending (Raw SQL)" + user);
                            cmd.Parameters.AddWithValue("@DateCreated", timeStamp);

                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //log
                            throw ex;
                        }
                        finally
                        {
                            if (myReader != null)
                            {
                                myReader.Close();
                            }
                            if (con != null)
                            {
                                con.Close();
                            }
                        }
                        #endregion

                        #endregion
                    }


                }

                #endregion

            }

            #endregion
            #region transfer retained earning and profit tax

            decimal earningAmount = 0;
            decimal profittax = 0;
            decimal earningAfterTaxAmount = 0;

            var profittaxRate = db.FinanceSettings.FirstOrDefault();
            var retainedAccount = db.GlRetainedEarningAccounts.Where(x => x.Type == EarningTypes.RetainedEarning).FirstOrDefault();
            var profitTaxAccount = db.GlRetainedEarningAccounts.Where(x => x.Type == EarningTypes.ProfitTax).FirstOrDefault();

            var expense = db.GLLedgerPostingLines.Where(x => x.GLPeriodId == sourcePeriod)
                      .Where(x => x.Type == LedgerPostTypes.Ending)
                      .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

            var revenue = db.GLLedgerPostingLines.Where(x => x.GLPeriodId == sourcePeriod)
                .Where(x => x.Type == LedgerPostTypes.Ending)
                .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

            earningAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);


            decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;
            profittax = earningAmount * rate;
            earningAfterTaxAmount = earningAmount - profittax;
            if (earningAfterTaxAmount != 0)
            {
                #region update earning
                var retainedEarningEnd = db.GLLedgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == retainedAccount.AccountCode.ToUpper())
                                        .Where(x => x.GLPeriodId == sourcePeriod)
                                        .Where(x => x.Type == LedgerPostTypes.Ending)
                                        .FirstOrDefault();
                var retainedEarningBeg = db.GLLedgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == retainedAccount.AccountCode.ToUpper())
                                        .Where(x => x.GLPeriodId == destPeriod)
                                        .Where(x => x.Type == LedgerPostTypes.Begining)
                                        .FirstOrDefault();


                if (retainedEarningBeg != null)
                {
                    decimal balance = earningAfterTaxAmount;
                    if (retainedEarningEnd != null)
                        balance = retainedEarningEnd.Balance + earningAfterTaxAmount;

                    if (balance > 0)
                        retainedEarningBeg.Credit = balance;
                    if (balance < 0)
                        retainedEarningBeg.Debit = Math.Abs(balance);

                    retainedEarningBeg.Balance = balance;

                    db.GLLedgerPostingLines.Attach(retainedEarningBeg);
                    db.Entry(retainedEarningBeg).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                        x.AccountCode.ToUpper() == retainedAccount.AccountCode.ToUpper());
                    if (ledger == null)
                    {
                        #region create ledger

                        var ledgerPosting = new GLLedgerPosting
                        {
                            AccountCode = retainedAccount.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        ledgerPosting.AccountDesc = accountHelper.GetAccountDesc(ledgerPosting.AccountCode);
                        ledgerPosting.AccountType = accountHelper.GetAccountType(ledgerPosting.AccountCode);
                        ledgerPosting.CreatedBy = "System earning-" + user;
                        ledgerPosting.DateCreated = DateTime.Now;
                        db.GLLedgerPostings.Add(ledgerPosting);
                        db.SaveChanges();
                        #endregion
                        #region add beginning ledger line for earning


                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledgerPosting,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = 0,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        decimal balance = earningAfterTaxAmount;
                        if (retainedEarningEnd != null)
                            balance = retainedEarningEnd.Balance + earningAfterTaxAmount;

                        if (balance > 0)
                            newEarningLine.Credit = balance;
                        if (balance < 0)
                            newEarningLine.Debit = Math.Abs(balance);

                        newEarningLine.Balance = balance;

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region add beginning ledger line for earning


                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = ledger.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = earningAfterTaxAmount,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        decimal balance = earningAfterTaxAmount;
                        if (retainedEarningEnd != null)
                            balance = retainedEarningEnd.Balance + earningAfterTaxAmount;

                        if (balance > 0)
                            newEarningLine.Credit = balance;
                        if (balance < 0)
                            newEarningLine.Debit = Math.Abs(balance);

                        newEarningLine.Balance = balance;

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                }
                #endregion
            }

            if (profittax != 0)
            {
                #region update profit tax

                var profitTaxEnd = db.GLLedgerPostingLines
                                      .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == profitTaxAccount.AccountCode.ToUpper())
                                      .Where(x => x.GLPeriodId == sourcePeriod)
                                      .Where(x => x.Type == LedgerPostTypes.Ending)
                                      .FirstOrDefault();

                var profitTaxBeg = db.GLLedgerPostingLines
                                       .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == profitTaxAccount.AccountCode.ToUpper())
                                       .Where(x => x.GLPeriodId == destPeriod)
                                       .Where(x => x.Type == LedgerPostTypes.Begining)
                                       .FirstOrDefault();

                if (profitTaxBeg != null)
                {
                    decimal balance = profittax;
                    if (profitTaxEnd != null)
                        balance = profitTaxEnd.Balance + profittax;

                    if (balance > 0)
                        profitTaxBeg.Credit = balance;
                    if (balance < 0)
                        profitTaxBeg.Debit = Math.Abs(balance);

                    profitTaxBeg.Balance = balance;

                    db.GLLedgerPostingLines.Attach(profitTaxBeg);
                    db.Entry(profitTaxBeg).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                        x.AccountCode.ToUpper() == profitTaxAccount.AccountCode.ToUpper());
                    if (ledger == null)
                    {
                        #region create ledger

                        var ledgerPosting = new GLLedgerPosting
                        {
                            AccountCode = profitTaxAccount.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        ledgerPosting.AccountDesc = accountHelper.GetAccountDesc(ledgerPosting.AccountCode);
                        ledgerPosting.AccountType = accountHelper.GetAccountType(ledgerPosting.AccountCode);
                        ledgerPosting.CreatedBy = "System-" + user;
                        ledgerPosting.DateCreated = DateTime.Now;
                        db.GLLedgerPostings.Add(ledgerPosting);
                        db.SaveChanges();
                        #endregion
                        #region add beginning ledger line for profit tax


                        var newProfitTaxLine = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledgerPosting,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = 0,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        decimal balance = profittax;
                        if (profitTaxEnd != null)
                            balance = profitTaxEnd.Balance + profittax;

                        if (balance > 0)
                            newProfitTaxLine.Credit = balance;
                        if (balance < 0)
                            newProfitTaxLine.Debit = Math.Abs(balance);

                        newProfitTaxLine.Balance = balance;

                        db.GLLedgerPostingLines.Add(newProfitTaxLine);
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region add beginning ledger line for profit tax


                        var newProfitTaxLine = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = ledger.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = profittax,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        decimal balance = profittax;
                        if (profitTaxEnd != null)
                            balance = profitTaxEnd.Balance + profittax;

                        if (balance > 0)
                            newProfitTaxLine.Credit = balance;
                        if (balance < 0)
                            newProfitTaxLine.Debit = Math.Abs(balance);

                        newProfitTaxLine.Balance = balance;

                        db.GLLedgerPostingLines.Add(newProfitTaxLine);
                        db.SaveChanges();

                        #endregion
                    }
                }
                #endregion
            }

            #endregion           

        }
        //original method, EF
        public void TransferFiscalYearEnding(int sourcePeriod, int destPeriod, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            DateTime timeStamp = DateTime.Now;
            #region transfer permanent accounts
            //var ledgerLines = db.GLLedgerPostingLines
            //    .Where(x => x.GLPeriodId == sourcePeriod || x.GLPeriodId ==destPeriod)
            //    .Where(x => x.Type == LedgerPostTypes.Begining ||x.Type == LedgerPostTypes.Ending)
            //    .AsNoTracking().AsQueryable();

            var postings = db.GLLedgerPostings
                .Include(x => x.GLLedgerPostingLines)
                .Where(x => x.AccountType != GlAccountTypes.Expense)
                .Where(x => x.AccountType != GlAccountTypes.Revenue)
                .Where(x => x.AccountCode == "1-100-110-21408-00000")//test
                .OrderBy(x => x.GLLedgerPostingID)
               .Select(x => new
               {
                   x.GLLedgerPostingID,
                   x.AccountType,
                   x.AccountCode,
                   GLLedgerPostingLines = x.GLLedgerPostingLines
                                                .Where(a => a.GLPeriodId == sourcePeriod || a.GLPeriodId == destPeriod)
                                                .Where(a => a.Type == LedgerPostTypes.Begining || a.Type == LedgerPostTypes.Ending)
               })
                .ToList();
            foreach (var posting in postings)
            {
                //db = new ExceedDbContext();                           

                //var posting =ledger;
                //var selectedLines = ledgerLines.Where(x => x.GLLedgerPosting.GLLedgerPostingID == posting.GLLedgerPostingID);

                #region beginning period

                var endRecord = posting.GLLedgerPostingLines.Where(x => x.GLPeriodId == sourcePeriod)
                   .FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);

                var begRecord = posting.GLLedgerPostingLines.Where(x => x.GLPeriodId == destPeriod)
                    .FirstOrDefault(x => x.Type == LedgerPostTypes.Begining);


                if (endRecord != null)
                {
                    if (begRecord != null)
                    {
                        #region balance type
                        if (posting.AccountType == GlAccountTypes.Revenue ||
                               posting.AccountType == GlAccountTypes.Expense)
                        {
                            begRecord.Debit = 0;
                            begRecord.Credit = 0;
                            begRecord.Balance = 0;
                        }
                        if (posting.AccountType == GlAccountTypes.Asset)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                begRecord.Debit = endRecord.Balance;
                                begRecord.Credit = 0;
                                begRecord.Balance = endRecord.Balance;
                            }
                            else
                            {
                                begRecord.Debit = 0;
                                begRecord.Credit = Math.Abs(endRecord.Balance);
                                begRecord.Balance = endRecord.Balance;
                            }

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                begRecord.Debit = 0;
                                begRecord.Credit = endRecord.Balance;
                                begRecord.Balance = endRecord.Balance;

                            }
                            else
                            {
                                begRecord.Debit = Math.Abs(endRecord.Balance);
                                begRecord.Credit = 0;
                                begRecord.Balance = endRecord.Balance;
                            }
                        }
                        #endregion
                        begRecord.ModifiedBy = "Transfer Ending" + user;
                        begRecord.LastModified = timeStamp;
                        db.GLLedgerPostingLines.Attach(begRecord);
                        db.Entry(begRecord).State = EntityState.Modified;
                    }
                    else
                    {
                        #region add beginning ledger line
                        var beginningPeriod = db.GLPeriods.Find(destPeriod);

                        var newBeginning = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = posting.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };


                        #region balance type

                        if (posting.AccountType == GlAccountTypes.Revenue ||
                              posting.AccountType == GlAccountTypes.Expense)
                        {
                            newBeginning.Debit = 0;
                            newBeginning.Credit = 0;
                            newBeginning.Balance = 0;
                        }

                        if (posting.AccountType == GlAccountTypes.Asset)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                newBeginning.Debit = endRecord.Balance;
                                newBeginning.Credit = 0;
                                newBeginning.Balance = endRecord.Balance;
                            }
                            else
                            {
                                newBeginning.Debit = 0;
                                newBeginning.Credit = Math.Abs(endRecord.Balance);
                                newBeginning.Balance = endRecord.Balance;
                            }

                        }
                        else if (posting.AccountType == GlAccountTypes.Liability ||
                                 posting.AccountType == GlAccountTypes.Capital)
                        {
                            if (endRecord.Balance >= 0)
                            {
                                newBeginning.Debit = 0;
                                newBeginning.Credit = endRecord.Balance;
                                newBeginning.Balance = endRecord.Balance;


                            }
                            else
                            {
                                newBeginning.Debit = Math.Abs(endRecord.Balance);
                                newBeginning.Credit = 0;
                                newBeginning.Balance = endRecord.Balance;
                            }
                        }
                        #endregion
                        db.GLLedgerPostingLines.Add(newBeginning);
                        //db.SaveChanges();

                        #endregion
                    }

                    db.SaveChanges();
                }

                #endregion

            }

            #endregion
            #region transfer retained earning and profit tax

            decimal earningAmount = 0;
            decimal profittax = 0;
            decimal earningAfterTaxAmount = 0;

            var profittaxRate = db.FinanceSettings.FirstOrDefault();
            var retainedAccount = db.GlRetainedEarningAccounts.Where(x => x.Type == EarningTypes.RetainedEarning).FirstOrDefault();
            var profitTaxAccount = db.GlRetainedEarningAccounts.Where(x => x.Type == EarningTypes.ProfitTax).FirstOrDefault();

            var expense = db.GLLedgerPostingLines.Where(x => x.GLPeriodId == sourcePeriod)
                      .Where(x => x.Type == LedgerPostTypes.Ending)
                      .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense).ToList();

            var revenue = db.GLLedgerPostingLines.Where(x => x.GLPeriodId == sourcePeriod)
                .Where(x => x.Type == LedgerPostTypes.Ending)
                .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue).ToList();

            earningAmount = revenue.Sum(x => x.Balance) - expense.Sum(x => x.Balance);


            decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;
            profittax = earningAmount * rate;
            earningAfterTaxAmount = earningAmount - profittax;
            if (earningAfterTaxAmount != 0)
            {
                #region update earning
                var retainedEarning = db.GLLedgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == retainedAccount.AccountCode.ToUpper())
                                        .Where(x => x.GLPeriodId == destPeriod)
                                        .Where(x => x.Type == LedgerPostTypes.Begining)
                                        .FirstOrDefault();

                if (retainedEarning != null)
                {
                    if (earningAfterTaxAmount > 0)
                        retainedEarning.Credit = retainedEarning.Credit + earningAfterTaxAmount;
                    if (earningAfterTaxAmount < 0)
                        retainedEarning.Debit = retainedEarning.Debit + Math.Abs(earningAfterTaxAmount);

                    retainedEarning.Balance = retainedEarning.Balance + earningAfterTaxAmount;

                    db.GLLedgerPostingLines.Attach(retainedEarning);
                    db.Entry(retainedEarning).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                        x.AccountCode.ToUpper() == retainedAccount.AccountCode.ToUpper());
                    if (ledger == null)
                    {
                        #region create ledger

                        var ledgerPosting = new GLLedgerPosting
                        {
                            AccountCode = retainedAccount.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        ledgerPosting.AccountDesc = accountHelper.GetAccountDesc(ledgerPosting.AccountCode);
                        ledgerPosting.AccountType = accountHelper.GetAccountType(ledgerPosting.AccountCode);
                        ledgerPosting.CreatedBy = "System earning-" + user;
                        ledgerPosting.DateCreated = DateTime.Now;
                        db.GLLedgerPostings.Add(ledgerPosting);
                        db.SaveChanges();
                        #endregion
                        #region add beginning ledger line for earning
                        var beginningPeriod = db.GLPeriods.Find(destPeriod);

                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledgerPosting,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = earningAfterTaxAmount,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        if (earningAfterTaxAmount > 0)
                            newEarningLine.Credit = earningAfterTaxAmount;
                        if (earningAfterTaxAmount < 0)
                            retainedEarning.Debit = Math.Abs(earningAfterTaxAmount);

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region add beginning ledger line for earning
                        var beginningPeriod = db.GLPeriods.Find(destPeriod);

                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = ledger.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = earningAfterTaxAmount,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        if (earningAfterTaxAmount > 0)
                            newEarningLine.Credit = earningAfterTaxAmount;
                        if (earningAfterTaxAmount < 0)
                            retainedEarning.Debit = Math.Abs(earningAfterTaxAmount);

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                }
                #endregion
            }

            if (profittax != 0)
            {
                #region update profit tax

                var profitTax = db.GLLedgerPostingLines
                                       .Where(x => x.GLLedgerPosting.AccountCode.ToUpper() == profitTaxAccount.AccountCode.ToUpper())
                                       .Where(x => x.GLPeriodId == destPeriod)
                                       .Where(x => x.Type == LedgerPostTypes.Begining)
                                       .FirstOrDefault();

                if (profitTax != null)
                {
                    if (profittax > 0)
                        profitTax.Credit = profitTax.Credit + profittax;
                    if (profittax < 0)
                        profitTax.Debit = profitTax.Debit + Math.Abs(profittax);

                    profitTax.Balance = profitTax.Balance + profittax;

                    db.GLLedgerPostingLines.Attach(profitTax);
                    db.Entry(profitTax).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var ledger = db.GLLedgerPostings.FirstOrDefault(x =>
                        x.AccountCode.ToUpper() == profitTaxAccount.AccountCode.ToUpper());
                    if (ledger == null)
                    {
                        #region create ledger

                        var ledgerPosting = new GLLedgerPosting
                        {
                            AccountCode = profitTaxAccount.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        ledgerPosting.AccountDesc = accountHelper.GetAccountDesc(ledgerPosting.AccountCode);
                        ledgerPosting.AccountType = accountHelper.GetAccountType(ledgerPosting.AccountCode);
                        ledgerPosting.CreatedBy = "System-" + user;
                        ledgerPosting.DateCreated = DateTime.Now;
                        db.GLLedgerPostings.Add(ledgerPosting);
                        db.SaveChanges();
                        #endregion
                        #region add beginning ledger line for profit tax
                        var beginningPeriod = db.GLPeriods.Find(destPeriod);

                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = ledgerPosting,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = profittax,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        if (profittax > 0)
                            profitTax.Credit = profittax;
                        if (profittax < 0)
                            profitTax.Debit = Math.Abs(profittax);

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region add beginning ledger line for profit tax
                        var beginningPeriod = db.GLPeriods.Find(destPeriod);

                        var newEarningLine = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = ledger.GLLedgerPostingID,
                            JournalNumber = "Beginning-" + beginningPeriod.Name,
                            JournalReference = "Beginning-" + beginningPeriod.Name,
                            EffectiveDate = beginningPeriod.DateFrom,
                            JournalName = "Beginning-" + beginningPeriod.Name,
                            CategoryName = "Beginning-" + beginningPeriod.Name,
                            SourcesName = "Beginning-" + beginningPeriod.Name,
                            GLPeriodId = beginningPeriod.GLPeriodId,
                            GlFiscalYearId = beginningPeriod.GlFiscalYearId,
                            Type = LedgerPostTypes.Begining,
                            Debit = 0,
                            Credit = 0,
                            Balance = profittax,
                            CreatedBy = "Transfer " + user,
                            DateCreated = timeStamp
                        };

                        if (profittax > 0)
                            profitTax.Credit = profittax;
                        if (profittax < 0)
                            profitTax.Debit = Math.Abs(profittax);

                        db.GLLedgerPostingLines.Add(newEarningLine);
                        db.SaveChanges();

                        #endregion
                    }
                }
                #endregion
            }

            #endregion           

        }

        public void DetectAndRemoveDuplicateLedgerLines()
        {
            ExceedDbContext db = new ExceedDbContext();
            db.Database.CommandTimeout = 7200;//added
            List<AccountChekerVm> chekerVms = new List<AccountChekerVm>();
            List<GLPeriod> affectedPeriods = new List<GLPeriod>();
            #region detect duplicate ledger line

            var duplicateLines = db.GLLedgerPostingLines
                .Where(x => x.Type == LedgerPostTypes.Standard)
                .GroupBy(x => x.JournalNumber)
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() })
                .ToList();
            foreach (var duplicate in duplicateLines)
            {
                #region fixe duplicate ledger line

                var duplicateledgerlines = db.GLLedgerPostingLines.Where(x => x.JournalNumber == duplicate.Element).ToList();
                var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                db.GLLedgerPostingLines.RemoveRange(toberemoved);
                db.SaveChanges();
                //update balance monthly end
                var minelinObj = db.GLLedgerPostingLines.Find(minline);
                int firstPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(minelinObj.GlFiscalYearId);
                if (firstPeriodId != 0)
                {
                    var period = db.GLPeriods.Find(firstPeriodId);
                    affectedPeriods.Add(period);
                }
                #endregion
            }
            #endregion

            #region update the starting from the first period of each affected year
            var years = affectedPeriods.Select(x => x.GlFiscalYearId).Distinct().ToList();
            foreach(var y in years)
            {
                var periods = affectedPeriods.Where(x => x.GlFiscalYearId == y).ToList();
                var minPeriod = periods.Min(x => x.DateFrom);
                var minPeriodObj = periods.Where(x => x.DateFrom == minPeriod).FirstOrDefault();
                if(minPeriodObj!=null)
                    UpdateAllBegEndBalancesFastP(minPeriodObj.GLPeriodId, "System");
            }
            #endregion

            DetectAndRemoveDuplicateLedgerAccountCode();
        }

        public void DetectAndRemoveDuplicateLedgerAccountCode()
        {
            ExceedDbContext db = new ExceedDbContext();
            List<int> ledgerid = new List<int>();
            List<GLPeriod> affectedPeriods = new List<GLPeriod>();
            #region detect duplicate ledger account

            var duplicates = db.GLLedgerPostings
                    .GroupBy(x => x.AccountCode)
                    .Where(g => g.Count() > 1)
                    .Select(y => new { Element = y.Key, Counter = y.Count() })
                    .ToList();
            foreach (var duplicate in duplicates)
            {
                #region fix duplicate ledger 

                var duplicateLedgers = db.GLLedgerPostings.Where(x => x.AccountCode == duplicate.Element).ToList();
                var minline = duplicateLedgers.Min(x => x.GLLedgerPostingID);
                var toberemoved = duplicateLedgers.Where(x => x.GLLedgerPostingID != minline).ToList();
                #region transfer to be removed lines
                foreach(var remove in toberemoved)
                {
                    var lines = db.GLLedgerPostingLines.Where(x => x.GLLedgerPostingID == remove.GLLedgerPostingID).ToList();
                    foreach(var line in lines)
                    {
                        line.GLLedgerPostingID = minline;
                        db.GLLedgerPostingLines.Attach(line);
                        db.Entry(line).State = EntityState.Modified;
                        db.SaveChanges();
                        var period = db.GLPeriods.Find(line.GLPeriodId);
                        if(period!=null)
                         affectedPeriods.Add(period);
                    }
                        
                }
                #endregion
                db.GLLedgerPostings.RemoveRange(toberemoved);
                db.SaveChanges();
                #endregion
            }
            #endregion

            #region update the starting from the first period of each affected year
            var years = affectedPeriods.Select(x => x.GlFiscalYearId).Distinct().ToList();
            foreach (var y in years)
            {
                var periods = affectedPeriods.Where(x => x.GlFiscalYearId == y).ToList();
                var minPeriod = periods.Min(x => x.DateFrom);
                var minPeriodObj = periods.Where(x => x.DateFrom == minPeriod).FirstOrDefault();
                if (minPeriodObj != null)
                    UpdateAllBegEndBalancesFastP(minPeriodObj.GLPeriodId, "System-ledger");
            }
            #endregion

        }

    }
}
