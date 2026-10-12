using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeavePeriodsObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetLeavePeriods()
        {
            List<GlFiscalYear> list = new List<GlFiscalYear> { };


            var periods = db.GLFiscalYears.ToList();

            if (!periods.Any())
            {
                return list;
            }

            return periods;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}