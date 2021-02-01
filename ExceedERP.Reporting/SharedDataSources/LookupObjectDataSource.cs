using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LookupObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetLookup()
        {
            List<Lookup> list = new List<Lookup> { };


            var lookups = db.Lookups.ToList();
            if (!lookups.Any())
            {
                return list;
            }

            return lookups;
        }
    }
}