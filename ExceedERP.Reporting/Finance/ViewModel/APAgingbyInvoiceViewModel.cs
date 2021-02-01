
namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class ApAgingbyInvoiceViewModel
    {
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public string InvoiceNo { get; set; }
        public string Reference { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal Aging1Amount { get; set; }
        public decimal Aging2Amount { get; set; }
        public decimal Aging3Amount { get; set; }
        public decimal Aging4Amount { get; set; }
        public int BranchId { get; set; }
        public int ActualAge { get; set; }
    }
}
