using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class AttendanceList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string LateDays { get; set; }
        public string LeaveType { get; set; }
        public double LeaveAmount { get; set; }
        public List<AttendanceDetail> AttendanceDetails { get; set; }


    }
}