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
    public class TeminationForManagertNewObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TerminationVM GetTeminationForManagertNew(string empId)
        {
            TerminationVM EmployeeInfo = new TerminationVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.Find(employeeId);
                if (employee != null)
                {
                    EmployeeInfo.FullName = employee.FullNameAmharic;
                }
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId);
                if(placement != null)
                {
                    var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                    if (lookup != null)
                    {
                        EmployeeInfo.JobTitle = lookup.Description;
                    }
                    var businessUnit = _db.BusinessUnits.Find(placement.BusinessUnitId);
                    if (businessUnit != null)
                    {
                        EmployeeInfo.BusinessUnit = businessUnit.Name;
                    }
                    EmployeeInfo.Salary = placement.Salary;
                   
                    var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                    if (branch != null)
                    {
                        EmployeeInfo.Branch = branch.Name;
                    }
                }
                var termination = _db.Terminations.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsOnlineApproved);
                if (termination != null)
                {
                  
                    EmployeeInfo.RequestDate = termination.DateCreated?.ToString("dd.MM.yyyy");
                    EmployeeInfo.TeminatedDate = termination.TerminationDate.Value.ToString("dd.MM.yyyy");
                    EmployeeInfo.TeminatedDateAm = termination.TerminationDateAm;
                    EmployeeInfo.TeminationDateRequest = termination.OnlineApprovedTime?.ToString("dd.MM.yyyy");

                 
                   

                }
              
            }

            return EmployeeInfo;
           
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetTerminatedEmployees()
        {
            var empIds = _db.Terminations.Where(x=>x.IsOnlineApproved).Select(x => x.EmployeeId).Distinct();
            var employees = _db.Person_Employee.Where(x => empIds.Contains(x.EmployeeId)).Where(x => !x.IsActive).Select(
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
