using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GlBeginningPeriod : TrackUserSettingOperation
    {
        public int GlBeginningPeriodId { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
    }
}
