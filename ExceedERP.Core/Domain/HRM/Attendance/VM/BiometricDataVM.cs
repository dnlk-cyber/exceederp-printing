using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Attendance.VM
{
    public class DepartmentDataVM
    {
        public string id { get; set; }
        public string dept_code { get; set; }
        public string dept_name { get; set; }
        public string parent_dept { get; set; }
    }
    public class BiometricDataVM
    {
        public string employee_id { get; set; }
        public string regular { get; set; }
        public string day_ot { get; set; }
        public string night_ot { get; set; }
        public string off_ot { get; set; }
        public string holiday_ot { get; set; }
        public string absent_hour { get; set; }
        public string absent_day { get; set; }
        public string off_day { get; set; }
        public string holiday { get; set; }
    }

    public class AttendanceRecord
    {
        public string EmpId { get; set; }
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public decimal? NeedPresent { get; set; }
        public decimal? Present { get; set; }
        public decimal? Absence { get; set; }
        public decimal? Holiday { get; set; }
        public string PresentRate { get; set; }
        public string AbsenceRate { get; set; }
    }
}
