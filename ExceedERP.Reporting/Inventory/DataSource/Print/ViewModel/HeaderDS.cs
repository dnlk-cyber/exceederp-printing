using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class StoreReturnHeaderVm : InventoryVoucherHeaderVm
    {
        

        public int VoucherId { get; set; }
        public string VoucherStore { get; set; }
        public string VoucherDepartment { get; set; }
        public string VoucherTo { get; set; }
        public string VoucherDate { get; set; }
        public string Reference { get; set; }
        public string Letter { get; set; }
        public string Reason { get; set; }
        public string VoucherApprover { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNo { get; set; }
        public string VoucherApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
