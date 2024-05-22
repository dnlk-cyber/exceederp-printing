using ExceedERP.Core.Domain.Common;
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
    public class ResiponsiblityLevelODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetResiponsiblityLevel()
        {
            List<string> list = Enum.GetValues(typeof(ResponsibilityLevel))
                                       .Cast<ResponsibilityLevel>()
                                       .Select(e => e.ToString()).ToList();

            return list;
        }
    }
}
