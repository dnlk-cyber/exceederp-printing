using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class BusinessUnitOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BusinessUnitVm> GetBusinessUnits()
        {
            List<BusinessUnitVm> businessUnits = db.BusinessUnits
                .Select(x=> new BusinessUnitVm
                { BusinessUnitId=x.BusinessUnitId,
                    Name =x.Name,
                }
                ).ToList();
            return businessUnits;
        }


    }

    public class BusinessUnitVm
    {
        public int BusinessUnitId { get; set; }
        public string Name { get; set; }
        
    }
}
