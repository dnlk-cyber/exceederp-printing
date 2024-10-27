using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class AccountAnalysisDataSource : List<AccountAnalysisViewModel>, IDisposable
    {
        private readonly ExceedDbContext _db;
        public AccountAnalysisDataSource()
        {
            _db=new ExceedDbContext();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountAnalysisViewModel> AccountAnalysis(
         string category, string source, string fromperiod, string toperiod,
         string postStatus, string approvalstatus, string createdby, string branchid)
        {
            _db.Database.CommandTimeout = 7200; // added
            List<AccountAnalysisViewModel> list = new List<AccountAnalysisViewModel>();

            if (!string.IsNullOrEmpty(fromperiod) && !string.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;
               
                var journalQuery = _db.GLRecordJournalDetails
                    //.Include(x => x.GLRecordJournal) // Eagerly load the GLRecordJournal
                    .Where(x => x.GLRecordJournal.isVoid == false &&
                               // x.GLRecordJournal.IsOnlineApproved &&
                                x.GLRecordJournal.EffectiveDate >= dt11 &&
                                x.GLRecordJournal.EffectiveDate <= dt22)
                    .AsNoTracking(); // Disable tracking for better performance

                // Apply filters dynamically
                if (!string.IsNullOrEmpty(approvalstatus))
                {
                    bool isApproved = approvalstatus == "APPROVED";
                    journalQuery = journalQuery.Where(x => x.GLRecordJournal.IsOnlineApproved == isApproved);
                }

                if (!string.IsNullOrEmpty(postStatus))
                {
                    bool isPosted = postStatus == "POSTED";
                    journalQuery = journalQuery.Where(x => x.GLRecordJournal.isPosted == isPosted);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    journalQuery = journalQuery.Where(x =>x.GLRecordJournal.CategoryName==category);
                }

                if (!string.IsNullOrEmpty(source))
                {
                    journalQuery = journalQuery.Where(x => x.GLRecordJournal.SourcesName == source);
                }

                if (!string.IsNullOrEmpty(createdby))
                {
                    journalQuery = journalQuery.Where(x => x.GLRecordJournal.CreatedBy == createdby);
                }

                if (!string.IsNullOrEmpty(branchid))
                {
                    int branchId = int.Parse(branchid);
                    journalQuery = journalQuery.Where(x => x.GLRecordJournal.BranchId == branchId);
                }

                // Project results to AccountAnalysisViewModel
                list = journalQuery.Select(x => new AccountAnalysisViewModel
                {
                    JournalName = x.GLRecordJournal.JournalName,
                    Description = x.GLRecordJournal.Description,
                    Period = x.GLRecordJournal.PeriodName,
                    EffectiveDate = x.GLRecordJournal.EffectiveDate,
                    JournalReference = x.GLRecordJournal.JournalNumber,
                    Category = x.GLRecordJournal.CategoryName,
                    AccountCode = x.AccountCode,
                    AccountDescription = x.AccountDesc,
                    AdditionalDescription = x.AdditionalDescription,
                    Debit = x.Debit,
                    Credit = x.Credit,
                    Remark = x.GLRecordJournal.Remark,
                    CreatedBy = x.CreatedBy,
                    ReferenceDoc = x.GLRecordJournal.ReferenceDoc,
                    PaymentReference = x.GLRecordJournal.PaymentReference,
                    InvoiceReferenceDoc = x.GLRecordJournal.InvoiceReferenceDoc,
                    PostStatus = x.GLRecordJournal.isPosted ? "Posted" : "Not Posted"
                }).ToList();
            }

            return list;
        }
        public void Dispose()
        {
            _db?.Dispose();
        }

    }


}
