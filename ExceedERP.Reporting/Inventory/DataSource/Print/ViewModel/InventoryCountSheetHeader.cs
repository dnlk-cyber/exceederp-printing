using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class InventoryCountSheetHeader : Organization
    {
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public string StoreFullName { get; set; }

    }
}