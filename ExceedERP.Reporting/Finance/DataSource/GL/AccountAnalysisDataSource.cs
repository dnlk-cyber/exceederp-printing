using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class AccountAnalysisDataSource : List<AccountAnalysisViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountAnalysisViewModel> AccountAnalysis(string category,string source, string fromperiod, string toperiod,string postStatus,string approvalstatus,string createdby,string branchid)
        {

            List<AccountAnalysisViewModel> list = new List<AccountAnalysisViewModel> { };
            AccountAnalysisViewModel record;
            if (category == null)
                category = "";

            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var journals = _db.GLRecordJournalDetails
                    .Where(x => x.GLRecordJournal.isVoid==false)
                    .Where(x => x.GLRecordJournal.EffectiveDate >= dt11)
                    .Where(x => x.GLRecordJournal.EffectiveDate <= dt22)
                    .Select(x=>new{x.GLRecordJournal,x.AccountCode,x.AccountDesc,x.AdditionalDescription,x.Debit,x.Credit,x.CreatedBy})
                    .AsNoTracking()
                    .AsQueryable();

                if (!String.IsNullOrEmpty(approvalstatus))
                {
                    if(approvalstatus== "APPROVED")
                    {
                        journals =journals
                            .Where(x => x.GLRecordJournal.IsOnlineApproved)
                            .AsNoTracking().AsQueryable();
                        
                    }
                    else
                    {
                        journals =journals
                                .Where(x => x.GLRecordJournal.IsOnlineApproved==false)
                                .AsNoTracking().AsQueryable();
                    }
                }

                if (!String.IsNullOrEmpty(postStatus))
                {
                    if (postStatus == "POSTED")
                    {
                        journals =journals
                                .Where(x => x.GLRecordJournal.isPosted)
                                .AsNoTracking().AsQueryable();

                    }
                    else
                    {
                        journals =journals
                                .Where(x => x.GLRecordJournal.isPosted == false)
                                .AsNoTracking().AsQueryable();
                    }
                }

                if (!String.IsNullOrEmpty(category))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.CategoryName.ToUpper() == category.ToUpper())
                            .AsNoTracking().AsQueryable();
                }

                if (!String.IsNullOrEmpty(source))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.SourcesName.ToUpper() == source.ToUpper()).AsNoTracking().AsQueryable();
                }

                if (!String.IsNullOrEmpty(createdby))
                {
                    journals =
                        journals.Where(x => x.GLRecordJournal.CreatedBy== createdby).AsNoTracking().AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                {
                    int did = int.Parse(branchid);
                    journals =
                        journals.Where(x => x.GLRecordJournal.BranchId == did).AsQueryable();
                }


                foreach (var journal in journals)
                {
                    record = new AccountAnalysisViewModel
                    {
                        JournalName = journal.GLRecordJournal.JournalName,
                        Description = journal.GLRecordJournal.Description,
                        Period = journal.GLRecordJournal.PeriodName,
                        EffectiveDate = journal.GLRecordJournal.EffectiveDate.ToShortDateString(),
                        JournalReference = journal.GLRecordJournal.JournalNumber,
                        Category = journal.GLRecordJournal.CategoryName,
                        AccountCode = journal.AccountCode,
                        AccountDescription = journal.AccountDesc,
                        AdditionalDescription = journal.AdditionalDescription,
                        Debit = journal.Debit,
                        Credit = journal.Credit,
                        Remark=journal.GLRecordJournal.Remark,
                        CreatedBy = journal.CreatedBy,
                        ReferenceDoc = journal.GLRecordJournal.ReferenceDoc,
                        PaymentReference = journal.GLRecordJournal.PaymentReference,
                        InvoiceReferenceDoc = journal.GLRecordJournal.InvoiceReferenceDoc,
                    };
                    list.Add(record);
                }


            }
            return list;

        }

       
    }


}
