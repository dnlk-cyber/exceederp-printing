
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Planning.Project;
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
    public class ProjectKPIODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectKPI> GetProjectKPI()
        {

            return db.ProjectKPIs.ToList();
        }
    }

    [DataObject]
    public class ProjectSubKPIODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectSubKPI> GetProjectSubKPI(string kpId)
        {
            int.TryParse(kpId, out int kpIId);
            return db.ProjectSubKPIs.Where(x=>x.ProjectKPIId== kpIId).ToList();
        }
    }

}
