
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class RegionDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetRegions()
        {
            var regions = Enum.GetValues(typeof(Region))
                .Cast<Region>()
                .Select(v => v.ToString())
                .ToList();
            return regions;
        }

    }
}
