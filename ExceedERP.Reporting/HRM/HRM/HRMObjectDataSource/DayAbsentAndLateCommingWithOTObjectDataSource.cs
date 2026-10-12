using ExceedERP.Core.Domain.Common;
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
    public class DayAbsentAndLateCommingWithOTObjectDataSourceObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceDayAbssentAndLateCommingList> GetDayAbssentAndLateCommingWithOTReport(
                                                string period, string branchId, string dept, string user,
                                                string bizUnitId, string status, string employementType)
        {
            var result = new List<AttendanceDayAbssentAndLateCommingList>();

            if (string.IsNullOrEmpty(period))
                return result;

            int.TryParse(period, out int glPeriodId);
            var periodDate = db.GLPeriods.Find(glPeriodId);
            if (periodDate == null)
                return result;

            // ---------------------------------------------------------
            // 1. BUILD BASE PLACEMENT QUERY (NO .ToList() YET)
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
            var employeeIds = placements.Select(x => x.EmployeeId).ToList();

            // 2. PRELOAD RELATED DATA (ONLY ONCE)
            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId) && x.IsActive)
                .ToDictionary(x => x.EmployeeId);

            var noDaysWorked = db.EmpPayrollNoDaysWorkeds
                .Where(x => employeeIds.Contains(x.EmployeeId) && x.GLPeriodId == glPeriodId)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Days));

            var attendanceAll = db.AttendanceNews
                .Where(x => employeeIds.Contains(x.EmployeeId) && x.GlPeriodId == glPeriodId)
                .ToList();

            var lateComingAll = db.LateCommings
                .Where(x => employeeIds.Contains(x.EmployeeId)
                    && x.Date >= periodDate.DateFrom
                    && x.Date <= periodDate.DateTo)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Time));

            // 1. Build lookup of approver IDs (in memory)
            var approverLookup = attendanceAll
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ApproverEmpId).Distinct().ToList()
                );

            // 2. Flatten all unique approver IDs (LINQ to Objects)
            var approverIds = approverLookup
                .Values
                .SelectMany(a => a)
                .Distinct()
                .ToList();

            // 3. Now query EF using the flattened list
            var approverNames = db.Person_Employee
                .Where(x => approverIds.Contains(x.EmployeeId))
                .ToDictionary(
                    x => x.EmployeeId,
                    x => (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).Trim()
                );
            var OtDatas = db.OverTimes.Where(x => x.GLPeriodId == glPeriodId).ToList();

            var lookUps = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                            || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                            || x.LookupType == LookUpTypes.Jobtitle_External_Experience)
                                            .ToDictionary(x => x.LookupId);

            var applyPeriodDays = PayrollConfHelper.ApplyPeriodDays();
            double totalDays;

            if (applyPeriodDays.Item1)
            {
                decimal days;
                decimal.TryParse(applyPeriodDays.Item2, out days);

                if (days > 20)
                    totalDays = (double)days;
                else
                    totalDays = (periodDate.DateTo - periodDate.DateFrom).Days + 1;
            }
            else
            {
                totalDays = 30;
            }
            // MAIN LOOP (O(1) lookups)
            foreach (var pl in placements)
            {
                if (!employees.ContainsKey(pl.EmployeeId))
                    continue;
                var personOTDatas = OtDatas.Where(x => x.EmployeeId == pl.EmployeeId).ToList();
                var emp = employees[pl.EmployeeId];

                var report = new AttendanceDayAbssentAndLateCommingList
                {
                    EmpID = emp.EmployeeCode,
                    FullName = emp.EmployeeCode+" "+emp.FullNameaEnglish,
                    Period = periodDate.Name
                };

                // Approver Names
                if (approverLookup.ContainsKey(pl.EmployeeId))
                {
                    var names = approverLookup[pl.EmployeeId]
                        .Where(id => approverNames.ContainsKey(id))
                        .Select(id => approverNames[id]);
                    report.ApprovedBy = string.Join(", ", names);
                }

                lookUps.TryGetValue(pl.LookupId, out var lookUp);

                report.JobTitle = lookUp?.Description ?? lookUp?.DescriptionEng;
               
                // Department
                var dep = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                report.Department = dep?.Name;

                // Absent
                double fromPayroll =  (double)(noDaysWorked.ContainsKey(pl.EmployeeId) ? noDaysWorked[pl.EmployeeId] : 0);

                double morningAbsent = attendanceAll
                    .Count(x => x.EmployeeId == pl.EmployeeId &&
                                x.MorningAttendance == TakeAttendance.Absent &&
                                x.IsApproved && x.IsDepartmentApproved) * 0.5;

                double afternoonAbsent = attendanceAll
                    .Count(x => x.EmployeeId == pl.EmployeeId &&
                                x.AfternoonAttendance == TakeAttendance.Absent &&
                                x.IsApproved && x.IsDepartmentApproved) * 0.5;

                report.NoAbsent = fromPayroll + morningAbsent + afternoonAbsent;

                // Late coming
                if (lateComingAll.ContainsKey(pl.EmployeeId))
                    report.LateDays = (decimal)lateComingAll[pl.EmployeeId] / 8;

                // Field days + Leave days
                decimal fieldDays = HrHelper.GetFieldDays(emp.EmployeeId, periodDate.GLPeriodId, null, null);
                decimal leaveDays = HrHelper.GetLeaveDays(emp.EmployeeId, periodDate.GLPeriodId, null, null);

                report.FieldDays = fieldDays >= 0m && fieldDays <= 31m ? fieldDays : 0m;
                report.LeaveDays = leaveDays;
                report.NoAbsentMinute = 0;
                report.NormalOT = personOTDatas.Any()? personOTDatas.Sum(x=>x.NormalHours):0;
                report.NightOT = personOTDatas.Any() ? personOTDatas.Sum(x => x.NightHours) : 0;
                report.RestTimedOT = personOTDatas.Any()? personOTDatas.Sum(x=>x.RestdayHours):0;
                report.HolidayOT = personOTDatas.Any()? personOTDatas.Sum(x=>x.HolidayHours):0;

                //// Working days
                //double totalDays = PayrollConfHelper.ApplyPeriodDays()
                //    ? (periodDate.DateTo - periodDate.DateFrom).Days + 1
                //    : 30;

                double unWorkedDays = emp.HiredDate > periodDate.DateFrom
                    ? (emp.HiredDate - periodDate.DateFrom).TotalDays
                    : 0;

                report.NoWorkingDays = totalDays - (report.NoAbsent + unWorkedDays + (double)report.LateDays);

                result.Add(report);
            }

            return result;
        }

        //public List<AttendanceDayAbssentAndLateCommingList> GetDayAbssentAndLateCommingReport(string period, string branchId, string dept, string user,
        //                                    string bizUnitId,string status, string employementType)
        //{

        //    List<AttendanceDayAbssentAndLateCommingList> attendanceList = new List<AttendanceDayAbssentAndLateCommingList>();

        //    var placements = db.Placements.Where(x => x.isCurrent).ToList();
        //    if (!string.IsNullOrEmpty(employementType))
        //    {
        //        placements = placements.Where(x => x.EmployementType.ToString() == employementType).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(period))
        //    {
        //        int.TryParse(period, out int glPeriodId);
        //        var periodDate = db.GLPeriods.Find(glPeriodId);
        //        if (periodDate == null)
        //        {
        //            return attendanceList;
        //        }
        //        HashSet<int> employeeIds = new HashSet<int>(db.EmpPayrollNoDaysWorkeds.Where(x => x.GLPeriodId == glPeriodId).Select(x => x.EmployeeId));
        //        var empIds = db.Person_Employee.Where(x => x.HiredDate <= periodDate.DateTo).Select(x => x.EmployeeId);
        //        placements = placements.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //        placements = placements.Union(db.Placements.Where(x => x.isCurrent).Where(x => !employeeIds.Contains(x.EmployeeId))).ToList();
        //        placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
        //        if (!string.IsNullOrEmpty(branchId))
        //        {
        //            int.TryParse(branchId, out int braId);
        //            placements = placements.Where(x => x.BranchId == braId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(dept))
        //        {
        //            int.TryParse(dept, out int orgStructureId);
        //            placements = placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(bizUnitId))
        //        {
        //            int.TryParse(bizUnitId, out int businessUnitid);
        //            placements = placements.Where(x => x.BusinessUnitId == businessUnitid).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(status))
        //        {

        //            if (status == "Pend")
        //                placements = placements.Where(x => x.IsPend).ToList();


        //            else if (status == "Unpend")
        //                placements = placements.Where(x => !x.IsPend).ToList();
        //        }

        //        foreach (var att in placements)
        //        {
        //            double unWorkedDays = 0;
        //            double numberOfAbsent = 0;
        //            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.IsActive);
        //            if (emp != null)
        //            {
        //                if (emp.HiredDate.Date > periodDate.DateFrom.Date)
        //                {
        //                    unWorkedDays = (double)(emp.HiredDate - periodDate.DateFrom).TotalDays;
        //                }
        //                var attendance = new AttendanceDayAbssentAndLateCommingList();
        //                attendance.EmpID = emp.EmployeeCode;
        //                attendance.FullName = emp.FullNameaEnglish;
        //                var attendanceOneEmpApproverIds = db.AttendanceNews.Where(x => x.EmployeeId == att.EmployeeId).Select(x => x.ApproverEmpId).ToList();
        //                var approvers = db.Person_Employee.Where(x => attendanceOneEmpApproverIds.Contains(x.EmployeeId)).ToList();
        //                foreach (var app in approvers)
        //                {
        //                    attendance.ApprovedBy += attendance.ApprovedBy + app.FullNameaEnglish + ", ";
        //                }
        //                var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == att.OrgStructureId);
        //                if (depo != null)
        //                {
        //                    attendance.Department = depo.Name;
        //                }
        //                var attendanceMorningAbsentDayss = db.AttendanceNews.Where(x => x.EmployeeId == att.EmployeeId
        //                                                    && x.MorningAttendance == TakeAttendance.Absent
        //                                                    && x.IsApproved
        //                                                    && x.GlPeriodId == glPeriodId
        //                                                    && x.IsDepartmentApproved).ToList();
        //                var attendanceAfternoonAbsentDayss = db.AttendanceNews.Where(x => x.EmployeeId == att.EmployeeId
        //                                                    && x.AfternoonAttendance == TakeAttendance.Absent
        //                                                    && x.GlPeriodId == glPeriodId
        //                                                    && x.IsApproved
        //                                                    && x.IsDepartmentApproved).ToList();
        //                var absentDaysFromAtt = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == att.EmployeeId && x.GLPeriodId == glPeriodId).ToList();
        //                var absentDays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == att.EmployeeId && x.GLPeriodId == glPeriodId).ToList();
        //                numberOfAbsent = absentDays.Count() > 0 ? (double)absentDays.Sum(x => x.Days) : 0
        //                    + attendanceMorningAbsentDayss.Count * 0.5
        //                    + attendanceAfternoonAbsentDayss.Count * 0.5; ;
        //                attendance.NoAbsent = numberOfAbsent;
        //                var lateCome = db.LateCommings.Where(x => x.EmployeeId == att.EmployeeId && x.Date >= periodDate.DateFrom && x.Date <= periodDate.DateTo).ToList();
        //                if (lateCome != null)
        //                {
        //                    attendance.LateDays = ((decimal)lateCome.Sum(x => x.Time)) / 8;
        //                }
        //                attendance.Period = periodDate.Name;
        //                attendance.NoAbsent = numberOfAbsent;
        //                double totalDays = PayrollConfHelper.ApplyPeriodDays() ? (periodDate.DateTo - periodDate.DateFrom).Days + 1 : 30;
        //                decimal fieldDays = HrHelper.GetFieldDays(emp.EmployeeId, periodDate.GLPeriodId,null,null);
        //                decimal leaveDays = HrHelper.GetLeaveDays(emp.EmployeeId, periodDate.GLPeriodId,null,null);
        //                attendance.LeaveDays =  leaveDays;
        //                attendance.FieldDays = (fieldDays >= 0m && fieldDays <= 31m) ? fieldDays : 0m;
        //                //attendance.FieldDays = (fieldDays >= 0m && fieldDays <= 31m) ? fieldDays : 0m;
        //                //attendance.NoWorkingDays = 30.0 - numberOfAbsent - unWorkedDays- (double)attendance.LateDays;
        //                attendance.NoWorkingDays = totalDays - (numberOfAbsent + unWorkedDays + (double)attendance.LateDays);
        //                attendanceList.Add(attendance);
        //            }


        //        }
        //    }
        //    return attendanceList;
        //}

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

        private decimal GetFieldDays(int empId, int glperiodId)
        {
            decimal days = 0;
            var periodObj = db.GLPeriods.Find(glperiodId);
            var fieldDsys = db.ApTravelOrderDepartures
            .Where(d => d.TravelOrder.IsOnlineApproved
                    && !d.TravelOrder.IsVoid &&
                    d.TravelOrder.EmployeeId == empId &&
                   (d.Type == DepartureType.Departure || d.Type == DepartureType.Return) &&
                   d.ArrivalDate >= periodObj.DateFrom &&
                   d.Date <= periodObj.DateTo)
            .GroupBy(d => d.ApTravelOrderId) // Group by travel order to process each travel separately
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
            }).AsNoTracking().ToList();
            if (fieldDsys.Any())
                return fieldDsys.Sum(g => (g.EndDate - g.StartDate).Days + 1);
            return days;
        }
        private decimal GetLeaveDays(int empId, GLPeriod gLPeriod)
        {
            var leaveDays = db.LeaveRequest
                .AsNoTracking()
                .ToList()
                .Where(x => x.IsOnlineApproved
                            && !x.IsVoid
                            && x.EmployeeId == empId
                            && (x.From.Date <= gLPeriod.DateTo.Date
                               && x.To.Date >= gLPeriod.DateFrom.Date)).ToList(); // Includes partial and full overlaps
                
             

            return leaveDays.Any()?leaveDays.Sum(x=>x.LeaveDays):0; // Ensure no null values
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}