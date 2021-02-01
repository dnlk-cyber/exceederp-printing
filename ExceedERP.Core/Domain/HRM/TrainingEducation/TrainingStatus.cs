using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class TrainingStatus
    {
        [Key]
        public int TrainingStatusId { get; set; }
        [Display(Name = "HR_TrainingEducation_TrainingStatus_TraineeId", ResourceType = typeof(Localization.Resources))]
        public int TraineeId { get; set; }
        public virtual Trainee  Trainee{ get; set; }
        [Display(Name = "HR_TrainingEducation_TrainingStatus_TrainingOutCome", ResourceType = typeof(Localization.Resources))]
        public string TrainingOutCome { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}