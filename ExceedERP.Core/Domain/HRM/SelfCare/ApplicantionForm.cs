using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    /// <summary>   Form for viewing the applicantion. </summary>
    ///
    /// <remarks>   Mekanehiwot, 11/9/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ApplicantionForm:TrackUserOperation 
    {
        [Key]
        public int ApplicantionFormId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }

        [Display(Name = "HR_SelfCare_ApplicantionForm_EmployeeId", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_SelfCare_ApplicantionForm_ApplicationType", ResourceType = typeof(Localization.Resources))]
   
        public string ApplicationType { get; set; }
        [Required ]
        [Display(Name = "HR_SelfCare_ApplicantionForm_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        public virtual Vacancy Vacancy { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_ApplicantionForm_TotalResult", ResourceType = typeof(Localization.Resources))]
        public decimal TotalResult { get; set; }

        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        public string MediaVacancyCode { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_LookupId", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public string IDAndTitle { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        // public virtual MainCompanyStructure MainCompanyStructure { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}