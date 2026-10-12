using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class AttendanceLetterODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceLeterVM> GetAttendance(DateTime from, DateTime to, string id)
        {

            List<AttendanceLeterVM> attendanceList = new List<AttendanceLeterVM>();

            if (id != null)
            {
                int.TryParse(id, out int empId);
                var employees = EmployeeHelper.GetSubordinates_FilteredByHead(empId);
                //var employees = GetEmployeesUnderBoss(empId);
                var attendances = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => x.Date >= from && x.Date <= to).OrderBy(x => x.Date).ToList();

                int index = 0;

                foreach (var att in attendances)
                {
                    var attendanceMorning = new AttendanceLeterVM();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);

                    if (person != null)
                    {
                        // Same name must be used for both Morning/Afternoon rows of a day, otherwise the
                        // report groups them as two different employees (om/en cultures used the Amharic
                        // name here but the English name below).
                        attendanceMorning.Name = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            ? person.FullNameAmharic
                            : person.FullNameaEnglish;

                        attendanceMorning.Date = att.Date;
                        attendanceMorning.Day = GetDayName(att.Date.DayOfWeek.ToString(), cultureName);
                        if (cultureName.ToLower() == "om")
                        {
                            attendanceMorning.Shift = "Ganama";
                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            attendanceMorning.Shift = "Morning";
                        }
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {
                            attendanceMorning.Shift = "ጠዋት";
                        }

                           
                        attendanceMorning.Index = index;

                        if (att.MorningAttendance==TakeAttendance.Present)
                        {
                            attendanceMorning.Value = "P";
                        }
                        else if(att.MorningAttendance == TakeAttendance.Absent)
                        {
                            attendanceMorning.Value = "A";
                        }
                        else if(att.MorningAttendance == TakeAttendance.Field)
                        {
                            attendanceMorning.Value = "Field";
                        }
                        else if(att.MorningAttendance == TakeAttendance.Leave)
                        {
                            attendanceMorning.Value = "L";
                        }
                        else if(att.MorningAttendance == TakeAttendance.Others)
                        {
                            attendanceMorning.Value = "O";
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

                        attendanceAfternoon.Date = att.Date;
                        attendanceAfternoon.Day = GetDayName(att.Date.DayOfWeek.ToString(),cultureName);
                        attendanceMorning.Index = index;
                        attendanceAfternoon.Name = person.FullNameaEnglish;
                        if (cultureName.ToLower() == "om")
                        {
                            attendanceAfternoon.Shift = "Waaree booda";
                            attendanceAfternoon.Name = person.FullNameaEnglish;

                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            attendanceAfternoon.Shift = "Afternoon";
                            attendanceAfternoon.Name = person.FullNameaEnglish;

                        }
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {
                            attendanceAfternoon.Shift = "ከሰአት";
                            attendanceAfternoon.Name = person.FullNameAmharic;

                        }
                        if (att.IsPresentAfternoon &&att.AfternoonAttendance == TakeAttendance.Present)
                        {
                            attendanceAfternoon.Value = "P";
                        }
                        else if (att.AfternoonAttendance == TakeAttendance.Absent)
                        {
                            attendanceAfternoon.Value = "A";
                        }
                        else if (att.AfternoonAttendance == TakeAttendance.Field)
                        {
                            attendanceAfternoon.Value = "Field";
                        }
                        else if (att.AfternoonAttendance == TakeAttendance.Leave)
                        {
                            attendanceAfternoon.Value = "L";
                        }
                        else if (att.AfternoonAttendance == TakeAttendance.Others)
                        {
                            attendanceAfternoon.Value = "O";
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
        public string GetDayName(string day, string cultureName)
        {
            switch (day.ToLower())
            {
                case "monday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "ሰኞ" :
                           cultureName.ToLower() == "om" ? "Wiixata" : "Monday";

                case "tuesday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "ማክሰኞ" :
                           cultureName.ToLower() == "om" ? "Kabala" : "Tuesday";

                case "wednesday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "ረቡዕ" :
                           cultureName.ToLower() == "om" ? "Roobii" : "Wednesday";

                case "thursday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "ሐሙስ" :
                           cultureName.ToLower() == "om" ? "Kamiisa" : "Thursday";

                case "friday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "አርብ" :
                           cultureName.ToLower() == "om" ? "Jimaata" : "Friday";

                case "saturday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "ቅዳሜ" :
                           cultureName.ToLower() == "om" ? "Sanbata Duraa" : "Saturday";

                case "sunday":
                    return cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et" ? "እሁድ" :
                           cultureName.ToLower() == "om" ? "Dilbata" : "Sunday";

                default:
                    return null; // If the day is not recognized
            }
        }

        //public string GetDaysAmharic(string day)
        //{
        //    switch (day.ToLower())
        //    {
        //        case "monday":
        //            return "ሰኞ";
        //        case "tuesday":
        //            return "ማክሰኞ";
        //        case "wednesday":
        //            return "ረቡዕ";
        //        case "thursday":
        //            return "ሐሙስ";
        //        case "friday":
        //            return "አርብ";
        //        case "saturday":
        //            return "ቅዳሜ";
        //        case "sunday":
        //            return "እሁድ";
        //    }

        //    return null;
        //}

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
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
