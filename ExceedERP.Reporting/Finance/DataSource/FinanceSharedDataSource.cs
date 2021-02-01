using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    class FinanceSharedDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLChartOfAccountLocation> BranchList()
        {
            List<GLChartOfAccountLocation> list = new List<GLChartOfAccountLocation>();
            list.AddRange(_db.GLChartOfAccountLocations.ToList());
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
