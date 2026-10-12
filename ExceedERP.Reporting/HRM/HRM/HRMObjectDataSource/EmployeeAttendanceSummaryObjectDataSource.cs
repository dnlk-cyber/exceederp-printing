using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeAttendanceSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeAttendanceSummaryVM> GetEmployeeAttendanceSummary(string employee, string branch, DateTime? from, DateTime? to, string attendanceStatus)
        {
            int.TryParse(employee, out int employeeId);
            int.TryParse(branch, out int branchId);
            var result = new List<EmployeeAttendanceSummaryVM>();

            //var query = db.EmployeeAttendanceSummaries.AsNoTracking().AsQueryable();

            //// Align placement query with AttendanceMonthlySummary to exclude attendance-exempt employees and placements
            //var placementQuery = db.Placements
            //    .Where(x => x.isCurrent && !x.IsAttendanceExempt && !x.Employee.IsAttendanceExempt && x.Employee.IsActive)
            //    .AsNoTracking().AsQueryable();

            //var leaveRequestsQuery = db.LeaveRequest
            //    .Where(x => x.IsOnlineApproved && !x.IsVoid)
            //    .AsNoTracking().AsQueryable();

            //if (employeeId > 0)
            //{
            //    query = query.Where(x => x.EmployeeId == employeeId);
            //    placementQuery = placementQuery.Where(x => x.EmployeeId == employeeId);
            //    leaveRequestsQuery = leaveRequestsQuery.Where(x => x.EmployeeId == employeeId);
            //}

            //if (branchId > 0)
            //{
            //    query = query.Where(x => x.BranchId == branchId);
            //    placementQuery = placementQuery.Where(x => x.BranchId == branchId);
            //}

            //if (from.HasValue)
            //{
            //    query = query.Where(x => x.Date >= from.Value);
            //    leaveRequestsQuery = leaveRequestsQuery.Where(x => x.To >= from.Value);
            //}

            //if (to.HasValue)
            //{
            //    query = query.Where(x => x.Date <= to.Value);
            //    leaveRequestsQuery = leaveRequestsQuery.Where(x => x.From <= to.Value);
            //}

            //var attendanceData = query.ToList();
            //var placements = placementQuery.ToList();
            //var leaveRequests = leaveRequestsQuery.ToList();

            //if (!placements.Any())
            //    return new List<EmployeeAttendanceSummaryVM>();

            //var employees = db.Person_Employee.ToDictionary(e => e.EmployeeId);
            //var mainCompanyStructures = db.MainCompanyStructures.ToDictionary(e => e.OrgStructureId);
            //var branches = db.Branch.ToDictionary(e => e.BranchId);

            //var attendanceLookup = attendanceData.ToLookup(x => new
            //{
            //    x.EmployeeId,
            //    Date = x.Date.Date
            //});

            //DateTime startDate = from?.Date ?? DateTime.Today;
            //DateTime endDate = to?.Date ?? startDate;

            //var dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
            //    .Select(i => startDate.AddDays(i))
            //    .ToList();


            //foreach (var placement in placements)
            //{
            //    employees.TryGetValue(placement.EmployeeId, out var emp);
            //    branches.TryGetValue(placement.BranchId, out var br);
            //    mainCompanyStructures.TryGetValue(placement.OrgStructureId, out var org);

            //    var empLeaves = leaveRequests.Where(l => l.EmployeeId == placement.EmployeeId).ToList();

            //    foreach (var date in dates)
            //    {
            //        if (date.DayOfWeek == DayOfWeek.Sunday)
            //            continue;

            //        var attendance = attendanceLookup[new
            //        {
            //            placement.EmployeeId,
            //            Date = date
            //        }].FirstOrDefault();

            //        var activeLeave = empLeaves.FirstOrDefault(l => date.Date >= l.From.Date && date.Date <= l.To.Date);

            //        if (attendance == null)
            //        {
            //            decimal leaveVal = activeLeave != null ? 1 : 0;
            //            decimal absentVal = activeLeave != null ? 0 : 1;

            //            result.Add(new EmployeeAttendanceSummaryVM
            //            {
            //                EmpId = emp?.EmployeeCode,
            //                EmpName = emp != null
            //                    ? $"{emp.EmployeeCode} - {emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}"
            //                    : "",
            //                Department = org?.Name,
            //                ProjectOrBranch = br?.Name,
            //                PunchDate = date,
            //                Day = date.DayOfWeek.ToString(),
            //                Leave = leaveVal,
            //                Absent = absentVal,
            //                TimeIn = null,
            //                TimeOut = null,
            //                LateTime = 0,
            //                TotalWorkingHrs = 0,
            //                OTHours = 0
            //            });

            //            continue;
            //        }

            //        result.Add(new EmployeeAttendanceSummaryVM
            //        {
            //            EmpId = attendance.EmployeeCode,
            //            EmpName = emp != null
            //                ? $"{emp.EmployeeCode} - {emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}"
            //                : "",
            //            Department = org?.Name,
            //            ProjectOrBranch = br?.Name,
            //            PunchDate = attendance.Date,
            //            Day = attendance.Date.DayOfWeek.ToString(),
            //            Leave = attendance.Leave > 0 ? attendance.Leave : (activeLeave != null ? 1 : 0),
            //            Absent = attendance.Absent,
            //            TimeIn = attendance.CheckInTime,
            //            TimeOut = attendance.CheckOutTime,
            //            LateTime = attendance.LateComing,
            //            TotalWorkingHrs = attendance.TotalHrs,
            //            OTHours = attendance.OvertimeHours
            //        });
            //    }
            //}

            //if (!string.IsNullOrWhiteSpace(attendanceStatus))
            //{
            //    switch (attendanceStatus)
            //    {
            //        case "Absent":
            //            result = result.Where(x => x.Absent > 0).ToList();
            //            break;
            //        case "Late":
            //            result = result.Where(x => x.LateTime > 0).ToList();
            //            break;
            //        case "Leave":
            //            result = result.Where(x => x.Leave > 0).ToList();
            //            break;
            //        case "OT":
            //            result = result.Where(x => x.OTHours > 0).ToList();
            //            break;
            //    }
            //}

            return result;
        }
        //public List<EmployeeAttendanceSummaryVM> GetEmployeeAttendanceSummary(string employee, string branch, DateTime? from, DateTime? to, string attendanceStatus)
        //{
        //    int.TryParse(employee, out int employeeId);
        //    int.TryParse(branch, out int branchId);

        //    var query = db.EmployeeAttendanceSummaries.AsNoTracking().AsQueryable();
        //    var placementQuery = db.Placements.Where(x => x.isCurrent).AsNoTracking().AsQueryable();
        //    var leaveRequestsQuery = db.LeaveRequest.Where(x => x.IsOnlineApproved && !x.IsVoid).AsNoTracking().AsQueryable();

        //    if (employeeId > 0)
        //    {
        //        query = query.Where(x => x.EmployeeId == employeeId);
        //        placementQuery = placementQuery.Where(x => x.EmployeeId == employeeId);
        //        leaveRequestsQuery = leaveRequestsQuery.Where(x => x.EmployeeId == employeeId);
        //    }

        //    if (branchId > 0)
        //    {
        //        query = query.Where(x => x.BranchId == branchId);
        //        placementQuery = placementQuery.Where(x => x.BranchId == branchId);
        //    }

        //    if (from.HasValue)
        //    {
        //        query = query.Where(x => x.Date >= from.Value);
        //        leaveRequestsQuery = leaveRequestsQuery.Where(x => x.To >= from.Value);
        //    }

        //    if (to.HasValue)
        //    {
        //        query = query.Where(x => x.Date <= to.Value);
        //        leaveRequestsQuery = leaveRequestsQuery.Where(x => x.From <= to.Value);
        //    }

        //    var attendanceData = query.ToList();
        //    var placements = placementQuery.ToList();
        //    var leaveRequests = leaveRequestsQuery.ToList();

        //    if (!placements.Any())
        //        return new List<EmployeeAttendanceSummaryVM>();

        //    var employees = db.Person_Employee.ToDictionary(e => e.EmployeeId);
        //    var mainCompanyStructures = db.MainCompanyStructures.ToDictionary(e => e.OrgStructureId);
        //    var branches = db.Branch.ToDictionary(e => e.BranchId);

        //    var attendanceLookup = attendanceData.ToLookup(x => new
        //    {
        //        x.EmployeeId,
        //        Date = x.Date.Date
        //    });

        //    DateTime startDate = from?.Date ?? DateTime.Today;
        //    DateTime endDate = to?.Date ?? startDate;

        //    var dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
        //        .Select(i => startDate.AddDays(i))
        //        .ToList();

        //    var result = new List<EmployeeAttendanceSummaryVM>();

        //    foreach (var placement in placements)
        //    {
        //        employees.TryGetValue(placement.EmployeeId, out var emp);
        //        branches.TryGetValue(placement.BranchId, out var br);
        //        mainCompanyStructures.TryGetValue(placement.OrgStructureId, out var org);

        //        // Filter leave requests specific to this employee
        //        var empLeaves = leaveRequests.Where(l => l.EmployeeId == placement.EmployeeId).ToList();

        //        foreach (var date in dates)
        //        {
        //            if (date.DayOfWeek == DayOfWeek.Sunday)
        //                continue;

        //            var attendance = attendanceLookup[new
        //            {
        //                placement.EmployeeId,
        //                Date = date
        //            }].FirstOrDefault();

        //            var activeLeave = empLeaves.FirstOrDefault(l => date.Date >= l.From.Date && date.Date <= l.To.Date);

        //            if (attendance == null)
        //            {
        //                // If there is an approved leave, mark as Leave instead of Absent
        //                decimal leaveVal = activeLeave != null ? 1 : 0;
        //                decimal absentVal = activeLeave != null ? 0 : 1;

        //                result.Add(new EmployeeAttendanceSummaryVM
        //                {
        //                    EmpId = emp?.EmployeeCode,
        //                    EmpName = emp != null
        //                        ? $"{emp.EmployeeCode} - {emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}"
        //                        : "",
        //                    Department = org?.Name,
        //                    ProjectOrBranch = br?.Name,
        //                    PunchDate = date,
        //                    Day = date.DayOfWeek.ToString(),
        //                    Leave = leaveVal,
        //                    Absent = absentVal,
        //                    TimeIn = null,
        //                    TimeOut = null,
        //                    LateTime = 0,
        //                    TotalWorkingHrs = 0,
        //                    OTHours = 0
        //                });

        //                continue;
        //            }

        //            result.Add(new EmployeeAttendanceSummaryVM
        //            {
        //                EmpId = attendance.EmployeeCode,
        //                EmpName = emp != null
        //                    ? $"{emp.EmployeeCode} - {emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}"
        //                    : "",
        //                Department = org?.Name,
        //                ProjectOrBranch = br?.Name,
        //                PunchDate = attendance.Date,
        //                Day = attendance.Date.DayOfWeek.ToString(),
        //                Leave = attendance.Leave > 0 ? attendance.Leave : (activeLeave != null ? 1 : 0),
        //                Absent = attendance.Absent,
        //                TimeIn = attendance.CheckInTime,
        //                TimeOut = attendance.CheckOutTime,
        //                LateTime = attendance.LateComing,
        //                TotalWorkingHrs = attendance.TotalHrs,
        //                OTHours = attendance.OvertimeHours
        //            });
        //        }
        //    }

        //    if (!string.IsNullOrWhiteSpace(attendanceStatus))
        //    {
        //        switch (attendanceStatus)
        //        {
        //            case "Absent":
        //                result = result.Where(x => x.Absent > 0).ToList();
        //                break;
        //            case "Late":
        //                result = result.Where(x => x.LateTime > 0).ToList();
        //                break;
        //            case "Leave":
        //                result = result.Where(x => x.Leave > 0).ToList();
        //                break;
        //            case "OT":
        //                result = result.Where(x => x.OTHours > 0).ToList();
        //                break;
        //        }
        //    }

        //    return result;
        //}


        public void Dispose()
        {
            db.Dispose();
        }
    }
}