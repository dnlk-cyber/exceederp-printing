using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
   public class TrainingProviderODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompany> GetProvider()
        {
            var providers = db.Companies.Where(x=>x.Category == "1").ToList();

            return providers;
        }
    }
}
