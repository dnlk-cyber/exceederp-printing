namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class OvertimeHoursList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public decimal NormalHours { get; set; }
        public decimal NightHours { get; set; }
        public decimal HolidayHours { get; set; }
        public decimal RestdayHours { get; set; }
    }
}