using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollPeriodsObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPayrollPeriods()
        {
            List<string> list = new List<string> { };


            var periods = from perio in db.GLPeriods

                join stat in db.PeriodStatuses on perio.GLPeriodId equals stat.GLPeriodId
                where (stat.Status != PeriodStatusList.NEW)
                select perio; //new{periodSelector = perio.PeriodName};
            if (!periods.Any())
            {
                return list;
            }

            return periods.Select(x => x.Name).ToList();
        }
    }
}