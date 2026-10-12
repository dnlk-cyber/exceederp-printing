using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    public class EmployeePayrollReportDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePayrollSummaryViewModel> GetEmployeePayrollSummary(
            object[] branchId,
            string businessUnitId,
            string orgStructureId,
            string employeeType,
            string status,
            string employee)
        {
            // Increase timeout for heavy analytical queries
            db.Database.CommandTimeout = 180;

            var isLocal = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" || cultureName.ToLower() == "om";
            var datePreference = HrHelper.GetDatePreference();
            var resultList = new List<EmployeePayrollSummaryViewModel>();

            // 1. Placement Filters
            var placements = db.Placements.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status.ToLower() == "active";
                placements = placements.Where(x => x.isCurrent == isActive);
            }
            else
            {
                // Default to current placements only
                placements = placements.Where(x => x.isCurrent);
            }

            if (branchId != null && branchId.Any())
            {
                var branchIds = branchId
                    .Where(b => b != null)
                    .Select(b => Convert.ToInt32(b))
                    .ToList();
                placements = placements.Where(x => branchIds.Contains(x.BranchId));
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = placements.Where(x => x.OrgStructureId == orgId);
            }

            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bussUnitId))
            {
                placements = placements.Where(x => x.BusinessUnitId == bussUnitId);
            }

            if (!string.IsNullOrEmpty(employee) && int.TryParse(employee, out int empId))
            {
                placements = placements.Where(x => x.EmployeeId == empId);
            }

            // Materialize filtered placements in memory first before grouping
            var placementsList = placements.ToList();

            var distinctPlacements = placementsList
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.AssignmentDate).FirstOrDefault())
                .Where(x => x != null)
                .ToList();

            var empIds = distinctPlacements.Select(x => x.EmployeeId).Distinct().ToList();
            if (!empIds.Any()) return resultList;

            var empIdStrings = empIds.Select(x => x.ToString()).ToList();
            var educationRecords = db.Education
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId))
                .ToList();
            var educationsByEmployee = educationRecords
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var educationLookupIds = educationRecords
                .SelectMany(x => new[] { x.EduLevel, x.EduField })
                .Distinct()
                .ToList();

            var allEmployeePlacements = db.Placements
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId))
                .OrderByDescending(x => x.AssignmentDate)
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var placementHistories = db.PlacementHistories
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId))
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var externalExperiences = db.Experiences
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId))
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var terminations = db.Terminations
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId) && x.IsOnlineApproved)
                .OrderByDescending(x => x.TerminationDate)
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            // 2. Pre-load ONLY the required reference data for these employees
            var branchIdsInUse = distinctPlacements.Select(x => x.BranchId).Distinct().ToList();
            var structureIdsInUse = distinctPlacements.Select(x => x.OrgStructureId).Distinct().ToList();
            var lookupIdsInUse = distinctPlacements.Select(x => x.LookupId).Distinct().ToList();

            var allBranches = db.Branch
                .AsNoTracking()
                .Where(x => branchIdsInUse.Contains(x.BranchId))
                .ToDictionary(x => x.BranchId);

            var allStructures = db.MainCompanyStructures
                .AsNoTracking()
                .Where(x => structureIdsInUse.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId);

            var employees = db.Person_Employee
                .AsNoTracking()
                .Where(x => empIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            // Add Nation Lookups
            var nationLookupIds = employees.Values
                .Where(x => !string.IsNullOrEmpty(x.Nation))
                .Select(x => { int.TryParse(x.Nation, out int id); return id; })
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var combinedLookupIds = lookupIdsInUse.Union(nationLookupIds).Union(educationLookupIds).ToList();
            var lookups = db.Lookups
                .AsNoTracking()
                .Where(x => combinedLookupIds.Contains(x.LookupId))
                .ToDictionary(x => x.LookupId);

            // Pre-load Bank Accounts filtering on string Reference in SQL
            var bankAccounts = db.BankReferences
                .AsNoTracking()
                .Where(x => x.Type == BankAccountType.PAYROLL && empIdStrings.Contains(x.Reference))
                .ToList()
                .Where(x => int.TryParse(x.Reference, out int _))
                .GroupBy(x => Convert.ToInt32(x.Reference))
                .ToDictionary(g => g.Key, g => g.First().AccountNumber);

            // Pre-load Phone Numbers filtering on string Reference in SQL
            var phoneCategories = new[] { AddressType.MOBILE_PHONE_1, AddressType.HOME_PHONE, AddressType.OFFICE_PHONE };
            var phoneNumbers = db.Addresses
                .AsNoTracking()
                .Where(x => phoneCategories.Contains(x.Category) && empIdStrings.Contains(x.Reference))
                .ToList()
                .Where(x => int.TryParse(x.Reference, out int _))
                .GroupBy(x => Convert.ToInt32(x.Reference))
                .ToDictionary(g => g.Key, g => g.First().Value);

            // Pre-load Allowances including AllowanceDefination
            var employeeAllowances = db.AllowancePerEmployees
                .AsNoTracking()
                .Include(x => x.AllowanceDefination)
                .Where(x => empIds.Contains(x.EmployeeId) && !x.Pend)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3. Populate ViewModel
            foreach (var pl in distinctPlacements)
            {
                if (!employees.TryGetValue(pl.EmployeeId, out var emp)) continue;

                var item = new EmployeePayrollSummaryViewModel
                {
                    EmployeeId = emp.EmployeeId,
                    EmployeeCode = emp.EmployeeCode,
                    FullName = isLocal
                        ? $"{emp.FirstName ?? ""} {emp.MiddleName ?? ""} {emp.LastName ?? ""}".Trim()
                        : $"{emp.FirstNameEnglish ?? ""} {emp.MiddleNameEnglish ?? ""} {emp.LastNameEnglish ?? ""}".Trim(),
                    Gender = HrHelper.GetGender(emp.Gender),
                    Age = GetAge(emp.DateOfBirth),
                    MaritalStatus = HrHelper.GetMartialStatus(emp),
                    BasicSalary = pl.Salary,
                    ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel),
                    EmploymentType = HrHelper.GetAssignmentType(pl.AssignmentType),
                    EmploymentDate = datePreference ? emp.HiredDateAmharic : emp.HiredDate.ToString("dd.MM.yyyy"),
                    DateOfBirth = datePreference ? emp.DateOfBirthAm : emp.DateOfBirth.ToString("dd.MM.yyyy"),
                    RetirementDate = datePreference
                        ? XAPI.EthiopicDateTime.GetEthiopicDate(emp.DateOfBirth.AddYears(60).Day, emp.DateOfBirth.AddYears(60).Month, emp.DateOfBirth.AddYears(60).Year)
                        : emp.DateOfBirth.AddYears(60).ToString("dd.MM.yyyy")
                };

                // Branch & Department
                if (allBranches.TryGetValue(pl.BranchId, out var br))
                    item.BranchName = br.AmharicName ?? br.Name;

                if (allStructures.TryGetValue(pl.OrgStructureId, out var st))
                    item.Department = st?.Name;

                // Job Title
                if (lookups.TryGetValue(pl.LookupId, out var titleLookup))
                    item.JobTitle = titleLookup?.Description ?? pl.JobTitleName;
                else
                    item.JobTitle = pl.JobTitleName;

                // Nation
                if (!string.IsNullOrEmpty(emp.Nation) && int.TryParse(emp.Nation, out int nationId) && lookups.TryGetValue(nationId, out var nationLookup))
                    item.Nation = nationLookup?.Description;

                if (educationsByEmployee.TryGetValue(emp.EmployeeId, out var employeeEducations))
                {
                    var educationBackground = new List<string>();
                    var educationInstitutions = new List<string>();
                    foreach (var education in employeeEducations)
                    {
                        lookups.TryGetValue(education.EduLevel, out var educationLevel);
                        lookups.TryGetValue(education.EduField, out var educationField);
                        var levelName = isLocal ? educationLevel?.Description : (educationLevel?.DescriptionEng ?? educationLevel?.Description);
                        var fieldName = isLocal ? educationField?.Description : (educationField?.DescriptionEng ?? educationField?.Description);
                        var background = string.Join(" - ", new[] { levelName, fieldName }.Where(x => !string.IsNullOrWhiteSpace(x)));
                        if (!string.IsNullOrWhiteSpace(background)) educationBackground.Add(background);
                        if (!string.IsNullOrWhiteSpace(education.OrganizationName)) educationInstitutions.Add(education.OrganizationName);
                    }

                    item.EducationBackground = string.Join("; ", educationBackground.Distinct());
                    item.EducationInstitutions = string.Join("; ", educationInstitutions.Distinct());
                }

                // Match EmployeeMasterObjectDataSource: internal placements/history plus prior external experience.
                double totalExperienceDays = 0;
                if (allEmployeePlacements.TryGetValue(emp.EmployeeId, out var employeePlacements))
                {
                    var today = DateTime.Today;
                    terminations.TryGetValue(emp.EmployeeId, out var termination);
                    DateTime? previousEndDate = today;
                    bool firstPlacement = true;

                    foreach (var employeePlacement in employeePlacements)
                    {
                        DateTime? endDate = null;
                        if (employeePlacement.isCurrent)
                            endDate = today;
                        else if (firstPlacement && termination?.TerminationDate.HasValue == true &&
                                 termination.TerminationDate.Value > employeePlacement.AssignmentDate)
                            endDate = termination.TerminationDate.Value;
                        else if (!firstPlacement)
                            endDate = previousEndDate;

                        if (endDate.HasValue)
                            totalExperienceDays += (endDate.Value - employeePlacement.AssignmentDate).TotalDays;

                        previousEndDate = employeePlacement.AssignmentDate;
                        firstPlacement = false;
                    }
                }

                if (placementHistories.TryGetValue(emp.EmployeeId, out var employeePlacementHistories))
                    totalExperienceDays += employeePlacementHistories.Sum(x => (x.To - x.From).TotalDays);
                if (externalExperiences.TryGetValue(emp.EmployeeId, out var employeeExternalExperiences))
                    totalExperienceDays += employeeExternalExperiences.Sum(x => (x.To - x.From).TotalDays);

                item.TotalExperience = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et"
                    ? EmployeeHelper.GetDaystoWordsAmharic(totalExperienceDays)
                    : EmployeeHelper.GetDaystoWords(totalExperienceDays);

                // Phone & Bank Account
                item.PhoneNo = phoneNumbers.TryGetValue(emp.EmployeeId, out var phone) ? phone : emp.PhoneNo;
                if (bankAccounts.TryGetValue(emp.EmployeeId, out var accNo))
                    item.AccountNo = accNo;

                // Allowances
                if (employeeAllowances.TryGetValue(emp.EmployeeId, out var allowances))
                {
                    foreach (var al in allowances)
                    {
                        var amt = al.AllowanceDefination?.Amount ?? 0;
                        switch (al.AllowanceDefination?.AllowanceType)
                        {
                            case AllowanceType.RESPONSIBILITY:
                                item.ResponsibilityAllowance = amt;
                                break;
                            case AllowanceType.PROFESSIONAL:
                                item.ProfessionalAllowance = amt;
                                break;
                            case AllowanceType.PERDIEM_TAXABLE:
                                item.TaxablePerDiem = amt;
                                break;
                            case AllowanceType.PERDIEM_NON_TAXABLE:
                                item.NonTaxableAllowance = amt;
                                break;
                            case AllowanceType.PROJECT:
                                item.ProjectAllowance = amt;
                                item.OtherAllowance += amt;
                                break;
                            case AllowanceType.DESERT:
                                item.DesertAllowance = amt;
                                break;
                            case AllowanceType.HARDSHIP:
                                item.HardshipAllowance = amt;
                                break;
                            case AllowanceType.TRANSPORT:
                                item.TransportAllowance = amt;
                                item.OtherAllowance += amt;
                                break;
                            default:
                                item.OtherAllowance += amt;
                                break;
                        }
                    }
                }

                item.TotalAllowance = item.ResponsibilityAllowance + item.ProfessionalAllowance +
                                      item.TaxablePerDiem + item.NonTaxableAllowance + item.DesertAllowance +
                                      item.HardshipAllowance + item.OtherAllowance;

                item.GrossPay = item.BasicSalary + item.TotalAllowance;
                item.NetPay = HrHelper.CalculateNetSalary(item.GrossPay);

                resultList.Add(item);
            }

            return resultList;
        }

        private static int GetAge(DateTime dateOfBirth)
        {
            decimal diff = (decimal)(DateTime.Now - dateOfBirth).TotalDays / 365.25m;
            return (int)diff;
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}