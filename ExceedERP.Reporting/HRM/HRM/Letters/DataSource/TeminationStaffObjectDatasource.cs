using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource
{
    [DataObject]
    public class TeminationStaffObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TerminationVM GetTeminationStaff(string empId)
        {
            TerminationVM EmployeeInfo = new TerminationVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.Find(employeeId);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                    EmployeeInfo.FullName =  employee.FullNameAmharic;
                }
                var termination = _db.Terminations.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsOnlineApproved);
                if (termination != null)
                {
                   
                  
                    EmployeeInfo.ApplicationDate= termination.ApplicationDate.ToString("dd.MM.yyyy");
                    EmployeeInfo.RequestDate = termination.ApplicationDate.ToString("dd.MM.yyyy");
                    EmployeeInfo.TeminatedDate = termination.TerminationDate.Value.ToString("dd.MM.yyyy");
                    EmployeeInfo.TeminationDateRequest = termination.OnlineApprovedTime?.ToString("dd.MM.yyyy");
                    EmployeeInfo.TeminatedDateAm = termination.TerminationDateAm;

                   
                }
               
            }

            return EmployeeInfo;

           
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetEmployees()
        {
            var empIds = _db.Terminations.Select(x => x.EmployeeId).Distinct();
            var employees = _db.Person_Employee.Where(x=>empIds.Contains(x.EmployeeId)).Where(x=>!x.IsActive).Select(
                 i => new EmployeeViewModel
                 {
                     FullName = i.EmployeeCode + "-" + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                     FullAmharicName = i.EmployeeCode + "-" + i.FirstName + " " + i.MiddleName + " " + i.LastName,
                     EmpId = i.EmployeeId,
                     EmpCodeWithName = i.EmployeeCode + "-" + i.FirstName + " " + i.MiddleName + " " + i.LastName,
                     EmpCodeWithNameEng = i.EmployeeCode + "_" + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish
                 }).AsQueryable();

            return employees.ToList();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
