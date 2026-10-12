using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class DayAbsentReportDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DayAbsentFlatViewModel> GetDayAbsentSummary(object[] branchId, string businessUnitId,string orgStructureId,
                                                                string employee, int? fiscalYearId)
        {
            var resultList = new List<DayAbsentFlatViewModel>();

            // 1. Resolve Fiscal Year
            var fiscalYear = fiscalYearId.HasValue
                ? db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId.Value)
                : db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);

            if (fiscalYear == null)
                return resultList;

            // 2. Fetch only the existing GLPeriods for this Fiscal Year, sorted by Index
            var existingPeriods = db.GLPeriods
                .Where(p => p.GlFiscalYearId == fiscalYear.GlFiscalYearId)
                .OrderBy(p => p.Index)
                .ToList();

            if (!existingPeriods.Any())
                return resultList;

            var periodIds = existingPeriods.Select(p => p.GLPeriodId).ToList();

            // 3. Filter Placements
            var placements = db.Placements.Where(x => x.isCurrent).AsQueryable();

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

            var distinctPlacements = placements
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.AssignmentDate).FirstOrDefault())
                .ToList();

            var empIds = distinctPlacements.Select(x => x.EmployeeId).ToList();
            if (!empIds.Any())
                return resultList;

            // 4. Preload reference data
            var employees = db.Person_Employee
                .Where(x => empIds.Contains(x.EmployeeId) && x.IsActive)
                .ToDictionary(x => x.EmployeeId);

            var structures = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var branches = db.Branch.ToDictionary(x => x.BranchId);

            // 5. Preload Absent records
            var absentRecords = db.EmpPayrollNoDaysWorkeds
                .Where(x => empIds.Contains(x.EmployeeId) && periodIds.Contains(x.GLPeriodId)).ToList();


            // 6. Build the cartesian product (Employee x Existing Periods)
            foreach (var pl in distinctPlacements)
            {
                if (!employees.TryGetValue(pl.EmployeeId, out var emp))
                    continue;

                string fullName = $"{emp.FirstNameEnglish ?? emp.FirstName} {emp.MiddleNameEnglish ?? emp.MiddleName} {emp.LastNameEnglish ?? emp.LastName}".Trim();
                string workUnit = structures.TryGetValue(pl.OrgStructureId, out var st) ? st.Name : "-";
                string branchName = branches.TryGetValue(pl.BranchId, out var br) ? (br.Name ?? br.AmharicName) : "-";

                foreach (var period in existingPeriods)
                {
                    decimal days = absentRecords
                        .Where(x => x.EmployeeId == emp.EmployeeId && x.GLPeriodId == period.GLPeriodId)
                        .Sum(x => (decimal?)x.Days) ?? 0m;

                    resultList.Add(new DayAbsentFlatViewModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeCode = emp.EmployeeCode,
                        FullName = fullName,
                        WorkUnit = workUnit,
                        BranchName = branchName,
                        GLPeriodId = period.GLPeriodId,
                        PeriodIndex = period.Index,
                        PeriodName = period.Name,
                        AbsentDays = days
                    });
                }
            }

            return resultList.OrderBy(x => x.WorkUnit).ThenBy(x => x.FullName).ToList();
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

//namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
//{
//    [DataObject]
//    public class DayAbsentReportDataSource : IDisposable
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public List<DayAbsentReportViewModel> GetDayAbsentSummary(
//            object[] branchId,
//            string businessUnitId,
//            string orgStructureId,
//            string employee,
//            int? fiscalYearId)
//        {
//            var resultList = new List<DayAbsentReportViewModel>();

//            // 1. Resolve Fiscal Year
//            var fiscalYear = fiscalYearId.HasValue
//                ? db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId.Value)
//                : db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);

//            if (fiscalYear == null)
//            {
//                return resultList;
//            }

//            DateTime startDate = fiscalYear.DateFrom;
//            DateTime endDate = fiscalYear.DateTo;

//            // 2. Preload all GLPeriods belonging to this Fiscal Year
//            var periodsInYear = db.GLPeriods
//                .Where(p => p.GlFiscalYearId == fiscalYear.GlFiscalYearId || (p.DateFrom >= startDate && p.DateTo <= endDate))
//                .ToList();

//            var periodIds = periodsInYear.Select(p => p.GLPeriodId).ToList();

//            // 3. Filter Placements
//            var placements = db.Placements.Where(x => x.isCurrent).AsQueryable();

//            if (branchId != null && branchId.Any())
//            {
//                var branchIds = branchId
//                    .Where(b => b != null)
//                    .Select(b => Convert.ToInt32(b))
//                    .ToList();
//                placements = placements.Where(x => branchIds.Contains(x.BranchId));
//            }

//            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
//            {
//                placements = placements.Where(x => x.OrgStructureId == orgId);
//            }

//            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bussUnitId))
//            {
//                placements = placements.Where(x => x.BusinessUnitId == bussUnitId);
//            }

//            if (!string.IsNullOrEmpty(employee) && int.TryParse(employee, out int empId))
//            {
//                placements = placements.Where(x => x.EmployeeId == empId);
//            }

//            var distinctPlacements = placements
//                .GroupBy(x => x.EmployeeId)
//                .Select(g => g.OrderByDescending(x => x.AssignmentDate).FirstOrDefault())
//                .ToList();

//            var empIds = distinctPlacements.Select(x => x.EmployeeId).ToList();
//            if (!empIds.Any()) return resultList;

//            // 4. Preload Lookups
//            var employees = db.Person_Employee
//                .Where(x => empIds.Contains(x.EmployeeId))
//                .ToDictionary(x => x.EmployeeId);

//            var structures = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
//            var branches = db.Branch.ToDictionary(x => x.BranchId);

//            // 5. Preload Absent Days from your actual table: EmpPayrollNoDaysWorked
//            var absentRecords = db.EmpPayrollNoDaysWorkeds
//                .Where(x => empIds.Contains(x.EmployeeId) && periodIds.Contains(x.GLPeriodId))
//                .ToList();

//            // 6. Map Each Employee's Monthly Absences
//            foreach (var pl in distinctPlacements)
//            {
//                if (!employees.TryGetValue(pl.EmployeeId, out var emp)) continue;

//                var empAbsents = absentRecords.Where(x => x.EmployeeId == pl.EmployeeId).ToList();

//                // Helper to get total days absent for a given calendar month
//                int GetMonthAbsent(int calendarMonth)
//                {
//                    var matchingPeriodIds = periodsInYear
//                        .Where(p => p.DateFrom.Month == calendarMonth || p.DateTo.Month == calendarMonth)
//                        .Select(p => p.GLPeriodId)
//                        .ToList();

//                    decimal sumDays = empAbsents
//                        .Where(x => matchingPeriodIds.Contains(x.GLPeriodId))
//                        .Sum(x => (decimal?)x.Days) ?? 0m;

//                    return (int)Math.Round(sumDays);
//                }

//                var item = new DayAbsentReportViewModel
//                {
//                    EmployeeId = emp.EmployeeId,
//                    EmployeeCode = emp.EmployeeCode,
//                    FullName = $"{emp.FirstNameEnglish ?? emp.FirstName} {emp.MiddleNameEnglish ?? emp.MiddleName} {emp.LastNameEnglish ?? emp.LastName}".Trim(),
//                    WorkUnit = structures.TryGetValue(pl.OrgStructureId, out var st) ? st.Name : "-",
//                    BranchName = branches.TryGetValue(pl.BranchId, out var br) ? (br.Name ?? br.AmharicName) : "-",

//                    // Ethiopian Fiscal Year: July (Month 7) to June (Month 6)
//                    Month1_Jul = GetMonthAbsent(7),
//                    Month2_Aug = GetMonthAbsent(8),
//                    Month3_Sep = GetMonthAbsent(9),
//                    Month4_Oct = GetMonthAbsent(10),
//                    Month5_Nov = GetMonthAbsent(11),
//                    Month6_Dec = GetMonthAbsent(12),
//                    Month7_Jan = GetMonthAbsent(1),
//                    Month8_Feb = GetMonthAbsent(2),
//                    Month9_Mar = GetMonthAbsent(3),
//                    Month10_Apr = GetMonthAbsent(4),
//                    Month11_May = GetMonthAbsent(5),
//                    Month12_Jun = GetMonthAbsent(6)
//                };

//                item.TotalAbsentDays = item.Month1_Jul + item.Month2_Aug + item.Month3_Sep +
//                                       item.Month4_Oct + item.Month5_Nov + item.Month6_Dec +
//                                       item.Month7_Jan + item.Month8_Feb + item.Month9_Mar +
//                                       item.Month10_Apr + item.Month11_May + item.Month12_Jun;

//                resultList.Add(item);
//            }

//            return resultList.OrderBy(x => x.WorkUnit).ThenBy(x => x.FullName).ToList();
//        }

//        public void Dispose()
//        {
//            db?.Dispose();
//        }
//    }
//}