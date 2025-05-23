
using ExceedERP.Core.Domain.Finance.GL;
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
    public class ProjectTypeODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ConsProjectType> GetProjectType()
        {

            return db.ConsProjectTypes.ToList();
        }
    }

}
