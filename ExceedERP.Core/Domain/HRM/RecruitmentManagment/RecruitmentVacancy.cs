using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class RecruitmentVacancy : TrackUserOperation
    {
        [Key]
        public int RecruitmentVacancyId { get; set; }

        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        public virtual Vacancy Vacancy { get; set; }
        //[Display(Name = "HR_RecruitmentManagment_Vacancy_Media", ResourceType = typeof(Localization.Resources))]
        //public int LookupId { get; set; }
        //public virtual Lookup Lookups { get; set; }

        [Display(Name = "Recruitment_Vacancy_MediaVacancyCode", ResourceType = typeof(Localization.Resources))]
        public string MediaVacancyCode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string FromAmh { get; set; }
        public string ToAmh { get; set; }
        public string Remark { get; set; }
    }
}