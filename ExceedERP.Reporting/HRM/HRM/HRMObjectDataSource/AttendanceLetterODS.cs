using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class AttendanceLetterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceLeterVM> GetAttendance(DateTime from, DateTime to, string id)
        {
            List<AttendanceLeterVM> attendanceList = new List<AttendanceLeterVM>();

            if (id != null)
            {
                int.TryParse(id, out int empId);

                var employees = GetEmployeesUnderBoss(empId);

                var attendances = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => x.Date >= from && x.Date <= to).OrderBy(x => x.Date).ToList();

                int index = 0;

                foreach (var att in attendances)
                {
                    var attendanceMorning = new AttendanceLeterVM();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);

                    if (person != null)
                    {
                        attendanceMorning.Name = person.FullNameAmharic;

                        attendanceMorning.Date = att.Date;
                        attendanceMorning.Day = GetDaysAmharic(att.Date.DayOfWeek.ToString());
                        attendanceMorning.Shift = "ጠዋት";
                        attendanceMorning.Index = index;

                        if (att.IsPresentMorning)
                        {
                            attendanceMorning.Value = "P";
                        }
                        else
                        {
                            attendanceMorning.Value = "A";
                        }

                        var leaveTakenM = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.Date && x.To >= att.Date);

                        if (leaveTakenM != null)
                        {
                            if (leaveTakenM.Day == LeaveDay.Half_Day && leaveTakenM.MorningOrAfternoon == MorningOrAfternoon.Morning)
                            {
                                attendanceMorning.Value = GetLeaveInAbbrvation(leaveTakenM.LeaveType);
                            }
                            else
                            {
                                attendanceMorning.Value = GetLeaveInAbbrvation(leaveTakenM.LeaveType);
                            }
                        }

                        attendanceList.Add(attendanceMorning);

                        index++;

                        var attendanceAfternoon = new AttendanceLeterVM();

                        attendanceAfternoon.Name = person.FullNameAmharic;
                        attendanceAfternoon.Date = att.Date;
                        attendanceAfternoon.Day = GetDaysAmharic(att.Date.DayOfWeek.ToString());
                        attendanceAfternoon.Shift = "ከሰአት";
                        attendanceMorning.Index = index;

                        if (att.IsPresentAfternoon)
                        {
                            attendanceAfternoon.Value = "P";
                        }
                        else
                        {
                            attendanceAfternoon.Value = "A";
                        }

                        var leaveTakenA = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.Date && x.To >= att.Date);

                        if (leaveTakenA != null)
                        {
                            if (leaveTakenA.Day == LeaveDay.Half_Day && leaveTakenA.MorningOrAfternoon == MorningOrAfternoon.Morning)
                            {
                                attendanceAfternoon.Value = GetLeaveInAbbrvation(leaveTakenA.LeaveType);
                            }
                            else
                            {
                                attendanceAfternoon.Value = GetLeaveInAbbrvation(leaveTakenA.LeaveType);
                            }
                        }

                        attendanceList.Add(attendanceAfternoon);

                        index++;
                    }
                }
            }

            return attendanceList;
        }

        public string GetDaysAmharic(string day)
        {
            switch (day.ToLower())
            {
                case "monday":
                    return "ሰኞ";
                case "tuesday":
                    return "ማክሰኞ";
                case "wednesday":
                    return "ረቡዕ";
                case "thursday":
                    return "ሐሙስ";
                case "friday":
                    return "አርብ";
                case "saturday":
                    return "ቅዳሜ";
                case "sunday":
                    return "እሁድ";
            }

            return null;
        }

        public string GetLeaveInAbbrvation(LeaveType leaveType)
        {
            string type = "";
            switch (leaveType)
            {
                case LeaveType.ANNUAL:
                    type = "A/L";
                    break;
                case LeaveType.SICK_LEAVE:
                    type = "S/L";
                    break;
                case LeaveType.MARRIAGE:
                    type = "MR/L";
                    break;
                case LeaveType.MATERNITY:
                    type = "MT/L";
                    break;
                case LeaveType.PATERNITY:
                    type = "P/L";
                    break;
                case LeaveType.Leave_With_Out_Pay:
                    type = "LWP/L";
                    break;
                case LeaveType.Bereavement:
                    type = "B/L";
                    break;
                case LeaveType.OTHER:
                    type = "O/L";
                    break;
            }

            return type;
        }
        public List<int> GetEmployeesUnderBoss(int id)
        {
            List<int> employeesIdList = new List<int>();

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                var placements = db.Placements.Where(x => x.OrgStructureId == placement.OrgStructureId && x.isCurrent);
                if (placements.Any())
                {
                    var empIdList = placements.Select(x => x.EmployeeId).ToList();
                    employeesIdList.AddRange(empIdList);
                }

                var employeesUnderThisParent = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
                foreach (var und in employeesUnderThisParent)
                {
                    var employeesPlacedInThisParent = db.Placements.Where(x => x.OrgStructureId == und.OrgStructureId).ToList();
                    if (employeesPlacedInThisParent.Any())
                    {
                        var empIds = employeesPlacedInThisParent.Select(x => x.EmployeeId).ToList();

                        employeesIdList.AddRange(empIds);
                    }

                }

            }

            return employeesIdList;
        }

    }
}
