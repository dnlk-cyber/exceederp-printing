using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Severance
{
    public class SeverancePayableSetting: TrackUserOperation
    {
        [Key]
        public int SeverancePayableSettingId { get; set; }
        [Display(Name = "Responsibility Level")]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "Minimum Years of Service")]
        public int MinYearsOfService { get; set; }

        [Display(Name = "Maximum Years of Service")]
        public int? MaxYearsOfService { get; set; } // null = no upper limit
        [Display(Name = "Salary Value in %")]
        public decimal SalaryValue { get; set; }

        [Display(Name = "Number of years paid with full salary")]
        public decimal? YearsPaidWithFullSalary { get; set; }

        public string Description { get; set; }
    }

    public class SeveranceCalculationSetting : TrackUserOperation
    {
        [Key]
        public int SeveranceCalculationSettingId { get; set; }

        [Display(Name = "Calculation Formula")]
        public SeveranceFormulaType FormulaType { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }

    public enum SeveranceFormulaType
    {
        OldFormula = 0,
        NewFormula = 1
    }
}
