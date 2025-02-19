using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBirthDateObjectDataSource : List<EmployeeInfo>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeInfo> GetEmployeeBirthDate(/*string branchId,*/ object[] branchId, string orgStructureId, int agemin, int ageMax, string status)
        {
            List<EmployeeInfo> EmployeeInfoList = new List<EmployeeInfo>();
            string cultureName = GlobalHelper.GetCurrentCulture();
            var placements = db.Placements.ToList();
            var employeesQuery = db.Person_Employee.AsQueryable();
            var branches = branchId.ToList();

            if (branchId != null && branches.Any())
            {
                List<Placement> filteredPlacements = new List<Placement>();
                HashSet<int> branchIds = new HashSet<int>();
                foreach (var b in branches)
                {
                    branchIds.Add(Convert.ToInt32(b));
                    var filt = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, Convert.ToInt32(b)).ToList();

                    filteredPlacements.AddRange(filt);
                }

                placements = placements.Where(x => branchIds.Contains(x.BranchId)).ToList();
                HashSet<int> empIds = new HashSet<int>(placements.Select(x=>x.EmployeeId));
                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));

            }

            //if (branchId != null)
            //{
            //    int.TryParse(branchId, out int braId);
            //    placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            //}
            if (!string.IsNullOrEmpty(orgStructureId))
            {
                int.TryParse(orgStructureId, out int orgId);
                //placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                HashSet<int> empIds = new HashSet<int>(placements.Where(x => x.OrgStructureId==orgId).Select(x => x.EmployeeId));
                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }
           
            if (!string.IsNullOrEmpty(status) && status.ToLower() == "active")
            {
                // placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
                employeesQuery = employeesQuery.Where(x => x.IsActive);

            }
            if (!string.IsNullOrEmpty(status) && status.ToLower() == "inactive")
            {
                // placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).DistinctByField(x => x.EmployeeId).ToList();
                employeesQuery = employeesQuery.Where(x => !x.IsActive);
            }
            if (!string.IsNullOrEmpty(status) && status.ToLower() == "inactive")
            {
                // placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).DistinctByField(x => x.EmployeeId).ToList();
                employeesQuery = employeesQuery.Where(x => !x.IsActive);
            }
            if (ageMax != 0 && agemin >= 0 && ageMax > agemin)
            {
                DateTime minDate = DateTime.Now.AddYears(-agemin);
                DateTime MaxDate = DateTime.Now.AddYears(-ageMax);
                employeesQuery = employeesQuery.Where(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= agemin && (DateTime.Now.Year - x.DateOfBirth.Year) <= ageMax);
                //employeesQuery = employeesQuery.Where(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= agemin && (DateTime.Now.Year - x.DateOfBirth.Year)<=ageMax);

            }

            foreach (var employee in employeesQuery)
            {
                EmployeeInfo EmployeeInfo = new EmployeeInfo();
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.isCurrent);
                decimal diff = (decimal)(DateTime.Now - employee.DateOfBirth).TotalDays / 365.25m;
                int diffage = (int)diff;
                //decimal diff = DateTime.Now.Year - employee.DateOfBirth.Year;
                if (ageMax > 0 && agemin >= 0 && agemin <= ageMax)
                {
                    if (diffage >= agemin && diffage <= ageMax)
                    {

                        EmployeeInfo.IdNumber = employee.EmployeeCode;
                        EmployeeInfo.BirthDate = employee.DateOfBirthAm;

                        EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);

                        EmployeeInfo.FullName = employee.FullNameAmharic;

                        EmployeeInfo.Dob = employee.DateOfBirthAm;



                        EmployeeInfo.CriteriaName = GetAge(employee.DateOfBirth).ToString();

                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }
                else
                {


                    EmployeeInfo.IdNumber = employee.EmployeeCode;
                    EmployeeInfo.BirthDate = employee.DateOfBirthAm;
                    EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);

                    EmployeeInfo.FullName = employee.FullNameAmharic;

                    EmployeeInfo.Dob = employee.DateOfBirthAm;

                    if (place != null)
                    {
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.Name;
                        }
                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Department = deptt.Name;
                        }
                    }


                    EmployeeInfo.CriteriaName = GetAge(employee.DateOfBirth).ToString();

                    EmployeeInfoList.Add(EmployeeInfo);
                }

            }

            return EmployeeInfoList;
        }

        private static int GetAge(DateTime dateOfBirth)
        {
            decimal diff = (decimal)(DateTime.Now - dateOfBirth).TotalDays / 365.25m;

            // int age = DateTime.Now.Year - dateOfBirth.Year;

            //// Adjust for leap years and birthdays not yet occurring in the current year
            // if (DateTime.Now < dateOfBirth.AddYears(age))
            // {

            //     age--;

            // }

            //return age;
            return (int)diff;
        }
    }
}