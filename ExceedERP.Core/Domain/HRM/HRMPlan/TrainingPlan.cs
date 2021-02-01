using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.HRMPlan
{
    public class TrainingPlan:TrackUserOperation 
    {
        [Key]
        public int TrainingPlanId { get; set; }
       
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_WorkUnit", ResourceType = typeof(Localization.Resources))]
        public int WorkUnit { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_Course", ResourceType = typeof(Localization.Resources))]
        public int Course { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_NoofTrainee", ResourceType = typeof(Localization.Resources))]
        public int NoofTrainee { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_TargetGroup", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup  Lookup { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_TrainingProvider", ResourceType = typeof(Localization.Resources))]
     
        public int MainCompanyId { get; set; }
        public virtual MainCompany Company { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_CostPerPerson", ResourceType = typeof(Localization.Resources))]
        public double CostPerPerson { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_TrainingPlan_TotalCost", ResourceType = typeof(Localization.Resources))]
        public double TotalCost { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}