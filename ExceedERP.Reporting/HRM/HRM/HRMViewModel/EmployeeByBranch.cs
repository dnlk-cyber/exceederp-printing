using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeByBranch
    {
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Education{ get; set; }
        public string Training { get; set; }
        public string Institution { get; set; }
        public string JobTitle { get; set; }
        public string Type { get; set; }
        public string HireDate { get; set; }
        public decimal Salary { get; set; }
    }
}