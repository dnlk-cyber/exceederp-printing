using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollCostCenterObjectDataSource : List<GLChartOfAccountSegment>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPayrollCostCenters()
        {
            List<string> list = new List<string> { };
            //var periods = db.PayrollPeriods.Where(x => x.Status != "NEW").ToList();
            var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
            var costCenters = glChartOfAccountCostCenters.ToList().Select(s => new
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