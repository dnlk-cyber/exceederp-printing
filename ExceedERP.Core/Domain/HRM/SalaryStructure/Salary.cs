using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.SalaryStructure
{
    public class Salary

    {
        [Key ]
        public int SalaryStructureId { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Salary_RankId", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }
        public virtual Rank Rank { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Salary_StepId", ResourceType = typeof(Localization.Resources))]
        public int StepId { get; set; }
        public virtual Step Step { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Salary_SalaryAmount", ResourceType = typeof(Localization.Resources))]
        public decimal SalaryAmount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}