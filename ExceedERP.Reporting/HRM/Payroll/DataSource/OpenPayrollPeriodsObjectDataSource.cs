using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class OpenPayrollPeriodsObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPayrollPeriods()
        {
            List<string> list = new List<string> { };


            var periods = from perio in db.GLPeriods
                join g in db.PayrollPeriods
                    on perio.Name equals g.Name
                where g.Status == FiscalYearStatus.Open
                select perio; //new{periodSelector = perio.PeriodName};
            if (!periods.Any())
            {
                return list;
            }

            return periods.Select(x => x.Name).ToList();
        }
    }
}