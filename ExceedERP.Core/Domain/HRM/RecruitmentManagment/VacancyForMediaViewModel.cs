using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class VacancyForMediaViewModel
    {
        public int VacancyForMediaId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int WorkUnitId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VacancyCode { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_MediaVacancyCode", ResourceType = typeof(Localization.Resources))]
        public string MediaVacancyCode { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public string JobtitleName { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_NoOfVacantPost", ResourceType = typeof(Localization.Resources))]
        public int NoOfVacantPost { get; set; }
        public int RecruitmentPlanId { get; set; }
        public DateTime? From { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }

        public DateTime? To { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RecruitmentType", ResourceType = typeof(Localization.Resources))]
        public VacancyType RecruitmentType { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_RecruSource", ResourceType = typeof(Localization.Resources))]
        public RecruitmentSource RecruitmentSource { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Check { get; set; }

    }
}
