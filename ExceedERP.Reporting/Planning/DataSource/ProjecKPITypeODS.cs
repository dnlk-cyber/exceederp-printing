using ExceedERP.Core.Domain.Construction;
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
    public class ProjecKPITypeODS
    {

        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetProjecKPIType()
        {

            List<string> list = Enum.GetValues(typeof(ProjectKPIType))
                                       .Cast<ProjectKPIType>()
                                       .Select(e => e.ToString()).ToList();

            return list;

        }
    }
    
}
