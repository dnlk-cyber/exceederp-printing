using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
   

    [DataObject]
    public class WorkandCostSummaryPeriodODS :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetTrainingCostSummariesPeriod()
        {
            List<GlFiscalYear> list = new List<GlFiscalYear> { };

            var period = db.GLFiscalYears.ToList();
            if (!period.Any())
            {
                return list;
            }
            return period;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
