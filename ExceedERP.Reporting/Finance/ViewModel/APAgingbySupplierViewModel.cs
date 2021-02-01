
namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class ApAgingbySupplierViewModel
    {
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal Aging1Amount { get; set; }
        public decimal Aging2Amount { get; set; }
        public decimal Aging3Amount { get; set; }
        public decimal Aging4Amount { get; set; }
    }
}
