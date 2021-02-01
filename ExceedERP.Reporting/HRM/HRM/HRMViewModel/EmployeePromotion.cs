using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePromotion
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public string FromStep { get; set; }
        public string ToStep { get; set; }
        public decimal Salary { get; set; }
        public string JobTitle { get; set; }
        public int YearOfPromotion { get; set; }


    }
}