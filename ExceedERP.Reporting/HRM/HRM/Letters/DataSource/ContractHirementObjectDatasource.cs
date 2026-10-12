using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource
{
    [DataObject]
    public class ContractHirementObjectDataSource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ContractHirementVM GetContractHirement(string empId, DateTime? vaccancyDate)
        {
            ContractHirementVM EmployeeInfo = new ContractHirementVM();
            if (vaccancyDate.HasValue)
            {
                EmployeeInfo.VaccancyDate = vaccancyDate.Value.ToString("dd.MM.yyyy");

            }
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId
                                                                       && x.EmployementType == EmployeeStatus.CONTRACT);
                if (placement != null)
                {
                    var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                    if(employee != null)
                    {
                        EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                        EmployeeInfo.FullName =  employee.FullNameAmharic;
                        
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
                    var probation = _db.ProbationsSettings.FirstOrDefault();
                    EmployeeInfo.Probation = probation !=null? probation.ProbationDays/30:0;
                    EmployeeInfo.ProbationInword = XAPI.NumberExtensions.toWordsAmharic((double)EmployeeInfo.Probation);
                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWordsAmharic((double)EmployeeInfo.Salary);
                    
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
