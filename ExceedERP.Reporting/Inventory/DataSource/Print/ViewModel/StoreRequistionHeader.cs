using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class StoreRequistionHeader : InventoryVoucherHeaderVm
    {
        public string StoreRequsitionNo { get; set; }
        public string WorkUnit { get; set; }
        public string RequestedDate { get; set; }
        public string ApprovedDate { get; set; }
        public string Requestor { get; set; }
        public string Reviewer { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AuthorizedBy { get; set; }
        public string BranchName { get; set; }
        public string StoreName { get; set; }
        public string Model { get; set; }
        public string PlateNo { get; set; }
    }
}