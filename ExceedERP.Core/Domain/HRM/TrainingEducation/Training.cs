using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    /// <summary>   An emp training education. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public  class Training :TrackUserOperation 
    {
        [Key ]
        public int EmpEduTrainId { get; set; }
        [Required]
        [Display(Name = "HR_TrainingEducation_Training_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_TrainingEducation_Training_OrganizationName", ResourceType = typeof(Localization.Resources))]
        public string OrganizationName { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_OrgAddress", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string OrgAddress { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_IsTraining", ResourceType = typeof(Localization.Resources))]
        public bool IsTraining { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_TrainingEducation_Training_From", ResourceType = typeof(Localization.Resources))]
        public DateTime? From { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_From", ResourceType = typeof(Localization.Resources))]
        public string FromTrAm { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_TrainingEducation_Training_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_To", ResourceType = typeof(Localization.Resources))]
        public string ToTraAm { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_Score", ResourceType = typeof(Localization.Resources))]
        public decimal Score { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_TotalCost", ResourceType = typeof(Localization.Resources))]
        public double TotalCost { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_PercentCoverdByOrg", ResourceType = typeof(Localization.Resources))]
        public decimal PercentCoverdByOrg { get; set; }
        [Display(Name = "HR_TrainingEducation_Training_TrainingName", ResourceType = typeof(Localization.Resources))]
        public string TrainingName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}