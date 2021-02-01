using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource
{

    [DataObject]
    public class PeriodObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLPeriod> GetFiscalYear(string fiscalYear)
        {
            List<GLPeriod> fiscalYears = _db.GLPeriods.Where(x=>x.GlFiscalYearId.ToString() == fiscalYear).ToList();
            return fiscalYears;
        }
    }
}