using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Payroll;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class AttendanceDayAbsentAndLateCommingListObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceDayAbssentAndLateCommingList> GetDayAbssentAndLateCommingReport(
            string period, string branchId, string dept, string user,
            string bizUnitId, string status, string employementType)
        {
            var result = new List<AttendanceDayAbssentAndLateCommingList>();

            if (string.IsNullOrEmpty(period))
                return result;

            if (!int.TryParse(period, out int glPeriodId))
                return result;

            var periodDate = db.GLPeriods.Find(glPeriodId);
            if (periodDate == null)
                return result;

            // ---------------------------------------------------------
            // 1. BUILD BASE PLACEMENT QUERY
            // ---------------------------------------------------------
            var placementsQuery = db.Placements.Where(x => x.isCurrent);

            if (!string.IsNullOrEmpty(employementType))
                placementsQuery = placementsQuery.Where(x => x.EmployementType.ToString() == employementType);

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
                placementsQuery = placementsQuery.Where(x => x.BranchId == braId);

            if (!string.IsNullOrEmpty(dept) && int.TryParse(dept, out int orgStructureId))
                placementsQuery = placementsQuery.Where(x => x.OrgStructureId == orgStructureId);

            if (!string.IsNullOrEmpty(bizUnitId) && int.TryParse(bizUnitId, out int bizId))
                placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == bizId);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Pend")
                    placementsQuery = placementsQuery.Where(x => x.IsPend);
                else if (status == "Unpend")
                    placementsQuery = placementsQuery.Where(x => !x.IsPend);
            }

            var placements = placementsQuery.ToList();
            var employeeIds = placements.Select(x => x.EmployeeId).Distinct().ToList();

            // ---------------------------------------------------------
            // 2. PRELOAD RELATED DATA (EXCLUDE HIRED AFTER PERIOD END)
            // ---------------------------------------------------------
            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId)
                         && x.IsActive
                         && x.HiredDate <= periodDate.DateTo) // Exclude employees hired after the period ended
                .ToDictionary(x => x.EmployeeId);

            // Re-filter candidate IDs to only those active and hired on or before period end
            var validEmployeeIds = employees.Keys.ToList();

            var noDaysWorked = db.EmpPayrollNoDaysWorkeds
                .Where(x => validEmployeeIds.Contains(x.EmployeeId) && x.GLPeriodId == glPeriodId)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Days));

            var attendanceAll = db.AttendanceNews
                .Where(x => validEmployeeIds.Contains(x.EmployeeId) && x.GlPeriodId == glPeriodId)
                .ToList();

            var lateComingAll = db.LateCommings
                .Where(x => validEmployeeIds.Contains(x.EmployeeId)
                         && x.Date >= periodDate.DateFrom
                         && x.Date <= periodDate.DateTo)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Time));

            // Approver lookups
            var approverLookup = attendanceAll
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ApproverEmpId).Distinct().ToList()
                );

            var approverIds = approverLookup
                .Values
                .SelectMany(a => a)
                .Distinct()
                .ToList();

            var approverNames = db.Person_Employee
                .Where(x => approverIds.Contains(x.EmployeeId))
                .ToDictionary(
                    x => x.EmployeeId,
                    x => (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).Trim()
                );

            // Preload departments to avoid N+1 query inside loop
            var depIds = placements.Select(x => x.OrgStructureId).Distinct().ToList();
            var departments = db.MainCompanyStructures
                .Where(x => depIds.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            // Calculate Period Total Days
            var applyPeriodDays = PayrollConfHelper.ApplyPeriodDays();
            double totalDays;

            if (applyPeriodDays.Item1)
            {
                if (decimal.TryParse(applyPeriodDays.Item2, out decimal days) && days > 20)
                    totalDays = (double)days;
                else
                    totalDays = (periodDate.DateTo - periodDate.DateFrom).Days + 1;
            }
            else
            {
                totalDays = 30;
            }

            // ---------------------------------------------------------
            // 3. MAIN DATA POPULATION LOOP
            // ---------------------------------------------------------
            foreach (var pl in placements)
            {
                // Skips employees who are inactive or hired after periodDate.DateTo
                if (!employees.ContainsKey(pl.EmployeeId))
                    continue;

                var emp = employees[pl.EmployeeId];

                var report = new AttendanceDayAbssentAndLateCommingList
                {
                    EmpID = emp.EmployeeCode,
                    FullName = emp.FullNameaEnglish,
                    Period = periodDate.Name
                };

                // Approver Names
                if (approverLookup.TryGetValue(pl.EmployeeId, out var appIds))
                {
                    var names = appIds
                        .Where(id => approverNames.ContainsKey(id))
                        .Select(id => approverNames[id]);
                    report.ApprovedBy = string.Join(", ", names);
                }

                // Department
                if (departments.TryGetValue(pl.OrgStructureId, out string deptName))
                {
                    report.Department = deptName;
                }

                // Absent Calculation
                double fromPayroll = (double)(noDaysWorked.ContainsKey(pl.EmployeeId) ? noDaysWorked[pl.EmployeeId] : 0);

                double morningAbsent = attendanceAll
                    .Count(x => x.EmployeeId == pl.EmployeeId
                             && x.MorningAttendance == TakeAttendance.Absent
                             && x.IsApproved
                             && x.IsDepartmentApproved) * 0.5;

                double afternoonAbsent = attendanceAll
                    .Count(x => x.EmployeeId == pl.EmployeeId
                             && x.AfternoonAttendance == TakeAttendance.Absent
                             && x.IsApproved
                             && x.IsDepartmentApproved) * 0.5;

                report.NoAbsent = fromPayroll + morningAbsent + afternoonAbsent;

                // Late Coming Calculation
                if (lateComingAll.ContainsKey(pl.EmployeeId))
                {
                    report.LateDays = (decimal)lateComingAll[pl.EmployeeId] / 8;
                }

                // Field days & Leave days
                decimal fieldDays = HrHelper.GetFieldDays(emp.EmployeeId, periodDate.GLPeriodId, null, null);
                decimal leaveDays = HrHelper.GetLeaveDays(emp.EmployeeId, periodDate.GLPeriodId, null, null);

                report.FieldDays = fieldDays >= 0m && fieldDays <= 31m ? fieldDays : 0m;
                report.LeaveDays = leaveDays;

                // Calculate unworked days if hired mid-period
                double unWorkedDays = emp.HiredDate > periodDate.DateFrom
                    ? (emp.HiredDate.Date - periodDate.DateFrom.Date).TotalDays
                    : 0;

                // Calculate Net Working Days with floor clamp at 0
                double calculatedWorkingDays = totalDays - (report.NoAbsent + unWorkedDays + (double)report.LateDays);
                report.NoWorkingDays = calculatedWorkingDays < 0 ? 0 : calculatedWorkingDays;

                result.Add(report);
            }

            return result;
        }

        public List<string> GetUserBranchEmployees(int dept, List<string> branchId = null)
        {
            var placments = from p in db.Placements
                            join e in db.Person_Employee on p.EmployeeId equals e.EmployeeId
                            where p.isCurrent && e.IsActive
                            select new
                            {
                                EmployeeId = p.EmployeeId,
                                p.BranchId,
                                p.OrgStructureId
                            };

            if (branchId != null)
            {
                placments = placments.Where(x => branchId.Contains(x.BranchId.ToString()));
            }

            if (dept != 0)
            {
                placments = placments.Where(x => x.OrgStructureId == dept);
            }

            if (placments.Any())
            {
                return placments.Select(x => x.EmployeeId.ToString()).ToList();
            }

            return new List<string>();
        }

        private decimal GetFieldDays(int empId, int glperiodId)
        {
            var periodObj = db.GLPeriods.Find(glperiodId);
            if (periodObj == null)
                return 0;

            var fieldDsys = db.ApTravelOrderDepartures
                .Where(d => d.TravelOrder.IsOnlineApproved
                         && !d.TravelOrder.IsVoid
                         && d.TravelOrder.EmployeeId == empId
                         && (d.Type == DepartureType.Departure || d.Type == DepartureType.Return)
                         && d.ArrivalDate >= periodObj.DateFrom
                         && d.Date <= periodObj.DateTo)
                .GroupBy(d => d.ApTravelOrderId)
                .Select(g => new
                {
                    StartDate = g.Where(d => d.Type == DepartureType.Departure)
                                 .OrderBy(d => d.Date)
                                 .Select(d => d.Date)
                                 .FirstOrDefault(),
                    EndDate = g.Where(d => d.Type == DepartureType.Return)
                               .OrderByDescending(d => d.ArrivalDate)
                               .Select(d => d.ArrivalDate)
                               .FirstOrDefault()
                })
                .AsNoTracking()
                .ToList();

            if (fieldDsys.Any())
                return fieldDsys.Sum(g => (g.EndDate - g.StartDate).Days + 1);

            return 0;
        }

        private decimal GetLeaveDays(int empId, GLPeriod gLPeriod)
        {
            if (gLPeriod == null)
                return 0;

            var leaveDays = db.LeaveRequest
                .AsNoTracking()
                .Where(x => x.IsOnlineApproved
                         && !x.IsVoid
                         && x.EmployeeId == empId
                         && (x.From <= gLPeriod.DateTo && x.To >= gLPeriod.DateFrom))
                .ToList();

            return leaveDays.Any() ? leaveDays.Sum(x => x.LeaveDays) : 0;
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}