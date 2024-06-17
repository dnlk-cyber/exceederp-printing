using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    
    public class RiskMitigationPlan: TrackUserSettingOperation
    {
        [Key]
        public int RiskMitigationPlanId { get; set; }
        [Display(Name = "Identified Risk")]
        public string IdentifiedRisk { get; set; }
        [Display(Name = "Risk Level")]
        public MitigationRiskLevel RiskLevel { get; set; }
        [Display(Name = "Treatment Method")]
        public string TreatmentMethod { get; set; }
        [Display(Name = "Action to be taken")]
        public string ActionToBeTaken { get; set; }
        [Display(Name = "Main")]
        public string MainResponsibly { get; set; }
        [Display(Name = "Support")]
        public string SupportResponsibly { get; set; }
        [Display(Name = "Opportunity")]
        public string Opportunity { get; set; }
        public string Time { get; set; }
        [Display(Name = "Monitoring Method")]
        public string MonitoringMethod { get; set; }
        [Display(Name = "Evaluation")]
        public string Evaluation { get; set; }
        [Display(Name = "Plan Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PlanDate { get; set; }
    }
    public enum MitigationRiskLevel
    {
        Critical,
        High,
        Medium,
        Low
    }
    public class RiskFollowUp
    {
        [Key]
        public int RiskFollowUpId { get; set; }
        [Display(Name = " Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PlanDate { get; set; }
        [Display(Name = "Description of Risk ")]
        public string Description { get; set; }
        [Display(Name = "Method of TreatMent")]
        public string TreatmentMethod { get; set; }
        [Display(Name = "Planned Action to be taken")]
        public string PlannedActionToBeTaken { get; set; }
        [Display(Name = "Actual Action Taken to mitgate Risk")]
        public string ActualActionTaken { get; set; }
        [Display(Name = " Current Status")]
        public string CurrentStatus { get; set; }
        public int RiskMitigationPlanId { get; set; }
        public virtual RiskMitigationPlan MitigationPlan{ get; set; }



    }
}
