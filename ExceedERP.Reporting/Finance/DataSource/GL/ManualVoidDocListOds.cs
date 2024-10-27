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
    public class ManualVoidDocListOds : List<ManualVoidDocVm>,IDisposable
    {
        private readonly ExceedDbContext _db;
        public ManualVoidDocListOds()
        {
            _db = new ExceedDbContext();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ManualVoidDocVm> GetVoidDocs(string year, string periodfrom, string periodto)
        {
            List<ManualVoidDocVm> list = new List<ManualVoidDocVm>();

            if (!String.IsNullOrEmpty(periodfrom) && !String.IsNullOrEmpty(periodto))
            {
                int from = int.Parse(periodfrom);
                int to = int.Parse(periodto);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo.Date;
                var records = _db.GLRecordJournals
                    .Where(x => x.IsVoidDoc && x.EffectiveDate >= dt11 && x.EffectiveDate <= dt22)
                    .Select(x => new ManualVoidDocVm
                    {
                        JournalName = x.JournalName,
                        Description = x.Description,
                        FiscalYear = x.FiscalYear,
                        PeriodName = x.PeriodName,
                        CategoryName = x.CategoryName,
                        SourcesName = x.SourcesName,
                        JournalNumber = x.JournalNumber,
                        EffectiveDate = x.EffectiveDate,
                        Remark = x.Remark
                    })
                    .AsNoTracking()
                    .ToList();

                list.AddRange(records);
            }

            return list;
        }



        public void Dispose()
        {
            _db?.Dispose();
        }

    }
}
