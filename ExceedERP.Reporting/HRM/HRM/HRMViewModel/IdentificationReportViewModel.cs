using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class IdentificationReportViewModel
    {
        public int IdentificationId { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Type { get; set; }
        public string IdNumber { get; set; }
        public string Description { get; set; }
        public DateTime? IssueDate { get; set; }
        public string IssueDateIdAm { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateAm { get; set; }
        public string Issuer { get; set; }
        public string Remark { get; set; }
    }
}
