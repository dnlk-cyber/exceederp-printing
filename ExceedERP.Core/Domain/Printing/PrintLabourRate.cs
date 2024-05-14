using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintLabourRate : TrackUserSettingOperation
    {
        [Key]
        public int PrintLaborRateId { get; set; }
        [Display(Name ="Period Code")]
        public string PeriodCode { get; set; }
        [Display(Name = "PeriodName")]
        public string PeriodName { get; set; }
        [Display(Name = "Normal Hour Rate")]
        public decimal NormalHourRate { get; set; }
        [Display(Name = "Overtime hour rate")]
        public decimal OverTimeHourRate { get; set; }
        [Display(Name = "Over head rate")]
        public decimal OverHeadRate { get; set; }
    }
}
