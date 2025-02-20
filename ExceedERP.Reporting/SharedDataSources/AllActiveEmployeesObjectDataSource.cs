using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AllActiveEmployeesObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetAllActivePlacedEmployees()
        {
            List<EmployeeViewModel> list = new List<EmployeeViewModel>();

            var employees = db.Person_Employee.Where(x => x.IsActive);
            foreach (var employee in employees)
            {
                var employeeList = new EmployeeViewModel();
                employeeList.FullName = employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
                employeeList.FullAmharicName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                employeeList.EmpId = employee.EmployeeId;
                employeeList.EmpCodeWithName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                employeeList.EmpCodeWithNameEng = employee.EmployeeCode + "_" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
                list.Add(employeeList);
            }

            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetAllActivePlacedContractEmployees()
        {
            return db.Placements
                .Where(x => x.isCurrent && x.EmployementType == Core.Domain.Common.EmployeeStatus.CONTRACT)
                .Join(db.Person_Employee,
                      placement => placement.EmployeeId,
                      employee => employee.EmployeeId,
                      (placement, employee) => new EmployeeViewModel
                      {
                          FullName = employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish,
                          FullAmharicName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName,
                          EmpId = employee.EmployeeId,
                          EmpCodeWithName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName,
                          EmpCodeWithNameEng = employee.EmployeeCode + "_" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish
                      })
                .ToList();
        }
        //public List<EmployeeViewModel> GetAllActivePlacedContractEmployees()
        //{
        //    List<EmployeeViewModel> list = new List<EmployeeViewModel>();

        //    var placements = db.Placements.Where(x => x.isCurrent&&x.EmployementType==Core.Domain.Common.EmployeeStatus.CONTRACT);
        //    foreach (var placement in placements)
        //    {
        //        var employeeList = new EmployeeViewModel();
        //        var employee = db.Person_Employee.Find(placement.EmployeeId);
        //        if (employee != null)
        //        {
        //            employeeList.FullName = employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
        //            employeeList.FullAmharicName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
        //            employeeList.EmpId = employee.EmployeeId;
        //            employeeList.EmpCodeWithName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
        //            employeeList.EmpCodeWithNameEng = employee.EmployeeCode + "_" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
        //            list.Add(employeeList);
        //        }
        //    }

        //    return list;
        //}
    }
}
