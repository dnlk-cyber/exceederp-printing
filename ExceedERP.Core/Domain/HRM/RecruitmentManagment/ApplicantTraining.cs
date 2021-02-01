using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantTraining
    {
        [Key]
        public int ApplicantTrainingId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        public virtual ApplicantRegistration ApplicantRegistration { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantTraining_TrainingName", ResourceType = typeof(Localization.Resources))]
        public string TrainingName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}