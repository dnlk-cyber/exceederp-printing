using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class HealthValueListViewModel
    {
        public string TreatedDate { get; set; }
        public DateTime RecoredDate { get; set; }
        public string ReferalDate { get; set; }      
        public string FiscalYear { get; set; }
        public string Month { get; set; }
        public string AllowedAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public string RemainingAmount { get; set; }
    }
}
