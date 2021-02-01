
using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.IFRS
{
  

    public class CashFlowReportVeiwModel
    {
        public List<CashFlowReportItemVeiwModel> CashFlowReportItems { get; set; }
        public CashFlowReportVeiwModel()
        {
            CashFlowReportItems = new List<CashFlowReportItemVeiwModel>();
        }
    }
    public class CashFlowReportItemVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
    }

    
}
