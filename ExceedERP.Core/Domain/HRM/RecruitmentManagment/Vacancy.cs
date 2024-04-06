using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using System.Web.Mvc;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class Vacancy : TrackUserOperation
    {

        public int VacancyId { get; set; }      
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VaccancyCode { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int RecruitmentPlanId { get; set; }
 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Date_Posted", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_From", ResourceType = typeof(Localization.Resources))]
        public string FromVacAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Deadline_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_To", ResourceType = typeof(Localization.Resources))]
        public string ToVacAm { get; set; }
        [Display(Name = "ExternalExperienceJobTitle", ResourceType = typeof(Localization.Resources))]
        public int JobTitle { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_RecruSource", ResourceType = typeof(Localization.Resources))]
        public RecruitmentSource RecruSource { get; set; }
       // [Display(Name = "Branch")]
        [Display(Name = "Rpt_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Check", ResourceType = typeof(Localization.Resources))]
        public bool Check { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Department", ResourceType = typeof(Localization.Resources))]
        public int Department { get; set; }
        [Display(Name = "HR_EmployeeDetail_Committee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Recruitment_NoOfVaccantPostion", ResourceType = typeof(Localization.Resources))]
        public int NoOfVacant { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? WrittenExamSchedule { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? InterviewSchedule { get; set; }
        public int Location { get; set; }
        [Display(Name = "Rpt_Salary", ResourceType = typeof(Localization.Resources))]
        public string Salary { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_Experience_Level", ResourceType = typeof(Localization.Resources))]
        public ExperienceLevel ExperienceLevel { get; set; }
       // [Display(Name = "Total Year Of Experience")]
        [Display(Name = "TotalExperience", ResourceType = typeof(Localization.Resources))]
        public int TotalYearOfExperience { get; set; }
        [AllowHtml]
        [Display(Name = "JobRequirement", ResourceType = typeof(Localization.Resources))]
        public string JobRequirement { get; set; }
        [AllowHtml]
        [Display(Name = "Enum_AllowanceType_RESPONSIBILITY", ResourceType = typeof(Localization.Resources))]
        public string Responsibility { get; set; }

        public string EthioJobsCode { get; set; }
        [NotMapped]
        [Display(Name ="Branch")]
        public string BranchName { get; set; }
        [NotMapped]
        [Display(Name = "Location")]
        public string LocationName { get; set; }
        [NotMapped]
        [Display(Name = "ExternalExperienceJobTitle", ResourceType = typeof(Localization.Resources))]
        public string JobtitleName { get; set; }
        [NotMapped]
        [Display(Name = "HR_EmployeeDetail_Placement_Department", ResourceType = typeof(Localization.Resources))]
        public string DepartmentName { get; set; }
        [Display(Name = "Common_LanguageProficiency_LanguageId", ResourceType = typeof(Localization.Resources))]
        public Languages Language { get; set; }
        [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public GenderForVacancyGender Gender { get; set; }
    }
}