using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePayrollSummaryViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Nation { get; set; }
        public string MaritalStatus { get; set; }
        public string BranchName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string ResponsibilityLevel { get; set; }
        public string EmploymentType { get; set; }
        public string EmploymentDate { get; set; }
        public string DateOfBirth { get; set; }
        public string RetirementDate { get; set; }
        public string EducationBackground { get; set; }
        public string EducationInstitutions { get; set; }
        public string TotalExperience { get; set; }
        public string PhoneNo { get; set; }
        public string AccountNo { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal ResponsibilityAllowance { get; set; }
        public decimal ProfessionalAllowance { get; set; }
        public decimal TaxablePerDiem { get; set; }
        public decimal NonTaxableAllowance { get; set; }
        public decimal ProjectAllowance { get; set; }
        public decimal DesertAllowance { get; set; }
        public decimal HardshipAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowance { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
    }
}
