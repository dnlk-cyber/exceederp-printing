using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class ParamODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlobalKPI> GetGlobalKPI()
        {
            
            return db.GlobalKPIs.ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlobalSubKPI> GetGlobalSubKPI(string id)
        {
            
            return db.GlobalSubKPIs.Where(x=>x.GlobalKPIId.ToString()==id).ToList();
        }
    }
    
}
