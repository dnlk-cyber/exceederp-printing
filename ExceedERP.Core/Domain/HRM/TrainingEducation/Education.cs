using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class Education
    {
        [Key ]
        public int EmpEduId { get; set; }
        [Required]
        [Display(Name = "HR_TrainingEducation_Education_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_TrainingEducation_Education_OrganizationName", ResourceType = typeof(Localization.Resources))]
        public string OrganizationName { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_OrgAddress", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string OrgAddress { get; set; }

        [Display(Name = "HR_TrainingEducation_Education_From", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? From { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_From", ResourceType = typeof(Localization.Resources))]
        public string FromEAmharic { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_TrainingEducation_Education_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_To", ResourceType = typeof(Localization.Resources))]
        public string ToEAmharic { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_Score", ResourceType = typeof(Localization.Resources))]
        public decimal Score { get; set; }
        [Required ]
        [Display(Name = "HR_TrainingEducation_Education_EduLevel", ResourceType = typeof(Localization.Resources))]
        public int EduLevel { get; set; }
        [Required ]
        [Display(Name = "HR_TrainingEducation_Education_EduField", ResourceType = typeof(Localization.Resources))]
        public int EduField { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_TotalCost", ResourceType = typeof(Localization.Resources))]
        public double TotalCost { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_PercentCoverdByOrg", ResourceType = typeof(Localization.Resources))]
        public decimal PercentCoverdByOrg { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_IsHighLevel", ResourceType = typeof(Localization.Resources))]
        public bool IsHighLevel { get; set; }
    }
}