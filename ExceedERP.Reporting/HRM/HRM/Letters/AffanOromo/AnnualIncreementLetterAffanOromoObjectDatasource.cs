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
    public class AnnualIncreementLetterAffanOromoObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ContractHirementVM GetAnnualIncreementLetter(string empId)
        {
            ContractHirementVM EmployeeInfo = new ContractHirementVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                    EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
                    EmployeeInfo.HiredDate =  employee.HiredDate.ToString("dd.MM.yyyy");
                    EmployeeInfo.FullName =  employee.FullNameaEnglish;
                    var placementPrev = _db.Placements.Where(x => !x.isCurrent && x.EmployeeId == employee.EmployeeId).OrderByDescending(x=>x.PlacementId).FirstOrDefault();
                    if (placementPrev != null)
                    {
                        var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placementPrev.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.FromRank = rank.RankName;
                        }
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placementPrev.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.FromJobTitle = lookup.Description;
                        }
                        EmployeeInfo.Salary = placementPrev.Salary;
                        var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placementPrev.BusinessUnitId);
                        if (businessUnit != null)
                        {
                            EmployeeInfo.FromBusinessUnit = businessUnit.Name;
                        }
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placementPrev.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.FromWorkUnit = department.Name;
                        }
                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placementPrev.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.FromBranch = branch.Name;
                        }
                        EmployeeInfo.FromAssignmentDate = placementPrev.AssignmentDate.ToString("dd.MM.yyyy");
                        //EmployeeInfo.AssignmentDate = placement.AssignmentDate;
                        EmployeeInfo.FromAssignmentDateAmh = placementPrev.AssignmentDateAmharic;
                        EmployeeInfo.FromSalary = placementPrev.Salary;
                        EmployeeInfo.FromSalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

                    }
                    var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placementPrev.BusinessUnitId);
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
                        //EmployeeInfo.AssignmentDate = placement.AssignmentDate;
                        EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
                        EmployeeInfo.Salary = placement.Salary;
                        var probation = _db.ProbationsSettings.FirstOrDefault();
                        EmployeeInfo.Probation = probation != null ? probation.ProbationDays / 30 : 0;
                        EmployeeInfo.ProbationInword = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Probation);
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
    //[DataObject]
    //public class ContractHirement1ObjectDatasource : ContractHirementVM
    //{
    //    private ExceedDbContext _db = new ExceedDbContext();
    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public ContractHirementVM GetContractHirement(string empId)
    //    {

    //        ContractHirementVM EmployeeInfo = new ContractHirementVM();
    //        if (empId != null)
    //        {
    //            int.TryParse(empId, out int employeeId);
    //            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
    //            if (employee != null)
    //            {
    //                EmployeeInfo = new ContractHirementVM();
    //                EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
    //                EmployeeInfo.FullName = employee.EmployeeCode+"-"+employee.FullNameAmharic;
    //                var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId 
    //                                                                    && x.EmployementType == EmployeeStatus.CONTRACT);
    //                if (placement != null)
    //                {
    //                    var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
    //                    if (rank != null)
    //                    {
    //                        EmployeeInfo.Rank = rank.RankName;
    //                    }
    //                    var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
    //                    if (lookup != null)
    //                    {
    //                        EmployeeInfo.JobTitle = lookup.Description;
    //                    }
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
    //                    if (department != null)
    //                    {
    //                        EmployeeInfo.WorkUnit = department.Name;
    //                    }
    //                    var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
    //                    if (branch != null)
    //                    {
    //                        EmployeeInfo.Branch = branch.Name;
    //                    }
    //                    EmployeeInfo.AssignmentDate = placement.AssignmentDate;
    //                    EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

    //                }

    //            }
    //        }
    //        return EmployeeInfo;
    //    }

    //}
}
