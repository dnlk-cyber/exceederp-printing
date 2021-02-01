
using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.GAAP
{
    public class BalanceSheetReportVeiwModel
    {
        public List<BalanceSheetReportItemVeiwModel> BalanceSheetReportItems { get; set; }
        public BalanceSheetReportVeiwModel()
        {
            BalanceSheetReportItems = new List<BalanceSheetReportItemVeiwModel>();
        }
    }
    public class BalanceSheetReportItemVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
    }

   
  
}
