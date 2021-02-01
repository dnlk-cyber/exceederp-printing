using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceRank
    {
        [Key]
        public int PerformanceRankId { get; set; }
        [Display(Name = "HR_PerformanceManagment_PerformanceRank_Minimum", ResourceType = typeof(Localization.Resources))]
        public decimal Minimum { get; set; }
        [Display(Name = "HR_PerformanceManagment_PerformanceRank_Maximum", ResourceType = typeof(Localization.Resources))]
        public decimal Maximum { get; set; }
        [StringLength( 100)]
        [Display(Name = "HR_PerformanceManagment_PerformanceRank_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_PerformanceManagment_PerformanceRank_Rate", ResourceType = typeof(Localization.Resources))]
        public decimal BonusRate  { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}