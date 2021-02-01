using System;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class ItemTransferReceiveHeader : InventoryVoucherHeaderVm
    {
        public string ReceiveNumber { get; set; }
        public string TransferNumber { get; set; }
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public string FromProject { get; set; }
        public string ToProject { get; set; }
        public string TransferDate { get; set; }
        public string Deliverer { get; set; }
        public string PlateNo { get; set; }
        public string SenderName { get; set; }
        public string ReceiveDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApproverName { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ReceivedDate { get; set; }
        public string RequestionNo { get; set; }
        public string DocumentRef { get; set; }

    }
}