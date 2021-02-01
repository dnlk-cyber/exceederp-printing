using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class PromotionRequest
    {
        [Key]
        public int PromotionRequestId { get; set; }
        [Required]
        //[Display(Name = "HR_SelfCare_PromotionRequest_PlanType", ResourceType = typeof(Localization.Resources))]
        public JobTitlePlan PlanType { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Required]
        [Display(Name = "HR_SelfCare_PromotionRequest_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "HR_SelfCare_PromotionRequest_NoOfPostion", ResourceType = typeof(Localization.Resources))]
        public int NoOfPostion { get; set; }
              
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_EduLevel", ResourceType = typeof(Localization.Resources))]
        public int EduLevel { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_RelevantExp", ResourceType = typeof(Localization.Resources))]
        public string RelevantExp { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_Experience", ResourceType = typeof(Localization.Resources))]
        public decimal Experience { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_SpecialSkill", ResourceType = typeof(Localization.Resources))]
        public string SpecialSkill { get; set; }
        [Display(Name = "HR_SelfCare_PromotionRequest_PlannedSalary", ResourceType = typeof(Localization.Resources))]
        public decimal PlannedSalary { get; set; }
        [Required ]
        [Display(Name = "HR_SelfCare_PromotionRequest_RankId", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }
        public virtual Rank Rank { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark;

    }
}