using ExceedERP.Core.Domain.Construction;
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
    public class ProjectODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectInformation> GetProjectInfo()
        {
            
            return db.ProjectInformation.ToList();
        }
    }
    
}
