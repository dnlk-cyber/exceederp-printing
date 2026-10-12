using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class AttendanceList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public decimal LateDays { get; set; }
        public string LeaveType { get; set; }
        public double LeaveAmount { get; set; }
        public double NoAbsent { get; set; }
        public decimal FieldDays { get; set; }
        public string Period { get; set; }
        public string ApprovedBy { get; set; }
        //  public List<AttendanceDetail> AttendanceDetails { get; set; }


    }
    public class AttendanceDayAbssentAndLateCommingList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public decimal LateDays { get; set; }
        public decimal FieldDays { get; set; }
        public string LeaveType { get; set; }
        public decimal LeaveDays { get; set; }
        public decimal NoAbsentMinute { get; set; }
        public decimal NormalOT { get; set; }
        public decimal NightOT { get; set; }
        public decimal RestTimedOT { get; set; }
        public decimal HolidayOT { get; set; }
        public double LeaveAmount { get; set; }
        public double NoAbsent { get; set; }
        public double NoWorkingDays { get; set; }
        public string Period { get; set; }
        public string JobTitle { get; set; }
        public string ApprovedBy { get; set; }
    }
}