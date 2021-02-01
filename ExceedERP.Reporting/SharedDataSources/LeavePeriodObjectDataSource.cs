using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LeavePeriodObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetLeavePeriod()
        {
            List<GlFiscalYear> list = new List<GlFiscalYear>();
            var period = db.GLFiscalYears.ToList();
            if (!period.Any())
            {
                return list;
            }

            return period;//.
        }
    }
}