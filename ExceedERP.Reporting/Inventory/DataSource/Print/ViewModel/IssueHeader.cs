using System;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class IssueHeader : InventoryVoucherHeaderVm
    {
        public string IssueNumber { get; set; }
        public string IssueTo { get; set; }
        public string StoreName { get; set; }
        public string StoreRequistionNo { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string StoreKeeperName { get; set; }
        public string CreatedBy { get; set; }
        public string ApproverName { get; set; }
        public string Requestor { get; set; }
        public string Recipient { get; set; }
        public string IssueDate { get; set; }
        public DateTime ? ApprovedDate { get; set; }
        public string Datecreated { get; set; }
        public string ODOMeter { get; set; }
        public string PlateNo { get; set; }
        public string ExternalDriver { get; set; }
        public string ExternalSupplier { get; set; }
    }
}