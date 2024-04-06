using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeavePlanList
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string LeavePeriod { get; set; }

        public List<LeavePlanDetailList> LeavePlanDetails { get; set; }
       

    }
    public class LeavePlanDetailList
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}