using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class Branches
    {
       ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetBranches()
            {
                var branches = db.Branch.ToList();
                return branches; 
            }
        
    }
}
