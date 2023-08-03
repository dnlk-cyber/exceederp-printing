using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    class FinanceSharedDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLChartOfAccountSegment> BranchList()
        {
            List<GLChartOfAccountSegment> list = new List<GLChartOfAccountSegment>();

            var locationSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var glChartOfAccountLocations = _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            list.AddRange(glChartOfAccountLocations.ToList());
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> TrialBalanceAmountType()
        {
            List<string> list = new List<string> { "PTD (period-to-date)", "YTD (year-to-date)", "PJTD (project-to-date)" };
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> TrialBalanceBalancingSegment()
        {
            List<string> list = new List<string> { "Account", "Company", "Cost Center" ,"Sub Account"};
            return list;
        }
    }
}
