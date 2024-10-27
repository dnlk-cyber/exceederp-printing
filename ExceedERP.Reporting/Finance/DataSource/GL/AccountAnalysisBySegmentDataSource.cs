using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class AccountAnalysisBySegmentDataSource : List<AccountAnalysisViewModel>, IDisposable
    {
        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper ;
        public AccountAnalysisBySegmentDataSource()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
        }
       

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountAnalysisViewModel> AccountAnalysisBySegment(
    string category, string source, string acctFrom, string acctTo,
    string fromperiod, string toperiod, string segmentType, string segmentValue,
    string createdby, string branchid)
        {
            _db.Database.CommandTimeout = 7200; // added
            List<AccountAnalysisViewModel> list = new List<AccountAnalysisViewModel>();

            if (!string.IsNullOrEmpty(fromperiod) && !string.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var journals = _db.GLRecordJournalDetails
                                // .Include(x => x.GLRecordJournal) // Eagerly load the GLRecordJournal
                    .Where(x => string.Compare(x.AccountSegment, acctFrom, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                string.Compare(x.AccountSegment, acctTo, StringComparison.OrdinalIgnoreCase) <= 0 &&
                                x.GLRecordJournal.IsOnlineApproved &&
                                !x.GLRecordJournal.isVoid &&
                                x.GLRecordJournal.EffectiveDate >= dt11 &&
                                x.GLRecordJournal.EffectiveDate <= dt22)
                    .Select(x => new
                    {
                        x.GLRecordJournal,
                        x.AccountCode,
                        x.AccountDesc,
                        x.AdditionalDescription,
                        x.Debit,
                        x.Credit,
                        x.CreatedBy,
                        x.AccountSegment,
                        x.BranchSegment,
                        x.CostCenterSegment,
                        x.SubAccountSegment,
                        x.Segment1,
                        x.Segment2,
                        x.Segment3,
                        x.Segment4,
                        x.Segment5,
                        x.Segment6,
                        x.Segment7,
                        x.Segment8
                    })
                    .AsNoTracking();

                // Combine filters instead of chaining
                if (!string.IsNullOrEmpty(category))
                {
                    journals = journals.Where(x => x.GLRecordJournal.CategoryName == category);
                }

                if (!string.IsNullOrEmpty(source))
                {
                    journals = journals.Where(x => x.GLRecordJournal.SourcesName == source);
                }

                if (!string.IsNullOrEmpty(createdby))
                {
                    journals = journals.Where(x => x.GLRecordJournal.CreatedBy == createdby);
                }

                if (!string.IsNullOrEmpty(branchid))
                {
                    int did = int.Parse(branchid);
                    journals = journals.Where(x => x.GLRecordJournal.BranchId == did);
                }

                if (!string.IsNullOrEmpty(segmentType) && !string.IsNullOrEmpty(segmentValue))
                {
                    var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                    int index = setup?.Sequence ?? 0;

                    switch (index)
                    {
                        case 1: journals = journals.Where(x => x.Segment1 == segmentValue); break;
                        case 2: journals = journals.Where(x => x.Segment2 == segmentValue); break;
                        case 3: journals = journals.Where(x => x.Segment3 == segmentValue); break;
                        case 4: journals = journals.Where(x => x.Segment4 == segmentValue); break;
                        case 5: journals = journals.Where(x => x.Segment5 == segmentValue); break;
                        case 6: journals = journals.Where(x => x.Segment6 == segmentValue); break;
                        case 7: journals = journals.Where(x => x.Segment7 == segmentValue); break;
                        case 8: journals = journals.Where(x => x.Segment8 == segmentValue); break;
                    }
                }

                // Add results to list in one operation instead of iterating
                list = journals.Select(journal => new AccountAnalysisViewModel
                {
                    JournalName = journal.GLRecordJournal.JournalName,
                    Description = journal.GLRecordJournal.Description,
                    Period = journal.GLRecordJournal.PeriodName,
                    EffectiveDate = journal.GLRecordJournal.EffectiveDate,
                    JournalReference = journal.GLRecordJournal.JournalNumber,
                    Category = journal.GLRecordJournal.CategoryName,
                    AccountCode = journal.AccountCode,
                    AccountDescription = journal.AccountDesc,
                    AdditionalDescription = journal.AdditionalDescription,
                    Debit = journal.Debit,
                    Credit = journal.Credit,
                    Remark = journal.GLRecordJournal.Remark,
                    CreatedBy = journal.CreatedBy,
                    ReferenceDoc = journal.GLRecordJournal.ReferenceDoc,
                    PaymentReference = journal.GLRecordJournal.PaymentReference,
                    InvoiceReferenceDoc = journal.GLRecordJournal.InvoiceReferenceDoc,
                }).ToList();
            }

            return list;
        }

        public List<AccountAnalysisViewModel> AccountAnalysisBySegment_old(string category, string source, string acctFrom, string acctTo, string fromperiod, string toperiod,string segmentType,string segmentValue,string createdby, string branchid)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<AccountAnalysisViewModel> list = new List<AccountAnalysisViewModel> { };
           
            AccountAnalysisViewModel record;

            bool isValidRange = true;
            string acctsFrom = "";
            string acctsTo = "";
            string branchFrom = "";
            string branchTo = "";
            string costFrom = "";
            string costTo = "";
            string subFrom = "";
            string subTo = "";
            string newAccountCode = "";
            


            //var accountRange  = _db.GLChartOfAccountAccounts
            //    .Where(x => x.Values.ToUpper().CompareTo(acctFrom) >= 0)
            //    .Where(x => x.Values.ToUpper().CompareTo(acctTo) <= 0)
            //    .Select(x => x.Values)
            //    .ToList();

            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var journals = _db.GLRecordJournalDetails

                    .Where(x => x.AccountSegment.ToUpper().CompareTo(acctFrom) >= 0)//new
                    .Where(x => x.AccountSegment.ToUpper().CompareTo(acctTo) <= 0)//new
                    .Where(x => x.GLRecordJournal.IsOnlineApproved)
                    .Where(x => x.GLRecordJournal.isVoid == false)
                    .Where(x => x.GLRecordJournal.EffectiveDate >= dt11)
                    .Where(x => x.GLRecordJournal.EffectiveDate <= dt22)
                    .Select(x => new { x.GLRecordJournal, x.AccountCode, x.AccountDesc, x.AdditionalDescription, x.Debit, x.Credit, x.CreatedBy,x.AccountSegment,x.BranchSegment,x.CostCenterSegment,x.SubAccountSegment,x.Segment1,x.Segment2,x.Segment3,x.Segment4,x.Segment5,x.Segment6,x.Segment7,x.Segment8 })
                    .AsQueryable()
                    .AsNoTracking();//new

                if (!String.IsNullOrEmpty(category))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.CategoryName.ToUpper() == category.ToUpper())
                            .AsQueryable();
                }

                if (!String.IsNullOrEmpty(source))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.SourcesName.ToUpper() == source.ToUpper()).AsQueryable();
                }

                if (!String.IsNullOrEmpty(createdby))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.CreatedBy == createdby).AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                {
                    int did = int.Parse(branchid);
                    journals =
                        journals.Where(x => x.GLRecordJournal.BranchId == did).AsQueryable();
                }


                //journals = journals.Where(x => accountRange.Contains(x.AccountSegment)).AsQueryable();

                if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                {
                    var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                    int index = setup?.Sequence ?? 0;
                    switch (index)
                    {
                        case 1:
                            journals = journals.Where(x => x.Segment1 == segmentValue).AsQueryable();
                            break;

                        case 2:
                            journals = journals.Where(x => x.Segment2 == segmentValue).AsQueryable();
                            break;

                        case 3:
                            journals = journals.Where(x => x.Segment3 == segmentValue).AsQueryable();
                            break;

                        case 4:
                            journals = journals.Where(x => x.Segment4 == segmentValue).AsQueryable();
                            break;

                        case 5:
                            journals = journals.Where(x => x.Segment5 == segmentValue).AsQueryable();
                            break;

                        case 6:
                            journals = journals.Where(x => x.Segment6 == segmentValue).AsQueryable();
                            break;
                        case 7:
                            journals = journals.Where(x => x.Segment7 == segmentValue).AsQueryable();
                            break;
                        case 8:
                            journals = journals.Where(x => x.Segment8 == segmentValue).AsQueryable();
                            break;
                    }
                }

      

                foreach (var journal in journals)
                {
                    record = new AccountAnalysisViewModel
                    {
                        JournalName = journal.GLRecordJournal.JournalName,
                        Description = journal.GLRecordJournal.Description,
                        Period = journal.GLRecordJournal.PeriodName,
                        EffectiveDate = journal.GLRecordJournal.EffectiveDate,
                        JournalReference = journal.GLRecordJournal.JournalNumber,
                        Category = journal.GLRecordJournal.CategoryName,
                        AccountCode = journal.AccountCode,
                        AccountDescription = journal.AccountDesc,
                        AdditionalDescription = journal.AdditionalDescription,
                        Debit = journal.Debit,
                        Credit = journal.Credit,
                        Remark = journal.GLRecordJournal.Remark,
                        CreatedBy=journal.CreatedBy,
                        ReferenceDoc = journal.GLRecordJournal.ReferenceDoc,
                        PaymentReference = journal.GLRecordJournal.PaymentReference,
                        InvoiceReferenceDoc = journal.GLRecordJournal.InvoiceReferenceDoc,
                    };
                    list.Add(record);
                }

                return list;
            }
            return list;

        }

        public void Dispose()
        {
            _db?.Dispose();
        }


    }
}
