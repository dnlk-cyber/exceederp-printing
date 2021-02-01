using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollCostCenterObjectDataSource : List<GLChartOfAccountCostCenter>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPayrollCostCenters()
        {
            List<string> list = new List<string> { };
            //var periods = db.PayrollPeriods.Where(x => x.Status != "NEW").ToList();
            var costCenters = db.GLChartOfAccountCostCenters.ToList().Select(s => new
            {
                valueSelector = s.Values
            }).OrderBy(x => x.valueSelector);


            if (!costCenters.Any())
            {
                return list;
            }

            return costCenters.Select(x => x.valueSelector).ToList();
        }
    }
}