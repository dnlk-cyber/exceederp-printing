using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource.AffanOromo
{
    [DataObject]
    public class TeminationEmployeementLetterObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TerminationVM GetTemination(string empId)
        {
            TerminationVM EmployeeInfo = new TerminationVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.Find(employeeId);
                if (employee != null)
                {
                    EmployeeInfo.EmployeeCode = employee.EmployeeCode;
                    EmployeeInfo.FullName = employee.FullNameaEnglish;
                }
                var placement = _db.Placements.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.PlacementId).FirstOrDefault();
                if(placement != null)
                {
                    var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
                    if (businessUnit != null)
                    {
                        EmployeeInfo.BusinessUnit = businessUnit.Name;
                    }
                    EmployeeInfo.Salary = placement.Salary;
                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)placement.Salary);
                    var looksup = _db.Lookups.Find(placement.LookupId);
                    if(looksup != null)
                    {
                        EmployeeInfo.JobTitle=looksup.Description;
                    }
                    var branch = _db.Branch.Find(placement.BranchId);
                    if(branch != null)
                    {
                        EmployeeInfo.Branch=branch.Name;
                    }
                    var grade = _db.Ranks.Find(placement.RankId);
                    if(grade != null)
                    {
                        EmployeeInfo.Branch=grade.RankName;
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
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
