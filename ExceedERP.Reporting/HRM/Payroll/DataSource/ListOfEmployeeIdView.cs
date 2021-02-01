using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    public static class ListOfEmployeeIdView
    {
        private static ExceedDbContext db = new ExceedDbContext();
        public static List<EmployeeIdView> GetEmployees()
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Any())
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdView.FullName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                EmployeeIdView.Status = emp.Status;
                EmployeeIdViews.Add(EmployeeIdView);
            }

            return EmployeeIdViews;
        }
    }
}