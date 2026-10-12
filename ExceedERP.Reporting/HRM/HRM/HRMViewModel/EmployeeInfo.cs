using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeInfo
    {
        public string IdNumber { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Allowance { get; set; }
        public DateTime PensionDate { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string CriteriaName { get; set; }
        public string Value { get; set; }
        public string Dob { get; set; }
        public string TIN { get; set; }
        public string Branch { get; set; }
        public string Phone { get; set; }
    }
}