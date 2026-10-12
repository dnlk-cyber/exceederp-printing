using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeHolidayGiftObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<HolidayGiftViewModel> GetEmployeeHolidayGift(string empId,string branchId, 
                                                                 string orgStructureId,string businessUnit,
                                                                 string jobTitle, DateTime? from, DateTime? to)
        {
            // Base query with LEFT JOIN (handles missing Employee reference)
            var query = from p in db.Placements
                        join e in db.Person_Employee
                            on p.EmployeeId equals e.EmployeeId into empGroup
                        from e in empGroup.DefaultIfEmpty()
                        where p.isCurrent && e.IsActive
                        select new { p, e };

            // Filters
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branId))
                query = query.Where(x => x.p.BranchId == branId);

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgStruct))
                query = query.Where(x => x.p.OrgStructureId == orgStruct);

            if (from.HasValue)
            {
                var fromDate = from.Value.Date;
                query = query.Where(x => x.e != null && x.e.HiredDate >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value.Date;
                query = query.Where(x => x.e != null && x.e.HiredDate <= toDate);
            }

            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
                query = query.Where(x => x.p.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
                query = query.Where(x => x.p.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(jobTitle) && int.TryParse(jobTitle, out int lookupId))
                query = query.Where(x => x.p.LookupId == lookupId);

            // Materialize minimal data only
            var data = query
                .OrderByDescending(x => x.p.AssignmentDate)
                .ThenByDescending(x => x.p.EmployeeId)
                .Select(x => new
                {
                    x.p.EmployeeId,
                    x.p.Salary,
                    x.p.LookupId,
                    x.p.JobTitleName,

                    EmployeeCode = x.e != null ? x.e.EmployeeCode : null,
                    HiredDate = x.e != null ? x.e.HiredDate : (DateTime?)null,

                    FirstName = x.e != null ? x.e.FirstName : "",
                    MiddleName = x.e != null ? x.e.MiddleName : "",
                    LastName = x.e != null ? x.e.LastName : "",

                    FirstNameEn = x.e != null ? x.e.FirstNameEnglish : "",
                    MiddleNameEn = x.e != null ? x.e.MiddleNameEnglish : "",
                    LastNameEn = x.e != null ? x.e.LastNameEnglish : ""
                })
                .ToList();

            // Lookup dictionary (FIXED)
            var jobTitleDict = db.Lookups
                .Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE
                          ||x.LookupType == Core.Domain.Common.LookUpTypes.OLD_JOB_TITLE
                          ||x.LookupType == Core.Domain.Common.LookUpTypes.Jobtitle_External_Experience)
                .ToDictionary(x => x.LookupId, x => x.Description);

            // Gift rate settings (small table → safe in memory)
            //var holidayGiftRates = db.HolidayGiftRateSettings.ToList();

            // Final mapping
            var result = data.Select(x =>
            {
                //  Correct salary range logic
                //var gift = holidayGiftRates.FirstOrDefault(r =>
                //    x.Salary >= r.MinGrossSalary &&
                //    x.Salary <= r.MaxGrossSalary);

                var fullName = (cultureName?.ToLower() == "am" || cultureName?.ToLower() == "am-et")
                    ? $"{x.FirstName} {x.MiddleName} {x.LastName}"
                    : $"{x.FirstNameEn} {x.MiddleNameEn} {x.LastNameEn}";

                return new HolidayGiftViewModel
                {
                    Code = x.EmployeeCode ?? "",

                    FullName = fullName.Trim(),

                    DateOfEmployment = x.HiredDate ?? DateTime.MinValue, // safe fallback

                    JobTitle = jobTitleDict.ContainsKey(x.LookupId)
                        ? jobTitleDict[x.LookupId]
                        : x.JobTitleName,

                    BasicSalary = x.Salary,

                    HolidayGift = /*gift?.GiftAmount ??*/ 0
                };
            }).ToList();

            return result;
        }
        //public List<HolidayGiftViewModel> GetEmployeeHolidayGift(string empId, string branchId,
        //     string orgStructureId, string businessUnit,string jobTitle,
        //    DateTime? from, DateTime? to)
        //{
        //    var placementsQuery = db.Placements.AsQueryable();

        //    // Filters
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branId))
        //        placementsQuery = placementsQuery.Where(x => x.BranchId == branId);



        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgStruct))
        //        placementsQuery = placementsQuery.Where(x => x.OrgStructureId == orgStruct);

        //    if (from.HasValue)
        //        placementsQuery = placementsQuery.Where(x => x.Employee.HiredDate >= from.Value);

        //    if (to.HasValue)
        //    {
        //        placementsQuery = placementsQuery.Where(x => x.Employee.HiredDate <= to.Value);

        //    }

        //    if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
        //        placementsQuery = placementsQuery.Where(x => x.EmployeeId == employeeId);

        //    if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
        //        placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == businessUnitId);

        //    if (!string.IsNullOrEmpty(jobTitle) && int.TryParse(jobTitle, out int lookUpId))
        //        placementsQuery = placementsQuery.Where(x => x.LookupId == lookUpId);
        //    // Materialize main placements list
        //    var filteredPlacements = placementsQuery.OrderByDescending(x => x.AssignmentDate).ThenByDescending(x => x.EmployeeId).ToList();

        //    var employeeIds = filteredPlacements.Select(p => p.EmployeeId).Distinct().ToList();

        //    // Preload related data in dictionaries
        //    var employeesDict = db.Person_Employee
        //        .Where(e => employeeIds.Contains(e.EmployeeId))
        //        .ToDictionary(e => e.EmployeeId);


        //    var jobtitleDic = db.Lookups.Where(x=>x.LookupType ==Core.Domain.Common.LookUpTypes.Jobtitle_External_Experience
        //                                        && x.LookupType ==Core.Domain.Common.LookUpTypes.JOB_TITLE
        //                                        && x.LookupType ==Core.Domain.Common.LookUpTypes.OLD_JOB_TITLE)
        //        .GroupBy(s => s.LookupId)
        //        .ToDictionary(g => g.Key, g => g.OrderBy(s => s.Description).ToList());

        //    var allPlacements = db.Placements
        //        .Where(p => employeeIds.Contains(p.EmployeeId)).ToList();
        //    var holidayGiftRates = db.HolidayGiftRateSettings.ToList();


        //    var employeeVMs = new List<HolidayGiftViewModel>();

        //    foreach (var place in filteredPlacements)
        //    {
        //        employeesDict.TryGetValue(place.EmployeeId, out var employee);

        //        jobtitleDic.TryGetValue(place.LookupId, out var lookup);

        //        var currentPlacement = allPlacements
        //            .Where(p => p.EmployeeId == place.EmployeeId && p.isCurrent)
        //            .OrderByDescending(p => p.PlacementId)
        //            .FirstOrDefault();

        //        var existingPlacement = allPlacements
        //            .Where(p => p.EmployeeId == place.EmployeeId && !p.isCurrent)
        //            .OrderByDescending(p => p.PlacementId)
        //            .FirstOrDefault();



        //        var holyGift = holidayGiftRates.Where(x => x.MinGrossSalary <= place.Salary && x.MaxGrossSalary < place.Salary).FirstOrDefault();
        //        decimal giftrate = holyGift?.GiftAmount ?? 0;


        //        var vm = new HolidayGiftViewModel
        //        {
        //            Code = employee?.EmployeeCode,
        //            FullName = employee != null
        //                            ? (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
        //                                ? $"{employee.FirstName} {employee.MiddleName} {employee.LastName}"
        //                                : $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
        //                            : string.Empty,
        //            DateOfEmployment = employee.HiredDate,
        //            JobTitle = lookup.FirstOrDefault()?.Description ?? place.JobTitleName,
        //            BasicSalary = place.Salary,
        //            HolidayGift = giftrate,

        //        };



        //        employeeVMs.Add(vm);
        //    }

        //    return employeeVMs;
        //}


        public void Dispose()
        {
            db.Dispose();
        }
    }
}