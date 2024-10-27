using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    class FinanceSharedDataSource : IDisposable
    {
        private readonly ExceedDbContext _db;
        public FinanceSharedDataSource()
        {
            _db = new ExceedDbContext();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLChartOfAccountSegment> BranchList()
        {
            // Initialize the list with an estimated capacity if possible
            var list = new List<GLChartOfAccountSegment>();

            // Single query for performance and reduce intermediate results
            var locationSegId = _db.GlSegmentSetups
                                   .Where(x => x.SegmentType == DynamicSegmentTypes.Location)
                                   .Select(x => x.GLSegmentSetupId)
                                   .FirstOrDefault(); // This will pull only the ID, not the entire row

            if (locationSegId != 0) // Assuming 0 is an invalid ID, modify if necessary
            {
                // Use AsNoTracking() to reduce memory usage and avoid tracking
                var glChartOfAccountLocations = _db.GlChartOfAccountSegments
                                                   .Where(x => x.GLSegmentSetupId == locationSegId)
                                                   .AsNoTracking() // If you don’t need to modify the data
                                                   .ToList(); // Collect the data directly to a list

                // If you expect a large number of items, you may pre-allocate memory in the list
                list.Capacity = glChartOfAccountLocations.Count;
                list.AddRange(glChartOfAccountLocations);
            }

            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> TrialBalanceAmountType()
        {
            return new List<string>
            {
                "PTD (period-to-date)",
                "YTD (year-to-date)",
                "PJTD (project-to-date)"
            };

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> TrialBalanceBalancingSegment()
        {
           return new List<string> { "Account", "Company", "Cost Center", "Sub Account" };
           
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
