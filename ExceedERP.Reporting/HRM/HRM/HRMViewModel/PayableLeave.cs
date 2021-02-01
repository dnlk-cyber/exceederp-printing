using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class PayableLeave
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public List<PaybaleLeaveList> PaybaleLeaveList { get; set; }
      
    }
}