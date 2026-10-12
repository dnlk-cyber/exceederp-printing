using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{


    [DataObject]
    public class EmployeeDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetEmployees()
        {
            var employees = db.Person_Employee.Select(
                 i => new EmployeeViewModel
                 {
                     FullName = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                     FullAmharicName = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                     EmpId = i.EmployeeId,
                     EmpCodeWithName = i.EmployeeCode + "-" + i.FirstName + " " + i.MiddleName + " " + i.LastName,
                     EmpCodeWithNameEng = i.EmployeeCode + "-" + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish
                 }).AsQueryable();

            return employees.ToList();
        }

    }




}
