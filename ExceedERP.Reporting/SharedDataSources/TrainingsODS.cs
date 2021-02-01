using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingsODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetTrainings()
        {
            List<Lookup> lookups = db.Lookups.Where(x=>x.LookupType == LookUpTypes.COURSE).ToList();
            return lookups;
        }
    }
   
}
