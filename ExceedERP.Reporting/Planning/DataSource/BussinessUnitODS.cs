using ExceedERP.Core.Domain.Common.HRM;

using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class BussinessUnitODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BusinessUnit> GetBusinessUnit()
        {

            return db.BusinessUnits.ToList();
        }
    }

}
