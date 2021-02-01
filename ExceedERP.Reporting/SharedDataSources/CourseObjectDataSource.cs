using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class CourseObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetCourse()
        {
            List<Lookup> list = new List<Lookup>();
            var courses = db.Lookups.Where(x => x.LookupType == LookUpTypes.COURSE && x.IsActive).ToList();
            if (!courses.Any())
            {
                return list;
            }

            return courses;//.
        }
    }
}