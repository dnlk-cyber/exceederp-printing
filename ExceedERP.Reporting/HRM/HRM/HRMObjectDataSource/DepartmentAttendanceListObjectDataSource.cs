using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class DepartmentAttendanceListObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceList> GetDepartmentAttendanceListReport(string period, string dept, string user, string BossEmpId)
        {

            List<AttendanceList> attendanceList = new List<AttendanceList>();

            if (!string.IsNullOrEmpty(BossEmpId))
            {
                int.TryParse(BossEmpId, out int empId);
                var employeesIds = GetEmployeesUnderBoss(empId);
                var attendances = db.AttendanceNews.Where(x => x.IsApproved && employeesIds.Contains(x.EmployeeId)).ToList();
                if (!string.IsNullOrEmpty(period ))
                {
                    int.TryParse(period, out int GlPeriodId);
                    attendances = attendances.Where(x => x.GlPeriodId == GlPeriodId).ToList();
                }
                //List<AttendanceNew> duplicateList = new List<AttendanceNew>();
                var attendancesDisticts = attendances.DistinctByField(x => x.EmployeeId).ToList();
                foreach (var att in attendancesDisticts)
                {
                    var attendance = new AttendanceList();

                    //var check = duplicateList.Where(x => x.EmployeeId == att.EmployeeId && x.GlPeriodId == att.GlPeriodId);
                    //if (check.Any())
                    //{
                    //    continue;
                    //}                    

                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);
                    if (emp != null)
                    {

                        // attendance.AttendanceDetails = new List<AttendanceDetail>();

                        attendance.EmpID = emp.EmployeeCode;
                        attendance.FullName = emp.FullNameAmharic;
                        var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId).ToList();
                        if (lateCome != null)
                        {
                            attendance.LateDays = lateCome.Sum(x => x.Time) / 8m;

                        }
                        var attendanceOneEmpApproverIds = attendances.Where(x=>x.EmployeeId == att.EmployeeId).Select(x=>x.ApproverEmpId).ToList();
                        var approvers = db.Person_Employee.Where(x => attendanceOneEmpApproverIds.Contains(x.EmployeeId)).ToList();
                       
                        foreach (var app in approvers)
                        {
                            attendance.ApprovedBy += attendance.ApprovedBy + app.FullNameAmharic + ", ";
                        }

                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                        if (placement != null)
                        {
                            var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                            if (depo != null)
                            {
                                attendance.Department = depo.Name;
                            }
                        }
                        var attendanceByMonth = attendances.Where(x => x.GlPeriodId == att.GlPeriodId && x.EmployeeId == att.EmployeeId).ToList();
                        double countMorningAbsent = attendanceByMonth.Where(x => x.MorningAttendance==TakeAttendance.Absent).Count();
                        double countAfternoonAbsent = attendanceByMonth.Where(x => x.MorningAttendance == TakeAttendance.Absent).Count();
                        attendance.NoAbsent = (countMorningAbsent + countAfternoonAbsent) / 2;
                        var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == att.GlPeriodId);
                        if (glPeriod != null)
                        {
                            attendance.Period = glPeriod.Name;
                        }
                        /// here below i will calculate leave for only absent days 
                        ///  if the absent day is not in that leave date range it will not be added
                        double leaveAmount = 0;
                        string leaveType = "";
                        var attendanceForLeave = attendances
                            .Where(x => x.EmployeeId == att.EmployeeId)
                            .ToList();

                        foreach (var atl in attendanceForLeave)
                        {
                            var leave = db.LeaveRequest
                                .FirstOrDefault(x => x.EmployeeId == att.EmployeeId 
                                && x.From <= glPeriod.DateFrom && x.To >= glPeriod.DateTo);

                            if (leave != null)
                            {
                                if (atl.AfternoonAttendance == TakeAttendance.Absent 
                                    && atl.MorningAttendance == TakeAttendance.Absent)
                                {
                                    leaveAmount = leaveAmount + 1;
                                    leaveType = leaveType + leave.Type.ToString() + ", ";
                                }
                                else if (atl.AfternoonAttendance== TakeAttendance.Absent 
                                    || atl.MorningAttendance == TakeAttendance.Absent)
                                {
                                    leaveAmount = leaveAmount + 0.5;
                                    leaveType = leaveType + leave.Type.ToString() + ", ";
                                }
                            }
                        }

                        attendance.LeaveAmount = leaveAmount;
                        attendance.LeaveType = leaveType;
                        //attendance.AttendanceDetails.Add(attDetail);
                        //}

                        //#region For Checking Duplication

                        //var dup = new AttendanceNew();

                        //dup.EmployeeId = att.EmployeeId;
                        //dup.GlPeriodId = att.GlPeriodId;

                        //duplicateList.Add(dup);

                        //#endregion

                        attendanceList.Add(attendance);
                    }
                }

            }
            return attendanceList;
        }
        public List<string> GetUserBranchEmployees(int dept, List<string> branchId = null)
        {
            var placments = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where p.isCurrent && e.IsActive && p.OrgStructureId == dept
                            select (new
                            {
                                EmployeeId = p.EmployeeId,
                                p.BranchId
                            });
            if (branchId != null)
            {
                placments = placments.Where(x => branchId.Contains(x.BranchId.ToString()));
            }

            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }

        public List<int> GetEmployeesUnderBoss(int id)
        {
            List<int> employeesIdList = new List<int>();

            var placements = db.Placements.Where(x => x.HeadId == id && x.isCurrent);
            if (placements.Any())
            {
                HashSet<int> empIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
                employeesIdList.AddRange(empIds);
            }

            return employeesIdList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
