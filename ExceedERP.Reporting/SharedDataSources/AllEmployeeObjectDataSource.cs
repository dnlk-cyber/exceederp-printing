using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AllEmployeeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeCodeViewModel> GetActiveandInActiveEmployeeCode()
        {
            List<EmployeeCodeViewModel> list = new List<EmployeeCodeViewModel>();
            var employees = db.Person_Employee.ToList();
            foreach (var personEmployee in employees)
            {
                var employee = new EmployeeCodeViewModel();
                employee.EmployeeName = personEmployee.FullNameAmharic;
                employee.EmployeeCode = personEmployee.EmployeeCode;
                employee.EmployeeId = personEmployee.EmployeeId;
                list.Add(employee);
            }

            return list;
        }
    }
}