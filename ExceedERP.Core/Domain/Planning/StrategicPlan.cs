using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Planning
{
    public class StrategicPlan : TrackUserSettingOperation
    {
        [Key]
        public int StrategicPlanId { get; set; }

        [Display(Name = "Is Departmental Plan")]
        public bool IsDepartnental { get; set; }

        [Display(Name = "Is Personal Plan")]
        public bool IsPersonal { get; set; }

        [Display(Name = "Description (Key Result Area)")]
        public string Description { get; set; }

        [Display(Name = "From Fiscal Year")]
        public int FromGlFiscalYearId { get; set; }

        [Display(Name = "End Fiscal Year")]
        public int ToGlFiscalYearId { get; set; }

        [Display(Name = "Strategic Objective")]
        public string StrategicObjective { get; set; }

        [Display(Name = "Total Budget (Financing)")]
        public decimal? TotalBudget { get; set; }

        public virtual GlFiscalYear FromGlFiscalYear { get; set; }
        public virtual GlFiscalYear ToGlFiscalYear { get; set; }
    }
}
