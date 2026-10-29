using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Planning
{
    public class QuickFixPlan : TrackUserSettingOperation
    {
        private const string Decimal18Scale2MaxValue = "9999999999999999.99";

        [Key]
        public int QuickFixPlanId { get; set; }

        [Required]
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }

        [Required]
        [Display(Name = "Annual Plan / From Plan")]
        public int PlanId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", Decimal18Scale2MaxValue)]
        [Display(Name = "Annual Plan")]
        public decimal AnnualPlan { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Up To Quarter")]
        public int UpToQuarter { get; set; }

        [Required]
        [Range(typeof(decimal), "0", Decimal18Scale2MaxValue)]
        [Display(Name = "Plan Up To Quarter")]
        public decimal PlanUpToQuarter { get; set; }

        [Required]
        [Range(typeof(decimal), "0", Decimal18Scale2MaxValue)]
        public decimal Actual { get; set; }

        [Required]
        [Range(typeof(decimal), "0", Decimal18Scale2MaxValue)]
        [Display(Name = "Previous Plan - Remaining Quarter(s)")]
        public decimal PreviousPlanRemainingQuarter { get; set; }

        [Required]
        [Range(typeof(decimal), "0", Decimal18Scale2MaxValue)]
        [Display(Name = "Revised / Improved Plan - Remaining Quarter(s)")]
        public decimal RevisedPlanRemainingQuarter { get; set; }

        public virtual GlFiscalYear FiscalYear { get; set; }
        public virtual Plan AnnualPlanSource { get; set; }
    }
}
