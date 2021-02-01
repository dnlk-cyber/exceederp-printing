using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class ProjectOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetProjects()
        {
            List<Branch> projects = db.Branch.Where(x=>x.BranchTypes==BranchTypes.PROJECT).ToList();
            return projects;
        }

    }
}
