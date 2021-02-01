using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class GoodReceiveHeader : InventoryVoucherHeaderVm
    {
        public string DocTitle { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string TenderNo { get; set; }
        public string GoodReciveNo { get; set; }
        public string Reference { get; set; }
        public string SupplierInvoice { get; set; }
        public string StoreName { get; set; }
        public string Deliverer { get; set; }
        public string CreatedBy { get; set; }
        public string Receiver { get; set; }
        public string Supplier { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
        public string ReceiveDate { get; set; }

        
    }
}