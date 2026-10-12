using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePerfromancePerCriteriaVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public List<PerformanceResultVM> PerformanceResults { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string BusinessUnit { get; set; }
        public string Status { get; set; }
        public string HireDate { get; set; }
        public string AssignmentDate { get; set; }
    }

    public class PerformanceResultVM
    {
        public string Period { get; set; }
        public string Criteria { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
    }
}