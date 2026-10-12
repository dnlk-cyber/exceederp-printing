using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    /// <summary>   A InternalVacancyEligibilitySetting. </summary>
    ///
    /// <remarks>   Kibrom Abera, 06/17/2026. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    public class InternalVacancyEligibilitySetting : TrackUserSettingOperation
    {
        [Key]
        public int InternalVacancyEligibilitySettingId { get; set; }

        [Display(
            Name = "Enforce Minimum Service Period",
            Description = "When enabled, employees must meet the minimum years of service requirement before applying for internal vacancies."
        )]
        public bool EnforceMinimumServicePeriod { get; set; }

        [Display(
            Name = "Minimum Years of Service",
            Description = "The minimum number of years an employee must have served to be eligible for internal vacancies."
        )]
        public decimal MinimumYearsOfService { get; set; }

        [Display(
            Name = "Allow Employees Below One Year of Service",
            Description = "When enabled, employees with less than one year of service may apply for internal vacancies."
        )]
       
        public bool AllowContractEmployees { get; set; }

        [Display(
            Name = "Allow Standby Technicians",
            Description = "When enabled, employees classified as Standby Technicians may apply for internal vacancies."
        )]
        public bool AllowStandbyTechnicians { get; set; }

        [Display(
            Name = "Allow HR Eligibility Override",
            Description = "When enabled, HR can manually override eligibility restrictions and permit otherwise ineligible employees to apply."
        )]
        public bool AllowHROverride { get; set; }
    }
}