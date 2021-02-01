using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.ViewModel
    {
    public class StockCardHeaderVm : Organization
        {
        public string AccountCode { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public string Unit { get; set; }
        }

    public class BinCardHeader : Organization
        {
        public string CompanyName { get; set; }
        public string BinNo { get; set; }
        public string Model { get; set; }
        public string MinBalance { get; set; }
        public string InterChargeable { get; set; }
        public string Reorder { get; set; }
        public string UnitOfIssue { get; set; }
        public string ShelfNo { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }

        }

    public class CountSheetHeader : Organization
        {
        public string CompanyName { get; set; }
        public string AccountYear { get; set; }
        public string StoreCode { get; set; }
        }

    }