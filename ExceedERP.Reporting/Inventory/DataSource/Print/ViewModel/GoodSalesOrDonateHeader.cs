using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class GoodSalesOrDonateHeader : InventoryVoucherHeaderVm
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public string Type { get; set; }
        public string SaleDate { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationAddress { get; set; }
        public string StoreName { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public string Receiver { get; set; }
        public string Supplier { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
        public string ReceiveDate { get; set; }

    }
}