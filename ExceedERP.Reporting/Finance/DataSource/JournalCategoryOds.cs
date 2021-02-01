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
    public class JournalCategoryOds : List<GLJournalCategoriers>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLJournalCategoriers> GetJournalCategoryList()
        {
            List<GLJournalCategoriers> list = new List<GLJournalCategoriers> { };

           var glJournalCategorierses = _db.GLJournalCategoriers.ToList();


           return glJournalCategorierses;
        }

       
    }
}
