using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.MCG.DataSource
{
    [DataObject]
    public class PromotionAndSalaryAdjustmentObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();
        private object placeWorkedBuilder;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PromotionAndSalaryAdjustmentVM GetPromotionAndSalaryAdjustment(string empId)
        {
            PromotionAndSalaryAdjustmentVM EmployeeInfo = new PromotionAndSalaryAdjustmentVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

                    EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
                    if (employee.Gender == Gender.FEMALE)
                    {
                        EmployeeInfo.FullName = employee.FullNameaEnglish;

                    }
                    if (employee.Gender == Gender.MALE)
                    {
                        EmployeeInfo.FullName = employee.FullNameaEnglish;

                    }
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        if (employee.Gender == Gender.FEMALE)
                        {
                            EmployeeInfo.FullName = employee.FullNameAmharic;

                        }
                        if (employee.Gender == Gender.MALE)
                        {
                            EmployeeInfo.FullName =  employee.FullNameAmharic;

                        }
                    }
                        var placements = _db.Placements.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x=>x.PlacementId);
                    var placement = placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.JobTitle = lookup.Description;
                        }
                        var grade = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        var Scale = _db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);


                        EmployeeInfo.GradeAndScale = $"G={grade?.RankName??null} & S={Scale?.StepName??null}";
                        
                        EmployeeInfo.Salary = placement.Salary;
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.WorkUnit = department.Name;
                        }
                        EmployeeInfo.Earnings = "Adjusted salary ";
                        EmployeeInfo.Salary = placement.Salary;
                        EmployeeInfo.LastAssignmentDate = placement.AssignmentDate.ToString("MMMM dd") +
                                                            " ," +
                                                            placement.AssignmentDate.ToString("yyyy");
                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        var individualPlacements = _db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).OrderBy(x => x.AssignmentDate).ToList();
                  
                        EmployeeInfo.EmployeeAllowances = new List<EmployeeAllowanceVM>();
                        var employeeAllowances = _db.AllowancePerEmployees.Where(x => x.EmployeeId == employeeId && !x.Pend).ToList();
                        foreach(var allowance in employeeAllowances)
                        {
                            var currentEmployeeExpriance = new EmployeeAllowanceVM();

                            var allowanceDef = _db.AllowanceDefinations.FirstOrDefault(x => x.AllowanceDefinationId == allowance.AllowanceDefinationId);

                            currentEmployeeExpriance.Earnings = allowanceDef?.AllowanceName+" "+ allowanceDef.AllowanceType.ToString();
                            currentEmployeeExpriance.Amounts = allowanceDef.Amount;

                            

                            EmployeeInfo.EmployeeAllowances.Add(currentEmployeeExpriance);
                        }
                      
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
