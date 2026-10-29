using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Planning
{
    public class AnnualGoal : TrackUserSettingOperation
    {
        public int AnnualGoalId { get; set; }
        public int StrategicPlanId { get; set; }

        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }

        [Display(Name = "Annual Goal")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Remark { get; set; }

        public virtual StrategicPlan StrategicPlan { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
}
