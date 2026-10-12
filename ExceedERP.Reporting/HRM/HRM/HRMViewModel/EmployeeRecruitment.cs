using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeRecruitment
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public string BusinessUnit { get; set; }
        public string Branch { get; set; }
        public string JobTitle { get; set; }        
        public decimal Salary { get; set; }
        public string HireDate { get; set; }
        public string AssignmentType { get; set; }
      
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }

    }
}