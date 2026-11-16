using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.HRMPlan
{
    public class RecruitmentPlan:TrackUserOperation 
    {
        [Key]
        public int RecruitmentPlanId { get; set; }
        [Required]
        [Display(Name = "HR_Company_JobTitleMapping_OrgStructureID", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }

        //public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Required]
        [Display(Name = "HR_Company_JobTitleMapping_WorkUnitId", ResourceType = typeof(Localization.Resources))]

        public int WorkUnitId { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        [Required ]

        [Display(Name = "HR_HRMPlan_RecruitmentPlan_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        //  public virtual Lookup Lookup { get; set; }
        //[NotMapped ]
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_JobtitleName", ResourceType = typeof(Localization.Resources))]
        public string JobtitleName { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_NoOfVacantPost", ResourceType = typeof(Localization.Resources))]
        public int NoOfVacantPost { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_EduLevel", ResourceType = typeof(Localization.Resources))]
        public string EduLevel { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RelevantExp", ResourceType = typeof(Localization.Resources))]
        public string RelevantExp { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Experience", ResourceType = typeof(Localization.Resources))]
        public string Experience { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_SpecialSkill", ResourceType = typeof(Localization.Resources))]
        public string SpecialSkill { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_PlannedSalary", ResourceType = typeof(Localization.Resources))]
        public string  PlannedSalary { get; set; }
        //[Display(Name = "HR_HRMPlan_RecruitmentPlan_RecruiTime", ResourceType = typeof(Localization.Resources))]
        // [DataType(DataType.Date)]
        //  [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        // public DateTime? RecruiTime { get; set; }
        // [Display(Name = "HR_HRMPlan_RecruitmentPlan_RecruiTime", ResourceType = typeof(Localization.Resources))]
        //  public string RecruiTimeAmharic { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_From", ResourceType = typeof(Localization.Resources))]
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? From { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_To", ResourceType = typeof(Localization.Resources))]
        public DateTime? To { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RecruitmentType", ResourceType = typeof(Localization.Resources))]
        public VacancyType RecruitmentType { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RankId", ResourceType = typeof(Localization.Resources))]
        public int? RankId { get; set; }
        public virtual Rank Rank { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        ////[NotMapped ]
        public int Status { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Check", ResourceType = typeof(Localization.Resources))]
        public bool Check { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VacancyCode { get; set; }
        
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
      
    }
}