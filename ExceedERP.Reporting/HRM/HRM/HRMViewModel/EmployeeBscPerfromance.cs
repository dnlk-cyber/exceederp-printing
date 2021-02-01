using System.Collections.Generic;


namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeBscPerfromance
    {
        public string EmployeeId { get; set; }

        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public string JobTitle { get; set; }
        public List<EvaluationPeiord> EvaluationPeiordList { get; set; }
        public decimal Total { get; set; }
        public string Rank { get; set; }
        public string branchName { get; set; }
        public string DeptName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}