using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeQuarterPerfromance
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string JobTitle { get; set; }           
        public decimal Total { get; set; }
        public List<QurterList> QurterList { get; set; }
    }

    public class QurterList
    {
        public decimal Score { get; set; }
        public string PeriodName { get; set; }
    }
}