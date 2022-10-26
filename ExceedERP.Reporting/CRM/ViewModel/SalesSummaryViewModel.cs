using System.Collections.Generic;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesSummaryReportViewModel
    {
        public string ItemCategoryCode { get; set; }
        public string ItemCategory { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Adds { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SalesSummaryReportLineItemViewModel> LineItems { get; set; } = new List<SalesSummaryReportLineItemViewModel>();
    }
    public class SalesSummaryReportLineItemViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Adds { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        
    }
}
