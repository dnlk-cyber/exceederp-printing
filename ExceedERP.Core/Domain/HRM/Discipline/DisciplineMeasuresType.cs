using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineMeasuresType: TrackUserOperation
    {
        [Key ]
        public int DisciplineTypeId { get; set; }
        public int DetailMeasuresId { get; set; }
        //public virtual DisciplineMeasuresDetail MeasuresDetail { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_MeasuresId", ResourceType = typeof(Localization.Resources))]
        public int MeasuresId { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_ExpireYear", ResourceType = typeof(Localization.Resources))]
        public decimal ExpireYear { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_PercentageEffectOnPromotion", ResourceType = typeof(Localization.Resources))]
        public decimal PercentageEffectOnPromotion { get; set; }
        [Display(Name = "Deduct From Salary")]
        public bool IsDeductedFromSalary { get; set; }
        [Display(Name = "Salary Deduction (%)")]
        public decimal? SalaryDeductionPercentage { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_IsByMoney", ResourceType = typeof(Localization.Resources))]
        public bool IsByMoney { get; set; }
        public decimal Month { get; set; }
        public decimal Day { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        public decimal? Rank { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}