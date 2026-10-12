using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
   public class BenefitTypeODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BenefitTypeVM> GetBenefitType()
        {
            return db.Lookups
                .Where(x => x.LookupType == LookUpTypes.OTHER_BENEFIT)
                .Select(x => new BenefitTypeVM
                {
                    Id = x.LookupId,
                    Name = x.Description
                })
                .ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }

    public class BenefitTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }        
    }
}
