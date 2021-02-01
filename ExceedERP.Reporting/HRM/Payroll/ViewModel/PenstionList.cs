namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class PenstionList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal PensionFromCompany { get; set; }
        public decimal PensionFromEmployee { get; set; }
        public decimal TotalPension { get; set; }
        public string Pension { get; set; }
        public string Status { get; set; }
    }
}