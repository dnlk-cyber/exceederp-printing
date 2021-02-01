using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NotPlacedEmployeesObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NotPlacedEmployeeVM> GetUnPlacedEmployees(string status)
        {
            List<NotPlacedEmployeeVM> employeeList = new List<NotPlacedEmployeeVM>();

            var employees = db.Person_Employee.ToList();
            if(status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList(); ;
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList(); ;
            }
            foreach (var emp in employees)
            {
                var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == emp.EmployeeId);
                if(placement == null)
                {
                    var notPlaced = new NotPlacedEmployeeVM();

                    notPlaced.EmployeeID = emp.EmployeeCode;
                    notPlaced.EmployementType = emp.Status.ToString().ToLower();
                    notPlaced.FullName = emp.FullNameaEnglish;
                    if(emp.IsActive)
                    {
                        notPlaced.Status = "Active";
                    }
                    else
                    {
                        notPlaced.Status = "In Active";
                    }
                    employeeList.Add(notPlaced);
                }
              
            }
            return employeeList;
        }
    }
}
