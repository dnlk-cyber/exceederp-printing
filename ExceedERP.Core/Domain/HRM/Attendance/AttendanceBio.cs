using System;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class AttendanceBio
    {
        public int AttendanceBioId { get; set; }
        public string EmpCode { get; set; }  
        public DateTime? Date { get; set; }
        public DateTime? PunchTime1 { get; set; }
        public DateTime? PunchTime2 { get; set; }
        public DateTime? PunchTime3 { get; set; }
        public DateTime? PunchTime4 { get; set; }
        public DateTime? PunchTime5 { get; set; }
        public DateTime? PunchTime6 { get; set; }
    }
}
