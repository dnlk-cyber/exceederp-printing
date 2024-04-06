using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class AttendanceList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public int LateDays { get; set; }
        public string LeaveType { get; set; }
        public double LeaveAmount { get; set; }
        public double NoAbsent { get; set; }
        public string Period { get; set; }
        public string ApprovedBy { get; set; }
        //  public List<AttendanceDetail> AttendanceDetails { get; set; }


    }
    public class AttendanceDayAbssentAndLateCommingList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public int LateDays { get; set; }
        public string LeaveType { get; set; }
        public double LeaveAmount { get; set; }
        public double NoAbsent { get; set; }
        public double NoWorkingDays { get; set; }
        public string Period { get; set; }
        public string ApprovedBy { get; set; }
    }
}