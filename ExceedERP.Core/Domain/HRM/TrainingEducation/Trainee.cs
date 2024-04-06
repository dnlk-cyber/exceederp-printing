using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class Trainee
    {
        [Key]
        public int TraineeId { get; set; }
        [Required ]
        [Display(Name = "HR_TrainingEducation_Trainee_CourseTrackingId", ResourceType = typeof(Localization.Resources))]
        public int CourseTrackingId { get; set; }
        public virtual CourseTracking CourseTracking { get; set; }
        [Display(Name = "HR_TrainingEducation_Trainee_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_TrainingEducation_Trainee_IsCompleteTraining", ResourceType = typeof(Localization.Resources))]
        public bool IsCompleteTraining { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "EducationLevel", ResourceType = typeof(Localization.Resources))]
        public int? EducationLevel { get; set; }

    }
}