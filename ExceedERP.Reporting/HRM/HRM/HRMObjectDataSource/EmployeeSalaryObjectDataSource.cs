using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeSalaryObjectDataSource : List<EmployeeJob>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeJob> GetEmployeeSalary(int min, int max, string branchId,string bussinessUnitId, string orgStructureId, DateTime from, DateTime to, string status)
        {
            List<EmployeeJob> EmployeeInfoList = new List<EmployeeJob>();

            var placements = db.Placements.OrderByDescending(x => x.PlacementId).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (bussinessUnitId != null)
            {
                int.TryParse(bussinessUnitId, out int uId);
                placements = placements.Where(x=>x.BusinessUnitId==uId).ToList();
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }

            if (status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).DistinctByField(x => x.EmployeeId).ToList();
            }

            if (from <= to)
            {
                placements = placements.Where(x => x.AssignmentDate.Date >= from.Date && x.AssignmentDate.Date <= to.Date).ToList();
            }

            placements = placements.DistinctByField(x => x.EmployeeId).ToList();

            foreach (var l in placements)
            {
                EmployeeJob EmployeeInfo = new EmployeeJob();

                if (l.Salary >= min && l.Salary <= max)
                {
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

                    if (employee != null)
                    {
                        EmployeeInfo.EmployeeId = employee.EmployeeCode;

                        EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);
                        EmployeeInfo.FullName = employee.FullNameAmharic;

                        var rank = db.Ranks.FirstOrDefault(x => x.RankId == l.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.Rank = rank.RankName;
                        }

                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);
                        if (look != null)
                        {
                            EmployeeInfo.JobTitle = look.Description;
                        }

                        EmployeeInfo.Salary = l.Salary;

                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == l.OrgStructureId);

                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }

                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }
                else if (min == 0 && max == 0)
                {
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

                    if (employee != null)
                    {
                        EmployeeInfo.EmployeeId = employee.EmployeeCode;

                        EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);
                        EmployeeInfo.FullName = employee.FullNameAmharic;

                        var rank = db.Ranks.FirstOrDefault(x => x.RankId == l.RankId);

                        if (rank != null)
                        {
                            EmployeeInfo.Rank = rank.RankName;
                        }

                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);

                        if (look != null)
                        {
                            EmployeeInfo.JobTitle = look.Description;
                        }

                        EmployeeInfo.Salary = l.Salary;

                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == l.OrgStructureId);

                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }

                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }

            }

            return EmployeeInfoList.OrderBy(x => x.EmployeeId).ToList();
        }
    }
}