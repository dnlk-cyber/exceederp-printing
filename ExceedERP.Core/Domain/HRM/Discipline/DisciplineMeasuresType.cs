using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineMeasuresType
    {
        [Key ]
        public int DisciplineTypeId { get; set; }
        public int DetailMeasuresId { get; set; }
    
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_MeasuresId", ResourceType = typeof(Localization.Resources))]
        public int MeasuresId { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_ExpireYear", ResourceType = typeof(Localization.Resources))]
        public decimal ExpireYear { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_PercentageEffectOnPromotion", ResourceType = typeof(Localization.Resources))]
        public decimal PercentageEffectOnPromotion { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_IsByMoney", ResourceType = typeof(Localization.Resources))]
        public bool IsByMoney { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuresType_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}