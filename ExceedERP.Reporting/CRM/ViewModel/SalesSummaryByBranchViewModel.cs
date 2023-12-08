namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesSummaryByBranchReportViewModel
    {
        public string Category { get; set; }
        public string ItemName { get; set; }
        public string UoM { get; set; }
        public string Branch { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }

    }
   
}
