using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]

    public class AttendanceListObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceList> GetAttendanceListReport(string period, string branchId, string dept, string user, string bizUnitId,string employeementType)
        {
            var attendancesQuery = db.AttendanceNews.Where(x => x.IsApproved);

            if (int.TryParse(period, out int glPeriodId))
            {
                attendancesQuery = attendancesQuery.Where(x => x.GlPeriodId == glPeriodId);
            }

            if (int.TryParse(branchId, out int braId))
            {
                var empIds = db.Placements.Where(x => x.BranchId == braId).Select(e => e.EmployeeId).Distinct();

                attendancesQuery = attendancesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (int.TryParse(dept, out int orgStructureId))
            {
                attendancesQuery = attendancesQuery.Where(x => x.OrgStructureId == orgStructureId);
            }
            if (!string.IsNullOrEmpty(employeementType))
            {
                var empIds = db.Placements.Where(x => x.EmployementType.ToString() == employeementType).Select(e=>e.EmployeeId).Distinct();
                attendancesQuery = attendancesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            var attendances = attendancesQuery.ToList();

            // Group by EmployeeId and GlPeriodId to avoid duplication
            var groupedAttendances = attendances
                .GroupBy(x => new { x.EmployeeId, x.GlPeriodId })
                .ToList();

            var employeeIds = groupedAttendances.Select(g => g.Key.EmployeeId).Distinct().ToList();
            var employeeInfo = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var approverIds = attendances.Select(x => x.ApproverEmpId).Union(attendances.Select(x => x.DepartmentApproverId)).Distinct().ToList();
            var approvers = db.Person_Employee.Where(x => approverIds.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var placements = db.Placements.Where(x => employeeIds.Contains(x.EmployeeId) && x.isCurrent).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var glPeriods = db.GLPeriods.ToDictionary(x => x.GLPeriodId);
            var leaveRequests = db.LeaveRequest
                                .Where(x => employeeIds.Contains(x.EmployeeId) && x.IsOnlineApproved && x.IsVoid)
                                .ToList();
            var lateCommings = db.LateCommings
                                .Where(x => employeeIds.Contains(x.EmployeeId))
                                .GroupBy(x => x.EmployeeId)
                                .ToDictionary(g => g.Key, g => g.Sum(x => x.Time));

            var attendanceList = new List<AttendanceList>();

            foreach (var group in groupedAttendances)
            {
                var empId = group.Key.EmployeeId;
                var periodId = group.Key.GlPeriodId;

                if (!employeeInfo.TryGetValue(empId, out var emp)) continue;

                var attendance = new AttendanceList
                {
                    EmpID = emp.EmployeeCode,
                    FullName = emp.FullNameaEnglish
                };

                if (lateCommings.TryGetValue(empId, out var lateTime))
                {
                    attendance.LateDays = lateTime / 8m;
                }

                var approverNames = group
                    .Select(x => x.ApproverEmpId)
                    .Union(group.Select(x => x.DepartmentApproverId))
                    .Distinct()
                    .Where(id => approvers.ContainsKey(id))
                    .Select(id => approvers[id].FullNameaEnglish);

                attendance.ApprovedBy = string.Join("\n", approverNames);

                if (placements.TryGetValue(empId, out var placement))
                {
                    if (departments.TryGetValue(placement.OrgStructureId, out var depo))
                    {
                        attendance.Department = depo.Name;
                    }
                }

                var recordsForPeriod = group.ToList();
                double countMorningAbsent = recordsForPeriod.Count(x => !x.IsPresentMorning);
                double countAfternoonAbsent = recordsForPeriod.Count(x => !x.IsPresentAfternoon);
                attendance.NoAbsent = (countMorningAbsent + countAfternoonAbsent) / 2;

                attendance.FieldDays = HrHelper.GetFieldDays(empId, periodId,null,null);
                if (attendance.FieldDays < 0) attendance.FieldDays = 0;

                if (glPeriods.TryGetValue(periodId, out var glPeriod))
                {
                    attendance.Period = glPeriod.Name;

                }

                // Leave type
                var leaveDates = recordsForPeriod
                    .Where(x => !x.IsPresentMorning || !x.IsPresentAfternoon)
                    .Select(x => x.Date)
                    .Distinct();

                var leaveTypes = leaveRequests
                    .Where(x => x.EmployeeId == empId
                    && leaveDates.Any(date => x.From <= date && x.To >= date))
                    .Select(x => x.Type.ToString())
                    .Distinct();

                attendance.LeaveType = string.Join(", ", leaveTypes);
                attendance.LeaveAmount =(double) HrHelper.GetLeaveDays(empId, periodId,null,null);

                attendanceList.Add(attendance);
            }

            return attendanceList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
        //public List<AttendanceList> GetAttendanceListReport(string period, string branchId, string dept, string user, string bizUnitId)
        //{

        //    List<AttendanceList> attendanceList = new List<AttendanceList>();

        //    var attendances = db.AttendanceNews.Where(x => x.IsApproved).ToList();

        //    if (!string.IsNullOrEmpty(period))
        //    {
        //        int.TryParse(period, out int GlPeriodId);
        //        attendances = attendances.Where(x => x.GlPeriodId == GlPeriodId).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(branchId))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        var empIds = db.Placements.Where(x => x.isCurrent && x.BranchId == braId).Select(x => x.EmployeeId).ToList();
        //        attendances = attendances.Where(x => empIds.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(dept))
        //    {
        //        int.TryParse(dept, out int orgStructureId);
        //        //var empIds = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == orgStructureId).Select(x => x.EmployeeId).ToList();
        //        attendances = attendances.Where(x=>x.OrgStructureId==orgStructureId)/*.Where(x => empIds.Contains(x.EmployeeId))*/.ToList();
        //    }

        //    List<AttendanceNew> duplicateList = new List<AttendanceNew>();
        //    foreach (var att in attendances)
        //    {
        //        var check = duplicateList.Where(x => x.EmployeeId == att.EmployeeId && x.GlPeriodId == att.GlPeriodId);
        //        if (check.Any())
        //        {
        //            continue;
        //        }

        //        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);
        //        if (emp != null)
        //        {
        //            var attendance = new AttendanceList();

        //            // attendance.AttendanceDetails = new List<AttendanceDetail>();

        //            attendance.EmpID = emp.EmployeeCode;
        //            attendance.FullName = emp.FullNameAmharic;
        //            var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId).ToList();
        //            if(lateCome != null)
        //            {
        //                attendance.LateDays = lateCome.Sum(x => x.Time)/8;

        //            }

        //            var attendanceOneEmpApproverIds = attendances.Where(x => x.EmployeeId == att.EmployeeId
        //                                                                    && x.AttendanceNewId == att.AttendanceNewId)
        //                                                                    .Select(x => x.ApproverEmpId).Distinct().ToList();
        //            var attendanceOneEmpDeApproverIds = attendances.Where(x => x.EmployeeId == att.EmployeeId
        //                                                                        &&x.AttendanceNewId==att.AttendanceNewId)
        //                                                                        .Select(x => x.DepartmentApproverId).Distinct().ToList();
        //            attendanceOneEmpApproverIds.AddRange(attendanceOneEmpDeApproverIds);
        //            var approvers = db.Person_Employee.Where(x => attendanceOneEmpApproverIds.Contains(x.EmployeeId)).ToList();

        //            foreach (var app in approvers)
        //            {
        //                attendance.ApprovedBy += $"{ app.FullNameAmharic}  \n";
        //            }

        //            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
        //            if (placement != null)
        //            {
        //                var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
        //                if (depo != null)
        //                {
        //                    attendance.Department = depo.Name;
        //                }
        //            }

        //            var attendanceByMonth = attendances.Where(x => x.GlPeriodId == att.GlPeriodId && x.EmployeeId == att.EmployeeId).ToList();

        //            //foreach (var atm in attendanceByMonth)
        //            //{
        //            //    var attDetail = new AttendanceDetail();

        //            double countMorningAbsent = attendanceByMonth.Where(x => !x.IsPresentMorning).Count();
        //            double countAfternoonAbsent = attendanceByMonth.Where(x => !x.IsPresentAfternoon).Count();
        //            attendance.FieldDays = HrHelper.GetFieldDays(emp.EmployeeId, att.GlPeriodId);
        //           if(attendance.FieldDays < 0)
        //            {
        //                attendance.FieldDays = 0;
        //            }
        //            attendance.NoAbsent = (countMorningAbsent + countAfternoonAbsent) / 2;

        //            var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == att.GlPeriodId);
        //            if (glPeriod != null)
        //            {
        //                attendance.Period = glPeriod.Name;
        //            }

        //            // attendance.AttendanceDetails.Add(attDetail);
        //            // }

        //            /// here below i will calculate leave for only absent days 
        //            ///  if the absent day is not in that leave date range it will not be added
        //            //double leaveAmount = 0;
        //            string leaveType = "";

        //            var attendanceForLeave = attendances.Where(x => x.EmployeeId == att.EmployeeId).Where(x => !x.IsPresentMorning || !x.IsPresentAfternoon).ToList();

        //            foreach (var atl in attendanceForLeave)
        //            {
        //                var leave = db.LeaveRequest
        //                              .FirstOrDefault(x => x.EmployeeId == att.EmployeeId 
        //                                                   && x.From <= atl.Date 
        //                                                   && x.To >= atl.Date
        //                                                   &&x.IsOnlineApproved 
        //                                                   &&x.IsVoid);

        //                if (leave != null)
        //                {
        //                    //if (!atl.IsPresentMorning && !atl.IsPresentAfternoon)
        //                    //{

        //                    leaveType = leaveType + leave.Type.ToString() + ", ";
        //                    //}
        //                    //else if (!atl.IsPresentMorning || !atl.IsPresentAfternoon)
        //                    //{
        //                    //    leaveAmount = leaveAmount + 0.5;
        //                    //    leaveType = leaveType + leave.Type.ToString() + ", ";
        //                    //}
        //                }
        //            }

        //            attendance.LeaveAmount = (double)HrHelper.GetLeaveDays(att.EmployeeId,att.GlPeriodId);
        //            attendance.LeaveType = leaveType;

        //            #region For Checking Duplication

        //            var dup = new AttendanceNew();
        //            dup.EmployeeId = att.EmployeeId;
        //            dup.GlPeriodId = att.GlPeriodId;

        //            duplicateList.Add(dup);
        //            #endregion

        //            attendanceList.Add(attendance);
        //        }

        //    }
        //    return attendanceList;
        //}

        //public List<string> GetUserBranchEmployees(int dept, List<string> branchId = null)
        //{
        //    var placments = from p in db.Placements
        //                    join
        //                        e in db.Person_Employee
        //                        on p.EmployeeId equals e.EmployeeId
        //                    where p.isCurrent && e.IsActive
        //                    select (new
        //                    {
        //                        EmployeeId = p.EmployeeId,
        //                        p.BranchId,
        //                        p.OrgStructureId
        //                    });
        //    if (branchId != null)
        //    {
        //        placments = placments.Where(x => branchId.Contains(x.BranchId.ToString()));
        //    }
        //    if (dept != 0)
        //    {

        //        placments = placments.Where(x => x.OrgStructureId == dept);
        //    }
        //    if (placments.Any())
        //    {
        //        var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
        //        return listOfEmployees;
        //    }
        //    return new List<string>();
        //}
        //private decimal GetFieldDays(int empId, int glperiodId)
        //{
        //    decimal days = 0;
        //    var periodObj = db.GLPeriods.Find(glperiodId);
        //    var fieldDsys = db.ApTravelOrderDepartures
        //    .Where(d => d.TravelOrder.IsOnlineApproved
        //            && !d.TravelOrder.IsVoid &&
        //            d.TravelOrder.EmployeeId == empId &&
        //           (d.Type == DepartureType.Departure || d.Type == DepartureType.Return) &&
        //           d.ArrivalDate >= periodObj.DateFrom &&
        //           d.Date <= periodObj.DateTo)
        //    .GroupBy(d => d.ApTravelOrderId) // Group by travel order to process each travel separately
        //    .Select(g => new
        //    {
        //        StartDate = g.Where(d => d.Type == DepartureType.Departure)
        //                     .OrderBy(d => d.Date)
        //                     .Select(d => d.Date)
        //                     .FirstOrDefault(),
        //        EndDate = g.Where(d => d.Type == DepartureType.Return)
        //                   .OrderByDescending(d => d.ArrivalDate)
        //                   .Select(d => d.ArrivalDate)
        //                   .FirstOrDefault()
        //    }).AsNoTracking().ToList();
        //    if (fieldDsys.Any())
        //        return fieldDsys.Sum(g => (g.EndDate - g.StartDate).Days + 1);
        //    return days;
        //}

    }
}