using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class InventoryVoucherHeaderVm: Organization
    {
        public int NoOfCopy { get; set; }
        public string CopyMembers { get; set; }

    }
}
