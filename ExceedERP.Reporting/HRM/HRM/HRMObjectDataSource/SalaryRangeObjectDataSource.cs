using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class SalaryRangeObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectSalaryRangeRowVM> GetSalaryRangeByProject(DateTime? from, DateTime? to, string branchId)
        {
            var query = from p in db.Placements.AsNoTracking().Where(x => x.isCurrent)
                        join emp in db.Person_Employee.AsNoTracking() on p.EmployeeId equals emp.EmployeeId
                        join b in db.Branch.AsNoTracking() on p.BranchId equals b.BranchId
                        select new
                        {
                            p.BranchId,
                            BranchName = b.Name,
                            p.Salary,
                            emp.Gender,
                            p.AssignmentDate
                        };

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int bId) && bId > 0)
                query = query.Where(x => x.BranchId == bId);

            if (from.HasValue)
                query = query.Where(x => x.AssignmentDate >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.AssignmentDate <= to.Value);

            var records = query.ToList();

            var grouped = records
                .GroupBy(x => new { x.BranchId, x.BranchName })
                .OrderBy(g => g.Key.BranchName)
                .ToList();

            var result = new List<ProjectSalaryRangeRowVM>();

            foreach (var grp in grouped)
            {
                var row = new ProjectSalaryRangeRowVM
                {
                    BranchId = grp.Key.BranchId,
                    ProjectName = grp.Key.BranchName
                };

                foreach (var item in grp)
                {
                    bool isFemale = item.Gender == Gender.FEMALE || item.Gender.ToString().Trim().Equals("Female", StringComparison.OrdinalIgnoreCase);
                    decimal sal = item.Salary;

                    if (sal <= 10000m) { if (isFemale) row.RangeA_Female++; else row.RangeA_Male++; }
                    else if (sal <= 20000m) { if (isFemale) row.RangeB_Female++; else row.RangeB_Male++; }
                    else if (sal <= 30000m) { if (isFemale) row.RangeC_Female++; else row.RangeC_Male++; }
                    else if (sal <= 40000m) { if (isFemale) row.RangeD_Female++; else row.RangeD_Male++; }
                    else if (sal <= 50000m) { if (isFemale) row.RangeE_Female++; else row.RangeE_Male++; }
                    else { if (isFemale) row.RangeF_Female++; else row.RangeF_Male++; }
                }

                result.Add(row);
            }

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DepartmentSalaryRangeRowVM> GetSalaryRangeByDepartment(DateTime? from, DateTime? to, string orgStructureId)
        {
            var query = from p in db.Placements.AsNoTracking().Where(x => x.isCurrent)
                        join emp in db.Person_Employee.AsNoTracking() on p.EmployeeId equals emp.EmployeeId
                        join dept in db.MainCompanyStructures.AsNoTracking() on p.OrgStructureId equals dept.OrgStructureId
                        select new
                        {
                            p.OrgStructureId,
                            DepartmentName = dept.Name,
                            p.Salary,
                            emp.Gender,
                            p.AssignmentDate
                        };

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int deptId) && deptId > 0)
            {
                query = query.Where(x => x.OrgStructureId == deptId);
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.AssignmentDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.AssignmentDate <= to.Value);
            }

            var records = query.ToList();

            var grouped = records
                .GroupBy(x => new { x.OrgStructureId, x.DepartmentName })
                .OrderBy(g => g.Key.DepartmentName)
                .ToList();

            var result = new List<DepartmentSalaryRangeRowVM>();

            foreach (var grp in grouped)
            {
                var row = new DepartmentSalaryRangeRowVM
                {
                    OrgStructureId = grp.Key.OrgStructureId,
                    DepartmentName = grp.Key.DepartmentName ?? "Unassigned Department"
                };

                foreach (var item in grp)
                {
                    bool isFemale = item.Gender == Gender.FEMALE || (item.Gender != null && item.Gender.ToString().Trim().Equals("Female", StringComparison.OrdinalIgnoreCase));
                    decimal sal = item.Salary;

                    if (sal <= 10000m)
                    {
                        if (isFemale) row.RangeA_Female++; else row.RangeA_Male++;
                    }
                    else if (sal <= 20000m)
                    {
                        if (isFemale) row.RangeB_Female++; else row.RangeB_Male++;
                    }
                    else if (sal <= 30000m)
                    {
                        if (isFemale) row.RangeC_Female++; else row.RangeC_Male++;
                    }
                    else if (sal <= 40000m)
                    {
                        if (isFemale) row.RangeD_Female++; else row.RangeD_Male++;
                    }
                    else if (sal <= 50000m)
                    {
                        if (isFemale) row.RangeE_Female++; else row.RangeE_Male++;
                    }
                    else
                    {
                        if (isFemale) row.RangeF_Female++; else row.RangeF_Male++;
                    }
                }

                result.Add(row);
            }

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}