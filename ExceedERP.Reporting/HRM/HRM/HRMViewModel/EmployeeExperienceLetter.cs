using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeExperienceLetter
    {
        public string ApplicantName { get; set; }
        public string ToName { get; set; }
        public string Branch { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string Grade { get; set; }
        public string Step { get; set; }
        public string FromBranch { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromBusinessUnit { get; set; }
        public string FromWorkUnit { get; set; }
        public string FromJobTitle { get; set; }
        public string FromGrade { get; set; }
        public string FromStep { get; set; }
        public string FromSalary { get; set; }
        public bool IsTerminated { get; set; }
        public string Date { get; set; }
        public string Allowance { get; set; }
        public string TotalExpriance { get; set; }
        public string BirthDate { get; set; }
        public string HiredDate { get; set; }
        public string EducationLevel { get; set; }
        public string EducationInfo { get; set; }
        public string Remark { get; set; }
        public decimal CurrentSalary { get; set; }
        public string SalaryInWord { get; set; }
        public List<CurrentEmployeeExpriance> Expriance { get; set; }
        public List<OtherEmployeeExpriance> OtherExpriance { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string HimHer { get; set; }
        public string HisHer { get; set; }
        public string CheckTermination { get; set; }
        public string CheckCurrent { get; set; }
        public string TerminationMassage { get; set; }
        public string TempOne { get; set; }
    }
    public class EmployeeSalary
    {
        public string ApplicantName { get; set; }  
        public decimal Salary { get; set; }
        public string SalaryInWord { get; set; }
  
    }
}