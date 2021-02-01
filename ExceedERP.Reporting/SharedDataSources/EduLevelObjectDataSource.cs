using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class EduLevelObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetEduLevel()
        {
            List<Lookup> list = new List<Lookup> { };


            var eduLevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL && x.IsActive).ToList();
            if (!eduLevels.Any())
            {
                return list;
            }

            return eduLevels;
        }
    }
}