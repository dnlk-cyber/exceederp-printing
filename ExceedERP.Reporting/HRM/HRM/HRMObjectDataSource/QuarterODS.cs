using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class QuarterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Quarter> GetQuarter(string quartLengthId)
        {
            int.TryParse(quartLengthId, out int quarterLengthId);

            var quarter = db.Quarters.Where(x => x.QuarterLengthId == quarterLengthId).ToList();

            return quarter;
        }
    }
}
