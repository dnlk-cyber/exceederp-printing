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
    public class ProjectObjectivesODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectObjective> GetProjectInfo()
        {
            
            return db.ProjectObjectives.ToList();
        }
    }
    
}
