using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.SubLedger.ViewModel
    {
    public class StockCardHeaderVm : Organization
        {
        public string AccountCode { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public string Unit { get; set; }
        }

   

    }