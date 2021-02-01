namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class DynamicColumn
    {
        public int rowNumber { get; set; }
        public string columnName { get; set; }
        public string cellContent { get; set; }
        public decimal values { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        
    }
}