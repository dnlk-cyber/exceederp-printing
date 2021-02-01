using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.Receivable.ViewModel
{
    public class DeferedRevenueVm
    {
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string SalesOrderNo { get; set; }
        public decimal SalesOrderAmount { get; set; }
        public string SalesOrderRef { get; set; }
        public int TotalSpot { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; }
        public decimal EarnedRevenue { get; set; }
        public decimal DeferedRevenue { get; set; }
        public decimal Total { get; set; }
    }

    public class PartnerListViewModel
    {
        [Display(Name = " Company Name")]
        public string Name { get; set; }
        [Display(Name = "Partner ID")]
        public int PartnerId { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        public string IsActive { get; set; }
    }
}
