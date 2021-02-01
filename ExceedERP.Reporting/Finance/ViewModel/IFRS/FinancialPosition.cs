
using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.IFRS
{
    public class SofpReportVeiwModel
    {
        public List<SofpReportItemVeiwModel> SofpReportItems { get; set; }
        public SofpReportVeiwModel()
        {
            SofpReportItems = new List<SofpReportItemVeiwModel>();
        }
    }
    public class SofpReportItemVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
    }

   
  
}
