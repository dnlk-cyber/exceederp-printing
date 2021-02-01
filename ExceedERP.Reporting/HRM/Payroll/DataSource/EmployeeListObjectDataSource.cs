using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    public class EmployeeListObjectDataSource : List<EmployeeIdView>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmployeesList()
        {
            List<string> EmployeeIdViews = new List<string>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Any())
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                //EmployeeIdView = new EmployeeIdView();
                //EmployeeIdView.PersonId = emp.PersonID;
                //EmployeeIdView.IsActive = emp.IsActive;
                //EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                //EmployeeIdView.Status = emp.Status;
                EmployeeIdViews.Add(emp.EmployeeCode);
            }

            return EmployeeIdViews;
        }
    }
}