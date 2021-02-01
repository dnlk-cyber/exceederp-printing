using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class InVacVM
    {
       
        [Display(Name = "HR_Company_JobTitleMapping_JobTitle", ResourceType = typeof(Core.Localization.Resources))]
        public string JobTitle { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_Experience", ResourceType = typeof(Core.Localization.Resources))]
        public string EduandExp { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Location", ResourceType = typeof(Core.Localization.Resources))]
        public string Location { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_Quntity", ResourceType = typeof(Core.Localization.Resources))]
        public int Quntity { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_Salary", ResourceType = typeof(Core.Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "Rpt_VacancyCode", ResourceType = typeof(Core.Localization.Resources))]
        public string VacancyCode { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_RecruitmentType", ResourceType = typeof(Core.Localization.Resources))]
        public VacancyType RecruitmentType { get; set; }
    }
}
