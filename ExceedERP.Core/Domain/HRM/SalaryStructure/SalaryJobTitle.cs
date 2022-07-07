using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SalaryStructure
{
    public class SalaryJobTitle : TrackUserOperation
    {
        [Key]
        public int SalJobTiId { get; set; }
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_RankId", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }

        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_StepId", ResourceType = typeof(Localization.Resources))]
        public int StepId { get; set; }
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_JobTitle", ResourceType = typeof(Localization.Resources))]
        public int? LookupId { get; set; }
        public string JobTitle { get; set; }
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
