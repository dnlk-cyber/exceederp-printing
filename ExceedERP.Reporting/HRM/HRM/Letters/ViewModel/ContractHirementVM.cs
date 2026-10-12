using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters.ViewModel
{
    public class ContractHirementVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string HiredDate { get; set; }
        public string BirthDate { get; set; }
        public string FromBusinessUnit { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string FromWorkUnit { get; set; }
        public string FromBranch { get; set; }
        public string Branch { get; set; }
        public string FromJobTitle { get; set; }
        public string JobTitle { get; set; }
        public string FromRank { get; set; }
        public string Rank { get; set; }
        public string VaccancyDate { get; set; }
        public string FromAssignmentDate { get; set; }
        public string FromAssignmentDateAmh { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentDateAmh { get; set; }
        public decimal FromSalary { get; set; }
        public decimal Salary { get; set; }
        public decimal Probation { get; set; }
        public string ProbationInword { get; set; }
        public string  SalaryInWord { get; set; }
        public string  FromSalaryInWord { get; set; }
        public string  AdditionalBirr { get; set; }
        public string EmployeementType { get; set; }
       


    }
    public class CLearanceVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string HiredDate { get; set; }
        public string BirthDate { get; set; }
        public string FromBusinessUnit { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string FromWorkUnit { get; set; }
        public string FromBranch { get; set; }
        public string Branch { get; set; }
        public string FromJobTitle { get; set; }
        public string JobTitle { get; set; }
        public string FromRank { get; set; }
        public string Rank { get; set; }
        public string ClearanceRequestDate { get; set; }
        public string FromAssignmentDate { get; set; }
        public string FromAssignmentDateAmh { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentDateAmh { get; set; }
        public decimal FromSalary { get; set; }
        public decimal Salary { get; set; }
        public decimal Probation { get; set; }
        public string ProbationInword { get; set; }
        public string  SalaryInWord { get; set; }
        public string  FromSalaryInWord { get; set; }
        public string  AdditionalBirr { get; set; }
        public string EmployeementType { get; set; }
        public string EducationField { get; set; }
        public string EducationLevel { get; set; }
        public string PensionId { get; set; }
        public string ResonForClearance { get; set; }
        public string LastSalaryDate { get; set; }
        public string AnnualLeave { get; set; }
        public string FileVolume { get; set; }
        public string Discipline { get; set; }


    }
    public class RehirementVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string Branch { get; set; }
        public string JobTitle { get; set; }
        public string Rank { get; set; }
        public string AssignmentDate { get; set; }
        public string TerminationDate { get; set; }
        public string ApplicationDate { get; set; }
        public string HireDate { get; set; }
        public string AssignmentDateAmh { get; set; }
        public decimal Salary { get; set; }
        public string  SalaryInWord { get; set; }
        public string EmployeementType { get; set; }


    }
    public class AnnualIncreementVM
    {
        public string BudgetYear { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string BusinessUnit { get; set; }
        public string WorkUnit { get; set; }
        public string Branch { get; set; }
        public string JobTitle { get; set; }
        public string Rank { get; set; }
        public string AssignmentDate { get; set; }
        public string TerminationDate { get; set; }
        public string ApplicationDate { get; set; }
        public string HireDate { get; set; }
        public string AssignmentDateAmh { get; set; }
        public decimal Salary { get; set; }
        public string  SalaryInWord { get; set; }
        public string EmployeementType { get; set; }


    }
    public class ExperienceVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Branch { get; set; }
        public string TerminationDate { get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string Expereince { get; set; }
        public string Earnings { get; set; }
        public decimal Salary { get; set; }
        public string SalaryInWord { get; set; }
        public List<EmployeeAllowanceVM> EmployeeAllowances { get; set; }

    }
    public class PromotionAndSalaryAdjustmentVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string LastAssignmentDate { get; set; }
        public string Sex { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string Earnings { get; set; }
        public string GradeAndScale { get; set; }
        public decimal Salary { get; set; }
        public string SalaryInWord { get; set; }
        public List<EmployeeAllowanceVM> EmployeeAllowances { get; set; }

    }
    public class ProbationVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string LastAssignmentDate { get; set; }
        public string Sex { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string JobTitle { get; set; }
        public string Earnings { get; set; }
        public string GradeAndScale { get; set; }
        public decimal Salary { get; set; }
        public decimal ProbationPeriod { get; set; }
        public string SalaryInWord { get; set; }
        public List<EmployeeAllowanceVM> EmployeeAllowances { get; set; }

    }

    public class EmployeeAllowanceVM
    {
        public string Earnings { get; set; }
        public decimal Amounts { get; set; }

    }
}
