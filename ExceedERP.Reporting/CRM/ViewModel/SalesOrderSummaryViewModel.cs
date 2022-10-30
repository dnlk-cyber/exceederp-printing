namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesOrderSummaryViewModel
    {
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
