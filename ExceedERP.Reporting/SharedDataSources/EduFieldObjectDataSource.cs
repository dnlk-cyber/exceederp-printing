using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class EduFieldObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetEduField()
        {
            List<Lookup> list = new List<Lookup> { };


            var eduFields = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD && x.IsActive).ToList();
            if (!eduFields.Any())
            {
                return list;
            }

            return eduFields;
        }
    }
}