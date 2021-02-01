using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeaveOneByOne
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public string LeavePeriod { get; set; }
        public decimal Entitlment { get; set; }
        public decimal TransferedFrom { get; set; }
        public decimal TotalAmountTaken { get; set; }
        public decimal Balance { get; set; }
        public List<LeaveTakenDetail> LeaveTakenDetails { get; set; }
        public List<LeaveFromOther> LeaveFromOthers { get; set; }
        public decimal LeavePaidInCash { get; set; }
    }
}