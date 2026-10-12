using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class GLFiscalYearObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetGLFiscalYears()
        {
            return db.GLFiscalYears.OrderByDescending(x => x.GlFiscalYearId).ToList();
        }
    }
}
