using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ContractEmployeeVM
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Branch { get; set; }
        public string HireDate{ get; set; }
        public string ExpireDate{ get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
    }
}