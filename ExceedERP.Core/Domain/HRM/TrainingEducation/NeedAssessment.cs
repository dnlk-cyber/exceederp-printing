using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    /// <summary>   A need assessment. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    /// <remarks>   Kibrom Abera, 10/11/2022. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class NeedAssessment:TrackUserOperation 
    {
        [Key]
        public int AssesmentId { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_OrganizationId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_NoOfStaff", ResourceType = typeof(Localization.Resources))]
        public int NoOfStaff { get; set; }
        [Display(Name = "Total Days")]
        public decimal? TotalDays { get; set; }
        [Display(Name = "Enum_LookUpTypes_TARGET_GROUP", ResourceType = typeof(Localization.Resources))]
        public int? RequiredTraining { get; set; }
        [Required]
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Course", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Type", ResourceType = typeof(Localization.Resources))]
        public string Type { get; set; }
        [Required]
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
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        public bool IsTransfered { get; set; }
        public bool IsSummarized { get; set; }
        public bool IsSendForApproval { get; set; }
        public bool IsSecondSendForApproval { get; set; }
        public bool IsThirdSendForApproval { get; set; }
        [NotMapped]
        public bool IsRejected { get; set; }
        [Display(Name = "From Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? From { get; set; }
        [Display(Name = "To Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? To { get; set; }
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
        public bool IsHRLevel { get; set; }
        public int? BranchId { get; set; }

        [NotMapped]
        public int[] SelectedBranchIds { get; set; }

    }
}
