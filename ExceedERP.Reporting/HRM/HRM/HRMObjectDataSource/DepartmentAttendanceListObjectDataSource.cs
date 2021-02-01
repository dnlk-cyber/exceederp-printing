using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
//using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.SharedDataSources;
using Microsoft.SqlServer.Server;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Reporting.HRM.Payroll;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class DepartmentAttendanceListObjectDataSource : List<AttendanceList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AttendanceList> GetDepartmentAttendanceListReport(string period, string week, string dept, string user)
        {
            List<AttendanceList> list = new List<AttendanceList> { };
            AttendanceList EmployeeInfo;
            AttendanceDetail attendanceDetail = new AttendanceDetail();
            var weekeds = db.Weekends.Select(x => x.Name.ToString()).ToList();
            int month = 0;
            string absentDate = "";
            int count = 0;
            List<string> branhList = null;


            var payroll = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == period);
            var employees = db.Person_Employee.ToList();
            //var empId= Helpers.Common .u 
            List<string> emloyeeList = GetUserBranchEmployees(Convert.ToInt32(dept), branhList);
            if (payroll != null)
            {

                //month = payroll.DateFrom.Month;
                //var weeklyAttendance = db.WeeklyAttendances.Where(x => x.Month == month);
                var weeklyAttendance = db.WeeklyAttendances.Where(x => x.GLPeriodId == payroll.GLPeriodId);
                if (week != null)
                {

                    if (weeklyAttendance.Any())
                    {
                        foreach (var e in emloyeeList)
                        {
                            double checkZero = 0;
                            double checkLate = 0;
                            double checkLeave = 0;
                            EmployeeInfo = new AttendanceList();
                            EmployeeInfo.AttendanceDetails = new List<AttendanceDetail>();
                            var employee = employees.FirstOrDefault(x => x.EmployeeId.ToString() == e);
                            if (employee != null)
                            {
                                EmployeeInfo.EmpID = employee.EmployeeCode;
                                EmployeeInfo.FullName = employee.FirstName + " " + employee.MiddleName + " " +
                                                        employee.LastName;


                                var lateDays = db.LateComingSummaries.Where(x => x.EmployeeId == employee.EmployeeId && x.GLPeriodId.ToString() == period);

                                if (lateDays.Any())
                                {
                                    var total = lateDays.Sum(x => x.TotalDays);
                                    EmployeeInfo.LateDays = total.ToString();//lateDays.ToString();
                                    checkLate = Convert.ToDouble(total);
                                }


                            }

                            var employeeAttendance = weeklyAttendance.Where(x => x.EmployeeId.ToString() == e).ToList();
                            {
                                attendanceDetail = new AttendanceDetail();

                                attendanceDetail.Week = Convert.ToInt32(week);
                                if (Convert.ToInt32(week) >= 4)
                                {
                                    attendanceDetail.Week = 4;


                                    if (employeeAttendance.Count(x => (!x.DayFive) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayFour) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayThree) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SUNDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DayOne) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }

                                    if (employeeAttendance.Count(x => (!x.DaySix) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SATURDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }
                                }
                                else
                                {

                                    if (employeeAttendance.Count(x => (!x.DayFive) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayFour) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayThree) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SUNDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DayOne) && x.Week.ToString() == week) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week.ToString() == week) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySix) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week.ToString() == week) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SATURDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week.ToString() == week) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week.ToString() == week) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }


                                    double leaveTotal = 0;
                                    LeaveType leaveT = LeaveType.ANNUAL;
                                    List<string> leaveTypeL = new List<string>();

                                    if (Convert.ToInt32(week) == 1)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 1);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }

                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }

                                    }
                                    if (Convert.ToInt32(week) == 2)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 2);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (Convert.ToInt32(week) == 3)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 3);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (Convert.ToInt32(week) == 4)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 4);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (Convert.ToInt32(week) == 5)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 5);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (Convert.ToInt32(week) == 6)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 6);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (Convert.ToInt32(week) == 7)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 7);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = one.LeaveType;
                                                leaveTypeL.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = two.LeaveType;
                                                leaveTypeL.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = three.LeaveType;
                                                leaveTypeL.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = four.LeaveType;
                                                leaveTypeL.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = five.LeaveType;
                                                leaveTypeL.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = six.LeaveType;
                                                leaveTypeL.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                leaveTotal = leaveTotal + 1;
                                                leaveT = seven.LeaveType;
                                                leaveTypeL.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }


                                    EmployeeInfo.LeaveAmount = leaveTotal;

                                    checkLeave = leaveTotal;

                                    var leaveTypeLists = leaveTypeL.Distinct();
                                    var ltl = "";
                                    foreach (var lt in leaveTypeLists)
                                    {
                                        ltl = ltl + lt + ",      ";
                                    }
                                    EmployeeInfo.LeaveType = ltl;

                                   
                                }

                            }
                            checkZero = attendanceDetail.NoAbsent;
                            if (attendanceDetail.NoAbsent > 0)
                            {
                                EmployeeInfo.AttendanceDetails.Add(attendanceDetail);
                            }
                            
                            if (EmployeeInfo.EmpID != null && (checkZero > 0 || checkLate > 0 || checkLeave > 0))
                            {
                                list.Add(EmployeeInfo);
                            }
                        }
                    }
                }

                else
                {
                    if (weeklyAttendance.Any())
                    {
                        foreach (var e in emloyeeList)
                        {
                            double checkZero = 0;
                            double checkZeroSecond = 0;
                            double checkLate = 0;
                            double checkLeave = 0;
                            EmployeeInfo = new AttendanceList();
                            EmployeeInfo.AttendanceDetails = new List<AttendanceDetail>();

                            double leaveTotal = 0;
                            LeaveType leaveT = LeaveType.ANNUAL;
                            List<string> leaveTypeList = new List<string>();

                            var employee = employees.FirstOrDefault(x => x.EmployeeId.ToString() == e);
                            if (employee != null)
                            {
                                EmployeeInfo.EmpID = employee.EmployeeCode;
                                EmployeeInfo.FullName = employee.FirstName + " " + employee.MiddleName + " " +
                                                        employee.LastName;

                                var lateDays = db.LateComingSummaries.Where(x => x.EmployeeId == employee.EmployeeId && x.GLPeriodId.ToString() == period);

                                if (lateDays.Any())
                                {
                                    var total = lateDays.Sum(x => x.TotalDays);

                                    EmployeeInfo.LateDays = total.ToString();

                                    checkLate = Convert.ToDouble(total);
                                }
                            }


                            var employeeAttendance = weeklyAttendance.Where(x => x.EmployeeId.ToString() == e).ToList();
                            for (int i = 1; i < 5; i++)
                            {
                                attendanceDetail = new AttendanceDetail();

                                attendanceDetail.Week = i;
                                if (i >= 4)
                                {
                                    attendanceDetail.Week = 4;

                                    if (employeeAttendance.Count(x => (!x.DayFive) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayFour) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayThree) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SUNDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DayOne) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySix) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week >= 4) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (!weekeds.Contains("SATURDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week >= 4) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }
                                }
                                else
                                {
                                    attendanceDetail.Week = i;

                                    if (employeeAttendance.Count(x => (!x.DayFive) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayFour) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayThree) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }

                                    if (!weekeds.Contains("SUNDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DayOne) && x.Week == i) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week == i) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySix) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week == i) == 1)
                                    {
                                        attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                    }
                                    if (!weekeds.Contains("SATURDAY"))
                                    {
                                        if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week == i) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                        if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week == i) == 1)
                                        {
                                            attendanceDetail.NoAbsent = attendanceDetail.NoAbsent + 0.5;
                                        }
                                    }


                                    if (i == 1)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 1);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }

                                    }
                                    if (i == 2)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 2);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (i == 3)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 3);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (i == 4)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 4);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (i == 5)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 5);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (i == 6)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 6);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }
                                    if (i == 7)
                                    {
                                        var leaves = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId);
                                        var singleAttendances = employeeAttendance.Where(x => x.EmployeeId == employee.EmployeeId && x.Week == 7);

                                        foreach (var att in singleAttendances)
                                        {
                                            var one = leaves.FirstOrDefault(x => x.From <= att.DayOneDate && x.To >= att.DayOneDate);
                                            if (one != null)
                                            {
                                                if (one.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (one.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = one.LeaveType;
                                                leaveTypeList.Add(one.LeaveType.ToString());
                                            }
                                            var two = leaves.FirstOrDefault(x => x.From <= att.DayTwoDate && x.To >= att.DayTwoDate);
                                            if (two != null)
                                            {
                                                if (two.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (two.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = two.LeaveType;
                                                leaveTypeList.Add(two.LeaveType.ToString());
                                            }
                                            var three = leaves.FirstOrDefault(x => x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);
                                            if (three != null)
                                            {
                                                if (three.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (three.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = three.LeaveType;
                                                leaveTypeList.Add(three.LeaveType.ToString());
                                            }
                                            var four = leaves.FirstOrDefault(x => x.From <= att.DayFourDate && x.To >= att.DayFourDate);
                                            if (four != null)
                                            {
                                                if (four.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (four.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = four.LeaveType;
                                                leaveTypeList.Add(four.LeaveType.ToString());
                                            }
                                            var five = leaves.FirstOrDefault(x => x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);
                                            if (five != null)
                                            {
                                                if (five.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (five.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = five.LeaveType;
                                                leaveTypeList.Add(five.LeaveType.ToString());
                                            }
                                            var six = leaves.FirstOrDefault(x => x.From <= att.DaySixDate && x.To >= att.DaySixDate);
                                            if (six != null)
                                            {
                                                if (six.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (six.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = six.LeaveType;
                                                leaveTypeList.Add(six.LeaveType.ToString());
                                            }
                                            var seven = leaves.FirstOrDefault(x => x.From <= att.DaySevenDate && x.To >= att.DaySevenDate);
                                            if (seven != null)
                                            {
                                                if (seven.Day == LeaveDay.ሙሉ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 1;
                                                }
                                                if (seven.Day == LeaveDay.ግማሽ_ቀን)
                                                {
                                                    leaveTotal = leaveTotal + 0.5;
                                                }
                                                leaveT = seven.LeaveType;
                                                leaveTypeList.Add(seven.LeaveType.ToString());
                                            }
                                        }
                                    }

                                    EmployeeInfo.LeaveAmount = leaveTotal;

                                    checkLeave = leaveTotal;

                                    var leaveTypeLists = leaveTypeList.Distinct();
                                    var ltl = "";
                                    foreach (var lt in leaveTypeLists)
                                    {
                                        ltl = ltl + lt + ",      ";
                                    }
                                    EmployeeInfo.LeaveType = ltl;
                                }
                                checkZero = attendanceDetail.NoAbsent;
                                checkZeroSecond = checkZeroSecond + attendanceDetail.NoAbsent;
                                if (attendanceDetail.NoAbsent > 0)
                                {
                                    EmployeeInfo.AttendanceDetails.Add(attendanceDetail);
                                }
                            }

                            if (EmployeeInfo.EmpID != null && (checkZeroSecond > 0 || checkLate > 0 || checkLeave > 0))
                            {
                                list.Add(EmployeeInfo);
                            }

                        }
                    }
                }

            }

            return list;
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

    }
}
