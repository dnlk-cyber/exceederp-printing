using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEmploymentTypeSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId,object[] orgStructureIds,string status,string source,string empStatus = "")
        {
            var list = GetEmployees(branchId, orgStructureIds, status);
            return list.OrderBy(x => x.SummaryName).ToList();
        }
        public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status)
        {
            IQueryable<Placement> placements = _db.Placements;
            IQueryable<Person_Employee> employees = _db.Person_Employee.Where(x => x.IsActive);

            // --- 1️⃣ Filter by branch ---
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branch))
            {
                var empIds = placements
                    .Where(x => x.BranchId == branch)
                    .Select(x => x.EmployeeId)
                    .Distinct();

                employees = employees.Where(x => empIds.Contains(x.EmployeeId));
            }

            // --- 2️⃣ Filter by OrgStructure ---
            if (orgStructureIds != null && orgStructureIds.Length > 0)
            {
                var orgIds = orgStructureIds
                    .Select(o => int.TryParse(o.ToString(), out int id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (orgIds.Any())
                {
                    var empIds = placements
                        .Where(x => orgIds.Contains(x.OrgStructureId))
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => empIds.Contains(x.EmployeeId));
                }
            }

            // --- 3️⃣ Active / Inactive Filter ---
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    var activeEmpIds = placements
                        .Where(x => x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => activeEmpIds.Contains(x.EmployeeId) && x.IsActive);
                }
                else if (status.Equals("InActive", StringComparison.OrdinalIgnoreCase))
                {
                    var inactiveEmpIds = placements
                        .Where(x => !x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => inactiveEmpIds.Contains(x.EmployeeId) && !x.IsActive);
                }
            }

            // --- 4️⃣ Execute query once ---
            var employeeList = employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Gender,
                    e.Status
                })
                .ToList();

            // --- 5️⃣ Prepare all EmployeeStatus values ---
            var allStatuses = Enum.GetValues(typeof(EmployeeStatus))
                .Cast<EmployeeStatus>()
                .Where(s => s != EmployeeStatus.NOTSELECTED)
                .ToList();

            var summaries = new List<EmployeeSummary>();

            foreach (var empStatus in allStatuses)
            {
                var byStatus = employeeList
                    .Where(x => x.Status == empStatus)
                    .ToList();

                var maleCount = byStatus.Count(x => x.Gender == Gender.MALE);
                var femaleCount = byStatus.Count(x => x.Gender == Gender.FEMALE);
                var othersCount = byStatus.Count(x => x.Gender != Gender.MALE && x.Gender != Gender.FEMALE);

                var summary = new EmployeeSummary
                {
                    Criteria = "Employment Type",
                    SummaryName = GetEnumDisplayName(empStatus), // localized name
                    NumberOfMaleEmployee = maleCount,
                    NumberOfFemaleEmployee = femaleCount,
                    NumberOfOtherEmployee = othersCount,
                    TotalEmployee = maleCount + femaleCount + othersCount
                };

                summaries.Add(summary);
            }

            // ✅ Ensure total matches dashboard aggregate
            //var totalSummary = new EmployeeSummary
            //{
            //    Criteria = "Employment Type",
            //    SummaryName = "Total",
            //    NumberOfMaleEmployee = summaries.Sum(x => x.NumberOfMaleEmployee),
            //    NumberOfFemaleEmployee = summaries.Sum(x => x.NumberOfFemaleEmployee),
            //    NumberOfOtherEmployee = summaries.Sum(x => x.NumberOfOtherEmployee),
            //    TotalEmployee = summaries.Sum(x => x.TotalEmployee)
            //};

            //summaries.Add(totalSummary);

            return summaries;
        }
        public string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                         .Cast<DisplayAttribute>()
                                         .FirstOrDefault();

            return displayAttribute?.GetName() ?? value.ToString();
        }

        //public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status)
        //{
        //    IQueryable<Placement> placements = db.Placements;
        //    IQueryable<Person_Employee> employees = db.Person_Employee;

        //    // Filter by branch
        //    if (!string.IsNullOrEmpty(branchId))
        //    {
        //        if (int.TryParse(branchId, out int braId))
        //        {
        //            var branchEmpIds = placements
        //                .Where(x => x.BranchId == braId)
        //                .Select(x => x.EmployeeId).Distinct();

        //            employees = employees.Where(x => branchEmpIds.Contains(x.EmployeeId));
        //        }
        //    }

        //    // Filter by org structure
        //    if (orgStructureIds != null && orgStructureIds.Length > 0)
        //    {
        //        var orgStructIds = new List<int>();
        //        foreach (var orgid in orgStructureIds)
        //        {
        //            int parsed;
        //            if (int.TryParse(orgid.ToString(), out parsed))
        //            {
        //                orgStructIds.Add(parsed);
        //            }
        //        }

        //        if (orgStructIds.Any())
        //        {
        //            var orgEmpIds = placements
        //                .Where(x => orgStructIds.Contains(x.OrgStructureId))
        //                .Select(x => x.EmployeeId).Distinct();

        //            employees = employees.Where(x => orgEmpIds.Contains(x.EmployeeId));
        //        }
        //    }

        //    // Status filtering
        //    if (status == "Active")
        //    {
        //        var activeEmpIds = placements
        //            .Where(x => x.isCurrent)
        //            .Select(x => x.EmployeeId).Distinct();

        //        employees = employees.Where(x => activeEmpIds.Contains(x.EmployeeId) && x.IsActive);
        //    }
        //    else if (status == "InActive")
        //    {
        //        var inactiveEmpIds = placements
        //            .Where(x => !x.isCurrent)
        //            .Select(x => x.EmployeeId).Distinct();

        //        employees = employees.Where(x => inactiveEmpIds.Contains(x.EmployeeId) && !x.IsActive);
        //    }

        //    // Execute query only once here
        //    var employeeList = employees.GroupBy(e => e.EmployeeId)
        //                        .Select(g => new
        //                        {
        //                            EmployeeId = g.Key,
        //                            Gender = g.FirstOrDefault().Gender,
        //                            Status = g.FirstOrDefault().Status
        //                        }).ToList();


        //    var males = new HashSet<int>(employeeList.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId));
        //    var females = new HashSet<int>(employeeList.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId));

        //    var allTypes = Enum.GetValues(typeof(EmployeeStatus))
        //           .Cast<EmployeeStatus>()
        //           .Where(n => n != EmployeeStatus.NOTSELECTED)
        //           .Select(s => s.ToString())
        //           .ToList();


        //    var result = new List<EmployeeSummary>();

        //    foreach (var type in allTypes)
        //    {
        //        var emplistByStatus = employeeList
        //            .Where(x => x.Status.ToString() == type).DistinctByField(e=>e.EmployeeId)
        //            .ToList();

        //        var summary = new EmployeeSummary
        //        {
        //            Criteria = "Employment Type",
        //            SummaryName = GetStatus(type),
        //            NumberOfFemaleEmployee = emplistByStatus.Count(x => females.Contains(x.EmployeeId)),
        //            NumberOfMaleEmployee = emplistByStatus.Count(x => males.Contains(x.EmployeeId)),
        //            TotalEmployee = emplistByStatus.Count
        //        };
        //        result.Add(summary);
        //    }

        //    return result;
        //}

        //public string GetStatus(string empStatus)
        //{
        //    switch (empStatus)
        //    {
        //        case "PERMANENT":
        //            return "Permanent";
        //        case "CONTRACT":
        //            return "Contract";
        //        case "PROBATION":
        //            return "Probation";
        //        case "FREELANCER":
        //            return "Freelancer";
        //        case "NO_STATUS":
        //            return "No Status";
        //        case "SPECIAL":
        //            return "Special";
        //        default:
        //            return empStatus;
        //    }
        //}

        //public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId, object[] orgStructureIds, string status, string source, string empStatus = "")
        //{
        //    var list = GetEmployees(branchId, orgStructureIds, status);

        //    return list.OrderBy(x => x.SummaryName).ToList();
        //}

        //public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status)
        //{
        //    List<EmployeeSummary> list = new List<EmployeeSummary>();

        //    var placements = db.Placements.ToList();
        //    //var employeeIds = new HashSet<int>(placementsss.Select(x=>x.EmployeeId));
        //    //var place = db.Placements.Where(x=>employeeIds.Contains(x.EmployeeId)).ToList();
        //    var employees = db.Person_Employee.ToList();
        //    if (!string.IsNullOrEmpty(branchId))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        // placementsss = placementsss.Where(x => x.BranchId == braId).ToList();
        //        var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
        //        employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
        //    }

        //    if (orgStructureIds !=null)
        //    {
        //        List<int> orgStructIds = new List<int>();
        //        var orgIds = orgStructureIds.ToList();

        //        foreach (var orgid in orgIds)
        //        {
        //            string orgstId = orgid.ToString();
        //            int.TryParse(orgstId, out int orgId);
        //            orgStructIds.Add(orgId);
        //        }
        //        if (orgStructIds.Any())
        //        {
        //            var placemenEmpIds = placements.Where(x => orgStructIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
        //            employees = employees.Where(x => placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

        //        }

        //    }


        //    if (status == "Active")
        //    {
        //        placements = placements.Where(x => x.isCurrent).ToList();
        //        var branchEmpId = placements.Select(x => x.EmployeeId).Distinct().ToList();

        //        employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)&&x.IsActive).DistinctByField(x => x.EmployeeId).ToList();

        //    }
        //    if (status == "InActive")
        //    {
        //        placements = placements.Where(x => !x.isCurrent).ToList();
        //        var branchEmpId = placements.Select(x => x.EmployeeId).Distinct().ToList();

        //        employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId) && !x.IsActive).DistinctByField(x => x.EmployeeId).ToList();


        //    }

        //    var males = employees.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
        //    var females = employees.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

        //    var allTypes = Enum.GetValues(typeof(EmployeeStatus))
        //                   .Cast<EmployeeStatus>()
        //                   .ToList().Select(s => new { Text = s.ToString(), Value = s.ToString() });


        //    foreach (var a in allTypes)
        //    {
        //        var emplistByStatus = employees.Where(x => x.Status.ToString() == a.Value).DistinctByField(x => x.EmployeeId).ToList();

        //        var employee = new EmployeeSummary();

        //        employee.Criteria = "Employement Type";

        //        employee.SummaryName = GetStatus(a.Value.ToString());
        //        employee.NumberOfFemaleEmployee = emplistByStatus.Where(x => females.Contains(x.EmployeeId)).Count();
        //        employee.NumberOfMaleEmployee = emplistByStatus.Where(x => males.Contains(x.EmployeeId)).Count();
        //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

        //        list.Add(employee);

        //    }
        //    return list;
        //}
        //public string GetStatus(string empStatus)
        //{
        //    switch (empStatus)
        //    {
        //        case "PERMANENT":
        //            return "Permanent";

        //        case "CONTRACT":
        //            return "Contract";

        //        case "PROBATION":
        //            return "Probation";
        //        case "FREELANCER":
        //            return "Freelancer";
        //        case "NO_STATUS":
        //            return "No Status";

        //        case "SPECIAL":
        //            return "Special";


        //    }
        //    return empStatus;
        //}
        public void Dispose()
        {
            _db.Dispose();
        }
    }

    public class EmployeesVm
    {
        public int EmployeeId { get; set; }
        public Gender Gender { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; }
        public int BranchId { get; set; }
        public int OrgStructureId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
    }
}