using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineMeasuerByMoney: DepartmentNewOperations
    {
        public int DisciplineMeasuerByMoneyId { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public int DisciplinaryCaseId { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_GLPeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_Months", ResourceType = typeof(Localization.Resources))]
        public int Months { get; set; }
        public int? Days { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_MonthlyAmount", ResourceType = typeof(Localization.Resources))]
        public decimal MonthlyAmount { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_Balance", ResourceType = typeof(Localization.Resources))]
        public decimal Balance { get; set; }
        [Display(Name = "Deduct From Salary")]
        public bool IsDeductedFromSalary { get; set; }
        [Display(Name = "Salary Deduction (%)")]
        public decimal? SalaryDeductionPercentage { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }

    }
}