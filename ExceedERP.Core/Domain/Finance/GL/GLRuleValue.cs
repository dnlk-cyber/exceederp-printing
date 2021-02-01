using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GlRuleValue : TrackUserSettingOperation
    {
        public int GlRuleValueId { get; set; }
       
        [Display(Name = "Number of Segment")]
        public int NumberOfSegment { get; set; }
       
        [Display(Name = "Is Dynamic CoA ?")]
        public bool DynamicCoA { get; set; }

        [Display(Name = "Calculate Profit Tax ?")]
        public bool CalculateProfitTax { get; set; }
    }
}
