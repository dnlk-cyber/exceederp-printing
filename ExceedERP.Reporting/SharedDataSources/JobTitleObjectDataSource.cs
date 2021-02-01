using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class JobTitleObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetJobTitle()
        {
            List<Lookup> list = new List<Lookup> { };


            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            if (!lookups.Any())
            {
                return list;
            }

            return lookups;
        }
    }
}