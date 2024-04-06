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

    public class AttendanceDayAbsentAndLateCommingListObjectDataSource : List<AttendanceDayAbssentAndLateCommingList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceDayAbssentAndLateCommingList> GetDayAbssentAndLateCommingReport(string period, string branchId, string dept, string user, string bizUnitId)
        {

            List<AttendanceDayAbssentAndLateCommingList> attendanceList = new List<AttendanceDayAbssentAndLateCommingList>();

            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            
            if (period != null)
            {
                int.TryParse(period, out int glPeriodId);
                var periodDate = db.GLPeriods.Find(glPeriodId);
                if (periodDate != null)
                {


                    var employeeIds = db.EmpPayrollNoDaysWorkeds.Where(x => x.GLPeriodId == glPeriodId).Select(x => x.EmployeeId).Distinct();
                    var empIds = db.Person_Employee.Where(x => x.HiredDate <= periodDate.DateTo).Select(x => x.EmployeeId);
                    placements = placements.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
                    placements = placements.Union(db.Placements.Where(x => x.isCurrent).Where(x => !employeeIds.Contains(x.EmployeeId))).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                }
            }
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = placements.Where(x => x.BranchId == braId).ToList();
            
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgStructureId);
                placements = placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
            }

            foreach (var att in placements)
            {
               
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.IsActive);

                if (emp != null)
                {
                    var attendance = new AttendanceDayAbssentAndLateCommingList();
                    attendance.EmpID = emp.EmployeeCode;
                    attendance.FullName = emp.FullNameaEnglish;
                    var attendanceOneEmpApproverIds = db.AttendanceNews.Where(x => x.EmployeeId == att.EmployeeId).Select(x => x.ApproverEmpId).ToList();
                    var approvers = db.Person_Employee.Where(x => attendanceOneEmpApproverIds.Contains(x.EmployeeId)).ToList();
                    foreach (var app in approvers)
                    {
                        attendance.ApprovedBy = attendance.ApprovedBy + app.FullNameaEnglish + ", ";
                    }                 
                    var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == att.OrgStructureId);
                    if (depo != null)
                    {
                        attendance.Department = depo.Name;
                    }                   
                    double numberOfAbsent = 0;
                    if (period != null)
                    {
                        int.TryParse(period, out int glPeriodId);
                        var absentDays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == att.EmployeeId&&x.GLPeriodId ==glPeriodId).ToList();
                        numberOfAbsent = absentDays.Count() > 0 ? (double)absentDays.Sum(x => x.Days) : 0;
                        attendance.NoAbsent = numberOfAbsent;
                        var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == glPeriodId);
                        if (glPeriod != null)
                        {
                            var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId&&x.Date>= glPeriod.DateFrom&&x.Date<= glPeriod.DateTo).ToList();
                            if (lateCome != null)
                            {
                                attendance.LateDays = lateCome.Sum(x => x.Time) / 8;

                            }
                          

                            if (glPeriod != null)
                            {
                                attendance.Period = glPeriod.Name;
                            }
                        }
                      

                        attendance.NoAbsent =  numberOfAbsent;

                        attendance.NoWorkingDays = 30.0 - numberOfAbsent;


                    }
                    else
                    {
                        var absentDays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == att.EmployeeId).ToList();
                        numberOfAbsent = absentDays.Count()>0?(double)absentDays.Sum(x=>x.Days):0;
                        var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId).ToList();
                        if (lateCome != null)
                        {
                            attendance.LateDays = lateCome.Sum(x => x.Time) / 8;

                        }

                       
                        attendance.NoAbsent = numberOfAbsent;
                        if (period != null)
                        {
                            int.TryParse(period, out int periodId);
                            var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == periodId);
                            if (glPeriod != null)
                            {
                                attendance.Period = glPeriod.Name;
                            }

                        }
                        attendance.NoWorkingDays = 30.0 - numberOfAbsent;

                    }
                    
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