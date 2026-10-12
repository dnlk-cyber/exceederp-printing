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
    public class ProbationLetterAffanOromoObjectDatasource :IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public RehirementVM GetProbationAffan(string empId)
        {
            RehirementVM EmployeeInfo = new RehirementVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    var gchiredDate = employee.HiredDate.ToString("dd.MM.yyyy");
                    EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
                    EmployeeInfo.FullName =  employee.FullNameaEnglish;
                    EmployeeInfo.HireDate =  $"{employee.HiredDateAmharic}({gchiredDate})";
                    var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
                        if (businessUnit != null)
                        {
                            EmployeeInfo.BusinessUnit = businessUnit.Name;
                        }
                        var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.Rank = rank.RankName;
                        }
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.JobTitle = lookup.Description;
                        }
                        EmployeeInfo.Salary = placement.Salary;
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.WorkUnit = department.Name;
                        }
                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.Name;
                        }
                        EmployeeInfo.AssignmentDate = placement.AssignmentDate.ToString("dd.MM.yyyy");
                        EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
                        EmployeeInfo.Salary = placement.Salary;
                        EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

                    }
                  

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
