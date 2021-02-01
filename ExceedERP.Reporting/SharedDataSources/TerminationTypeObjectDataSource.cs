using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TerminationTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetTerminationType()
        {
            List<Lookup> list = new List<Lookup> { };


            var terminations = db.Lookups.Where(x => x.LookupType == LookUpTypes.TERMINATIONTYPE && x.IsActive).ToList();
            if (!terminations.Any())
            {
                return list;
            }

            return terminations;//.Select(x => x.Description).ToList();
        }
    }
}