using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class Vacancy : TrackUserOperation
    {

        public int VacancyId { get; set; }
        //[Key]

        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VaccancyCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int RecruitmentPlanId { get; set; }
        public virtual RecruitmentPlan RecruitmentPlan { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_Vacancy_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_From", ResourceType = typeof(Localization.Resources))]
        public string FromVacAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_Vacancy_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_To", ResourceType = typeof(Localization.Resources))]
        public string ToVacAm { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Media", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookups { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_RecruSource", ResourceType = typeof(Localization.Resources))]
        public RecruitmentSource RecruSource { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Location", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Check", ResourceType = typeof(Localization.Resources))]
        public bool Check { get; set; }
        public int WorkUnitId { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Required]
        [Display(Name = "HR_Company_JobTitleMapping_OrgStructureID", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

    }
}