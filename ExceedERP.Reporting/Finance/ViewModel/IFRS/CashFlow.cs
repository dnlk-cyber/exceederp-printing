
using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.IFRS
{
  

    public class SocfReportVeiwModel
    {
        public List<SocfReportItemVeiwModel> SocfReportItems { get; set; }
        public SocfReportVeiwModel()
        {
            SocfReportItems = new List<SocfReportItemVeiwModel>();
        }
    }
    public class SocfReportItemVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
        public bool IsHeader { get; set; }
    }

    
}
