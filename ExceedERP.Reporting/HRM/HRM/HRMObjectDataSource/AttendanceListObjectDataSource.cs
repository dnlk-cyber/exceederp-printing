using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]

    public class AttendanceListObjectDataSource : List<AttendanceList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceList> GetAttendanceListReport(string period, string branchId, string dept, string user, string bizUnitId)
        {

            List<AttendanceList> attendanceList = new List<AttendanceList>();

            var attendances = db.AttendanceNews.Where(x => x.IsApproved).ToList();

            if (period != null)
            {
                int.TryParse(period, out int GlPeriodId);
                attendances = attendances.Where(x => x.GlPeriodId == GlPeriodId).ToList();
            }
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                var empIds = db.Placements.Where(x => x.isCurrent && x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                attendances = attendances.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgStructureId);
                var empIds = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == orgStructureId).Select(x => x.EmployeeId).ToList();
                attendances = attendances.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }

            List<AttendanceNew> duplicateList = new List<AttendanceNew>();
            foreach (var att in attendances)
            {
                var check = duplicateList.Where(x => x.EmployeeId == att.EmployeeId && x.GlPeriodId == att.GlPeriodId);
                if (check.Any())
                {
                    continue;
                }

                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);
                if (emp != null)
                {
                    var attendance = new AttendanceList();

                    // attendance.AttendanceDetails = new List<AttendanceDetail>();

                    attendance.EmpID = emp.EmployeeCode;
                    attendance.FullName = emp.FullNameAmharic;
                    var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId).ToList();
                    if(lateCome != null)
                    {
                        attendance.LateDays = lateCome.Sum(x => x.Time)/8;
                        
                    }
                   
                    

                    var attendanceOneEmpApproverIds = attendances.Where(x => x.EmployeeId == att.EmployeeId).Select(x => x.ApproverEmpId).ToList();
                    var approvers = db.Person_Employee.Where(x => attendanceOneEmpApproverIds.Contains(x.EmployeeId)).ToList();

                    foreach (var app in approvers)
                    {
                        attendance.ApprovedBy = attendance.ApprovedBy + app.FullNameAmharic + ", ";
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

                    //foreach (var atm in attendanceByMonth)
                    //{
                    //    var attDetail = new AttendanceDetail();

                    double countMorningAbsent = attendanceByMonth.Where(x => !x.IsPresentMorning).Count();
                    double countAfternoonAbsent = attendanceByMonth.Where(x => !x.IsPresentAfternoon).Count();

                    attendance.NoAbsent = (countMorningAbsent + countAfternoonAbsent) / 2;

                    var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == att.GlPeriodId);
                    if (glPeriod != null)
                    {
                        attendance.Period = glPeriod.Name;
                    }

                    // attendance.AttendanceDetails.Add(attDetail);
                    // }

                    /// here below i will calculate leave for only absent days 
                    ///  if the absent day is not in that leave date range it will not be added
                    double leaveAmount = 0;
                    string leaveType = "";

                    var attendanceForLeave = attendances.Where(x => x.EmployeeId == att.EmployeeId).Where(x => !x.IsPresentMorning || !x.IsPresentAfternoon).ToList();

                    foreach (var atl in attendanceForLeave)
                    {
                        var leave = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= atl.Date && x.To >= atl.Date);

                        if (leave != null)
                        {
                            if (!atl.IsPresentMorning && !atl.IsPresentAfternoon)
                            {
                                leaveAmount = leaveAmount + 1;
                                leaveType = leaveType + leave.LeaveType.ToString() + ", ";
                            }
                            else if (!atl.IsPresentMorning || !atl.IsPresentAfternoon)
                            {
                                leaveAmount = leaveAmount + 0.5;
                                leaveType = leaveType + leave.LeaveType.ToString() + ", ";
                            }
                        }
                    }

                    attendance.LeaveAmount = Convert.ToDouble(leaveAmount);
                    attendance.LeaveType = leaveType;

                    #region For Checking Duplication

                    var dup = new AttendanceNew();
                    dup.EmployeeId = att.EmployeeId;
                    dup.GlPeriodId = att.GlPeriodId;

                    duplicateList.Add(dup);

                    #endregion

                    attendanceList.Add(attendance);
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
                            where p.isCurrent && e.IsActive
                            select (new
                            {
                                EmployeeId = p.EmployeeId,
                                p.BranchId,
                                p.OrgStructureId
                            });
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
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }

    }
}