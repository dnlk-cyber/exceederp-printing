using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NationalBanckEmployeeEmploymentTypeSummaryODS : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NationalBankEMployeeTypeSummaryVM> GetEmployeeTypeSummary(string branchId, object[] orgStructureIds,string businessUnit, string status, string source, string empStatus = "")
        {
            return GetEmployees(branchId, orgStructureIds,businessUnit, status);
        }
        public List<NationalBankEMployeeTypeSummaryVM> GetEmployees(string branchId, object[] orgStructureIds, string businessUnit, string status)
        {
            IQueryable<Placement> placements = _db.Placements;
            IQueryable<Person_Employee> employees = _db.Person_Employee;

            // Branch filter
            if (!string.IsNullOrWhiteSpace(branchId) && int.TryParse(branchId, out int branch))
            {
                var empIds = placements
                    .Where(x => x.BranchId == branch)
                    .Select(x => x.EmployeeId);

                employees = employees.Where(x => empIds.Contains(x.EmployeeId));
            }
             
            // Branch filter
            if (!string.IsNullOrWhiteSpace(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
            {
                var empIds = placements.Where(x => x.BusinessUnitId == businessUnitId).Select(x => x.EmployeeId);

                employees = employees.Where(x => empIds.Contains(x.EmployeeId));
            }

            // Org structure filter
            if (orgStructureIds != null && orgStructureIds.Length > 0)
            {
                var orgIds = orgStructureIds
                    .Select(o => int.TryParse(o.ToString(), out int id) ? id : (int?)null)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList();

                if (orgIds.Any())
                {
                    var empIds = placements
                        .Where(x => orgIds.Contains(x.OrgStructureId))
                        .Select(x => x.EmployeeId);

                    employees = employees.Where(x => empIds.Contains(x.EmployeeId));
                }
            }

            // Active filter
            if (status == "Active")
                employees = employees.Where(x => x.IsActive);
            else if (status == "InActive")
                employees = employees.Where(x => !x.IsActive);

            var employeeList = employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeTypeId,
                    e.Gender,
                    e.Status
                })
                .ToList();

            if (!employeeList.Any())
                return new List<NationalBankEMployeeTypeSummaryVM>();

            var employeeIds = new HashSet<int>(employeeList.Select(x => x.EmployeeId));

            var placementList = placements.Where(p => employeeIds.Contains(p.EmployeeId))
                .Select(p => new
                {
                    p.EmployeeId,
                    p.isCurrent,
                    p.AssignmentDate
                }).ToList();

            var employeeTypeIds = employeeList.Select(e => e.EmployeeTypeId).Distinct();

            var employeeTypes = _db.EmployeeTypes
                .Where(x => employeeTypeIds.Contains(x.EmployeeTypeId)).ToList();

            // Fiscal year
            var fyearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = _db.GLFiscalYears.Find(fyearId);

            DateTime startDate = fiscalYear?.DateFrom ?? DateTime.Now.AddMonths(-6);
            DateTime endDate = fiscalYear?.DateTo ?? DateTime.Now;

            var vm = new NationalBankEMployeeTypeSummaryVM
            {
                Region = "All", // since no grouping
                PeriodEnding = fiscalYear?.DateTo != null
                    ? $"{fiscalYear.DateTo:MMMM dd, yyyy} G.C / {HrHelper.GetECDate(fiscalYear.DateTo)} E.C"
                    : "",
                EmployeeSummaries = new List<EmployeeSummaryFlat>()
            };

            foreach (var type in employeeTypes)
            {
                var empSet = new HashSet<int>(
                    employeeList
                        .Where(x => x.EmployeeTypeId == type.EmployeeTypeId)
                        .Select(x => x.EmployeeId)
                );

                var edqIds = new HashSet<int>(
                    placementList
                        .Where(p => empSet.Contains(p.EmployeeId)
                                    && p.AssignmentDate >= startDate
                                    && p.AssignmentDate <= endDate)
                        .Select(p => p.EmployeeId)
                );

                var tqeIds = new HashSet<int>(
                    placementList
                        .Where(p => empSet.Contains(p.EmployeeId) && p.isCurrent)
                        .Select(p => p.EmployeeId)
                );

                var row = new EmployeeSummaryFlat
                {
                    Criteria = type.Name,

                    EDQ_PermanentMale = employeeList.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.MALE),
                    EDQ_PermanentFemale = employeeList.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.FEMALE),
                    EDQ_TemporaryMale = employeeList.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.MALE),
                    EDQ_TemporaryFemale = employeeList.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.FEMALE),

                    TQE_PermanentMale = employeeList.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.MALE),
                    TQE_PermanentFemale = employeeList.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.FEMALE),
                    TQE_TemporaryMale = employeeList.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.MALE),
                    TQE_TemporaryFemale = employeeList.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.FEMALE)
                };

                vm.EmployeeSummaries.Add(row);
            }

            return new List<NationalBankEMployeeTypeSummaryVM> { vm };
        }
        //public List<NationalBankEMployeeTypeSummaryVM> GetEmployees(string branchId, object[] orgStructureIds, string status)
        //{
        //    IQueryable<Placement> placements = _db.Placements;
        //    IQueryable<Person_Employee> employees = _db.Person_Employee;

        //    //  Branch filter
        //    if (int.TryParse(branchId, out int branch))
        //    {
        //        var empIds = placements
        //            .Where(x => x.BranchId == branch)
        //            .Select(x => x.EmployeeId);

        //        employees = employees.Where(x => empIds.Contains(x.EmployeeId));
        //    }

        //    //  Org structure filter
        //    if (orgStructureIds != null && orgStructureIds.Length > 0)
        //    {
        //        var orgIds = orgStructureIds
        //            .Select(o => int.TryParse(o.ToString(), out int id) ? id : (int?)null)
        //            .Where(x => x.HasValue)
        //            .Select(x => x.Value)
        //            .ToList();

        //        if (orgIds.Any())
        //        {
        //            var empIds = placements
        //                .Where(x => orgIds.Contains(x.OrgStructureId))
        //                .Select(x => x.EmployeeId);

        //            employees = employees.Where(x => empIds.Contains(x.EmployeeId));
        //        }
        //    }

        //    //  Active filter
        //    if (status == "Active")
        //        employees = employees.Where(x => x.IsActive);
        //    else if (status == "InActive")
        //        employees = employees.Where(x => !x.IsActive);

        //    var employeeList = employees
        //        .Select(e => new
        //        {
        //            e.EmployeeId,
        //            e.EmployeeTypeId,
        //            e.Region,
        //            e.Gender,
        //            e.Status
        //        })
        //        .ToList();

        //    if (!employeeList.Any())
        //        return new List<NationalBankEMployeeTypeSummaryVM>();

        //    var employeeIds = new HashSet<int> (employeeList.Select(x => x.EmployeeId));

        //    var placementList = placements.Where(p => employeeIds.Contains(p.EmployeeId))
        //        .Select(p => new
        //        {
        //            p.EmployeeId,
        //            p.isCurrent,
        //            p.AssignmentDate
        //        }).ToList();
        //    var employeeTypIds = employeeList.Select(e => e.EmployeeTypeId).Distinct();
        //    var employeeTypes = _db.EmployeeTypes
        //        .Where(x => employeeTypIds.Contains(x.EmployeeTypeId))
        //        .ToList();

        //    // Fiscal year
        //    var fyearId = _fiscalYearHelper.GetCurrentFYearId();
        //    var fiscalYear = _db.GLFiscalYears.Find(fyearId);

        //    DateTime startDate = fiscalYear?.DateFrom ?? DateTime.Now.AddMonths(-6);
        //    DateTime endDate = fiscalYear?.DateTo ?? DateTime.Now;
        //    // FINAL RESULT
        //    var finalResult = new List<NationalBankEMployeeTypeSummaryVM>();

        //    var groupedByRegion = employeeList.GroupBy(e => e.Region);

        //    foreach (var regionGroup in groupedByRegion)
        //    {
        //        var regionEmployees = regionGroup.ToList();
        //        var regionEmpIds = new HashSet<int>(regionEmployees.Select(x => x.EmployeeId));
        //        var vm = new NationalBankEMployeeTypeSummaryVM
        //        {
        //            Region = regionGroup.Key.ToString(),
        //            PeriodEnding = fiscalYear?.DateTo != null ? $"{fiscalYear.DateTo:MMMM dd, yyyy} G.C / {HrHelper.GetECDate(fiscalYear.DateTo)} E.C" : "",

        //            EmployeeSummaries = new List<EmployeeSummaryFlat>()
        //        };

        //        foreach (var type in employeeTypes)
        //        {
        //            var empSet =new HashSet<int>( regionEmployees.Where(x => x.EmployeeTypeId == type.EmployeeTypeId).Select(x => x.EmployeeId));

        //            var edqIds = new HashSet<int>( placementList.Where(p => empSet.Contains(p.EmployeeId) && p.AssignmentDate >= startDate && p.AssignmentDate <= endDate).Select(p => p.EmployeeId));

        //            var tqeIds = new HashSet<int>( placementList.Where(p => empSet.Contains(p.EmployeeId) && p.isCurrent).Select(p => p.EmployeeId));

        //            var row = new EmployeeSummaryFlat
        //            {
        //                Criteria = type.Name,

        //                EDQ_PermanentMale = regionEmployees.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.MALE),
        //                EDQ_PermanentFemale = regionEmployees.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.FEMALE),
        //                EDQ_TemporaryMale = regionEmployees.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.MALE),
        //                EDQ_TemporaryFemale = regionEmployees.Count(x => edqIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.FEMALE),

        //                TQE_PermanentMale = regionEmployees.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.MALE),
        //                TQE_PermanentFemale = regionEmployees.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.PERMANENT && x.Gender == Gender.FEMALE),
        //                TQE_TemporaryMale = regionEmployees.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.MALE),
        //                TQE_TemporaryFemale = regionEmployees.Count(x => tqeIds.Contains(x.EmployeeId) && x.Status == EmployeeStatus.CONTRACT && x.Gender == Gender.FEMALE)
        //            };

        //            vm.EmployeeSummaries.Add(row);
        //        }

        //        finalResult.Add(vm);
        //    }

        //    return finalResult;
        //}

        public void Dispose()
        {
            _db.Dispose();
        }
    }

}