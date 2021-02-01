using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.GAAP
{
    public class IncomeStatementReportVeiwModel
    {
        public List<IncomeStatementReportItemVeiwModel> IncomeStatementReportItems { get; set; }
        public IncomeStatementReportVeiwModel()
        {
            IncomeStatementReportItems = new List<IncomeStatementReportItemVeiwModel>();
        }
    }
    public class IncomeStatementReportItemVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
    }

   
   
}
