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
    public class EmployeesInOneDepartmentODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Person_Employee> GetEmployeesInaDepartment(string id)
        {
            int.TryParse(id, out int empId);

            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);

            List<Person_Employee> personEmployes = new List<Person_Employee>();

            if (place != null)
            {
                var placements = db.Placements.Where(x => x.OrgStructureId == place.OrgStructureId && x.isCurrent).Select(x => x.EmployeeId).ToList();

                var persons = db.Person_Employee.Where(x => placements.Contains(x.EmployeeId)).ToList();

                return persons;
            }

            return personEmployes;
        }
    }
}
