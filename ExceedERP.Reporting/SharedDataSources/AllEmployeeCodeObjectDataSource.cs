using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AllEmployeeCodeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeCodeViewModel> GetEmployeeCode()
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
