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
    public class ManualVoidDocListOds : List<ManualVoidDocVm>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ManualVoidDocVm> GetVoidDocs(string year, string periodfrom, string periodto)
        {
            List<ManualVoidDocVm> list = new List<ManualVoidDocVm> { };

            var records = _db.GLRecordJournals.Where(x => x.IsVoidDoc)
                .Select(x=>new{x.EffectiveDate,x.JournalName,x.Description,x.FiscalYear,x.PeriodName,x.CategoryName,x.SourcesName,x.JournalNumber,x.Remark})
                .AsNoTracking().AsQueryable();
            if (!String.IsNullOrEmpty(periodfrom) && !String.IsNullOrEmpty(periodto))
            {
                int from = int.Parse(periodfrom);
                int to = int.Parse(periodto);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo.Date;
                records = records.Where(x => x.EffectiveDate >= dt11 && x.EffectiveDate <= dt22);

                foreach (var journal in records)
                {
                    var record = new ManualVoidDocVm
                    {
                        JournalName = journal.JournalName,
                        Description = journal.Description,
                        FiscalYear = journal.FiscalYear,
                        PeriodName = journal.PeriodName,
                        CategoryName = journal.CategoryName,
                        SourcesName = journal.SourcesName,
                        JournalNumber = journal.JournalNumber,
                        EffectiveDate = journal.EffectiveDate,
                        Remark = journal.Remark
                    };
                    list.Add(record);
                }
            }

            return list;
        }

       

        
    }
}
