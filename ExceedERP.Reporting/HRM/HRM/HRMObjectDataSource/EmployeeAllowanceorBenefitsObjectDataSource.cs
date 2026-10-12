using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeAllowanceorBenefitsObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeAllowanceorBenefitsVM> GetEmployeeAllowanceorBenefits(string empId, string branch,string orgStructureId, DateTime? from, DateTime? to)
        {

            var query = db.AllowancePerEmployees.AsNoTracking().Include(x => x.Employee).AsQueryable();

            // Employee filter
            if (!string.IsNullOrWhiteSpace(empId) && int.TryParse(empId, out int employeeId))
            {
                query = query.Where(x => x.EmployeeId == employeeId);
            }

            // Branch filter
            if (!string.IsNullOrWhiteSpace(branch) && int.TryParse(branch, out int branchId))
            {
                query = query.Where(x => x.Employee.BranchId == branchId);
            }

            // Organization structure filter
            if (!string.IsNullOrWhiteSpace(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                query = query.Where(x =>
                    db.Placements.Any(p =>
                        p.EmployeeId == x.EmployeeId &&
                        p.OrgStructureId == orgId &&
                        p.isCurrent));
            }

            // Date filters
            if (from.HasValue)
            {
                var fromDate = from.Value.Date;
                query = query.Where(x => x.From.HasValue && DbFunctions.TruncateTime(x.From) >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value.Date;
                query = query.Where(x => x.To.HasValue && DbFunctions.TruncateTime(x.To) <= toDate);
            }

            // Fetch data once
            var allowances = query.ToList();

            if (!allowances.Any())
                return new List<EmployeeAllowanceorBenefitsVM>();

            // Employee IDs
            var employeeIds = allowances
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // Current placements only for relevant employees
            var placements = db.Placements
                .AsNoTracking()
                .Where(x => x.isCurrent && employeeIds.Contains(x.EmployeeId))
                .ToList();

            var placementDict = placements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.First());

            // Lookup dictionaries
            var employeeDict = db.Person_Employee
                .AsNoTracking()
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(
                    x => x.EmployeeId,
                    x => new EmployeeInfo
                    {
                        EmployeeCode = x.EmployeeCode,
                        FullName = string.Join(" ",
                            new[]
                            {
                        x.FirstNameEnglish,
                        x.MiddleNameEnglish,
                        x.LastNameEnglish
                            }.Where(s => !string.IsNullOrWhiteSpace(s)))
                    });

            var jobTitleDict = db.Lookups
                .AsNoTracking()
                .ToDictionary(x => x.LookupId, x => x.Description);

            var departmentDict = db.MainCompanyStructures
                .AsNoTracking()
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            var branchDict = db.Branch
                .AsNoTracking()
                .ToDictionary(x => x.BranchId, x => x.Name);

            var allowanceDefinitionDict = db.AllowanceDefinations
                .AsNoTracking()
                .ToDictionary(x => x.AllowanceDefinationId);

            // Group result by employee
            var result = allowances
                .GroupBy(x => x.EmployeeId)
                .Select(group =>
                {
                    if (!placementDict.TryGetValue(group.Key, out var placement))
                        return null;

                    employeeDict.TryGetValue(group.Key, out var employeeInfo);

                    jobTitleDict.TryGetValue(placement.LookupId, out var title);
                    departmentDict.TryGetValue(placement.OrgStructureId, out var department);
                    branchDict.TryGetValue(placement.BranchId, out var branchName);

                   

                    return new EmployeeAllowanceorBenefitsVM
                    {
                        EmployeeCode = employeeInfo?.EmployeeCode ?? "",
                        EmployeeName = employeeInfo?.FullName ?? "",
                        JobTitle = title ?? "",
                        Department = department ?? "",
                        Branch = branchName ?? "",
                        AssignmentDate = placement.AssignmentDate.ToString("yyyy-MM-dd"),

                        AllowanceorBenefits = group
                            .Select(a =>
                            {
                                if (!allowanceDefinitionDict.TryGetValue(a.AllowanceDefinationId, out var def))
                                    return null;

                                return new AllowanceorBenefitsVM
                                {
                                    AllowanceOrAllowance =
                                        $"{def.AllowanceName} {def.AllowanceType}",
                                    Balance = def.Amount
                                };
                            })
                            .Where(x => x != null)
                            .ToList()
                    };
                })
                .Where(x => x != null)
                .ToList();

            return result;
        }
        //public List<EmployeeAllowanceorBenefitsVM> GetEmployeeAllowanceorBenefits(string empId, string branch,string orgStructureId, string JobTile, DateTime? from, DateTime? to)
        //{

        //    var allowancePerEmployeeQuerys = db.AllowancePerEmployees.AsQueryable();

        //    // Filters
        //    if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
        //    {
        //        allowancePerEmployeeQuerys = allowancePerEmployeeQuerys.Where(x => x.EmployeeId==employeeId);

        //    }

        //    if (!string.IsNullOrEmpty(branch) && int.TryParse(branch, out int branchId))
        //    {
        //        allowancePerEmployeeQuerys = allowancePerEmployeeQuerys.Where(x => x.Employee.BranchId == branchId);

        //    }




        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var employeeIdsInOrg = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent)
        //                                                     .Select(x => x.EmployeeId);
        //        allowancePerEmployeeQuerys = allowancePerEmployeeQuerys.Where(x => employeeIdsInOrg.Contains(x.EmployeeId));
        //    }



        //    if (from.HasValue)
        //    {                //
        //        allowancePerEmployeeQuerys = allowancePerEmployeeQuerys.Where(x => x.From.Value.Date >= from.Value.Date);


        //    }

        //    if (to.HasValue)
        //    {

        //        //under
        //        allowancePerEmployeeQuerys = allowancePerEmployeeQuerys.Where(x => x.To.Value.Date <= to.Value.Date);


        //    }

        //    // Materialize queries
        //    var allowancePerEmployees = allowancePerEmployeeQuerys.ToList();

        //    if (!allowancePerEmployees.Any())
        //        return new List<EmployeeAllowanceorBenefitsVM>();

        //    //  Dictionaries with GroupBy (avoids duplicate key crash)
        //    var fiscalYearDict = db.GLFiscalYears
        //        .GroupBy(x => x.GlFiscalYearId)
        //        .ToDictionary(g => g.Key, g => g.First().Name);
        //    var employeeDict = db.Person_Employee.GroupBy(x => x.EmployeeId)
        //                                    .ToDictionary(
        //                                        g => g.Key,
        //                                        g => new EmployeeInfo
        //                                        {
        //                                            EmployeeCode = g.First().EmployeeCode,
        //                                            FullName = $"{g.First().FirstNameEnglish} {g.First().MiddleNameEnglish} {g.First().LastNameEnglish}"
        //                                        }
        //                                    );


        //    var jobTitleDict = db.Lookups
        //        .GroupBy(x => x.LookupId)
        //        .ToDictionary(g => g.Key, g => g.First().Description);

        //    var departmentDict = db.MainCompanyStructures
        //        .GroupBy(x => x.OrgStructureId)
        //        .ToDictionary(g => g.Key, g => g.First().Name);
        //    var branchDict = db.Branch
        //        .GroupBy(x => x.BranchId)
        //        .ToDictionary(g => g.Key, g => g.First().Name);

        //    var placementDict = db.Placements
        //        .Where(x => x.isCurrent)
        //        .GroupBy(x => x.EmployeeId)
        //        .ToDictionary(g => g.Key, g => g.First());
        //    var allowanceDefintion  = db.AllowanceDefinations
        //        .GroupBy(x => x.AllowanceDefinationId)
        //        .ToDictionary(g => g.Key, g => g.First());

        //    var requestStatuses = db.RequestesStatus.ToList();

        //    var result = new List<EmployeeAllowanceorBenefitsVM>();

        //    // Group allowance per employee
        //    foreach (var employeeLeave in allowancePerEmployees.GroupBy(x => x.EmployeeId).Select(g => g.First()))
        //    {
        //        Placement placement;
        //        if (!placementDict.TryGetValue(employeeLeave.EmployeeId, out placement))
        //            continue;

        //        string empName = "";
        //        string code = "";
        //        if (employeeDict.TryGetValue(placement.EmployeeId, out var empInfo))
        //        {
        //            code = empInfo.EmployeeCode;
        //            empName = empInfo.FullName;
        //        }


        //        string title = "";
        //        jobTitleDict.TryGetValue(placement.LookupId, out title);

        //        string branchName= "";
        //        branchDict.TryGetValue(placement.BranchId, out branchName);

        //        string dept = "";
        //        departmentDict.TryGetValue(placement.OrgStructureId, out dept);

        //        var employeeAllowanceorBenefit = new EmployeeAllowanceorBenefitsVM
        //        {
        //            EmployeeCode = code,
        //            EmployeeName = empName,
        //            JobTitle = title,
        //            Department = dept,
        //            Branch = branchName,
        //            AssignmentDate = placement.AssignmentDate.ToString(),
        //            AllowanceorBenefits = new List<AllowanceorBenefitsVM>()
        //        };

        //        var allowancePerEmployee = allowancePerEmployees
        //            .Where(x => x.EmployeeId == employeeLeave.EmployeeId).ToList();

        //        foreach (var allowance in allowancePerEmployee)
        //        {
        //            if (!allowanceDefintion.TryGetValue(allowance.AllowanceDefinationId, out var allowanceDef))
        //                continue;

        //            employeeAllowanceorBenefit.AllowanceorBenefits.Add(new AllowanceorBenefitsVM
        //            {
        //                AllowanceOrAllowance = allowanceDef.AllowanceName+" "+allowanceDef.AllowanceType.ToString(),
        //                Balance = allowanceDef.Amount,

        //            });
        //        }



        //        result.Add(employeeAllowanceorBenefit);
        //    }

        //    return result;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
        public class EmployeeInfo
        {
            public string EmployeeCode { get; set; }
            public string FullName { get; set; }
        }


     
       

    }


}
