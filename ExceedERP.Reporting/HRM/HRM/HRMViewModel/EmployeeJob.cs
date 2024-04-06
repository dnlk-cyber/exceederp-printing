using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeJob
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public string JobTitle { get; set; }
        public string Rank { get; set; }
        public decimal  Salary { get; set; }
        public string Step { get;  set; }
        public string Remark { get; set; }
        // public string To { get; set; }


    }
    public class EmployeeJobTitle
    {
        public string Department { get; set; }
        public List<EmployeeJob> EmployeeJobs { get; set; } = new List<EmployeeJob>();
    }
}