namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class SummaryReconcilation
    {
        public string  Name { get; set; }
        public decimal MonthOne { get; set; }
        public decimal MonthTwo { get; set; }
        public decimal CurrentMonth { get; set; }
        public decimal DiffOne { get; set; }
        public decimal DiffTwo { get; set; }
        public string PeriodOne { get; set; }
        public string PeriodTwo { get; set; }
        public string CurrentPeriod { get; set; }

    }
}