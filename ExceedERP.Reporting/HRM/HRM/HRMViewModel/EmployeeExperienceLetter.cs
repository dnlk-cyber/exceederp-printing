using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeExperienceLetter
    {
        public string ApplicantName { get; set; }
        public string ApplicantNameAmharic { get; set; }
        public string ToName { get; set; }
        public string Branch { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleEnglish { get; set; }
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
        public decimal FromSalary { get; set; }
        public bool IsTerminated { get; set; }
        public string Date { get; set; }
        public string Allowance { get; set; }
        public string TotalExpriance { get; set; }
        public string BirthDate { get; set; }
        public string HiredDate { get; set; }
        public string HiredDateAm { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentDateAm { get; set; }
        public string EducationLevel { get; set; }
        public string EducationInfo { get; set; }
        public string Remark { get; set; }
        public decimal CurrentSalary { get; set; }
        public decimal Salary { get; set; }
        public string SalaryInWord { get; set; }
        public string SalaryInWordAmharic { get; set; }
        public string SalaryInWordAffanOromo { get; set; }
        public string SalaryInWordEnglish { get; set; }
        public string ParentStructure { get; set; }
        public string Age { get; set; }
        public string EmploymentType { get; set; }
        public string EmployeeAllowance { get; set; }
        public string PersonalTitle { get; set; }
        public string DisciplineTaken { get; set; }
        public string ProbationMonth { get; set; }
        public string ContratMonth { get; set; }
        public string ContratStartDate { get; set; }
        public string ContratEndDate { get; set; }
        public string DelegationFromDate { get; set; }
        public string DelegationToDate { get; set; }
        public string ContratStartDateAm { get; set; }
        public string ContratEndDateAm { get; set; }
        public string DelegationFromDateAm { get; set; }
        public string DelegationToDateAm { get; set; }
        public decimal ContractMonth { get; set; }
        public string TerminationReason { get; set; }
        public string EmployeeCode { get; set; }
        public string TerminationDate { get; set; }
        //allowance
        public decimal TransportAllowance { get; set; }
        public decimal TelephoneAllowance { get; set; }
        public decimal HomeAllowance { get; set; }
        public decimal PositionAllowance { get; set; }
        public decimal ProfessionalAllowance { get; set; }
        public decimal DesertAllowance { get; set; }
        public decimal ProjectAllowance { get; set; }
        public decimal ResponsibilityAllowance { get; set; }
        
        public string TerminationApplicationDate { get; set; }
        public string MarriageStatus { get; set; }
        public string Gender { get; set; }
        public string HouseNo { get; set; }
        public string TelephoneNo { get; set; }
        public string Region { get; set; }
        public string DistrictWoreda { get; set; }
        public string KebeleSubDistrict { get; set; }
        public string POBoxNo { get; set; }
        public string BirthPlace { get; set; }
        public string EmergencyContactPerson { get; set; }

        public string LanguageProficiency { get; set; }
        public List<CurrentEmployeeExpriance> Expriance { get; set; }
        public List<OtherEmployeeExpriance> OtherExpriance { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string HeadName { get; set; }
        public string HeadJobTitle { get; set; }
        public string ApprovedBy { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public byte[] LogoFile { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string Country { get; set; }
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