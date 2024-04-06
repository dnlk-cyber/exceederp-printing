
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{

    [DataObject]
    public class PeriodCascadeDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLPeriod> GetPeriods(string fiscalPeriodId)
        {
           int.TryParse(fiscalPeriodId, out int fiscalYearId);
           return db.GLPeriods.Where(x=>x.GlFiscalYearId==fiscalYearId).ToList();
           
        }

    }

}
