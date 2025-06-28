using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.SalesStores.Datasource
{
    [DataObject]
    public class SalesStoreODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesStore> GetSalsesStore()
        {
            ExceedDbContext db = new ExceedDbContext();

            var result = db.SalesStores.ToList();
            return result;

        }
    }
}
