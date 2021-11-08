using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
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
        //public virtual RecruitmentPlan RecruitmentPlan { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date Posted")]
        public DateTime From { get; set; }        
        public string FromVacAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Deadline Date")]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_To", ResourceType = typeof(Localization.Resources))]
        public string ToVacAm { get; set; }
        public int JobTitle { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_RecruSource", ResourceType = typeof(Localization.Resources))]
        public RecruitmentSource RecruSource { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Check", ResourceType = typeof(Localization.Resources))]
        public bool Check { get; set; }
        public int Department { get; set; }
        public bool IsActive { get; set; }       
        public int NoOfVacant { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? WrittenExamSchedule { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? InterviewSchedule { get; set; }
        public int Location { get; set; }
       // public VacancyStatus CurrentStatusTracking { get; set; }
        public string Salary { get; set; }
        [Display(Name = "Experience Level")]
        public ExperienceLevel ExperienceLevel { get; set; }
        [Display(Name = "Total Year Of Experience")]
        public int TotalYearOfExperience { get; set; }
        [AllowHtml]
        public string JobRequirement { get; set; }
        [AllowHtml]
        public string Responsibility { get; set; }
        public string EthioJobsCode { get; set; }
        [NotMapped]
        [Display(Name ="Branch")]
        public string BranchName { get; set; }
        [NotMapped]
        [Display(Name = "Location")]
        public string LocationName { get; set; }
        [NotMapped]
        [Display(Name = "Jobtitle")]
        public string JobtitleName { get; set; }
        [NotMapped]
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        public Languages Language { get; set; }
        public Gender Gender { get; set; }
    }
}