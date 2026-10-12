using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class GLPeriodObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLPeriod> GetGLPeriods(string glFiscalYearId)
        {
            int.TryParse(glFiscalYearId, out int fiscalYearId);

            return db.GLPeriods
                .Where(x => x.GlFiscalYearId == fiscalYearId)
                .OrderBy(x => x.Index)
                .ToList();
        }
    }
}
