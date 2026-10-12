using System;
using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeAttendanceSummaryVM
    {
        public string EmpId { get; set; }

        public string EmpName { get; set; }

        public string Department { get; set; }

        public string ProjectOrBranch { get; set; }

        public DateTime PunchDate { get; set; }
        public string Day { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public decimal Leave { get; set; }
        public decimal Absent { get; set; }
        public decimal LateTime { get; set; }
        public decimal? TotalWorkingHrs { get; set; }
        public decimal OTHours { get; set; }
    }
}