using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class ContractEmployeeDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Person_Employee> GetEmployees()
        {
            var employees = db.Person_Employee.Where(x => x.Status == EmployeeStatus.CONTRACT);
                            
               

            return employees.ToList();
        }

    }
}
