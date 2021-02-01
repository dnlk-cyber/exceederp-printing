using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePerfromance
    {
        public string EmployeeId { get; set; }

        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }

        public List<Result> Results { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string branchName { get; set; }
        public string DeptName { get; set; }
    }

    public class Result
    {
        public string Period { get; set; }
        public decimal Total { get; set; }
    }
}