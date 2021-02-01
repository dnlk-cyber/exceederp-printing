using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class JobTitleMappingDetail:TrackUserOperation 
    {
        [Key]
        public int JobTitleMappingDetailId { get; set; }
       
        [Display(Name = "HR_Company_JobTitleMappingDetail_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        //public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RelevantExp", ResourceType = typeof(Localization.Resources))]
        public string RelevantExp { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Experience", ResourceType = typeof(Localization.Resources))]
        public string Experience { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_SpecialSkill", ResourceType = typeof(Localization.Resources))]
        public string SpecialSkill { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_PlannedSalary", ResourceType = typeof(Localization.Resources))]
        public string PlannedSalary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int JobTitleMappingId { get; set; }
        public virtual JobTitleMapping JobTitleMapping { get; set; }
      
    }
}