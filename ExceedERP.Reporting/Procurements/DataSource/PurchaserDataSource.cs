using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    public class EmployeeViewModel
    {
        public int EmpId { get; set; }
        public string FullName { get; set; }
        public string FullAmharicName { get; set; }
    }

    [DataObject]
    public class PurchaserDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetEmployees()
        {
            List<EmployeeViewModel> list = new List<EmployeeViewModel>();
            var purchasers = db.Purchasers.ToList();

            foreach (var purchaser in purchasers)
            {
                var employee = db.Person_Employee.Find(purchaser.EmployeeId);
                if (employee != null)
                {
                    var p = new EmployeeViewModel
                    {
                        FullName = employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish,
                        FullAmharicName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName,
                        EmpId = employee.EmployeeId
                    };
                    list.Add(p);
                }
                
            }

              var mypurchasers = list.Select(
                i => new EmployeeViewModel
                { 
                    FullName = i.FullName,
                    FullAmharicName = i.FullAmharicName,
                    EmpId = i.EmpId
                }).AsQueryable();

            return mypurchasers.ToList();
        }

    }
}
