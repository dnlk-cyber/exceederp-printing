using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;


namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    public class JournalCategoryOds : List<GLJournalCategoriers>, IDisposable
    {
        private readonly ExceedDbContext _db;
        public JournalCategoryOds()
        {
            _db = new ExceedDbContext();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLJournalCategoriers> GetJournalCategoryList()
        {

            return _db.GLJournalCategoriers
                                  .AsNoTracking()
                                  .ToList();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }


    }
}
