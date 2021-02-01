using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class HospitalObjectDataSource : List<MainCompany>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompany> GetHospital()
        {
            List<MainCompany> list = new List<MainCompany> { };

            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive).ToList();

            if (!hospitals.Any())
            {
                return list;
            }

            return hospitals;
        }
    }
}