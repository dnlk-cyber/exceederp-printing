using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.IFRS
{
    public class ScoiReportVeiwModel
    {
        public List<ScoiReportItemVeiwModel> ScoiReportItems { get; set; }
        public ScoiReportVeiwModel()
        {
            ScoiReportItems = new List<ScoiReportItemVeiwModel>();
        }
    }
    public class ScoiReportItemVeiwModel
    {
        public int Index { get; set; }
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
        public string ProjectName { get; set; }
    }

   
   
}
