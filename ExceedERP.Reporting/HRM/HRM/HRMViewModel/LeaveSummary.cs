using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeaveSummary
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public List<LeaveSummaryList> LeaveSummaryLists { get; set; }
        LeaveSummaryList LeaveSummaryList { get; set; }
    }
}