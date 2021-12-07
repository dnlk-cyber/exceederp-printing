using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class EmployeePerformanceParametrODS : List<Person_Employee>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Person_Employee> GetEmployees()
        {
            var employees = db.Person_Employee.ToList();

            return employees;
        }
    }
}
