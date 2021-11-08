using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class TrainingAssessmentViewModel : TrackUserOperation 
    {
        public int TrainingAssessmentViewModelId { get; set; }

        //[Required]
        [Display(Name = "HR_TrainingEducation_NeedAssessment_OrganizationId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }

        [Display(Name = "HR_TrainingEducation_NeedAssessment_NoOfStaff", ResourceType = typeof(Localization.Resources))]
        public int NoOfStaff { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_RequiredTraining", ResourceType = typeof(Localization.Resources))]
        public string RequiredTraining { get; set; }
        //[Required]
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Course", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Type", ResourceType = typeof(Localization.Resources))]
        public string Type { get; set; }
        //[Required]
        [Display(Name = "HR_TrainingEducation_NeedAssessment_ProviderOrganization", ResourceType = typeof(Localization.Resources))]
        public int MainCompanyId { get; set; }
        public virtual MainCompany MainCompany { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_DevelopmentRequired", ResourceType = typeof(Localization.Resources))]
        public string DevelopmentRequired { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_DevelopmentBy", ResourceType = typeof(Localization.Resources))]
        public string DevelopmentBy { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public bool Check { get; set; }
       // [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public bool IsTransfered { get; set; }
    }
}
