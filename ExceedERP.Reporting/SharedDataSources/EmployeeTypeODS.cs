using ExceedERP.Core.Domain.HRM.EmployeeDetail;
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
   public class EmployeeTypeODS
   {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeType> GetEmployeeType()
        {
            return db.EmployeeTypes.ToList(); 
        }
    }
}
