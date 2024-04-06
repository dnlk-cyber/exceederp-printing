using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantExperience
    {
        [Key]
        public int ApplicantExpId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        public int ApplicantDetailId { get; set; }
        // public int ExperienceOrganizationTypeSettingId { get; set; }
        [Display(Name = "OrganizationType")]
        public OrganizationType OrganizationType { get; set; }
        [Display(Name = "JobTitle")]
        public int JobTitleId { get; set; }
        [Display(Name = "JobTitle")]
        public int? Jobtitle { get; set; }
        [Display(Name = "Year")]
        public int? Year { get; set; }
        [Display(Name = "OrganizationName")]
        public string OrganizationName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? From { get; set; }
        [Display(Name = "From")]
        public string FromAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? To { get; set; }
        [Display(Name ="To")]
        public string ToAm { get; set; }
        public string Remark { get; set; }
        [Display(Name = "TotalExperience")]
        public string TotalExperience { get; set; }
    }
}