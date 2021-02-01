using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeLeave
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string  Departement { get; set; }
        public LeaveType  LeaveType { get; set; }
        public decimal  AmountTaken { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FullOrHalf { get; set; }
        public decimal Balance { get; set; }

    }
}