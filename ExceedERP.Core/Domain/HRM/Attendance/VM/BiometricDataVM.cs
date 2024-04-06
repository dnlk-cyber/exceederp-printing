namespace ExceedERP.Core.Domain.HRM.Attendance.VM
{
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
}
