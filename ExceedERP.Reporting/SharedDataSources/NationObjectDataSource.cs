using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class NationObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetNation()
        {
            List<Lookup> list = new List<Lookup> { };


            var nations = db.Lookups.Where(x => x.LookupType == LookUpTypes.NATION && x.IsActive).ToList();
            if (!nations.Any())
            {
                return list;
            }

            return nations;//.Select(x => x.Description).ToList();
        }
    }
}