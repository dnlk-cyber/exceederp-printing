using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
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
            EmployeeInfo EmployeeInfo = new EmployeeInfo();

            var placements = db.Placements.ToList();

            var branches = branchId.ToList();
            if (branches.Any())
            {
                List<Placement> filteredPlacements = new List<Placement>();
                foreach (var b in branches)
                {
                    var filt = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, Convert.ToInt32(b)).ToList();

                    filteredPlacements.AddRange(filt);
                }

                placements = filteredPlacements;
            }

            //if (branchId != null)
            //{
            //    int.TryParse(branchId, out int braId);
            //    placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            //}
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }
            if (!string.IsNullOrEmpty(status) && status.ToLower() == "active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (!string.IsNullOrEmpty(status) && status.ToLower() == "inactive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).DistinctByField(x => x.EmployeeId).ToList();
            }

            foreach (var place in placements)
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if (employee != null)
                {
                    int diff = (DateTime.Now.Year - employee.DateOfBirth.Year);
                    if (ageMax != 0 && agemin != 0)
                    {
                        if (diff >= agemin && diff <= ageMax)
                        {
                            EmployeeInfo = new EmployeeInfo();

                            EmployeeInfo.IdNumber = employee.EmployeeCode;
                            EmployeeInfo.BirthDate = employee.DateOfBirthAm;
                            EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

                            EmployeeInfo.FullName = employee.FullNameAmharic;

                            EmployeeInfo.Dob = employee.DateOfBirthAm;

                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                            if(branch != null)
                            {
                                EmployeeInfo.Branch = branch.Name;
                            }
                            var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                            if (deptt != null)
                            {
                                EmployeeInfo.Department = deptt.Name;
                            }

                            EmployeeInfo.CriteriaName = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();

                            EmployeeInfoList.Add(EmployeeInfo);
                        }
                    }
                    else
                    {
                        EmployeeInfo = new EmployeeInfo();

                        EmployeeInfo.IdNumber = employee.EmployeeCode;
                        EmployeeInfo.BirthDate = employee.DateOfBirthAm;
                        EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

                        EmployeeInfo.FullName = employee.FullNameAmharic;

                        EmployeeInfo.Dob = employee.DateOfBirthAm;

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

                        EmployeeInfo.CriteriaName = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();

                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }
            }

            return EmployeeInfoList;
        }
    }
}