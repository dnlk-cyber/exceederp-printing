using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantRegistration : TrackUserOperation
    {

        [Key]
        public int ApplicantRegistrationId { get; set; }
       // [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_TotalResult", ResourceType = typeof(Localization.Resources))]
        public decimal TotalResult { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VacancyCode { get; set; }
 
        public int Rank { get; set; }
        public bool Check { get; set; }
        public bool IsTransfered { get; set; }
       
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_LookupId", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public string IDAndTitle { get; set; }
       // [NotMapped]
        public string MediaVacancyCode { get; set; }
       // [NotMapped]
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_JobtitleName", ResourceType = typeof(Localization.Resources))]
        public string JobtitleName { get; set; }
        [NotMapped]
        public int BranchId { get; set; }
    }
}